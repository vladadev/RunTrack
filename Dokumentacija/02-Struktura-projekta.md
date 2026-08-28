# 02 — Struktura MAUI projekta

> **Cilj:** da znaš čemu služi svaki fajl u projektu i da umeš da objasniš
> **kojim redom se aplikacija pokreće**.
>
> 💬 Ovo je gradivo za pitanje koje profesor gotovo sigurno postavlja:
> *„Objasnite mi strukturu MAUI projekta."*

---

## 1. Šta trenutno imamo

Komandom `dotnet new maui` napravljen je šablonski projekat — prazna aplikacija sa
dugmetom koje broji klikove. Ništa od toga nije naš kod, ali sve to moramo da razumemo
jer na tome gradimo.

```
RunTrack/
├── RunTrack.slnx                 ← "solution" — spisak projekata
└── RunTrack.App/                 ← naša MAUI aplikacija
    │
    ├── RunTrack.App.csproj       ← podešavanja projekta
    │
    ├── MauiProgram.cs            ← ⭐ ulazna tačka (start aplikacije)
    ├── App.xaml (+ .cs)          ← ⭐ objekat aplikacije, globalni resursi
    ├── AppShell.xaml (+ .cs)     ← ⭐ navigacija (kostur stranica)
    ├── MainPage.xaml (+ .cs)     ← ⭐ prva stranica koju korisnik vidi
    │
    ├── Platforms/                ← kod specifičan za svaki sistem
    │   ├── Android/
    │   ├── iOS/
    │   ├── MacCatalyst/
    │   └── Windows/
    │
    ├── Resources/                ← slike, fontovi, boje, stilovi
    │   ├── AppIcon/
    │   ├── Fonts/
    │   ├── Images/
    │   ├── Raw/
    │   ├── Splash/
    │   └── Styles/
    │
    ├── Properties/
    │
    ├── bin/    ← rezultat prevođenja (NE ide u git)
    └── obj/    ← privremeni fajlovi prevođenja (NE ide u git)
```

Zvezdicom ⭐ su označena četiri fajla koja čine „srce" svake MAUI aplikacije.
Njih obrađujemo prve.

---

## 2. Najvažnije: kako se aplikacija pokreće

Ovo je **ključno pitanje** za odbranu. Kad korisnik tapne ikonicu na telefonu,
dešava se sledeći lanac:

```
   1. MainActivity.cs          (Platforms/Android/)
      Android pokreće ovu klasu — to je "vrata" u Android svetu
                    │
                    ▼
   2. MauiProgram.CreateMauiApp()
      Podešava aplikaciju: fontovi, servisi, logovanje
      Vraća gotov MauiApp objekat
                    │
                    ▼
   3. App.xaml.cs  →  CreateWindow()
      Pravi prozor aplikacije i kaže: "u tebe ide AppShell"
                    │
                    ▼
   4. AppShell.xaml
      Definiše navigaciju — koje stranice postoje i kako se do njih stiže
                    │
                    ▼
   5. MainPage.xaml
      Prva stranica se konačno iscrtava na ekranu
```

> 💬 **Očekivano pitanje:** *„Šta se prvo izvršava kada se aplikacija pokrene?"*
> Odgovor: na Androidu `MainActivity`, koja poziva `MauiProgram.CreateMauiApp()`.
> To je jedina metoda koja mora da postoji — ona sastavlja i vraća aplikaciju.

---

## 3. Zašto su fajlovi u parovima (`.xaml` i `.xaml.cs`)

Primetićeš da uz skoro svaki `.xaml` fajl ide i `.xaml.cs`. To **nisu dva fajla —
to je jedna klasa podeljena na dva dela**:

| Fajl | Sadrži | Analogija |
|---|---|---|
| `MainPage.xaml` | **izgled** — šta se vidi | crtež, skica sobe |
| `MainPage.xaml.cs` | **ponašanje** — šta se dešava kad korisnik nešto uradi | električne instalacije u toj sobi |

U C#-u se to postiže ključnom rečju **`partial`**:

```csharp
public partial class MainPage : ContentPage
```

`partial` znači: *„ova klasa je opisana na više mesta, spoji ih pri prevođenju"*.
Kompajler od tvog XAML-a automatski generiše treći, skriveni deo klase — i sva tri
dela spoji u jednu.

