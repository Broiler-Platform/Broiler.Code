#!/usr/bin/env bash
# Creates the draft GitHub pre-release for one Broiler.Code version.
#
#   eng/release-draft.sh <version> <artifacts-dir>
#
# <artifacts-dir> holds the Publish workflow's artifacts, extracted as
# `gh run download` / actions/download-artifact leave them, one directory per head and
# variant:
#
#   <artifacts-dir>/broiler-code-win-x64-self-contained-<version>/Broiler.Code.Windows.exe
#   <artifacts-dir>/broiler-code-win-x64-framework-dependent-<version>/Broiler.Code.Windows.exe, *.dll, ...
#   <artifacts-dir>/broiler-code-linux-x64-self-contained-<version>/Broiler.Code.Linux
#   <artifacts-dir>/broiler-code-linux-x64-framework-dependent-<version>/Broiler.Code.Linux, *.dll, ...
#
# Each directory is zipped into one release asset, Broiler.Code-<version>-<rid>-<variant>.zip.
# The executable is recorded as rwxr-xr-x and everything else as rw-r--r--: the workflow
# artifact drops the Linux binary's executable bit, the release zip restores it.
#
# The tag code-v<version> must already exist on the remote; the release is created for it
# as a draft, so nothing is public until someone publishes it. Needs the GitHub CLI with
# GH_TOKEN (or a login) that may write releases, and Python 3.
set -euo pipefail

version=${1:?usage: eng/release-draft.sh <version> <artifacts-dir>}
artifacts=${2:?usage: eng/release-draft.sh <version> <artifacts-dir>}
tag="code-v$version"
out=$(mktemp -d)

python=$(command -v python3 || command -v python) || { echo "needs python3" >&2; exit 1; }

# Python writes the zip so each Unix mode is recorded explicitly instead of read from the
# file system, which on Windows has no executable bit to read.
asset() {
  local rid=$1 variant=$2 executable=$3
  local source="$artifacts/broiler-code-$rid-$variant-$version"
  local zip="$out/Broiler.Code-$version-$rid-$variant.zip"
  [ -f "$source/$executable" ] || { echo "missing $source/$executable" >&2; exit 1; }
  "$python" - "$source" "$zip" "$executable" <<'PY'
import os, sys, zipfile, time
source, target, executable = sys.argv[1:4]
stamp = time.localtime()[:6]
with zipfile.ZipFile(target, 'w') as z:
    for directory, dirs, files in os.walk(source):
        dirs.sort()
        for name in sorted(files):
            path = os.path.join(directory, name)
            entry = os.path.relpath(path, source).replace(os.sep, '/')
            info = zipfile.ZipInfo(entry, date_time=stamp)
            info.compress_type = zipfile.ZIP_DEFLATED
            info.create_system = 3          # Unix, so external_attr carries a mode
            mode = 0o755 if entry == executable else 0o644
            info.external_attr = (0o100000 | mode) << 16
            with open(path, 'rb') as f:
                z.writestr(info, f.read())
PY
  echo "$zip"
}

assets=(
  "$(asset win-x64 self-contained Broiler.Code.Windows.exe)"
  "$(asset win-x64 framework-dependent Broiler.Code.Windows.exe)"
  "$(asset linux-x64 self-contained Broiler.Code.Linux)"
  "$(asset linux-x64 framework-dependent Broiler.Code.Linux)"
)

cat > "$out/notes.md" <<NOTES
Broiler Code **$version**, a preview build for evaluation and testing, not for
production use.

## Downloads

| Platform | File | Run |
| --- | --- | --- |
| Windows x64 | \`Broiler.Code-$version-win-x64-self-contained.zip\` | unzip, start \`Broiler.Code.Windows.exe\` |
| Windows x64 | \`Broiler.Code-$version-win-x64-framework-dependent.zip\` | install the .NET 10 Runtime, unzip, start \`Broiler.Code.Windows.exe\` |
| Linux x64 | \`Broiler.Code-$version-linux-x64-self-contained.zip\` | unzip, run \`./Broiler.Code.Linux\` (X11) |
| Linux x64 | \`Broiler.Code-$version-linux-x64-framework-dependent.zip\` | install the .NET 10 Runtime, unzip, run \`./Broiler.Code.Linux\` (X11) |

A **self-contained** zip holds a single executable carrying the .NET runtime and every
assembly: nothing to install, nothing else to copy. A **framework-dependent** zip is much
smaller and runs on the .NET 10 runtime already installed on the machine. The executables
are not code-signed, so Windows SmartScreen may ask before the first start.
NOTES

gh release create "$tag" "${assets[@]}" \
  --verify-tag \
  --draft \
  --prerelease \
  --title "Broiler Code $version" \
  --notes-file "$out/notes.md" \
  --generate-notes
