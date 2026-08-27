# RunTrack

Mobilna aplikacija za pracenje trcanja i setnje pomocu GPS senzora,
sa sinhronizacijom podataka na sopstveni web servis.

**Predmet:** Programiranje mobilnih aplikacija
**Tehnologija:** .NET 10 / .NET MAUI (Android + Windows)
**Rok:** 10. septembar 2026.

---

## Sta aplikacija radi

Korisnik startuje trening, aplikacija u pozadini na svakih nekoliko sekundi
ocitava GPS poziciju, racuna predjenu razdaljinu, tempo i trajanje, i crta
rutu u realnom vremenu. Zavrsen trening se cuva u lokalnu SQLite bazu na
telefonu, a zatim sinhronizuje sa web servisom gde se formira rang lista
svih korisnika.

## Struktura repozitorijuma

```
.
├── Dokumentacija/          Detaljna dokumentacija projekta (za ucenje i odbranu)
├── Primer sa predavanja/   Primer koji je profesor dao na predavanju (referenca)
└── RunTrack/               Izvorni kod
    ├── RunTrack.App/       MAUI klijentska aplikacija (Android + Windows)
    ├── RunTrack.Api/       ASP.NET Core Web API (server)
    └── RunTrack.Shared/    Zajednicki modeli koje koriste i klijent i server
```

## Pokretanje

Detaljno uputstvo se nalazi u [Dokumentacija/01-Postavka-okruzenja.md](Dokumentacija/01-Postavka-okruzenja.md).

## Dokumentacija

Kompletna dokumentacija je u folderu [`Dokumentacija/`](Dokumentacija/).
Pisana je od nule, za nekoga ko se prvi put susrece sa MAUI-jem.