> 💡 Zato u `MainPage.xaml.cs` možeš da napišeš `CounterBtn.Text = "..."`, iako
> `CounterBtn` nigde nisi deklarisao u C#-u. Deklarisao si ga u XAML-u atributom
> `x:Name="CounterBtn"`, a kompajler je od toga napravio polje u klasi.

### Šta radi `InitializeComponent()`

U konstruktoru svake stranice stoji:

```csharp
public MainPage()
{
    InitializeComponent();
}
```

Ta metoda je **automatski generisana** i radi jednu stvar: pročita XAML i napravi
sve kontrole opisane u njemu. Bez nje bi stranica bila prazna.

> ⚠️ Nikad je ne briši i ne premeštaj ispod svog koda. Ako pokušaš da pristupiš
> kontroli **pre** `InitializeComponent()`, dobićeš `NullReferenceException`,
> jer kontrola u tom trenutku još ne postoji.

---

## 4. Četiri ključna fajla, jedan po jedan

### 4.1 `MauiProgram.cs` — ulazna tačka

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

**Red po red:**

| Linija | Šta radi |
|---|---|
| `MauiApp.CreateBuilder()` | Pravi „građevinara" — objekat koji sklapa aplikaciju deo po deo |
| `.UseMauiApp<App>()` | Kaže koja klasa predstavlja aplikaciju — naša `App` klasa |
| `.ConfigureFonts(...)` | Registruje fontove. Prvi argument je ime fajla, drugi je **nadimak** pod kojim ga zoveš u XAML-u |
| `#if DEBUG` | Kod unutar se prevodi **samo u Debug režimu**. U finalnoj verziji ga nema |
| `builder.Build()` | Sklapa sve i vraća gotovu aplikaciju |

> 💡 **Zašto „builder" šablon?** Zato što aplikacija ima mnogo podešavanja, a ovako
> se dodaju **redom, jedno po jedno**, umesto konstruktorom sa 15 argumenata.
> Ovde ćemo kasnije registrovati i naše servise (bazu, GPS, API).

> 💬 **Očekivano pitanje:** *„Gde biste registrovali servis za bazu podataka?"*
> Odgovor: u `MauiProgram.cs`, pre `builder.Build()`.

---

### 4.2 `App.xaml` + `App.xaml.cs` — objekat aplikacije

**`App.xaml`** drži **globalne resurse** — stvari dostupne iz cele aplikacije:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
            <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

`MergedDictionaries` znači „uvezi ove fajlove ovde". Time boje i stilovi definisani
u `Colors.xaml` i `Styles.xaml` postaju vidljivi **na svakoj stranici**.

**`App.xaml.cs`** pravi prozor aplikacije:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    return new Window(new AppShell());
}
```

Prevod na srpski: *„napravi prozor, a u njega stavi AppShell"*.

> 💡 Na telefonu je „prozor" ceo ekran, pa ti to deluje suvišno. Ali ista aplikacija
> na Windows-u radi u pravom prozoru koji se pomera i menja veličinu — zato MAUI
> uvek pravi `Window`.

---

### 4.3 `AppShell.xaml` — navigacija

```xml
<Shell x:Class="RunTrack.App.AppShell" ... Title="RunTrack.App">
    <ShellContent
        Title="Home"
        ContentTemplate="{DataTemplate local:MainPage}"
        Route="MainPage" />
