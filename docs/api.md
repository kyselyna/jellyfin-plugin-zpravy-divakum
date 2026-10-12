# API

Všechny endpointy vyžadují **správcovský** přístup: hlavička `Authorization: MediaBrowser Token="<API klíč>"`
(*Ovládací panel → API klíče*).

| Metoda | Cesta | Tělo | Odpověď |
|---|---|---|---|
| GET | `/ZpravyDivakum/Sessions` | | seznam připojených klientů |
| POST | `/ZpravyDivakum/Send` | `{"Header": "…", "Text": "…", "Seconds": 10, "Audience": "All" \| "Watching" \| "Selected", "SessionIds": ["…"]}` | `{"sent": 3}` |
| GET | `/ZpravyDivakum/Countdown` | | `{"Running": true, "RestartAt": "…", "SecondsLeft": 87, "Restart": true, "StartedBy": "…"}` |
| POST | `/ZpravyDivakum/Countdown` | `{"Seconds": 180, "Restart": true, "Text": "Údržba za {cas}."}` (vše volitelné) | stav; `409`, když už odpočet běží |
| DELETE | `/ZpravyDivakum/Countdown` | | `{"cancelled": true}` |

## Home Assistant

`configuration.yaml`:

```yaml
rest_command:
  jellyfin_zprava:
    url: "http://<jellyfin>:8096/ZpravyDivakum/Send"
    method: POST
    headers:
      Authorization: !secret jellyfin_auth_header   # MediaBrowser Token="…"
    content_type: "application/json"
    payload: '{"Text": "{{ text }}", "Audience": "All"}'
  jellyfin_restart_s_odpoctem:
    url: "http://<jellyfin>:8096/ZpravyDivakum/Countdown"
    method: POST
    headers:
      Authorization: !secret jellyfin_auth_header
    content_type: "application/json"
    payload: '{"Seconds": {{ seconds | default(180) }}, "Restart": true}'
```

Použití: `rest_command.jellyfin_zprava` s `text: "Večeře je hotová 🍝"`, nebo před aktualizací serveru
`rest_command.jellyfin_restart_s_odpoctem`.
