# Použití a nastavení

Stránka: *Ovládací panel → (levé menu) Zprávy pro diváky* nebo *Doplňky → Zprávy pro diváky*.

## Kdo je připojený

Seznam klientů aktivních v posledních 15 minutách (nastavitelné): uživatel, aplikace, zařízení a co právě přehrává (▶).
Zaškrtnutím vybereš příjemce pro volbu *Jen zaškrtnutým*.

## Poslat zprávu

| Pole | |
|---|---|
| Nadpis | výchozí „Zpráva od správce“ |
| Text | až 2000 znaků |
| Komu | všem připojeným / jen těm, kdo právě přehrávají / jen zaškrtnutým |
| Jak dlouho zobrazit | sekundy (výchozí 10) |

Výsledek ukáže, na kolik zařízení zpráva odešla.

## Restart s odpočtem

- **Restartovat za (minut)** – výchozí 3; desetinná čísla jdou (0,5 = 30 s).
- **Spustit odpočet a restartovat** – po potvrzení začne odpočet. Upozornění přijdou v krocích z nastavení
  (výchozí `180,120,60,30,20,10` s), které jsou kratší než zvolená doba; když doba není jedním z kroků, první upozornění přijde hned.
- **Jen vyzkoušet odpočet** – upozornění se pošlou, ale server se nerestartuje.
- **Zrušit odpočet** – zastaví ho a pošle „Restart serveru byl zrušen.“
- Stav (zbývající čas, kdo odpočet spustil) se na stránce obnovuje každou sekundu.
- Když je zapnuto *Když nikdo nic nepřehrává, restartovat hned*, a nikdo nic nepřehrává, restart proběhne za 5 s.

### Naplánovaný restart

*Ovládací panel → Naplánované úlohy → Zprávy pro diváky → Restart serveru s odpočtem* → přidat spouštěč
(např. denně ve 4:00). Úloha spustí odpočet; zrušením úlohy se zruší i odpočet.

## Nastavení

| Volba | Výchozí |
|---|---|
| Výchozí nadpis zprávy | Zpráva od správce |
| Výchozí doba zobrazení | 10 s |
| Kroky odpočtu | `180,120,60,30,20,10` |
| Nadpis odpočtu | Restart serveru |
| Text odpočtu | `Server bude restartován za {cas}. Přehrávání se na chvíli přeruší.` – `{cas}` = „3 minuty“, „1 minutu“, „30 sekund“… |
| Text při zrušení | Restart serveru byl zrušen. |
| Když nikdo nic nepřehrává, restartovat hned | zapnuto |
| Za připojené počítat klienty aktivní v posledních | 900 s |

## Poznámky

- Zprávu zobrazí jen klient, který je připojený přes WebSocket a podporuje vzdálené ovládání
  (většina oficiálních aplikací; některé TV aplikace zobrazí zprávu jen mimo přehrávač).
- Restart dělá Jellyfin sám (stejně jako tlačítko *Restartovat* v ovládacím panelu). Na Linuxu se balíčková instalace
  restartuje přes systemd; v Dockeru musí mít kontejner `restart: unless-stopped`.
