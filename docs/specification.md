# Specifikace pro zápočtový projekt C\#
Tento dokument slouží jako specifikace zápočtového projektu do předmětu [Programování v C#].

## Kontext
[Jellyfin](https://jellyfin.org/) je open source self-hosted streamovací služba. Dokáže streamovat filmy, seriály, hudbu
a audioknihy. Je celá napsaná v C#. Má build in webový interface, ale existuje spoustu programů
a aplikací, přes které můžeme přistupovat k jeho obsahu. Jellyfin má zabudovanou podporu pro pluginy.

Jellyfin se snaží stahovat metadata k mediím co do nich vložíme. Bohužel to umí pouze pro filmy,
seriály a hudbu. Já sám ho ale používám primárně pro streamování audioknih, kterým metadata stahovat neumí.
Také spousta (hlavně českých) obchodů s audioknihamy nemá metadata připsaná do samotných souborů.

Tento projekt je plugin do jellyfinu, který stahuje metadata k audioknihám z internetových eshopů.

## Specifikace
Tento projekt bude plugin, který se přímo napojí do jellyfinu. Uživatel nebude muset dělat nic navíc.
Samotný jellyfin periodicky vyhledává a doplňuje media, tento plugin ho rozšíří o schopnost doplňování.
Jellyfin bude moci k audioknížkám přidat:

- Title - Plný název knížky
- Autor - Celé jméno autora
- Publisher - jméno vydavatelství
- Narrators - Jméno hlasového herce
- Overview - Stručný popis knihy
- CommunityRating - Hodnocení čtenáři
- Tags - zařazení knihy (typicky žánr a dobový kontext)
- Artwork - přebal

## Zdroje
Aktuálně jediný podporovaný zdroj je [audible.com](https://audible.com/),
ale software je napsaný tak, aby šli snadno přidávat další zdroje.

