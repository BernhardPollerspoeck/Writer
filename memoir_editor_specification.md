```markdown
# Memoir Editor - Vollständige UI-Spezifikation

## Überblick
Ein intuitiver, benutzerfreundlicher Editor für die Erstellung von Memoirs mit Live-Vorschau, hierarchischer Kapitelorganisation und präziser Layoutkontrolle. Die Anwendung ist als WPF-Desktop-Anwendung konzipiert.

Wichtig: Alle in dieser Spezifikation genannten Maße sind Start- oder Design-Vorgaben. Das Fenster muss vollständig resizeable (größbar/verkleinerbar) sein und alle UI-Bereiche müssen sich dynamisch an die verfügbare Fenstergröße anpassen. Verwende prozentuale/relative Layouts (z. B. Grid mit Star-Sizing), Min-/Max-Werte und ScrollViewer, damit die Oberfläche bei allen Fenstergrößen brauchbar bleibt.

## Gesamtlayout

### Fensteraufteilung
- Standard-Startgröße: 1200x800 Pixel (dies ist nur der initiale Startwert)
- Fenster ist vollständig resizeable; alle Unterbereiche skalieren proportional bzw. adaptiv.
- Empfohlene Mindestgröße der Hauptfenster: 800x600 Pixel (um Kernfunktionen nicht zu verlieren)
- **Linke Seite**: initial ~48% Breite (Startwert 580px) — skalierbar und mit MinWidth (~320px)
- **Rechte Seite**: initial ~48% Breite (Startwert 580px) — skalierbar und mit MinWidth (~320px)
- **Trenner**: 2px vertikale Linie (visuell, flexibel in Höhe)

Hinweis zur Umsetzung: Verwende WPF-Grid-Spalten mit Star-Werten (z. B. "1*" und "1*") oder feste Startbreiten kombiniert mit proportionaler Verteilung. Setze MinWidth/MinHeight für Panels und Controls, damit die Inhalte nicht kollabieren.

---

## Linke Seite (initial 580px breit, skalierbar)

> Alle folgenden Pixelangaben sind Startwerte / Design-Vorgaben. Implementiere Layouts so, dass diese Bereiche mit der Fensterbreite und -höhe mitwachsen oder schrumpfen. Verwende relative Maße, ViewBoxen oder Zoom/Scaling bei Bedarf.

### 1. Vollansicht Buch (Oberer Bereich - empfohlen 3/5 der Höhe)
- Initial-Abmessungen (Design): 560x468px
- Bereich soll sich in der Höhe proportional zur linken Seite verändern (z. B. Grid RowDefinition mit 3* für Vorschau und 2* für Kapitel-Baum)
- **Hintergrund**: #fafafa mit #e0e0e0 Rand

#### Features:
- Zwei aufgeschlagene Seiten nebeneinander dargestellt
- Jede Seite verhält sich responsiv: Seiteninhalt skaliert oder wird mit Scrollbars/Zoom angepasst
- Live-Vorschau des aktuellen Buchinhalts; Vorschau verwendet QuestPDF für das Rendering, um exakte Übereinstimmung mit dem PDF-Export zu gewährleisten. QuestPDF generiert Seiten (PDF-Dateien/Streams), die in der Preview angezeigt werden; die Anzeige erfolgt über eine WebView (WebView2) für beste Qualität und Interaktion.
- Die Vorschau skaliert bei Fensteränderung und bietet bei kleinen Breiten ein Zoom/Skalierungsverhalten statt harten Beschnitts

#### Seiteninhalt:
- Seitenzahlen oben links (z.B. "Seite 34", "Seite 35")
- Kapitelüberschriften mit Unterstrichen
- Formatierter Fließtext in echter Buchdarstellung — Textfluss und Seitenumbrüche müssen bei Größenänderung neu berechnet werden (QuestPDF-Neugenerierung / Reflow)
- Bilder und Layout-Elemente wie im fertigen Buch

#### Navigation:
- Linker / rechter Pfeil-Button als kreisförmige, skalierbare Buttons (Design-Vorgabe: blau #4a90e2)
- Seitenzähler zentral; skaliert oder reduziert Text bei geringer Breite (z. B. "Seite 34-35 von 128" → "34-35 / 128")

### 2. Kapitel & Blöcke (Unterer Bereich - empfohlen 2/5 der Höhe)
- Initial-Abmessungen (Design): 560x280px
- Bereich ist scrollbar; die Höhe passt sich an die verfügbare Höhe der linken Seite an

#### Steuerelemente:
- "+ Hinzufügen" Button: Design-Farbe Grün (#48bb78). Größe als Startwert 50x20px, aber skalierbar bzw. in einem responsiven Layout platziert.
- "🗑️ Löschen" Button: Rot (#e53e3e), Startwert 50x20px, ebenfalls responsive.

#### Hierarchische Struktur:
- Scrollbarer Bereich: Startwert 540x235px; ScrollViewer verwendet, damit Inhalte bei kleinen Höhen zugänglich bleiben

##### Kapitel-Darstellung:
- Kapitel-Zeile: Startwert 520x25px, hellblau (#e8f4f8); Breite passt sich an Container an
- Format: "📖 Kapitel X: Titel" + Seitenzahl rechts
- Ausgewähltes Kapitel: Blau (#4a90e2) hervorgehoben

##### Block-Darstellung:
- Block-Zeile: Startwert 500x20px, eingerückt unter Kapiteln; Breite adaptiv
- Format: "📄 Blocktitel"
- Ausgewählter Block: Hellblau (#bee3f8) hervorgehoben

#### Drag & Drop:
- Kapitel und Blöcke per Drag & Drop neu anordnen; Visuals und Drop-Zonen müssen bei Größenänderung korrekt positioniert werden

#### Scrollbar:
- Designhinweis: Breite ca. 10px; optische Darstellung (Hintergrund #f5f5f5, Thumb #cbd5e0) ist unabhängig von tatsächlicher Scrollbar-Breite des Systems; implementiere via Custom ScrollViewer-Template falls nötig.

---

## Rechte Seite (initial 580px breit, skalierbar)

### 1. Projektverwaltung (Oberer Bereich)
- Initial-Abmessungen (Design): 560x80px
- Bereich soll in der Breite die verfügbare rechte Fensterhälfte füllen und in der Höhe relativ klein bleiben; bei Platzmangel kann er umschwenken oder Controls in mehrere Zeilen umbrechen
- **Hintergrund**: Weiß mit #d0d0d0 Rand

#### Datei-Operationen:
- Buttons: "Neu", "Laden", "Speichern", "Export" — Startgrößen angegeben (z. B. 40-60px Breite), sollten aber im responsiven Layout nebeneinander bleiben oder bei geringer Breite in zwei Zeilen umbrechen

#### Seitenformat-Einstellungen:
- Eingabefelder für Breite/Höhe (Startwerte 50x20px) — setze MinWidth, benutze TextBox mit numeric validation; Felder skalieren mit Container

#### Ränder-Einstellungen:
- Eingabefelder für L/R/O/U — Startwerte 35x20px; responsive Darstellung empfohlen (z. B. Grid mit Spalten, die bei Platzmangel umbrechen)

#### Status-Anzeige:
- "Auto-Save": "✓ Auto" grün (#f0fff4) — Anzeige bleibt sichtbar, passt sich aber platzbedingt an (eventuell als Icon + Tooltip)

### 2. Schreibbereich
- Initial-Abmessungen (Design): 560x690px; dieser Bereich soll den verbleibenden Platz auf der rechten Seite ausfüllen und dynamisch in Höhe und Breite skalieren
- **Hintergrund**: Weiß mit #d0d0d0 Rand

#### Header mit Kapitel-Einstellungen (Start: 560x80px):
- Hintergrund: #f8f9fa
- Inhalt reflowt bei geringerer Breite (Checkboxes/Inputs können in mehrere Reihen umbrechen oder als Dropdown/Overflow-Menü erscheinen)

##### Titel-Information:
- Haupttitel: "Schreibfenster" (fett)
- Untertitel: "Kapitel X: Titel > Aktueller Block"

##### Kapitel-Einstellungen:
- Checkboxen und Eingaben: Startgrößen als Designhinweis; komponenten sollen sich bei Resize anordnen (WrapPanel/AdaptivePanel)

#### Toolbar (Start: 560x40px):
- Hintergrund: Weiß mit #e0e0e0 Rand
- Formatierungs-Buttons, Font-Dropdowns und Spezial-Buttons sollten in einer ToolBar verbleiben und bei geringer Breite ein Overflow-Menü nutzen

##### Formatierungs-Buttons:
- B, I, U — Startgröße 30x24px, skalierbar; Icons sollen per vector graphics (Path) gerendert werden, damit sie bei Skalierung scharf bleiben

##### Schrift-Einstellungen:
- Schriftart- und -größen-Dropdowns: Startgrößen 80x24px / 50x24px, bei Platzmangel in ein Popover auslagern

##### Spezial-Buttons:
- Bild-, Seite-, Einstellungen-Buttons — Startfarben wie definiert; responsiv angezeigt

#### Text-Editor (Start: 540x430px):
- TextEditor füllt verfügbaren Raum; bei kleiner Höhe zeigt er Scrollbars
- Hintergrund: Weiß
- Der Editor und seine Inhalte (WYSIWYG) müssen beim Skalieren korrekt umfließen; Schriftgrößen bleiben konstant (oder folgen Zugriffs-Optionen wie Zoom), der Viewport passt sich

##### Features:
- WYSIWYG-Editor mit Rich-Text-Funktionalität
- Titel-Formatierung: größere Schrift, fett
- Fließtext: Georgia 12pt, 1.5 Zeilenhöhe (typische Default-Formatierung; der Editor selbst sollte optional Zoom unterstützen)
- Bild-Integration: Inline-Bilder mit Dateinamen; Bildgrößen können relativ zur Editor-Breite skaliert oder mit festen MaxWidth gebunden werden
- Cursor: Blauer blinkender Cursor (#4a90e2)

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

#### Status-Bar (Start: 560x90px):
- Hintergrund: #f8f9fa
- Die Status-Bar zeigt mehrere Zeilen mit Statistiken; bei Platzmangel sollten weniger kritische Informationen in ein Tooltip oder ein ausklappbares Panel verschoben werden

##### Anzeige-Informationen:
- Zeile 1: "Wörter: 1,247 | Zeichen: 7,891 | Bilder: 1"
- Zeile 2: "Geschätzte Seiten: 4.2 | Letztes Speichern: vor 2 Minuten"
- Zeile 3: "Schriftart: Georgia 12pt | Zeilenhöhe: 1.5"
- Zeile 4: "Format: 148x210mm | Ränder: L:20 R:15 O:25 U:20mm"

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
- QuestPDF-basiertes Rendering für Preview und Export: Die Live-Vorschau nutzt QuestPDF, um Seiten so zu rendern, wie sie im finalen PDF erscheinen. Dadurch ist die Vorschau layout-identisch zum Export.
- Echte Seitendarstellung basierend auf Seitenformat und Rändern
- Automatische Seitenumbrüche entsprechend dem Text-Layout (QuestPDF-Layout-Pass)
- Navigation durch alle Seiten mit Buttons
- Aktualisierung bei jeder Text-Änderung (inkrementelles Rendering / Debounce + Hintergrund-Render-Queue empfohlen)

#### 2. Hierarchische Organisation
- Drag & Drop für Kapitel und Blöcke
- Verschachtelbare Struktur: Kapitel → Blöcke
- Visuelle Hierarchie mit verschiedenen Farben und Einrückungen
- Seitenzählung pro Kapitel automatisch

#### 3. Flexibles Schreiben
- WYSIWYG-Editor mit Standard-Formatierungen
- Bild-Integration per Button mit Datei-Dialog
- Seitenumbruch-Kontrolle per Button
- Live Word-Count und Statistiken

#### 4. Layout-Kontrolle  
- Präzise Seitenformat-Einstellung in mm
- Individuelle Ränder für alle 4 Seiten
- Kapitel-spezifische Einstellungen für Seitenpositionierung
- Live-Vorschau aller Layout-Änderungen

#### 5. Projekt-Management
- Speichern/Laden von .memoir Dateien
- Auto-Save Funktionalität
- PDF-Export mit exakter Layout-Wiedergabe
- Backup-System für Datensicherheit

### Benutzer-Workflows

#### Memoir erstellen:
1. "Neu" klicken → Leeres Projekt
2. Seitenformat einstellen → Buchgröße definieren  
3. Erstes Kapitel hinzufügen → "+ Hinzufügen"
4. Text schreiben → Im Editor rechts
5. Live-Vorschau prüfen → Links oben
6. Weitere Kapitel → Hierarchisch organisieren
7. Export → Fertiges PDF

#### Kapitel organisieren:
1. Kapitel auswählen → Im Baum links unten
2. Drag & Drop → Reihenfolge ändern
3. Einstellungen → Seitenposition festlegen
4. Blöcke hinzufügen → Unterkapitel erstellen
5. Live-Vorschau → Auswirkungen sehen

#### Text formatieren:
1. Text selektieren → Im Editor
2. Formatierung → Toolbar verwenden
3. Bilder einfügen → "🖼️ Bild" Button
4. Seitenumbrüche → "📄 Seite" Button  
5. Vorschau → Sofortige Aktualisierung

---

## Technische Implementation (WPF)

Hinweis: Wir verwenden in der Anwendung volle Dependency Injection (DI) über Host.CreateApplicationBuilder und das MVVM Community Toolkit für ViewModel-Pattern, Source-Generatoren (ObservableObject, RelayCommand, usw.) und Messaging. Zusätzlich ist ein finaler PDF-Export vorgesehen (QuestPDF wird für Layout/Export verwendet; siehe Rendering/Live Preview).

### Dependency Injection & Host
- Nutze das Generic Host (Microsoft.Extensions.Hosting) für die App-Lebenszeit und Service-Registrierung.
- Initialisierung (Beispiel in App.xaml.cs / Program.cs):
```csharp
// Program.cs oder App.xaml.cs (Startup)
var builder = Host.CreateApplicationBuilder(args);

