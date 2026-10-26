#syntax=docker/dockerfile:1
# -----------------------------------------------------------------------------
# Broiler.SDK CI Container: Browser
#
# Extends 'base' with:
# - Node.js LTS (v22.x) and npm
# - Headless Chromium via Playwright (browser binaries & system dependencies)
# - Checkout of test262 (ECMAScript Test Suite)
# - Checkout of WPT (Web Platform Tests)
# - Xvfb (Virtual Framebuffer) for headless graphics / test runs
# -----------------------------------------------------------------------------

ARG BASE_IMAGE=ghcr.io/broiler-platform/broiler-sdk-base:latest
FROM ${BASE_IMAGE} AS browser

LABEL org.opencontainers.image.title="Broiler.SDK CI Browser" \
      org.opencontainers.image.description="Broiler Platform CI image with Chromium headless, Playwright, test262 and WPT" \
      org.opencontainers.image.source="https://github.com/Broiler-Platform/Broiler.Code" \
      org.opencontainers.image.vendor="Broiler Platform" \
      org.opencontainers.image.licenses="Apache-2.0"

ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright \
    PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD=0 \
    PLAYWRIGHT_HOST_PLATFORM_OVERRIDE=ubuntu24.04-x64 \
    PLAYWRIGHT_CHROMIUM_FLAGS="--disable-dev-shm-usage" \
    TEST262_ROOT=/opt/test262 \
    WPT_ROOT=/opt/wpt \
    WPT_CHECKOUT_DIR=/opt/wpt \
    DISPLAY=:99 \
    BROILER_CONTAINER_ROLE=browser

ARG NODE_MAJOR=22
ARG PLAYWRIGHT_VERSION=1.59.1
ARG TEST262_REPO=https://github.com/tc39/test262.git
ARG TEST262_REF=ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
ARG WPT_REPO=https://github.com/web-platform-tests/wpt.git
ARG WPT_REF=master

# 1. Install Node.js from NodeSource and headless browser runtime dependencies
RUN mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_${NODE_MAJOR}.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
    && apt-get update && apt-get install -y --no-install-recommends \
        fonts-liberation \
        libasound2t64 \
        libatk-bridge2.0-0 \
        libatk1.0-0 \
        libcairo2 \
        libcups2 \
        libdbus-1-3 \
        libdrm2 \
        libgbm1 \
        libglib2.0-0t64 \
        libnspr4 \
        libnss3 \
        libpango-1.0-0 \
        libx11-6 \
        libx11-xcb1 \
        libxcb1 \
        libxcomposite1 \
        libxcursor1 \
        libxdamage1 \
        libxext6 \
        libxfixes3 \
        libxi6 \
        libxrandr2 \
        libxrender1 \
        libxss1 \
        libxtst6 \
        nodejs \
        xauth \
        xvfb \
    && rm -rf /var/lib/apt/lists/*

# 2. Install Playwright and Chromium headless with platform override
RUN mkdir -p "${PLAYWRIGHT_BROWSERS_PATH}" \
    && npm install -g "playwright@${PLAYWRIGHT_VERSION}" \
    && PLAYWRIGHT_HOST_PLATFORM_OVERRIDE=ubuntu24.04-x64 npx playwright install --with-deps chromium \
    && chmod -R 777 "${PLAYWRIGHT_BROWSERS_PATH}"

# 3. Checkout test262 (ECMAScript Conformance Test Suite)
RUN git clone --filter=blob:none "${TEST262_REPO}" "${TEST262_ROOT}" \
    && git -C "${TEST262_ROOT}" checkout "${TEST262_REF}" \
    && git -C "${TEST262_ROOT}" log -1 --oneline

# 4. Checkout WPT (Web Platform Tests)
RUN git clone --single-branch --branch "${WPT_REF}" --depth 1 "${WPT_REPO}" "${WPT_ROOT}" \
    && git -C "${WPT_ROOT}" log -1 --oneline

# 5. Pre-install Python runner dependencies for WPT and test harnesses
RUN python3 -m pip install --no-cache-dir --break-system-packages \
    requests \
    urllib3 \
    virtualenv

# 6. Verify installation
RUN node --version \
    && npm --version \
    && npx playwright --version \
    && test -d "${TEST262_ROOT}/test" \
    && test -d "${WPT_ROOT}/css"

WORKDIR /workspace

CMD ["/bin/bash"]
