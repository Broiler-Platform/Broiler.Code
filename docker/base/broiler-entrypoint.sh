#!/usr/bin/env bash
# =============================================================================
# broiler-entrypoint.sh - Container Entrypoint & Performance Diagnostics
#
# Inspects and displays runtime performance telemetry:
#   - Cgroup v1 / v2 CPU quotas, period, and throttling
#   - Cgroup memory limits, current usage, and swap
#   - Shared memory (/dev/shm) allocation (critical for Chromium/Playwright)
#   - Storage mount types (detects slow 9p/drvfs host bind mounts)
#   - .NET 10 runtime settings, JIT, and GC configurations
#   - Playwright / LibreOffice toolchain status
#   - Actionable performance recommendations
# =============================================================================
set -e

INFO_ONLY=false
if [ "${1:-}" = "--info-only" ]; then
    INFO_ONLY=true
    shift || true
fi

# Determine if output is an interactive terminal for ANSI colors
if [ -t 1 ] && [ -z "${NO_COLOR:-}" ]; then
    C_RESET='\033[0m'
    C_BOLD='\033[1m'
    C_DIM='\033[2m'
    C_GREEN='\033[32m'
    C_YELLOW='\033[33m'
    C_RED='\033[31m'
    C_CYAN='\033[36m'
    C_MAGENTA='\033[35m'
else
    C_RESET=''
    C_BOLD=''
    C_DIM=''
    C_GREEN=''
    C_YELLOW=''
    C_RED=''
    C_CYAN=''
    C_MAGENTA=''
fi

format_bytes() {
    local b="$1"
    if [ -z "$b" ] || [ "$b" = "max" ] || [ "$b" = "-1" ]; then
        echo "Unlimited"
        return
    fi
    if ! [[ "$b" =~ ^[0-9]+$ ]]; then
        echo "$b"
        return
    fi
    if [ "$b" -ge 1073741824 ]; then
        awk "BEGIN {printf \"%.2f GiB (%'d bytes)\", $b/1073741824, $b}"
    elif [ "$b" -ge 1048576 ]; then
        awk "BEGIN {printf \"%.2f MiB (%'d bytes)\", $b/1048576, $b}"
    elif [ "$b" -ge 1024 ]; then
        awk "BEGIN {printf \"%.2f KiB (%'d bytes)\", $b/1024, $b}"
    else
        printf "%d bytes\n" "$b"
    fi
}