// Services registrieren
builder.Services.AddSingleton<IWindowManager, WindowManager>();
builder.Services.AddSingleton<MainWindow>();
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddTransient<EditorViewModel>();
builder.Services.AddTransient<ChapterTreeViewModel>();
builder.Services.AddScoped<IProjectRepository, FileProjectRepository>();
builder.Services.AddSingleton<IQuestPdfRenderer, QuestPdfRenderer>(); // QuestPDF wrapper
builder.Services.AddSingleton<IRenderQueue, RenderQueueService>(); // Background rendering
builder.Services.AddSingleton<IWebViewService, WebViewService>(); // WebView2 wrapper for preview
// Weitere Services: IImageService, IExportService, IAutoSaveService, IConfiguration, ILogger, etc.

var host = builder.Build();

// Anwendung starten und DI-gestützte MainWindow-Instanz verwenden
var mainWindow = host.Services.GetRequiredService<MainWindow>();
mainWindow.Show();
```

- Setze sinnvolle Service-Lifetimes:
  - Singleton: UI-fähige Services, Cache, Konfiguration, Renderer-Manager
  - Scoped/Transient: kurzlebige ViewModels oder Operationen, je nach Bedarf
- Stelle sicher, dass Services thread-safe sind, wenn sie aus Background-Tasks (Rendering/Export) verwendet werden.

### MVVM — CommunityToolkit.Mvvm
- Verwende CommunityToolkit.Mvvm (Source Generators) für:
  - ObservableObject / ObservableRecipient (INotifyPropertyChanged-Generatoren)
  - [ObservableProperty] Attributes
  - RelayCommand / AsyncRelayCommand
  - WeakReferenceMessenger (für entkoppelte UI-Kommunikation z. B. Render-Status)
- Beispiel-ViewModel:
```csharp
public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private MemoirProject currentProject;

    public IAsyncRelayCommand RenderPreviewCommand { get; }

    public MainViewModel(IQuestPdfRenderer renderer)
    {
        RenderPreviewCommand = new AsyncRelayCommand(async () => await renderer.RenderPreviewAsync(CurrentProject));
    }
}
```
- Registriere ViewModels im DI-Container (siehe oben) und löse sie via constructor-injection in Views oder Window-Initialisierung.

### Rendering & Live Preview (QuestPDF + WebView2)
- QuestPDF ist die primäre Layout-Engine — sowohl für die Live-Vorschau als auch für den finalen PDF-Export. Dadurch bleiben Preview und Export layout-identisch.
- Anzeige-Strategie:
  - Erzeuge das PDF mit QuestPDF (in-memory byte[] oder temporäre Datei).
  - Lade das PDF ausschließlich in eine WebView (empfohlen: WebView2 für WPF) zur Darstellung in der UI. Die WebView bietet Zoom, Scroll, Textauswahl, Suche und eine qualitativ hochwertige Darstellung ohne manuelles Rasterisieren.
  - Hinweis: die Anwendung setzt auf WebView2-Verfügbarkeit; WebView2 Runtime (Edge Chromium) ist systemvoraussetzung und sollte in der Installations-/Systemanforderungsdokumentation vermerkt werden.
  - Diese Implementierung bietet konsistente Vektor-/Textdarstellung, native PDF-Viewer-Funktionen und geringere CPU-Last im Vergleich zu Pixel-Rasterisierung.
- Keine Fallback-Rasterisierung: Die Preview verwendet ausschließlich WebView2; die Annahme ist, dass moderne WPF-Umgebungen WebView2 zur Verfügung stellen. Rasterisierung/Bitmap-Rendering wird nicht als primärer oder sekundärer Pfad verwendet.

### Architektur-Komponenten
- IQuestPdfRenderer (Service) baut QuestPDF-Dokumente aus dem MemoirProject und liefert PDF-Bytearrays oder Streams.
- IWebViewService (Service) kapselt das Laden eines PDFs in die WebView2-Control (temp file / data URL / stream) und verwaltet Cleanup und Sicherheit.
- IRenderQueue / RenderQueueService: verwaltet Hintergrund-Rendering-Aufträge (Debounce, Priorisierung, Caching).
- Caching: Temp-PDF-Pfad-Management, Thumbnail-Cache (falls gewünscht), LRU-Strategie für RAM/NVRAM-Beschränkungen.

### Beispiel-Workflow: Preview via WebView2
1. Nutzer editiert Text → ViewModel sendet Render-Request an RenderQueue (debounced).
2. RenderQueue ruft IQuestPdfRenderer auf, erhält byte[] pdf.
3. IWebViewService schreibt pdf in temporäre Datei (oder stellt stream-basierten Zugriff bereit) und lädt sie in die WebView2-Control via NavigateToLocalStreamUri oder Navigate method.
4. WebView zeigt PDF; bei nachfolgenden Änderungen wird nur die geänderte PDF-Version neu geladen; vorherige Temp-Dateien werden gelöscht/überschrieben.

### Codebeispiel: PDF in temporäre Datei schreiben und in WebView2 laden
```csharp
// IQuestPdfRenderer erzeugt byte[] pdfBytes
var pdfBytes = await questPdfRenderer.GeneratePdfAsync(project);