</Shell>
```

`Shell` je MAUI-jev sistem navigacije. Umesto da ručno pišeš kod za prelazak sa
stranice na stranicu, ovde **deklarišeš spisak stranica**, a Shell pravi meni,
kartice i „nazad" dugme.

| Atribut | Značenje |
|---|---|
| `Title="Home"` | Naslov u zaglavlju |
| `ContentTemplate` | Koja stranica se prikazuje |
| `Route="MainPage"` | **Adresa** stranice — omogućava `GoToAsync("MainPage")` iz koda |

> 💡 `{DataTemplate local:MainPage}` znači *„napravi MainPage tek kad zatreba"*,
> a ne odmah pri pokretanju. Da se sve stranice prave unapred, pokretanje bi bilo sporo.

Ovaj fajl ćemo najviše menjati u sledećoj fazi — ovde ubacujemo naše kartice
(Trening, Istorija, Rang lista, Profil).

---

### 4.4 `MainPage.xaml` + `.cs` — prva stranica

Šablonska stranica sa dugmetom-brojačem. `MainPage.xaml.cs`:

```csharp
public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;
        CounterBtn.Text = $"Clicked {count} time";
    }
}
```

Ovo je **klasičan pristup preko event handler-a**: dugme u XAML-u ima
`Clicked="OnCounterClicked"`, i ta metoda direktno menja kontrolu.

> ⚠️ **Važno za ispit:** ovako **nećemo** pisati našu aplikaciju. Prelazimo na
> **MVVM** obrazac, gde stranica ne dira kontrole direktno, nego se „vezuje"
> za podatke. Zašto je to bolje — objašnjeno je u dokumentu
> [`04-MVVM-i-DataBinding.md`](04-MVVM-i-DataBinding.md).
>
> 💬 Ako profesor pita *„zašto ne menjate kontrole direktno iz code-behind-a?"* —
> to je jedno od najverovatnijih pitanja, i odgovor je u dokumentu 04.

---

## 5. Folder `Platforms/` — kod za pojedinačne sisteme

MAUI pokriva ~95% posla zajedničkim kodom. Preostalih 5% je ono što je stvarno
različito na svakom sistemu, i to živi ovde.

| Folder | Kada se koristi |
|---|---|
| `Android/` | Kad se aplikacija prevodi za Android |
| `iOS/`, `MacCatalyst/` | Za Apple uređaje — **nama nikad**, jer za to treba Mac |
| `Windows/` | Kad se prevodi za Windows |

> 💡 Fajlovi iz `Platforms/iOS/` se **uopšte ne prevode** kada praviš Android verziju.
> MAUI ih automatski isključuje. Zato ih slobodno ignoriši — ne smetaju i ne usporavaju.

### Fajlovi koji su nama bitni

**`Platforms/Android/MainActivity.cs`**
Android „vrata" u aplikaciju. Svaka Android aplikacija mora imati bar jednu Activity.
Ova je tanka — samo prosleđuje kontrolu MAUI-ju.

**`Platforms/Android/AndroidManifest.xml`** ⭐
Lična karta aplikacije za Android: ime, ikonica, verzija, i — najvažnije za nas —
**dozvole**.

Ovde ćemo kasnije dodati dozvolu za lokaciju:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

> ⚠️ Bez upisa u manifest, Android **neće ni pitati korisnika** za dozvolu — GPS će
> prosto ćutati. To je klasična greška koja košta sate traženja.
>
> 💬 **Očekivano pitanje:** *„Kako ste dobili pristup GPS-u?"* Odgovor: dozvola u
> `AndroidManifest.xml` + traženje dozvole od korisnika u toku rada
> (od Androida 6 je potrebno oboje).

---

## 6. Folder `Resources/` — sve što nije kod

| Folder | Sadrži | Napomena |
|---|---|---|
| `AppIcon/` | Ikonica aplikacije (`.svg`) | Iz jednog SVG-a MAUI pravi sve veličine za sve uređaje |
| `Splash/` | Slika pri pokretanju | Prikazuje se dok se aplikacija učitava |
| `Images/` | Slike koje koristiš u aplikaciji | Trenutno samo `dotnet_bot.png` |
| `Fonts/` | Fontovi (`.ttf`) | Moraju se registrovati u `MauiProgram.cs` |
| `Raw/` | Bilo kakvi fajlovi | Npr. početna baza podataka |
| `Styles/` | `Colors.xaml`, `Styles.xaml` | **Ovde ćemo dosta raditi** — boje, stilovi, trigeri |

> 💡 **Zašto SVG za ikonicu?** SVG je vektorska slika — opisana matematički, pa se
> uvećava bez gubitka kvaliteta. Android traži ikonicu u ~6 različitih veličina;
> MAUI ih sve generiše iz jednog SVG fajla pri prevođenju.

---

## 7. `RunTrack.App.csproj` — podešavanja projekta

Tekstualni fajl (XML) sa svim podešavanjima. Najvažnije stavke:

```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="...windows...">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
```

**`TargetFrameworks`** = za koje sisteme se aplikacija pravi. `net10.0-android` znači
„.NET 10, Android verzija". Množina (`Frameworks`) je bitna — jedan projekat,
više ciljnih sistema.

```xml
<ApplicationTitle>RunTrack</ApplicationTitle>
<ApplicationId>rs.vladadev.runtrack</ApplicationId>
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

