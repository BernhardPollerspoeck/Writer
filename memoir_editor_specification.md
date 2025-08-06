# Memoir Editor - Vollständige UI-Spezifikation

## Überblick
Ein intuitiver, benutzerfreundlicher Editor für die Erstellung von Memoirs mit Live-Vorschau, hierarchischer Kapitelorganisation und präziser Layoutkontrolle. Die Anwendung ist als WPF-Desktop-Anwendung konzipiert.

## Gesamtlayout

### Fensteraufteilung
- **Gesamtgröße**: 1200x800 Pixel
- **Linke Seite**: 580px breit (Vollansicht + Kapitelbaum)
- **Rechte Seite**: 580px breit (Schreibbereich)
- **Trenner**: 2px vertikale Linie

---

## Linke Seite (580px breit)

### 1. Vollansicht Buch (Oberer Bereich - 3/5 der Höhe)
**Abmessungen**: 560x468px
**Hintergrund**: #fafafa mit #e0e0e0 Rand

#### Features:
- **Zwei aufgeschlagene Seiten** nebeneinander dargestellt
- **Linke Seite**: 240x360px, weiß mit grauem Rand
- **Rechte Seite**: 240x360px, weiß mit grauem Rand
- **Live-Vorschau** des aktuellen Buchinhalts

#### Seiteninhalt:
- Seitenzahlen oben links (z.B. "Seite 34", "Seite 35")
- Kapitelüberschriften mit Unterstrichen
- Formatierter Fließtext in echter Buchdarstellung
- Bilder und Layout-Elemente wie im fertigen Buch