print_diagnostics() {
    echo -e "${C_BOLD}${C_CYAN}================================================================================${C_RESET}"
    echo -e "${C_BOLD}${C_CYAN}  BROILER PLATFORM CI CONTAINER — PERFORMANCE & RESOURCE DIAGNOSTICS${C_RESET}"
    echo -e "${C_BOLD}${C_CYAN}================================================================================${C_RESET}"

    # 1. OS & System Context
    local os_pretty="Linux"
    if [ -f /etc/os-release ]; then
        # shellcheck disable=SC1091
        os_pretty="$(. /etc/os-release && echo "$PRETTY_NAME")"
    fi
    local kernel_ver
    kernel_ver="$(uname -r 2>/dev/null || echo "Unknown")"
    local arch_name
    arch_name="$(uname -m 2>/dev/null || echo "Unknown")"
    local container_role="${BROILER_CONTAINER_ROLE:-unknown}"

    if [ "$container_role" = "unknown" ]; then
        if [ -d "/opt/test262" ] || [ -d "/ms-playwright" ]; then
            container_role="browser"
        elif [ -d "/opt/libreoffice-extras" ] || command -v soffice >/dev/null 2>&1; then
            container_role="office"
        elif command -v dotnet >/dev/null 2>&1; then
            container_role="base"
        fi
    fi

    echo -e "${C_BOLD}• Environment:${C_RESET} ${os_pretty} (${arch_name}) | Kernel: ${kernel_ver} | Role: ${C_GREEN}${container_role}${C_RESET}"

    # 2. Cgroup Version & Detection
    local cgroup_ver="v1"
    if [ -f /sys/fs/cgroup/cgroup.controllers ]; then
        cgroup_ver="v2 (unified)"
    elif [ -d /sys/fs/cgroup/unified ]; then
        cgroup_ver="hybrid"
    fi
    echo -e "${C_BOLD}• Cgroup Mode:${C_RESET} ${cgroup_ver}"

    # 3. CPU Telemetry & Quotas
    local host_cpus
    host_cpus="$(nproc 2>/dev/null || grep -c ^processor /proc/cpuinfo 2>/dev/null || echo 1)"
    local cpu_model
    cpu_model="$(grep -m 1 "model name" /proc/cpuinfo 2>/dev/null | sed 's/model name[[:space:]]*:[[:space:]]*//' || echo "Generic CPU")"

    local cpu_limit_str="Unlimited (all ${host_cpus} host cores)"
    local cpu_effective_cores="${host_cpus}"
    local cpu_throttled="0"
    local cpu_periods="0"
    local cpu_throttle_pct="0.0%"

    if [ -f /sys/fs/cgroup/cpu.max ]; then
        # Cgroup v2
        read -r quota period < /sys/fs/cgroup/cpu.max
        if [ "$quota" != "max" ] && [ -n "$period" ] && [ "$period" -gt 0 ]; then
            cpu_effective_cores="$(awk "BEGIN {printf \"%.2f\", $quota / $period}")"
            cpu_limit_str="${cpu_effective_cores} cores (${quota}us / ${period}us period)"
        fi
        if [ -f /sys/fs/cgroup/cpu.stat ]; then
            cpu_throttled="$(awk '$1 == "nr_throttled" {print $2}' /sys/fs/cgroup/cpu.stat 2>/dev/null || echo 0)"
            cpu_periods="$(awk '$1 == "nr_periods" {print $2}' /sys/fs/cgroup/cpu.stat 2>/dev/null || echo 0)"
        fi
    elif [ -f /sys/fs/cgroup/cpu/cpu.cfs_quota_us ]; then
        # Cgroup v1
        local quota
        quota="$(cat /sys/fs/cgroup/cpu/cpu.cfs_quota_us 2>/dev/null || echo -1)"
        local period
        period="$(cat /sys/fs/cgroup/cpu/cpu.cfs_period_us 2>/dev/null || echo 100000)"
        if [ "$quota" -gt 0 ] && [ "$period" -gt 0 ]; then
            cpu_effective_cores="$(awk "BEGIN {printf \"%.2f\", $quota / $period}")"
            cpu_limit_str="${cpu_effective_cores} cores (${quota}us / ${period}us period)"
        fi
        if [ -f /sys/fs/cgroup/cpu/cpu.stat ]; then
            cpu_throttled="$(awk '$1 == "nr_throttled" {print $2}' /sys/fs/cgroup/cpu/cpu.stat 2>/dev/null || echo 0)"
            cpu_periods="$(awk '$1 == "nr_periods" {print $2}' /sys/fs/cgroup/cpu/cpu.stat 2>/dev/null || echo 0)"
        fi
    fi

    if [ "$cpu_periods" -gt 0 ] && [ "$cpu_throttled" -gt 0 ]; then
        cpu_throttle_pct="$(awk "BEGIN {printf \"%.1f%%\", ($cpu_throttled / $cpu_periods) * 100}")"
    fi

    echo -e "${C_BOLD}• CPU Resources:${C_RESET}"
    echo -e "    Host Physical/Logical: ${host_cpus} cores (${cpu_model})"
    echo -e "    Cgroup Allocation:     ${C_GREEN}${cpu_limit_str}${C_RESET}"
    if [ "$cpu_throttled" -gt 0 ]; then
        echo -e "    CFS Throttling:        ${C_YELLOW}${cpu_throttled}/${cpu_periods} periods (${cpu_throttle_pct} throttled)${C_RESET}"
    else
        echo -e "    CFS Throttling:        None (0 periods throttled)"
    fi

    # 4. Memory Telemetry & Limits
    local host_mem_kb
    host_mem_kb="$(grep MemTotal /proc/meminfo 2>/dev/null | awk '{print $2}' || echo 0)"
    local host_mem_bytes=$((host_mem_kb * 1024))
    local host_mem_str
    host_mem_str="$(format_bytes "$host_mem_bytes")"

    local mem_limit_bytes=""
    local mem_current_bytes=""

    if [ -f /sys/fs/cgroup/memory.max ]; then
        # Cgroup v2
        mem_limit_bytes="$(cat /sys/fs/cgroup/memory.max 2>/dev/null || echo max)"
        mem_current_bytes="$(cat /sys/fs/cgroup/memory.current 2>/dev/null || echo 0)"
    elif [ -f /sys/fs/cgroup/memory/memory.limit_in_bytes ]; then
        # Cgroup v1
        mem_limit_bytes="$(cat /sys/fs/cgroup/memory/memory.limit_in_bytes 2>/dev/null || echo max)"
        mem_current_bytes="$(cat /sys/fs/cgroup/memory/memory.usage_in_bytes 2>/dev/null || echo 0)"
    fi

    # Check if memory limit is essentially unlimited (equal or greater than host RAM or max integer)
    if [ "$mem_limit_bytes" = "max" ] || [ -z "$mem_limit_bytes" ] || [ "$mem_limit_bytes" -ge "$host_mem_bytes" ] 2>/dev/null; then
        mem_limit_str="Unlimited (Host RAM: ${host_mem_str})"
    else
        mem_limit_str="$(format_bytes "$mem_limit_bytes")"
    fi
    local mem_current_str
    mem_current_str="$(format_bytes "$mem_current_bytes")"

    echo -e "${C_BOLD}• Memory Resources:${C_RESET}"
    echo -e "    Host Total RAM:        ${host_mem_str}"
    echo -e "    Cgroup Limit:          ${C_GREEN}${mem_limit_str}${C_RESET}"
    echo -e "    Current Consumption:   ${mem_current_str}"

    # 5. Shared Memory (/dev/shm) - Critical for Chromium/Playwright
    local shm_total_mb=0
    local shm_human="unknown"
    if [ -d /dev/shm ]; then
        shm_total_mb="$(df -m /dev/shm 2>/dev/null | awk 'NR==2 {print $2}' || echo 0)"
        shm_human="$(df -h /dev/shm 2>/dev/null | awk 'NR==2 {print $2}' || echo "unknown")"
    fi

    echo -e "${C_BOLD}• Shared Memory (/dev/shm):${C_RESET} ${shm_human} (${shm_total_mb} MiB)"

    # 6. Storage & Mount Analysis
    echo -e "${C_BOLD}• Storage & Mounts:${C_RESET}"
    local root_fstype
    root_fstype="$(findmnt -n -o FSTYPE / 2>/dev/null || awk '$2=="/" {print $3}' /proc/mounts 2>/dev/null | head -n1 || echo "overlay")"
    local tmp_fstype
    tmp_fstype="$(findmnt -n -o FSTYPE /tmp 2>/dev/null || awk '$2=="/tmp" {print $3}' /proc/mounts 2>/dev/null | head -n1 || echo "rootfs")"
    echo -e "    Root Filesystem (/):   ${root_fstype}"
    echo -e "    Temp Directory (/tmp): ${tmp_fstype}"

    local ws_mount_type="local"
    local ws_mount_fstype=""
    if [ -d /workspace ]; then
        ws_mount_fstype="$(findmnt -n -o FSTYPE /workspace 2>/dev/null || awk '$2=="/workspace" {print $3}' /proc/mounts 2>/dev/null | head -n1 || echo "")"
        if [ -n "$ws_mount_fstype" ]; then
            echo -e "    Workspace Mount:       ${ws_mount_fstype} on /workspace"
            case "$ws_mount_fstype" in
                9p|drvfs|virtiofs|cifs|smb|vboxsf|fuse.sshfs)
                    ws_mount_type="host-bind"
                    ;;
            esac
        else
            echo -e "    Workspace Mount:       container-local (overlay2)"
        fi
    fi

    # 7. .NET Runtime Configuration
    if command -v dotnet >/dev/null 2>&1; then
        local dotnet_ver
        dotnet_ver="$(dotnet --version 2>/dev/null || echo "Unknown")"
        echo -e "${C_BOLD}• .NET 10 Environment:${C_RESET}"
        echo -e "    SDK Version:           ${C_GREEN}${dotnet_ver}${C_RESET} (Root: ${DOTNET_ROOT:-/usr/share/dotnet})"
        echo -e "    Tiered PGO:            ${DOTNET_TieredPGO:-1} | QuickJitForLoops: ${DOTNET_TC_QuickJitForLoops:-1}"
        echo -e "    File Watcher:          Polling=${DOTNET_USE_POLLING_FILE_WATCHER:-false}"
        echo -e "    NuGet XML Docs:        ${NUGET_XMLDOC_MODE:-skip}"
    fi

    # 8. Toolchain Status (Browser / Office)
    if [ "$container_role" = "browser" ] || command -v npx >/dev/null 2>&1; then
        local node_ver
        node_ver="$(node --version 2>/dev/null || echo "not installed")"
        local pw_ver
        pw_ver="$(npx playwright --version 2>/dev/null || echo "not installed")"
        echo -e "${C_BOLD}• Browser & Conformance Tools:${C_RESET}"
        echo -e "    Node.js:               ${node_ver}"
        echo -e "    Playwright:            ${pw_ver} (Path: ${PLAYWRIGHT_BROWSERS_PATH:-/ms-playwright})"
        echo -e "    Platform Override:     ${PLAYWRIGHT_HOST_PLATFORM_OVERRIDE:-none}"
        echo -e "    test262 Directory:     ${TEST262_ROOT:-/opt/test262} $(test -d "${TEST262_ROOT:-/opt/test262}" && echo "[OK]" || echo "[Missing]")"
        echo -e "    WPT Directory:         ${WPT_ROOT:-/opt/wpt} $(test -d "${WPT_ROOT:-/opt/wpt}" && echo "[OK]" || echo "[Missing]")"
    fi

    if [ "$container_role" = "office" ] || command -v soffice >/dev/null 2>&1; then
        local lo_ver
        lo_ver="$(soffice --version 2>/dev/null | head -n1 || echo "not installed")"
        local poppler_ver
        poppler_ver="$(pdftoppm -v 2>&1 | head -n1 || echo "not installed")"
        echo -e "${C_BOLD}• Office & PDF Tools:${C_RESET}"
        echo -e "    LibreOffice:           ${lo_ver}"
        echo -e "    Poppler (pdftoppm):    ${poppler_ver}"
        echo -e "    sw/qa/extras Corpus:   ${BROILER_OFFICE_EXTRAS_DIR:-/opt/libreoffice-extras} $(test -d "${BROILER_OFFICE_EXTRAS_DIR:-/opt/libreoffice-extras}" && echo "[OK]" || echo "[Missing]")"
    fi

    # 9. Performance Alerts & Warnings
    local has_warning=false
    echo -e "${C_BOLD}• Performance Analysis & Alerts:${C_RESET}"

    # Alert A: /dev/shm size
    if [ "$shm_total_mb" -le 64 ]; then
        has_warning=true
        echo -e "  ${C_RED}${C_BOLD}[WARN: Low Shared Memory]${C_RESET} /dev/shm is only ${shm_total_mb} MiB (Docker 64MB default)!"
        echo -e "         Chromium & Playwright will thrash disk or crash without sufficient shared memory."
        echo -e "         ${C_YELLOW}→ Fix:${C_RESET} Run container with ${C_GREEN}--shm-size=2gb${C_RESET} or in docker-compose.yml set ${C_GREEN}shm_size: '2gb'${C_RESET}."
    fi

    # Alert B: Host bind mount
    if [ "$ws_mount_type" = "host-bind" ]; then
        has_warning=true
        echo -e "  ${C_YELLOW}${C_BOLD}[WARN: Cross-OS Host Mount]${C_RESET} /workspace is mounted on '${ws_mount_fstype}' (host filesystem)."
        echo -e "         Windows/host file sharing via 9p/drvfs is 5x to 20x slower for compilation (bin/obj) and node_modules."
        echo -e "         ${C_YELLOW}→ Fix:${C_RESET} Mount intermediate folders (bin, obj, node_modules) onto Docker named volumes or tmpfs."
    fi

    # Alert C: CPU quota
    if [ "$cpu_limit_str" != "Unlimited (all ${host_cpus} host cores)" ]; then
        if (( $(awk "BEGIN {print ($cpu_effective_cores < 2.0) ? 1 : 0}") )); then
            has_warning=true
            echo -e "  ${C_YELLOW}${C_BOLD}[WARN: Restricted CPU Quota]${C_RESET} Container is restricted to ${cpu_effective_cores} CPU cores (Host has ${host_cpus})."
            echo -e "         Parallel MSBuild compilation and test suites will be throttled."
            echo -e "         ${C_YELLOW}→ Fix:${C_RESET} Increase Docker Desktop / WSL2 CPU allocation or pass ${C_GREEN}--cpus=${host_cpus}${C_RESET}."
        fi
    fi

    # Alert D: CPU throttling active
    if [ "$cpu_throttled" -gt 0 ]; then
        has_warning=true
        echo -e "  ${C_YELLOW}${C_BOLD}[WARN: Active CPU Throttling]${C_RESET} CFS scheduler throttled ${cpu_throttled} periods (${cpu_throttle_pct})."
        echo -e "         The container hit its CPU quota ceiling. Increase cgroup CPU quota."
    fi

    if [ "$has_warning" = false ]; then
        echo -e "  ${C_GREEN}[OK]${C_RESET} No critical resource bottlenecks detected. Container is operating at native speed."
    fi

    echo -e "${C_BOLD}${C_CYAN}================================================================================${C_RESET}"
    echo -e "${C_DIM}Run 'broiler-info' at any time to re-check resource telemetry.${C_RESET}\n"
}

# Run diagnostics unless explicitly disabled
if [ "${BROILER_QUIET:-0}" != "1" ] && [ "${BROILER_DIAGNOSTICS:-1}" != "0" ]; then
    print_diagnostics
fi

if [ "$INFO_ONLY" = true ]; then
    exit 0
fi

if [ $# -eq 0 ]; then
    exec /bin/bash
else
    exec "$@"
fi
