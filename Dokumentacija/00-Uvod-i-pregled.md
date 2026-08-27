# 00 — Uvod i pregled projekta

> Ovo je „mapa" celog projekta. Ako se ikad izgubiš u dokumentaciji, vrati se ovde.

---

## 1. Šta pravimo

**RunTrack** — mobilna aplikacija za praćenje trčanja i šetnje pomoću GPS senzora,
sa sinhronizacijom podataka na sopstveni web servis.

### Scenario korišćenja

1. Korisnik se **prijavi** svojim nalogom (korisničko ime + lozinka)
2. Pritisne **START** → aplikacija na svakih nekoliko sekundi očitava GPS poziciju
3. Dok trči, na ekranu vidi **trajanje, pređenu razdaljinu, trenutni tempo** i
   **rutu koja se crta u realnom vremenu**
4. Pritisne **STOP** → trening se snimi u **lokalnu SQLite bazu** na telefonu
5. Kada ima interneta, trening se **sinhronizuje na server**
6. Na kartici **Rang lista** vidi rezultate svih korisnika, povučene sa servera

### Zašto baš ova tema

Uputstvo predmeta daje dve opcije — klijent-server aplikacija **ili** aplikacija sa
senzorima. RunTrack namerno radi **obe**, jer samo tako pokrivamo svih 18 stavki
gradiva. Detaljna mapa pokrivenosti je u [sekciji 5](#5-pokrivenost-gradiva-predmeta).

---

## 2. Arhitektura

Projekat se sastoji iz **tri dela**:

```
┌─────────────────────────────┐         ┌──────────────────────────┐
│      RunTrack.App           │  HTTP   │     RunTrack.Api         │
│   (MAUI, na telefonu)       │ ──────► │  (ASP.NET Core, server)  │
│                             │  JSON   │                          │
│  • GPS senzor               │ ◄────── │  • Prijava korisnika     │
│  • Lokalna SQLite baza      │         │  • Čuvanje treninga      │
│  • Korisnički interfejs     │         │  • Rang lista            │
└─────────────────────────────┘         └──────────────────────────┘
              │                                       │
              └───────────────┬───────────────────────┘
                              ▼
                     ┌──────────────────┐
                     │ RunTrack.Shared  │
                     │ Zajednički modeli│
                     │ (DTO klase)      │
                     └──────────────────┘
```

### Zašto tri projekta, a ne jedan

| Projekat | Uloga | Zašto je odvojen |
|---|---|---|
| **RunTrack.App** | Ono što se instalira na telefon | Prevodi se za Android; ne sme da sadrži serverski kod |
| **RunTrack.Api** | Server koji čuva podatke svih korisnika | Radi na cloudu, ne na telefonu |
| **RunTrack.Shared** | Klase koje opisuju podatke (npr. `TreningDto`) | Da ne pišemo **istu klasu dva puta**. Kad promeniš model na jednom mestu, promeni se i kod klijenta i kod servera — nemoguće je da se raziđu. |

> 💬 **Očekivano pitanje profesora:** *„Zašto ste izdvojili Shared projekat?"*
> Odgovor: da bi ugovor između klijenta i servera (oblik JSON poruka) bio definisan
> na **jednom mestu**. Time se eliminiše cela klasa grešaka gde klijent šalje jedan
> oblik podataka, a server očekuje drugi.

---

## 3. Gde šta radi

| Deo sistema | Gde se izvršava | Kada |
|---|---|---|
| Korisnički interfejs (XAML) | Telefon | Uvek |
| GPS očitavanje | Telefon | Tokom treninga |
| SQLite baza | Telefon (interna memorija aplikacije) | Uvek — radi i bez interneta |
| Web API | Cloud server | Kada ima interneta |

**Ključna odluka:** aplikacija radi **offline-first**. Trening se prvo uvek snimi
lokalno, pa se tek onda pokušava sinhronizacija. Ako nema signala, trening nije
izgubljen — sinhronizuje se kasnije.

Na ispitu je ovo odlična stvar za demonstraciju: uključiš avionski režim, snimiš
trening, isključiš avionski režim → trening „odleti" na server.

---

## 4. Plan rada (27.08. – 10.09.)

Plan je računat na **2–3 sata dnevno** (ukupno ~30 sati), jer se paralelno spremaju
i drugi predmeti.

| Dani | Faza | Rezultat na kraju faze |
|---|---|---|
| 27–28.08. | Postavka okruženja, Git, prvi prazan projekat | Aplikacija se pokreće na telefonu |
| 29–30.08. | Struktura, AppShell, TabbedPage navigacija | Sve stranice postoje, može se kretati kroz njih |
| 31.08. | Stilovi, resursi, trigeri | Aplikacija izgleda kao aplikacija, a ne kao prototip |
| 01–02.09. | MVVM, Data Binding, SQLite baza | Trening se može ručno uneti, snimiti i prikazati u listi |
| 03–04.09. | GPS senzor, praćenje u realnom vremenu | Pravo GPS praćenje radi |
| 05.09. | Custom kontrola — crtanje rute | Ruta se vidi na ekranu |
| 06–07.09. | Web API (server), prijava i sesija korisnika | Server radi lokalno |
| 08.09. | Povezivanje klijenta i servera, sinhronizacija, rad sa fajlovima | Klijent-server komunikacija radi |
| 09.09. | Deploy API-ja na cloud, finalna dokumentacija | Radi preko mobilnog interneta |
| 10.09. | **Rezerva** — namerno ostavljen prazan dan | Za ono što neizbežno pukne |

> ⚠️ Rezervni dan nije luksuz. U razvoju softvera uvek nešto pukne u poslednjem trenutku
> — najčešće baš ono što je nedelju dana radilo bez problema.

### 4.1 Šta namerno NE radimo

Da bi plan stao u 30 sati, sledeće je svesno izostavljeno. Ovo nisu propusti — ovo su
odluke, i vredi ih znati jer bi profesor mogao da pita „zašto niste uradili X".

| Izostavljeno | Zašto |
|---|---|
| **GPS praćenje u pozadini** (kad je aplikacija minimizovana) | Na Androidu zahteva *foreground service* sa stalnom notifikacijom i posebne dozvole — sam po sebi 5–6 sati posla. Naša aplikacija prati lokaciju dok je ekran treninga otvoren, što je za demonstraciju sasvim dovoljno. |
| **Prikaz rute na pravoj mapi** (Google Maps) | Traži API ključ vezan za platnu karticu. Umesto toga crtamo rutu u **sopstvenoj kontroli**, što usput pokriva stavku „korisničke kontrole" iz gradiva — dakle ne gubimo ništa, nego dobijamo. |
| **Izvoz u `.gpx` format** | Rad sa fajlovima pokrivamo jednostavnijim izvozom u `.csv` — ista stavka gradiva, upola manje posla. |
| **Šifrovanje lozinki po standardu (bcrypt/Argon2)** | Koristimo SHA-256 sa „solju", što je dovoljno da se pokaže princip. Prava produkcijska aplikacija bi koristila bcrypt. |
| **Automatski testovi** | Nisu deo gradiva predmeta. |

> 💬 **Očekivano pitanje:** *„Zašto lokacija ne radi kad je aplikacija minimizovana?"*
> Odgovor: zato što Android od verzije 8 ubija pozadinske procese radi štednje baterije.
> Rešenje je *foreground service* sa trajnom notifikacijom i dozvolom
> `ACCESS_BACKGROUND_LOCATION`, što je bilo van obima ovog projekta.

---

## 5. Pokrivenost gradiva predmeta

Ovo je najvažnija tabela u dokumentaciji — pokazuje **gde u projektu** se nalazi
svaka stavka iz zvaničnog spiska gradiva. Ako te profesor pita „gde ste koristili
trigere?", ovde nađeš odgovor.

| # | Stavka gradiva | Gde u projektu | Dokument |
|---|---|---|---|
| 1 | Organizacija projekta u .NET MAUI | Struktura tri projekta | `02` |
| 2 | AppShell, GUI, kretanje po stranicama | `AppShell.xaml` | `05` |
| 3 | XAML | Sve `.xaml` datoteke | `03` |
| 4 | Resursi | `Resources/Styles/` | `06` |
| 5 | Data Binding | Sve stranice ↔ ViewModel-i | `04` |
| 6 | MVVM arhitektura | Folder `ViewModels/` | `04` |
| 7 | Lokalna baza — SQLite | `Services/BazaService.cs` | `07` |
| 8 | Rad sa fajlovima | Izvoz treninga u `.gpx` fajl | `08` |
| 9 | Kontrole | Sve stranice | `03` |
| 10 | Trigeri | Dugme START/STOP menja boju | `06` |
| 11 | Stilovi | `Resources/Styles/Styles.xaml` | `06` |
| 12 | CollectionView | Lista treninga, rang lista | `03` |
| 13 | **Custom kontrola** | `Controls/RutaView.cs` — crtanje GPS rute | `13` |
| 14 | async/await, Task, kolekcija Task-ova | Sinhronizacija više treninga odjednom | `09` |
| 15 | Klijent-server asinhrona komunikacija | `Services/ApiService.cs` | `10` |
| 16 | Komunikacija sa web servisom (API) | `RunTrack.Api` | `10` |
| 17 | **Logovanje i sesija korisnika** | `Services/AuthService.cs` | `11` |
| 18 | Prikaz podataka sa web servisa | Stranica „Rang lista" | `10` |
| 19 | TabbedPage koncept | Glavna navigacija aplikacije | `05` |
| ★ | **Senzori (GPS)** | `Services/GpsService.cs` | `12` |

---

## 6. Kako koristiti ovu dokumentaciju

Dokumenti su numerisani i **namerno poređani redom kojim se projekat gradi**.
Ako ih čitaš redom, svaki naredni se oslanja samo na ono što si već pročitao.

| Dokument | Sadržaj |
|---|---|
| `00-Uvod-i-pregled.md` | ← ovde si sada |
| `01-Postavka-okruzenja.md` | Instalacija alata, povezivanje telefona |
| `02-Struktura-projekta.md` | Svaki folder i fajl — čemu služi |
| `03-XAML-osnove.md` | Jezik za korisnički interfejs, od nule |
| `04-MVVM-i-DataBinding.md` | Najvažniji koncept u MAUI-ju |
| `05-Navigacija-AppShell.md` | Kretanje po stranicama, TabbedPage |
| `06-Stilovi-Resursi-Trigeri.md` | Izgled aplikacije |
| `07-SQLite-baza.md` | Lokalno čuvanje podataka |
| `08-Rad-sa-fajlovima.md` | Izvoz i uvoz podataka |
| `09-Async-await-Task.md` | Asinhrono programiranje |
| `10-Web-API-i-klijent.md` | Server i komunikacija sa njim |
| `11-Login-i-sesija.md` | Prijava korisnika |
| `12-GPS-senzor.md` | Rad sa lokacijom |
| `13-Custom-kontrola.md` | Prava korisnička kontrola |
| `14-Fajl-po-fajl.md` | **Referenca** — svaki fajl u projektu, detaljno |
| `15-Ocekivana-pitanja.md` | **Za ispit** — pitanja i odgovori |

### Oznake koje se koriste

> 💡 **Objašnjenje** — dodatni kontekst, „zašto je to tako"

> ⚠️ **Pažnja** — česta greška ili zamka

> 💬 **Očekivano pitanje** — nešto što bi profesor mogao da pita, sa odgovorom

---

## Šta dalje

Kreni od [`01-Postavka-okruzenja.md`](01-Postavka-okruzenja.md).
