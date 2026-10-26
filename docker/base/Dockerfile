#syntax=docker/dockerfile:1
# -----------------------------------------------------------------------------
# Broiler.SDK CI Container: Base
#
# Base Ubuntu image configured with the latest .NET 10.0 SDK, C/C++ compilation
# and NativeAOT toolchains, Python, Git, and essential CI utilities.
# Serves as the foundation for 'browser' and 'office' specialised CI images.
# -----------------------------------------------------------------------------

# Use Ubuntu 24.04 LTS (Noble Numbat), matching GitHub Actions 'ubuntu-latest' runner LTS
ARG UBUNTU_VERSION=24.04
FROM ubuntu:${UBUNTU_VERSION} AS base

LABEL org.opencontainers.image.title="Broiler.SDK CI Base" \
      org.opencontainers.image.description="Broiler Platform CI base image with .NET 10.0 SDK on Ubuntu" \
      org.opencontainers.image.source="https://github.com/Broiler-Platform/Broiler.Code" \
      org.opencontainers.image.vendor="Broiler Platform" \
      org.opencontainers.image.licenses="Apache-2.0"

ENV DEBIAN_FRONTEND=noninteractive \
    TZ=UTC \
    LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_ROOT=/usr/share/dotnet \
    DOTNET_NOLOGO=true \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=0 \
    DOTNET_USE_POLLING_FILE_WATCHER=false \
    DOTNET_TieredPGO=1 \
    DOTNET_TC_QuickJitForLoops=1 \
    NUGET_XMLDOC_MODE=skip \
    BROILER_CONTAINER_ROLE=base

ENV PATH="${DOTNET_ROOT}:/root/.dotnet/tools:${PATH}"

ARG DOTNET_CHANNEL=10.0

# 1. Install system utilities, NativeAOT prerequisites, and development libraries
RUN apt-get update && apt-get install -y --no-install-recommends \
    apt-transport-https \
    build-essential \
    ca-certificates \
    clang \
    curl \
    git \
    gnupg \
    gzip \
    jq \
    libicu-dev \
    libssl-dev \
    pkg-config \
    procps \
    python3 \
    python3-pip \
    python3-venv \
    tar \
    tzdata \
    unzip \
    util-linux \
    wget \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

# 2. Install .NET 10.0 SDK using Microsoft official installer script
RUN mkdir -p /tmp/dotnet-installer \
    && curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-installer/dotnet-install.sh \
    && chmod +x /tmp/dotnet-installer/dotnet-install.sh \
    && /tmp/dotnet-installer/dotnet-install.sh --channel "${DOTNET_CHANNEL}" --install-dir "${DOTNET_ROOT}" \
    && ln -sf "${DOTNET_ROOT}/dotnet" /usr/bin/dotnet \
    && rm -rf /tmp/dotnet-installer

# 3. Install performance diagnostics and resource telemetry entrypoint
COPY broiler-entrypoint.sh /usr/local/bin/broiler-entrypoint.sh
RUN chmod +x /usr/local/bin/broiler-entrypoint.sh \
    && ln -sf /usr/local/bin/broiler-entrypoint.sh /usr/local/bin/broiler-info \
    && ln -sf /usr/local/bin/broiler-entrypoint.sh /usr/local/bin/broiler-perf

# 4. Verify .NET installation
RUN dotnet --info

WORKDIR /workspace

ENTRYPOINT ["/usr/local/bin/broiler-entrypoint.sh"]
CMD ["/bin/bash"]
