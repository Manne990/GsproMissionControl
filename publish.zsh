set -euo pipefail

dotnet publish -c Release -r win-x64 --self-contained true

OUTDIR="$HOME/Downloads/GsproMissionControl"
ZIPFILE="$HOME/Downloads/GsproMissionControl.zip"
PUBLISH="$HOME/Projects/gspro-poc/GsproMissionControl/bin/Release/net10.0/win-x64/publish"

rm -f "$ZIPFILE"
rm -rf "$OUTDIR"
mkdir -p "$OUTDIR"

# Kopiera ALLT inkl dolda filer, bevara struktur/attribut
cp -a "$PUBLISH"/. "$OUTDIR"/

# Skapa zip (tystare)
(cd "$HOME/Downloads" && zip -rq "$(basename "$ZIPFILE")" "$(basename "$OUTDIR")")