// Pfad erzeugen
var tempPath = Path.Combine(Path.GetTempPath(), $"memoir_preview_{Guid.NewGuid()}.pdf");
await File.WriteAllBytesAsync(tempPath, pdfBytes);

// WebView2 laden (auf UI-Thread)
webView.CoreWebView2.Navigate(new Uri(tempPath).AbsoluteUri);

// WebViewService kann zusätzliche Maßnahmen implementieren:
// - sichere Temp-File-Bereinigung
// - Content-Security-Policy / local file restrictions
// - Stream-Handling via WebResourceRequested falls benötigt
```

Hinweis: WebView2 bietet APIs zum Navigieren zu Dateien, Streams oder Data-URIs. Für in-memory-Streaming ohne Disk kannst du WebResourceRequested-Handler nutzen, die PDF-Bytes zurückliefern, aber das ist komplexer. Temp-Files sind einfacher und robust. Achte auf Cleanup und Berechtigungen.

### Performance & UX-Hinweise
- Rendering in Hintergrund-Threads (Task / ThreadPool).
- Debounce-Strategie: bei Textänderungen 300–800ms Verzögerung, um unnötiges Re-Rendering zu vermeiden.
- Für sehr interaktive Szenarien (z. B. während tippen) kann ein leichter, schneller Inline-Preview (FlowDocument) zusätzlich angeboten werden; echter WYSIWYG-Preview bleibt QuestPDF → WebView2.
- Thumbnail-Generierung für Miniatur-Ansichten kann über PDF-to-image Tools erfolgen, falls benötigt; die Live-Preview selbst bleibt WebView-basiert.
- Cleanup: temporäre PDF-Dateien nach einer gewissen Zeit oder OnExit löschen; Cache-Größen begrenzen.

### Bibliotheken
- QuestPDF für Layout/PDF-Generierung.
- Microsoft.Web.WebView2 (WebView2) für PDF-Anzeige in WPF.
- Optional: PdfiumSharp / SkiaSharp nur für spezielle Thumbnail-Anforderungen (nicht für die Live-Preview).
- Microsoft.Extensions.Hosting, Microsoft.Extensions.DependencyInjection, CommunityToolkit.Mvvm für Infrastruktur/MVVM.

### PDF-Export (Ende-zu-Ende)
- PDF-Export ist ausdrücklich unterstützt und verwendet QuestPDF als Engineschicht. Export läuft idealerweise als Background-Task mit Progress-Reporting und unterstützt "Save As" und optionales Streaming auf Disk.
- ExportService koordiniert QuestPDF-Generierung und Date-System-Operationen. Nach Export kann dieselbe Datei in der WebView2-Preview geöffnet werden.

### Services & Infrastruktur
- IAutoSaveService: Timer-basierte Speicherung, DI-registriert, konfigurierbar (z. B. 2 Minuten).
- IProjectRepository: Implementierungen für Dateibasierte Speicherung (.memoir JSON/XML) und mögliche Cloud-Backups.
- IExportService: Koordiniert QuestPDF-Generierung und dateibezogene Speicherung.
- IRenderQueue / IQuestPdfRenderer: Koordiniert Live-Rendering, Caching und Rasterisierung.
- IWebViewService: kapselt WebView2-Interaktionen, Temp-File-Handling, Fallback-Mechanismen (falls zukünftig benötigt).

### Threading, Logging & Fehlerbehandlung
- Verwende Microsoft.Extensions.Logging (serielle DI-Registrierung) für konsistente Logs.
- Auslagerung CPU-intensiver Tasks (Rendering, Export) in Hintergrund-Threads; sensible UI-Updates via Dispatcher.
- Fehler-Reporting/Telemetry optional als konfigurierbaren Service (z. B. Sentry/Serilog).
- WebView-spezifische Fehler (Runtime fehlt, Navigationsfehler) müssen erkannt und dem Nutzer klar kommuniziert werden (z. B. Installationshinweis); in der Annahme, dass WebView2 verfügbar ist, ist dies jedoch ein seltener Pfad.

### Empfehlungen zur Umsetzung
- Bevorzuge WebView2 für Preview; dokumentiere die Runtime-Anforderung (Edge Chromium) in der Installations- bzw. Systemanforderungsdoku.
- Teste Preview vs. Export auf verschiedenen DPI- und Zoom-Stufen, um Konsistenz sicherzustellen.
- Schreibe Integrationstests für Export-Path (QuestPDF-Integration) — verifiziere, dass Preview und Export layout-identisch sind.
- Dokumentiere Service-Schnittstellen und Lifecycle-Entscheidungen im Code-Repository.

---
Zusammenfassung: Die Anwendung nutzt Host.CreateApplicationBuilder für vollständige DI, registriert ViewModels/Services via Microsoft.Extensions.DependencyInjection, verwendet CommunityToolkit.Mvvm für das MVVM-Pattern. QuestPDF bleibt zentrale Engine für Layout/Export, die Live-Preview wird ausschließlich per WebView2 angezeigt (Temp-PDF / Stream); Export wird also ausdrücklich unterstützt und sollte als Background-Operation mit Progress-/Cancel-Handling implementiert werden.
```
