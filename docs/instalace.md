# Instalace

## Z katalogu doplňků (jen když je repozitář veřejný)

1. *Ovládací panel → Doplňky → Repozitáře → +*
   URL: `https://raw.githubusercontent.com/kyselyna/jellyfin-plugin-zpravy-divakum/main/manifest.json`
2. *Katalog → Zprávy pro diváky → Instalovat*, restart Jellyfinu.

## Ručně ze ZIPu

1. Stáhni `zpravy-divakum_<verze>.zip` z [Releases](https://github.com/kyselyna/jellyfin-plugin-zpravy-divakum/releases).
2. Rozbal `Jellyfin.Plugin.ZpravyDivakum.dll` do `plugins/Zpravy pro divaky_<verze>/` v datovém adresáři Jellyfinu
   (Debian/Ubuntu `/var/lib/jellyfin/plugins/`, Docker `/config/plugins/`).
3. Linux: `chown -R jellyfin:jellyfin "…/Zpravy pro divaky_<verze>"`
4. Restartuj Jellyfin; v *Doplňky* musí být **Zprávy pro diváky – Active**.

## Sestavení

```bash
dotnet build src/Jellyfin.Plugin.ZpravyDivakum -c Release
```

Nová verze: zvyš verzi v `build.yaml` a `.csproj`, doplň `CHANGELOG.md`, `git tag vX.Y.Z && git push --tags`
– GitHub Actions sestaví ZIP, vytvoří Release a zaktualizuje `manifest.json`.
Po velké aktualizaci Jellyfinu zvyš verze balíčků `Jellyfin.Controller`/`Jellyfin.Model` a `targetAbi`.