#### Navigation:
- **Linker Pfeil-Button**: Kreisförmig, blau (#4a90e2), für vorherige Seiten
- **Rechter Pfeil-Button**: Kreisförmig, blau (#4a90e2), für nächste Seiten
- **Seitenzähler**: Zentral, "Seite 34-35 von 128"

### 2. Kapitel & Blöcke (Unterer Bereich - 2/5 der Höhe)
**Abmessungen**: 560x280px
**Hintergrund**: #fafafa mit #e0e0e0 Rand

#### Steuerelemente:
- **"+ Hinzufügen" Button**: Grün (#48bb78), 50x20px
- **"🗑️ Löschen" Button**: Rot (#e53e3e), 50x20px

#### Hierarchische Struktur:
**Scrollbarer Bereich**: 540x235px mit Scrollbar

##### Kapitel-Darstellung:
- **Kapitel-Zeile**: 520x25px, hellblau (#e8f4f8)
- **Format**: "📖 Kapitel X: Titel" + Seitenzahl rechts
- **Ausgewähltes Kapitel**: Blau (#4a90e2) hervorgehoben

##### Block-Darstellung:
- **Block-Zeile**: 500x20px, eingerückt unter Kapiteln
- **Format**: "📄 Blocktitel"
- **Ausgewählter Block**: Hellblau (#bee3f8) hervorgehoben

#### Drag & Drop:
- **Funktion**: Kapitel und Blöcke per Drag & Drop neu anordnen
- **Visueller Hinweis**: "Drag & Drop zum Neu-Anordnen"

#### Scrollbar:
- **Breite**: 10px
- **Hintergrund**: #f5f5f5
- **Thumb**: #cbd5e0, 6px breit, abgerundet

---

## Rechte Seite (580px breit)

### 1. Projektverwaltung (Oberer Bereich)
**Abmessungen**: 560x80px
**Hintergrund**: Weiß mit #d0d0d0 Rand

#### Datei-Operationen:
- **"Neu" Button**: Blau (#4a90e2), 40x25px
- **"Laden" Button**: Grün (#48bb78), 50x25px  
- **"Speichern" Button**: Orange (#ed8936), 60x25px
- **"Export" Button**: Violett (#9f7aea), 50x25px

#### Seitenformat-Einstellungen:
- **Breite-Eingabe**: 50x20px, "148mm"
- **Höhe-Eingabe**: 50x20px, "210mm"
- **Custom-Werte** direkt editierbar

#### Ränder-Einstellungen:
- **L (Links)**: 35x20px, "20mm"
- **R (Rechts)**: 35x20px, "15mm"  
- **O (Oben)**: 35x20px, "25mm"
- **U (Unten)**: 35x20px, "20mm"

#### Status-Anzeige:
- **Auto-Save**: "✓ Auto" grün (#f0fff4)

### 2. Schreibbereich
**Abmessungen**: 560x690px
**Hintergrund**: Weiß mit #d0d0d0 Rand

#### Header mit Kapitel-Einstellungen (560x80px):
**Hintergrund**: #f8f9fa

##### Titel-Information:
- **Haupttitel**: "Schreibfenster" (fett)
- **Untertitel**: "Kapitel X: Titel > Aktueller Block"

##### Kapitel-Einstellungen:
- **"Neue Seite" Checkbox**: 12x12px
- **"Immer rechts" Checkbox**: 12x12px (aktiviert)
- **"Immer links" Checkbox**: 12x12px
- **"Leerzeilen" Eingabe**: 30x15px, Zahlenwert

#### Toolbar (560x40px):
**Hintergrund**: Weiß mit #e0e0e0 Rand

##### Formatierungs-Buttons:
- **Bold (B)**: 30x24px, #f0f0f0
- **Italic (I)**: 30x24px, #f0f0f0
- **Underline (U)**: 30x24px, #f0f0f0

##### Schrift-Einstellungen:
- **Schriftart-Dropdown**: 80x24px, "Schriftart"
- **Schriftgröße-Dropdown**: 50x24px, "12pt"

##### Spezial-Buttons:
- **"🖼️ Bild" Button**: Grün (#e8f5e8), 50x24px
- **"📄 Seite" Button**: Orange (#fff3e0), 60x24px
- **"⚙️ Einstellungen" Button**: Violett (#f3e5f5), 100x24px

#### Text-Editor (540x430px):
**Hintergrund**: Weiß

##### Features:
- **WYSIWYG-Editor** mit Rich-Text-Funktionalität
- **Titel-Formatierung**: Größere Schrift, fett
- **Fließtext**: Georgia 12pt, 1.5 Zeilenhöhe
- **Bild-Integration**: Inline-Bilder mit Dateinamen
- **Cursor**: Blauer blinkender Cursor (#4a90e2)

##### Beispiel-Layout:
```
Aufbruch nach Europa

Es war ein kalter Morgen im März, als ich endlich den Mut fasste,
meine Koffer zu packen. Die Entscheidung hatte lange in mir gereift,
aber nun war sie da – unwiderruflich und aufregend zugleich.

[📷 Bild: alte_koffer.jpg]

Meine Mutter stand am Fenster und schaute hinaus in den Garten,
wo die ersten Krokusse ihre Köpfe durch den noch gefrorenen
Boden streckten. Sie wusste, was dieser Tag bedeutete...
```

#### Status-Bar (560x90px):
**Hintergrund**: #f8f9fa

##### Anzeige-Informationen:
- **Zeile 1**: "Wörter: 1,247 | Zeichen: 7,891 | Bilder: 1"
- **Zeile 2**: "Geschätzte Seiten: 4.2 | Letztes Speichern: vor 2 Minuten"  
- **Zeile 3**: "Schriftart: Georgia 12pt | Zeilenhöhe: 1.5"
- **Zeile 4**: "Format: 148x210mm | Ränder: L:20 R:15 O:25 U:20mm"

---

## Funktionale Spezifikationen

### Datenstruktur
```
MemoirProject
├── Metadata
│   ├── Title
│   ├── Author
│   ├── PageFormat (Width, Height)
│   ├── Margins (Left, Right, Top, Bottom)
│   └── LastSaved
├── Chapters[]
│   ├── ChapterSettings
│   │   ├── StartsOnNewPage
│   │   ├── AlwaysLeft
│   │   ├── AlwaysRight
│   │   └── EmptyLinesFromPrevious
│   ├── Title
│   ├── Blocks[]
│   │   ├── Title
│   │   ├── Content (Rich Text)
│   │   ├── Images[]
│   │   └── WordCount
│   └── EstimatedPages
└── GlobalSettings
    ├── Font
    ├── FontSize
    ├── LineHeight
    └── AutoSave
```

### Kernen Features

#### 1. Live Buch-Vorschau
- **Echte Seitendarstellung** basierend auf Seitenformat und Rändern
- **Automatische Seitenumbrüche** entsprechend dem Text-Layout
- **Navigation** durch alle Seiten mit Buttons
- **Aktualisierung** bei jeder Text-Änderung

#### 2. Hierarchische Organisation
- **Drag & Drop** für Kapitel und Blöcke
- **Verschachtelbare Struktur**: Kapitel → Blöcke
- **Visuelle Hierarchie** mit verschiedenen Farben und Einrückungen
- **Seitenzählung** pro Kapitel automatisch

#### 3. Flexibles Schreiben
- **WYSIWYG-Editor** mit Standard-Formatierungen
- **Bild-Integration** per Button mit Datei-Dialog
- **Seitenumbruch-Kontrolle** per Button
- **Live Word-Count** und Statistiken

#### 4. Layout-Kontrolle  
- **Präzise Seitenformat-Einstellung** in mm
- **Individuelle Ränder** für alle 4 Seiten
- **Kapitel-spezifische Einstellungen** für Seitenpositionierung
- **Live-Vorschau** aller Layout-Änderungen

#### 5. Projekt-Management
- **Speichern/Laden** von .memoir Dateien
- **Auto-Save** Funktionalität
- **PDF-Export** mit exakter Layout-Wiedergabe
- **Backup-System** für Datensicherheit

### Benutzer-Workflows

#### Memoir erstellen:
1. **"Neu" klicken** → Leeres Projekt
2. **Seitenformat einstellen** → Buchgröße definieren  
3. **Erstes Kapitel hinzufügen** → "+ Hinzufügen"
4. **Text schreiben** → Im Editor rechts
5. **Live-Vorschau prüfen** → Links oben
6. **Weitere Kapitel** → Hierarchisch organisieren
7. **Export** → Fertiges PDF

#### Kapitel organisieren:
1. **Kapitel auswählen** → Im Baum links unten
2. **Drag & Drop** → Reihenfolge ändern
3. **Einstellungen** → Seitenposition festlegen
4. **Blöcke hinzufügen** → Unterkapitel erstellen
5. **Live-Vorschau** → Auswirkungen sehen

#### Text formatieren:
1. **Text selektieren** → Im Editor
2. **Formatierung** → Toolbar verwenden
3. **Bilder einfügen** → "🖼️ Bild" Button
4. **Seitenumbrüche** → "📄 Seite" Button  
5. **Vorschau** → Sofortige Aktualisierung

---

## Technische Implementation (WPF)

### UI-Controls
- **Left Panel**: `Grid` mit `RowDefinition` 3:2 Verhältnis
- **Book Preview**: Custom `UserControl` mit `FlowDocumentPageViewer`
- **Chapter Tree**: `TreeView` mit Custom `TreeViewItem` Templates
- **Text Editor**: `RichTextBox` oder `TextEditor` (AvalonEdit)
- **Toolbar**: `ToolBar` mit Standard-Controls
- **Settings**: `GroupBox` mit `CheckBox` und `NumericUpDown`

### Data Binding
- **MVVM Pattern** mit `INotifyPropertyChanged`
- **ObservableCollection** für Kapitel und Blöcke
- **Commands** für alle Buttons und Aktionen
- **Two-Way Binding** für alle Einstellungen

### Drag & Drop Implementation
```csharp
// TreeView mit AllowDrop="True"
private void TreeView_Drop(object sender, DragEventArgs e)
{
    // Reorder logic für Kapitel/Blöcke
}
```

### Live Preview
```csharp
// FlowDocument Generator
public FlowDocument GeneratePreview(MemoirProject project)
{
    // Conversion von Rich Text zu FlowDocument
    // Berücksichtigung von PageSize, Margins, etc.
}
```

### File Operations
- **Serialization**: JSON oder XML für .memoir Files
- **PDF Export**: Using PdfSharp oder iTextSharp
- **Auto-Save**: Timer-basiert alle 2 Minuten

---

## Design System

### Farbpalette
- **Primary Blue**: #4a90e2 (Selections, Buttons)
- **Success Green**: #48bb78 (Add, Success states)  
- **Warning Orange**: #ed8936 (Save, Warning states)
- **Danger Red**: #e53e3e (Delete, Error states)
- **Purple**: #9f7aea (Export, Special functions)
- **Light Gray**: #f8f9fa (Backgrounds)
- **Medium Gray**: #e0e0e0 (Borders)
- **Dark Gray**: #2c3e50 (Text)

### Typography
- **Headers**: Arial, 12-14pt, Bold
- **Body Text**: Georgia, 12pt, Regular
- **UI Text**: Arial, 9-11pt, Regular
- **Line Height**: 1.5 für Fließtext

### Spacing
- **Margins**: 10-20px zwischen Hauptbereichen
- **Padding**: 5-10px innerhalb von Controls
- **Button Height**: 20-25px
- **Input Height**: 15-20px

---

## Benutzerfreundlichkeit

### Intuitive Navigation
- **Klare Hierarchie** mit visuellen Indikatoren
- **Contextuelle Buttons** nur wenn benötigt
- **Keyboard Shortcuts** für häufige Aktionen
- **Tooltips** für alle Buttons

### Visuelles Feedback
- **Live Preview** für alle Änderungen
- **Status-Informationen** kontinuierlich sichtbar  
- **Progress Indicators** für längere Operationen
- **Error Messages** klar und hilfreich

### Workflow-Optimierung
- **Auto-Save** verhindert Datenverlust
- **Drag & Drop** für schnelle Reorganisation
- **Single-Click Access** zu allen Funktionen
- **Undo/Redo** für alle Textoperationen

---

Diese Spezifikation definiert einen vollständigen, benutzerfreundlichen Memoir-Editor, der deutlich intuitiver als komplexe DTP-Programme wie Scribus ist, dabei aber professionelle Resultate liefert.