| Stavka | Značenje |
|---|---|
| `ApplicationTitle` | Ime ispod ikonice na telefonu |
| `ApplicationId` | **Jedinstveni identifikator.** Po njemu Android razlikuje aplikacije. Obrnuta internet adresa je konvencija |
| `ApplicationDisplayVersion` | Verzija koju vidi korisnik (`1.0`) |
| `ApplicationVersion` | Interni broj — mora rasti pri svakoj objavi |

> 📝 Šablon je generisao `com.companyname.runtrack.app`. Promenili smo u
> `rs.vladadev.runtrack`, jer `companyname` je očigledno neizmenjena podrazumevana
> vrednost — sitnica, ali ostavlja utisak nedovršenog projekta.

```xml
<SupportedOSPlatformVersion ...'android'">21.0</SupportedOSPlatformVersion>
```

Najstariji Android koji podržavamo — API 21 (Android 5.0). Tvoj telefon ima
API 33 (Android 13), pa smo daleko iznad minimuma.

---

## 8. `bin/` i `obj/` — zašto ih nema u git-u

| Folder | Sadrži |
|---|---|
| `obj/` | Međurezultati prevođenja — generisani C# kod iz XAML-a, spiskovi paketa |
| `bin/` | Gotov proizvod — `.dll` fajlovi i `.apk` za telefon |

Oba se **automatski prave** pri svakom prevođenju. Zato su u `.gitignore`:

- zauzimaju stotine MB do nekoliko GB,
- menjaju se pri svakom build-u, pa bi git istorija bila nečitljiva,
- svako ih dobija sam kad prevede projekat.

> 💬 **Očekivano pitanje:** *„Šta ako obrišete bin i obj?"*
> Odgovor: ništa se ne gubi — naprave se ponovo pri sledećem prevođenju.
> To je i standardni prvi korak kad se pojavi čudna greška pri build-u
> („čist build").

---

## 9. `RunTrack.slnx` — solution fajl

```xml
<Solution>
  <Project Path="RunTrack.App/RunTrack.App.csproj" />
</Solution>
```

*Solution* je samo **spisak projekata** koji se otvaraju zajedno. Trenutno imamo jedan,
ali plan predviđa tri (`App`, `Api`, `Shared`) — vidi
[`00-Uvod-i-pregled.md`](00-Uvod-i-pregled.md#2-arhitektura).

> 💡 `.slnx` je novi, čitljiviji XML format (od .NET 10). Stariji `.sln` je bio
> nečitljiva mešavina brojeva i GUID-ova. Primer sa predavanja koristi isti `.slnx` format.

---

## 10. Sažetak — šta gde ide

Kad se pitaš „gde da napišem ovo?", koristi ovu tabelu:

| Hoću da... | Fajl |
|---|---|
| registrujem servis ili font | `MauiProgram.cs` |
| dodam boju ili stil za celu aplikaciju | `Resources/Styles/Colors.xaml` / `Styles.xaml` |
| dodam novu stranicu u navigaciju | `AppShell.xaml` |
| promenim izgled stranice | odgovarajući `.xaml` |
| napišem logiku stranice | odgovarajući ViewModel (od dokumenta 04) |
| tražim dozvolu za GPS | `Platforms/Android/AndroidManifest.xml` |
| promenim ime ili verziju aplikacije | `RunTrack.App.csproj` |
| dodam sliku | `Resources/Images/` |

---

## 11. Pitanja za samoproveru

Pokušaj da odgovoriš bez gledanja. Ako zapneš, odgovor je u naznačenoj sekciji.

1. Kojim redom se pokreću `MainActivity`, `App`, `AppShell`, `MauiProgram`? *(sekcija 2)*
2. Šta radi `InitializeComponent()` i zašto mora biti prvo u konstruktoru? *(sekcija 3)*
3. Zašto su klase označene sa `partial`? *(sekcija 3)*
4. Gde se registruju fontovi? *(sekcija 4.1)*
5. Čemu služi `Route` atribut u `AppShell.xaml`? *(sekcija 4.3)*
6. Gde se upisuje dozvola za pristup lokaciji? *(sekcija 5)*
7. Koja je razlika između `ApplicationDisplayVersion` i `ApplicationVersion`? *(sekcija 7)*
8. Zašto `bin/` i `obj/` nisu u git repozitorijumu? *(sekcija 8)*

---

Sledeći dokument: [`03-XAML-osnove.md`](03-XAML-osnove.md)
