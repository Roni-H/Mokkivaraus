# Mökkien varausjärjestelmä – .NET 8 MAUI

Valmis Windows-keskeinen .NET MAUI -pohja Villa­ge Newbies Oy:n varausjärjestelmälle.

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
- omat Windows platform -tiedostot, jotta `MauiWinUIApplication.CreateMauiApp()` on kunnossa

## Ajo Visual Studio 2022:ssa
1. Avaa `MokkiVaraus_MAUI.csproj`
2. Palauta NuGetit
3. Valitse käynnistyskohteeksi **Windows Machine**
4. Build → Rebuild
5. Run

## Huomio
Tämä on toimiva lähtöprojekti, jossa on valmiina perus-CRUD, raportointi, laskutus ja varauksen päällekkäisyystarkistus.
