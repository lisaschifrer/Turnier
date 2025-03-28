# Turnier

## Frontend
### 📌 Aktueller Stand (März 2025)

Das Frontend der Beach Cop Cup Anwendung wurde mit **Angular** und **Angular Material** entwickelt und bildet die Grundlage für das User Interface der Turnierverwaltung.

---

### ✅ Features der aktuellen Startseite

#### 🌐 Aufbau
- Die Anwendung läuft als **Single Page App** ohne Routing.
- Komponentenstruktur:
  - `AppComponent`: enthält die **Toolbar**
  - `StartseiteComponent`: enthält den **zentralen Inhalt**

---

#### 🖼️ Startseite (`StartseiteComponent`)
- Anzeige des **großen Turnierlogos** zentriert auf der Seite
- **Pflichtfeld** zur Eingabe des Turniernamens
  - Angular Material Input
  - Echtzeit-Validierung mit Fehlermeldung
- **"Turnier starten" Button**
  - Roter Material Button mit Icon (`+`)
  - Deaktiviert bis ein gültiger Name eingegeben wurde
  - Aktion bei Klick: aktuell einfache `console.log` Ausgabe
- Sauberes, zentriertes Layout optimiert für **Desktop-Anwendung**

---

#### 🧩 Designtechnische Basis
- Angular Material Module: `MatToolbar`, `MatFormField`, `MatInput`, `MatButton`, `MatIcon`
- Theme-orientiertes Styling:
  - Polizei-Dunkelblau für die Toolbar (`#0d47a1`)
  - Rot (`warn`) für Aktions-Button
- Schriftart via Google Fonts: z. B. **Bebas Neue** (optional)
- Kein Responsive Design – App wird **ausschließlich lokal auf Desktop** genutzt

---

### 🧪 Nächster Schritt (geplant)
- Anbindung an das Backend via HTTP POST
- Speichern des Turniernamens über `/api/tournament`
- Erweiterung um weitere Eingaben (Datum, Teams etc.)

---

### 📁 Verwendete Technologien
- [Angular](https://angular.io/)
- [Angular Material](https://material.angular.io/)
- [TypeScript](https://www.typescriptlang.org/)

## Backend
- C# ASP.Net

## Datenbank
- Docker MsSQL Server
