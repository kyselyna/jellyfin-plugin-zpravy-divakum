# Zprávy pro diváky – doplněk pro Jellyfin

Doplněk do Jellyfinu, který umí:

- **poslat zprávu na obrazovku** všem připojeným divákům, jen těm, kdo se právě dívají, nebo jen vybraným zařízením;
- **restartovat server s odpočtem** – všichni diváci dostanou upozornění
  *„Server bude restartován za 3 minuty“*, pak za 2 minuty, za 1 minutu, za 30, 20 a 10 sekund, a pak se Jellyfin restartuje;
- odpočet **zrušit** (diváci dostanou „Restart serveru byl zrušen.“);
- **naplánovat** restart s odpočtem (např. každou noc) jako běžnou naplánovanou úlohu Jellyfinu;
- když nikdo nic nepřehrává, restartovat **hned** (volitelné).

Zprávy se zobrazí jako okno přes přehrávání (Jellyfin Web, Android, Android TV, iOS, Kodi, Moonfin…
– každý klient, který podporuje vzdálené ovládání).

## Rychlý start

1. Nainstaluj – [docs/instalace.md](docs/instalace.md).
2. V levém menu ovládacího panelu se objeví **Zprávy pro diváky**.
3. Napiš zprávu a klikni **Poslat zprávu**, nebo nastav minuty a klikni **Spustit odpočet a restartovat**.

## Dokumentace

- [Instalace](docs/instalace.md)
- [Použití a nastavení](docs/pouziti.md)
- [API – ovládání z Home Assistantu a skriptů](docs/api.md)

## Požadavky

Jellyfin **12.1** nebo novější (.NET 10). Ovládat ho může jen **správce** serveru.

## Licence

[GPL-3.0](LICENSE).
