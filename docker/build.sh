#!/usr/bin/env bash
# =============================================================================
# build.sh - Build, test and optionally push Broiler.SDK CI container images
# =============================================================================
set -euo pipefail

REGISTRY="${REGISTRY:-ghcr.io}"
OWNER="${OWNER:-broiler-platform}"
TAG="${TAG:-latest}"
PUSH=false
TEST=false
TARGET="${1:-all}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

usage() {
    cat <<EOF
Usage: ./build.sh [TARGET] [OPTIONS]

Targets:
  all          Build base, browser, and office (default)
  base         Build base container (.NET 10.0 SDK)
  browser      Build browser container (Chromium, Playwright, test262, WPT)
  office       Build office container (LibreOffice, PDF tools, fonts, testcases)

Options:
  --registry <registry>   Container registry (default: ghcr.io)
  --owner <owner>         Repository owner / org (default: broiler-platform)
  --tag <tag>             Image tag (default: latest)
  --push                  Push images after building
  --test                  Run verification checks inside the images
  -h, --help              Show this help message

Environment variables:
  REGISTRY, OWNER, TAG
EOF
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        all|base|browser|office)
            TARGET="$1"
            shift
            ;;
        --registry)
            REGISTRY="$2"
            shift 2
            ;;
        --owner)
            OWNER="$2"
            shift 2
            ;;
        --tag)
            TAG="$2"
            shift 2
            ;;
        --push)
            PUSH=true
            shift
            ;;
        --test)
            TEST=true
            shift
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1" >&2
            usage
            exit 1
            ;;
    esac
done

BASE_IMAGE="${REGISTRY}/${OWNER}/broiler-sdk-base:${TAG}"
BROWSER_IMAGE="${REGISTRY}/${OWNER}/broiler-sdk-browser:${TAG}"
OFFICE_IMAGE="${REGISTRY}/${OWNER}/broiler-sdk-office:${TAG}"

build_base() {
    echo "==> Building base image: ${BASE_IMAGE}"
    docker build \
        -t "${BASE_IMAGE}" \
        -f "${SCRIPT_DIR}/base/Dockerfile" \
        "${SCRIPT_DIR}/base"

    if [ "$TEST" = true ]; then
        echo "==> Testing base image..."
        docker run --rm "${BASE_IMAGE}" dotnet --info
    fi

    if [ "$PUSH" = true ]; then
        echo "==> Pushing base image: ${BASE_IMAGE}"
        docker push "${BASE_IMAGE}"
    fi
}

build_browser() {
    echo "==> Building browser image: ${BROWSER_IMAGE}"
    docker build \
        --build-arg BASE_IMAGE="${BASE_IMAGE}" \
        -t "${BROWSER_IMAGE}" \
        -f "${SCRIPT_DIR}/browser/Dockerfile" \
        "${SCRIPT_DIR}/browser"

    if [ "$TEST" = true ]; then
        echo "==> Testing browser image..."
        docker run --rm "${BROWSER_IMAGE}" bash -c \
            "node --version && npx playwright --version && test -d /opt/test262/test && test -d /opt/wpt/css"
    fi

    if [ "$PUSH" = true ]; then
        echo "==> Pushing browser image: ${BROWSER_IMAGE}"
        docker push "${BROWSER_IMAGE}"
    fi
}

build_office() {
    echo "==> Building office image: ${OFFICE_IMAGE}"
    docker build \
        --build-arg BASE_IMAGE="${BASE_IMAGE}" \
        -t "${OFFICE_IMAGE}" \
        -f "${SCRIPT_DIR}/office/Dockerfile" \
        "${SCRIPT_DIR}/office"

    if [ "$TEST" = true ]; then
        echo "==> Testing office image..."
        docker run --rm "${OFFICE_IMAGE}" bash -c \
            "soffice --version && pdftoppm -v 2>&1 | head -n 1 && test -d /opt/libreoffice-extras"
    fi

    if [ "$PUSH" = true ]; then
        echo "==> Pushing office image: ${OFFICE_IMAGE}"
        docker push "${OFFICE_IMAGE}"
    fi
}

case "$TARGET" in
    base)
        build_base
        ;;
    browser)
        build_browser
        ;;
    office)
        build_office
        ;;
    all)
        build_base
        build_browser
        build_office
        ;;
esac

echo "==> Build completed successfully."
