#syntax=docker/dockerfile:1
# -----------------------------------------------------------------------------
# Broiler.SDK CI Container: Office
#
# Extends 'base' with:
# - LibreOffice headless (soffice)
# - Associated PDF tools (poppler-utils, qpdf, ghostscript)
# - Metric-compatible pinned font sets (Liberation, Carlito, Caladea, DejaVu)
# - Pinned LibreOffice testcases (sw/qa/extras corpus from LibreOffice core)
# -----------------------------------------------------------------------------

ARG BASE_IMAGE=ghcr.io/broiler-platform/broiler-sdk-base:latest
FROM ${BASE_IMAGE} AS office

LABEL org.opencontainers.image.title="Broiler.SDK CI Office" \
      org.opencontainers.image.description="Broiler Platform CI image with LibreOffice headless, PDF tools, pinned fonts, and sw/qa/extras testcases" \
      org.opencontainers.image.source="https://github.com/Broiler-Platform/Broiler.Code" \
      org.opencontainers.image.vendor="Broiler Platform" \
      org.opencontainers.image.licenses="Apache-2.0"

ENV BROILER_SOFFICE=/usr/bin/soffice \
    BROILER_PDFTOPPM=/usr/bin/pdftoppm \
    BROILER_OFFICE_FONT_DIR=/usr/share/fonts/truetype \
    BROILER_OFFICE_FONT_SET=office-fonts-1 \
    BROILER_OFFICE_EXTRAS_DIR=/opt/libreoffice-extras \
    BROILER_OFFICE_EXTRAS_URL=https://github.com/LibreOffice/core/tree/master/sw/qa/extras \
    SAL_USE_VCLPLUGIN=gen \
    BROILER_CONTAINER_ROLE=office

ARG LO_CORE_REPO=https://github.com/LibreOffice/core.git
ARG LO_CORE_BRANCH=master

# 1. Install LibreOffice headless, poppler PDF tools, and pinned metric-compatible fonts
RUN apt-get update && apt-get install -y --no-install-recommends \
    default-jre-headless \
    fontconfig \
    fonts-crosextra-caladea \
    fonts-crosextra-carlito \
    fonts-dejavu-core \
    fonts-liberation2 \
    ghostscript \
    libreoffice-calc \
    libreoffice-core \
    libreoffice-writer \
    poppler-utils \
    qpdf \
    && fc-cache -fv \
    && rm -rf /var/lib/apt/lists/*

# 2. Checkout the LibreOffice sw/qa/extras testcase corpus using sparse checkout
RUN mkdir -p /tmp/libreoffice-core \
    && git clone --depth 1 --filter=blob:none --sparse --branch "${LO_CORE_BRANCH}" "${LO_CORE_REPO}" /tmp/libreoffice-core \
    && cd /tmp/libreoffice-core \
    && git sparse-checkout set sw/qa/extras \
    && git checkout \
    && mv sw/qa/extras "${BROILER_OFFICE_EXTRAS_DIR}" \
    && rm -rf /tmp/libreoffice-core \
    && ls -la "${BROILER_OFFICE_EXTRAS_DIR}" | head -n 15

# 3. Verify installation
RUN soffice --version \
    && pdftoppm -v 2>&1 | head -n 1 \
    && fc-list : family | grep -i "Liberation" | head -n 4 \
    && test -d "${BROILER_OFFICE_EXTRAS_DIR}"

WORKDIR /workspace

CMD ["/bin/bash"]
