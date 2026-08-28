# 01 — Postavka razvojnog okruženja

> **Cilj ovog dokumenta:** da sa potpuno praznog računara dođeš do stanja u kojem
> možeš da pritisneš F5 u Visual Studio-u i da se aplikacija pokrene na tvom telefonu.
>
> Predviđeno vreme: **1–2 sata**, od čega je većina čekanje da se preuzme instalacija.

---

## Sadržaj

1. [Zatečeno stanje računara](#1-zatečeno-stanje-računara)
2. [Šta uopšte instaliramo i zašto](#2-šta-uopšte-instaliramo-i-zašto)
3. [Korak 1 — Visual Studio 2026 Community](#3-korak-1--visual-studio-2026-community)
4. [Korak 2 — Provera instalacije](#4-korak-2--provera-instalacije)
5. [Korak 3 — Priprema Android telefona](#5-korak-3--priprema-android-telefona)
6. [Korak 4 — Povezivanje telefona sa računarom](#6-korak-4--povezivanje-telefona-sa-računarom)
7. [Korak 5 — scrcpy (prikaz telefona na laptopu)](#7-korak-5--scrcpy-prikaz-telefona-na-laptopu)
8. [Česti problemi](#8-česti-problemi)

---

## 1. Zatečeno stanje računara

Pre početka, ovo je zatečeno na računaru (provereno 27.08.2026.):

| Stavka | Stanje |
|---|---|
| .NET SDK | ❌ nije instaliran |
| Visual Studio (IDE) | ❌ nema ga |
| VS 2026 Build Tools | ⚠️ postoji (3.5 GB), ali samo C++ alati — ne pomaže nam |
| Android SDK | ❌ nema ga |
| Java / JDK | ❌ nema ga |
| Git | ✅ verzija 2.55 |
| Slobodan prostor | ✅ 343 GB (treba nam ~20 GB) |
| RAM | 13.9 GB |
| Procesor | AMD Ryzen 5 3500U |
| Virtuelizacija (Hyper-V) | ❌ isključena u BIOS-u |

**Zaključak:** virtuelizacija je isključena, što znači da Android **emulator** ne bi
radio bez ulaska u BIOS. Pošto imamo fizički Android telefon, emulator nam **ne treba**
— telefon je i brži i ima pravi GPS, što nam je za ovu aplikaciju ključno.

---

## 2. Šta uopšte instaliramo i zašto

Ovo je važno da razumeš, jer je često pitanje „šta je šta" u .NET svetu.

### .NET SDK

**Šta je:** skup alata koji ume da prevede (kompajlira) C# kod u program koji se izvršava.
Sadrži kompajler, `dotnet` komandu i standardne biblioteke.

**Analogija:** ako je C# kod recept, .NET SDK je kuhinja sa svom opremom.

Nama treba **.NET 10**, jer primer sa predavanja cilja `net10.0-android`.

### .NET MAUI

**Šta je:** *Multi-platform App UI* — biblioteka koja omogućava da **iz jednog koda**
napraviš aplikaciju koja radi na Androidu, iOS-u, Windows-u i macOS-u.

**Kako radi:** ti napišeš `<Button Text="Kreni" />`, a MAUI to na Androidu pretvori
u pravi Android `Button`, na Windows-u u pravo Windows dugme, itd. Ne pravi „lažna"
dugmad kao neki drugi frejmvorci — koristi prave, nativne kontrole svakog sistema.

**Zašto je to bitno za nas:** isti projekat možemo pokrenuti i na telefonu (Android)
i na laptopu (Windows). Tokom razvoja koristimo Windows jer se pokreće za par sekundi,
a Android build traje 1–2 minuta.

### Android SDK

**Šta je:** alati koje Google isporučuje za pravljenje Android aplikacija — uključujući
`adb` (Android Debug Bridge), program koji preko USB-a komunicira sa telefonom.

**Ne moraš ga instalirati posebno** — Visual Studio ga povuče automatski.

### OpenJDK (Java)

**Šta je:** Android alati su pisani u Javi, pa je Java potrebna da bi oni radili.
Ti nećeš pisati ni jednu liniju Jave.

**Ne moraš ga instalirati posebno** — Visual Studio ga takođe povuče automatski.

### Visual Studio 2026 Community

**Šta je:** IDE (*Integrated Development Environment*) — program u kojem pišeš kod,
prevodiš ga, pokrećeš i tražiš greške. „Community" verzija je **besplatna** za
studente i ličnu upotrebu.

**Zašto baš Visual Studio, a ne VS Code:** VS ima ugrađenu podršku za MAUI —
padajući meni za izbor uređaja, XAML Hot Reload (menjaš izgled dok aplikacija radi),
i debugger koji se kači na telefon. Sa VS Code-om je sve to ručno i mučno.

> 💡 **Ključna stvar:** kad instaliraš Visual Studio i čekiraš MAUI workload,
> on ti **automatski** instalira .NET SDK, MAUI, Android SDK i Javu.
> Ne moraš ništa od toga da instaliraš odvojeno.

---

## 3. Korak 1 — Visual Studio 2026 Community

### 3.1 Preuzimanje

1. Otvori: **https://visualstudio.microsoft.com/downloads/**
2. Nađi **Visual Studio Community 2026** (besplatna verzija — ne Professional, ne Enterprise)
3. Klikni **Free download** — skinuće se mali fajl (~4 MB) po imenu
   `VisualStudioSetup.exe`. To je samo instalator, ne ceo VS.
4. Pokreni ga (tražiće administratorske privilegije — to je normalno)

### 3.2 Izbor komponenti (workload-ova)

Ovo je **najvažniji korak**. Instalator će ti prikazati mrežu kartica sa naslovom
„Workloads". Čekiraj **tačno ova dva**:

| Workload | Zašto nam treba |
|---|---|
| ☑️ **.NET Multi-platform App UI development** | Ovo je MAUI. Povlači .NET 10 SDK, Android SDK, OpenJDK — sve što treba za mobilnu aplikaciju. **Ovo je obavezno.** |
| ☑️ **ASP.NET and web development** | Za naš server (Web API) koji ćemo pisati u drugoj nedelji. |

**NE čekiraj** ostale (Desktop development with C++, Game development, Azure, itd.) —
samo bespotrebno zauzimaju prostor.

### 3.3 Instalacija

- Ukupna veličina: **~15–20 GB**
- Vreme: zavisi od interneta, računaj **30–60 minuta**
- Klikni **Install** i pusti da radi

> ⚠️ Ne prekidaj instalaciju na pola. Ako moraš, pokreni je ponovo — nastaviće odakle je stala.

### 3.4 Prvo pokretanje

Kad se završi, pokreni Visual Studio. Tražiće da se prijaviš Microsoft nalogom —
**možeš preskočiti** klikom na „Not now, maybe later". Zatim biraš temu (Dark/Light),
što je čisto stvar ukusa.

---

## 4. Korak 2 — Provera instalacije

Otvori **PowerShell** (Windows tipka → ukucaj `powershell` → Enter) i pokreni:

```bash
dotnet --version
```

Očekivani rezultat: nešto što počinje sa `10.` (npr. `10.0.100`).

Ako piše `dotnet nije prepoznat kao komanda` — vidi [Česte probleme](#8-česti-problemi).

Zatim proveri da li je MAUI zaista instaliran:

```bash
dotnet workload list
```

Očekivani rezultat: na listi mora da postoji **`maui`** (ili `maui-android` i `maui-windows`).

I na kraju:

```bash
dotnet --list-sdks
```

Ako sve tri komande rade — okruženje je spremno. ✅

---

## 5. Korak 3 — Priprema Android telefona

Android telefoni po fabričkom podešavanju **ne dozvoljavaju** da im računar instalira
aplikacije. To se otključava režimom za programere (*Developer options*), koji je
namerno sakriven da ga običan korisnik ne bi slučajno uključio.

### 5.1 ⚠️ Prvo — dva različita „Developer settings" menija

Ovo je zamka u koju upada skoro svako. Na Androidu postoje **dva potpuno različita**
menija sa sličnim imenom:

| Meni | Gde se nalazi | Šta sadrži | Da li nam treba |
|---|---|---|---|
| **TalkBack developer settings** | Settings → Accessibility → TalkBack → Settings → Advanced | „Display speech output", „Echo recognized speech", „Explore by touch", „Enable node tree debugging" | ❌ **NE** — ovo su podešavanja čitača ekrana za slepe osobe |
| **Developer options** (pravi) | Settings → Additional settings → Developer options | „USB debugging", „Install via USB", „Stay awake", „OEM unlocking" | ✅ **DA** — ovo nam treba |

**Kako da znaš da si u pravom meniju:** pravi Developer options ima na vrhu veliki
prekidač i **desetine stavki** podeljenih u sekcije (Debugging, Networking, Drawing,
Media, Apps…). Ako vidiš samo 8-9 stavki koje sve pominju govor, TalkBack ili gestove
— u pogrešnom si meniju.

> 💡 Pravi **Developer options se uopšte ne prikazuje** u Podešavanjima dok ga ne
> otključaš postupkom iz sledeće sekcije. Ako ga tražiš i ne nalaziš — to je zato
> što još ne postoji, a ne zato što si ga promašio.

### 5.2 Otključavanje režima za programere — realme GT Neo 2

realme GT Neo 2 koristi **realme UI** (nadgradnja Androida, srodna ColorOS-u), gde je
putanja malo drugačija nego na „čistom" Androidu — `Build number` je sakriven **jedan
nivo dublje**, iza kartice `Version`.

1. Otvori **Settings** (Podešavanja)
2. Skroluj skroz dole → **About device** (na starijim realme UI verzijama: *About phone*)
3. Tapni na karticu **Version**
   > ⚠️ Ovo je korak koji većina propusti. Na realme UI-ju `Build number` **nije**
   > odmah na ekranu „About device" — moraš prvo da uđeš u `Version`.
4. Sada vidiš listu (Android version, Build number, Baseband version…).
   Nađi **Build number** i **tapni ga 7 puta zaredom**
5. Posle 3-4 tapa pojaviće se odbrojavanje: *„You are now 4 steps away from being a developer"*
6. Tražiće ti **PIN / šifru / otisak** telefona
7. Pojaviće se poruka: *„You are now a developer!"* / *„Developer mode has been enabled"*

### 5.3 Uključivanje USB debugging-a

1. Vrati se u **Settings**
2. Idi u **Additional settings** (Dodatna podešavanja)
   > Na realme UI 5.0 / Android 14 se ova stavka zove **System settings**
3. Na dnu te liste sada postoji **Developer options** — otvori je
4. Uključi glavni prekidač na vrhu ako već nije uključen
5. Skroluj do sekcije **Debugging** i uključi:
   - ☑️ **USB debugging** — obavezno
   - ☑️ **Install via USB** — **takođe obavezno na realme/OPPO telefonima**
6. Potvrdi upozorenja sa **OK**

> 📱 **realme / OPPO specifičnost:** `USB debugging` sam po sebi dozvoljava računaru
> da *komunicira* sa telefonom, ali **ne i da instalira aplikacije**. Za instalaciju
> je potreban zaseban prekidač **`Install via USB`**. Ako ga preskočiš, `adb devices`
> će lepo prikazati telefon, ali će Visual Studio prijaviti grešku pri deploy-u.
>
> Kod nekih realme UI verzija `Install via USB` traži da telefon ima **umetnut SIM
> i aktivan mobilni internet** dok ga uključuješ (mera protiv prevara). Ako je opcija
> zasivljena — uključi mobilne podatke i probaj ponovo.

> 🔎 **Ako `Developer options` i dalje ne vidiš** u Additional settings: znači da
> tapkanje po `Build number` nije registrovano. Vrati se na korak 5.2 i tapći
> **brže i bez pauze** — Android broji tapove samo unutar kratkog vremenskog prozora.

### 5.4 Šta USB debugging zapravo radi

Otvara na telefonu kanal preko kojeg računar može da:

- instalira i pokrene aplikaciju,
- čita `Debug.WriteLine` poruke iz koda,
- zaustavi aplikaciju na breakpoint-u i pokaže ti vrednosti promenljivih.

Bez njega bi svaku izmenu morao ručno da prebacuješ kao `.apk` fajl i instaliraš — nezamislivo sporo.

> 🔒 **Da li je bezbedno?** Da, dok kačiš telefon na svoj računar. Isključi USB debugging
> kad završiš projekat, čisto reda radi.

---

## 6. Korak 4 — Povezivanje telefona sa računarom

1. Poveži telefon sa laptopom **USB kablom**

   > ⚠️ Mora biti **data kabl**, ne kabl samo za punjenje. Jeftini kablovi često imaju
   > samo žice za struju. Ako se telefon puni ali ga računar ne vidi — probaj drugi kabl.
   > **Najbolje je koristiti originalni kabl koji je došao uz telefon.**

2. Na telefonu će iskočiti prozor: **„Allow USB debugging?"** sa RSA otiskom ključa
   - ☑️ čekiraj **„Always allow from this computer"**
   - klikni **Allow**

3. Na telefonu prevuci notifikaciju o USB-u i izaberi režim **File transfer (MTP)**
   (na nekim telefonima je podrazumevano „Charging only", što zna da pravi problem)

4. Provera na računaru — u PowerShell-u:

```bash
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" devices
```

Očekivani rezultat:

```
List of devices attached
R58N12ABCDE     device
```

- Ako piše `device` → ✅ sve radi
- Ako piše `unauthorized` → nisi potvrdio dijalog na telefonu (otključaj ekran, pojaviće se)
- Ako je lista prazna → vidi [Česte probleme](#8-česti-problemi)

5. U Visual Studio-u, kada otvoriš MAUI projekat, u gornjoj traci pored zelenog
   dugmeta „Play" postoji padajući meni. Tu treba da se pojavi **ime tvog telefona**.
   Izabereš ga i pritisneš F5.

---

## 6.1 Alternativa — bežično povezivanje (Wireless debugging)

Ako kabl pravi problem, Android 11 i noviji podržavaju povezivanje **preko WiFi-ja**,
bez kabla. realme GT Neo 2 ovo podržava.

> ⚠️ Za razvoj je odlično, ali **za ispit obavezno reši i kabl** — školski WiFi može
> da blokira komunikaciju između uređaja, ili da ga uopšte nema.

**Uslov:** telefon i laptop moraju biti na **istoj WiFi mreži**.

1. Na telefonu: Developer options → **Wireless debugging** → uključi
2. Tapni na sam natpis „Wireless debugging" (ne na prekidač) da uđeš u meni
3. Izaberi **Pair device with pairing code**
4. Pojaviće se šestocifreni kod i adresa oblika `192.168.1.55:37421`
5. Na računaru, u PowerShell-u (adresu i kod prepiši sa telefona):

```bash
& "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe" pair 192.168.1.55:37421
```

6. Kad pita `Enter pairing code:` — ukucaj šestocifreni kod sa telefona
7. Vrati se na ekran „Wireless debugging" — tamo piše **druga** adresa i port
   (pod „IP address & Port"). Na nju se sada kačiš:

```bash
& "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe" connect 192.168.1.55:41983
```

8. Provera: `adb devices` treba da prikaže uređaj sa IP adresom umesto serijskog broja

> 💡 Uparivanje se radi **samo jednom**. Kasnije je dovoljna `adb connect` komanda,
> ali se port menja pri svakom restartu telefona.

---

## 7. Korak 5 — scrcpy (prikaz telefona na laptopu)

Ovo **nije obavezno**, ali je jako korisno za demonstraciju na ispitu — profesor gleda
jedan ekran umesto da mu dodaješ telefon u ruke.

**scrcpy** je besplatan alat koji prikaže ekran telefona u prozoru na računaru,
i dozvoljava da telefonom upravljaš mišem.

1. Idi na: **https://github.com/Genymobile/scrcpy/releases**
2. Skini `scrcpy-win64-vX.X.zip`
3. Raspakuj bilo gde (npr. `C:\Alati\scrcpy`)
4. Sa povezanim telefonom, pokreni `scrcpy.exe`
5. Prozor sa ekranom telefona se otvori — to je to, nema instalacije

---

## 8. Česti problemi

### `dotnet` nije prepoznat kao komanda

Instalacija je dodala `dotnet` u PATH, ali PowerShell to ne vidi dok se ne restartuje.

**Rešenje:** zatvori i ponovo otvori PowerShell. Ako i dalje ne radi — restartuj računar.

### `dotnet workload list` ne prikazuje `maui`

Nisi čekirao MAUI workload tokom instalacije.

**Rešenje:** Windows tipka → `Visual Studio Installer` → dugme **Modify** kod tvoje
instalacije → čekiraj *.NET Multi-platform App UI development* → **Modify**.

### `adb devices` prikazuje praznu listu

Redom probaj:

1. Drugi USB kabl (najčešći uzrok — kabl samo za punjenje)
2. Drugi USB port na laptopu (probaj i USB 2.0 port ako ga ima)
3. Na telefonu: Developer options → **Revoke USB debugging authorizations** → otkači kabl → zakači ponovo
4. Restartuj `adb`:

```bash
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" kill-server
```

5. Instaliraj USB drajver proizvođača telefona (Samsung: „Samsung USB Driver for Mobile Phones")

### U „Developer settings" nema opcije USB debugging

Otvorio si **TalkBack** developer settings umesto pravih **Developer options**.
Vidi [sekciju 5.1](#51-️-prvo--dva-različita-developer-settings-menija) — tamo je
tabela po kojoj razlikuješ ta dva menija.

### Telefon se vidi u `adb devices`, ali deploy iz Visual Studio-a pukne

Na realme/OPPO telefonima nije dovoljan samo `USB debugging` — treba uključiti i
**`Install via USB`** u istoj sekciji Developer options. Vidi [sekciju 5.3](#53-uključivanje-usb-debugging-a).

### `adb devices` prikazuje `unauthorized`

Dijalog „Allow USB debugging?" čeka na telefonu, a ekran ti je zaključan.

**Rešenje:** otključaj telefon, prihvati dijalog, čekiraj „Always allow".

### Build traje jako dugo (5+ minuta)

Prvi Android build je uvek spor jer se preuzimaju i prevode Android biblioteke.
Svaki sledeći je znatno brži. Ako je i dalje sporo, dodaj folder projekta u
**izuzetke Windows Defender-a** — antivirus skenira svaki od hiljada generisanih fajlova.

### Visual Studio ne vidi telefon u padajućem meniju

1. Proveri da `adb devices` radi (gore)
2. U VS-u: **Debug → Options → Android** → proveri da je putanja do Android SDK-a popunjena
3. Restartuj Visual Studio sa već povezanim telefonom

---

## 9. Svakodnevni rad

### 9.1 Otvaranje projekta u Visual Studio-u

**File → Open → Project/Solution** → izaberi `RunTrack\RunTrack.slnx`
(ili dupli klik na taj fajl u Explorer-u).

| Deo prozora | Čemu služi |
|---|---|
| **Solution Explorer** (desno) | Stablo fajlova — dupli klik otvara fajl |
| **Padajući meni uređaja** (gore, pored ▶) | Bira gde se aplikacija pokreće |
| **Output** (dole) | Poruke pri prevođenju |
| **Error List** (dole) | Spisak grešaka, dupli klik skače na liniju |

### 9.2 Gde pokrenuti aplikaciju

| Izbor u meniju | Kada | Vreme |
|---|---|---|
| **Windows Machine** | Svakodnevni rad — provera izgleda i logike | ~10 s |
| **RMX3370** (telefon) | GPS, senzori, finalno testiranje | ~2 min |

- **F5** — prevedi, instaliraj i pokreni **sa debagerom** (breakpoint-i rade)
- **Ctrl+F5** — pokreni **bez debagera**, osetno brže

> 💡 Pravilo: sve što nije vezano za senzore razvijaj na **Windows Machine**.
> Razlika 10 sekundi naspram 2 minuta se preko celog projekta meri satima.

### 9.3 Kačenje i otkačinjanje telefona

**Kad se telefon sme otkačiti:** bilo kad, osim dok traje instalacija na uređaj.

- Aplikacija **ostaje instalirana** na telefonu i radi bez kabla. Kabl služi samo da
  se **nova verzija** prebaci na telefon.
- Pri ponovnom kačenju `adb` sam prepozna uređaj za sekund-dva. Dijalog
  „Allow USB debugging?" se ne pojavljuje ponovo — telefon je zapamtio ključ računara
  (zato se čekira „Always allow from this computer").
- Ako se kabl iščupa **usred instalacije**, aplikacija ostane nedovršeno instalirana.
  Rešenje: pokreni deploy ponovo. Ništa se trajno ne kvari.

**Šta se može raditi bez telefona:**

| Radnja | Bez telefona |
|---|---|
| Pisanje i izmena koda | ✅ |
| Prevođenje za Android (provera grešaka) | ✅ |
| Pokretanje Windows verzije | ✅ |
| Dokumentacija, git commit i push | ✅ |
| Instalacija i pokretanje na telefonu | ❌ |
| Slikanje ekrana i čitanje grešaka sa uređaja | ❌ |

### 9.4 ⚠️ Ne gradi na dva mesta istovremeno

Ako Visual Studio i komandna linija grade projekat u isto vreme, oba pišu u iste
`bin/` i `obj/` foldere. Windows tada zaključa fajl, a build pukne greškom tipa
*„The process cannot access the file … because it is being used by another process"*.

**Rešenje ako se desi:** sačekaj da se jedan build završi, pa pokreni drugi. Ako
greška ostane, zatvori Visual Studio, obriši `bin/` i `obj/` i prevedi ponovo.

---

## Šta dalje

Kada ovaj dokument odradiš do kraja i sve tri provere iz [Koraka 2](#4-korak-2--provera-instalacije)
prođu — javi, i krećemo sa pravljenjem projekta.

Sledeći dokument: [`02-Struktura-projekta.md`](02-Struktura-projekta.md)
