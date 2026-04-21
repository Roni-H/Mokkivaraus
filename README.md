# Mökkien varausjärjestelmä – .NET 8 MAUI

Villa­ge Newbies Oy:n varausjärjestelmä

## Sisältö
- alueiden hallinta
- palveluiden hallinta
- mökkien hallinta ja vapaan majoituksen haku
- asiakashallinta
- mökkivarausten hallinta
- laskujen hallinta ja seuranta
- majoittumisraportointi aikavälillä ja alueittain
- palveluraportointi aikavälillä ja alueittain
- paperilasku ja sähköpostilasku

## Tekninen rakenne
- .NET 8 MAUI
- Windows target: `net8.0-windows10.0.19041.0`
- SQLite
- MVVM
- Shell-navigointi
- omat Windows platform -tiedostot, `MauiWinUIApplication.CreateMauiApp()`

Toimiva lähtöprojekti, jossa on valmiina perus-CRUD, raportointi, laskutus ja varauksen päällekkäisyystarkistus.
