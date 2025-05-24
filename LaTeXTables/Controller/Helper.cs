using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace LaTeXTables.Controller
{
    // --- Hilfsklasse zur Schriftartenverwaltung ---
    public static class FontHelper
    {
        // --- Lädt eine Schriftart direkt aus einer .ttf-Datei ---
        // Parameter:
        // - path: Pfad zur .ttf-Datei
        // - size: Schriftgröße in Punkten
        // - style: Schriftstil (Standard: Regular)
        public static Font LadeAusDatei(string path, float size, FontStyle style = FontStyle.Regular)
        {
            var fonts = new PrivateFontCollection();           // Container für Schriftarten erstellen
            fonts.AddFontFile(path);                           // Schriftdatei laden
            return new Font(fonts.Families[0], size, style);   // Font-Objekt zurückgeben
        }

        // --- Lädt eine Schriftart aus dem "Resources"-Verzeichnis der App ---
        // Beispiel: "OpenSans-Light" wird als "Resources/OpenSans-Light.ttf" erwartet
        public static Font HoleSchrift(string fontNameWithoutExtension, float size, FontStyle style = FontStyle.Regular)
        {
            string fileName = fontNameWithoutExtension + ".ttf"; // Dateinamen zusammensetzen
            string path = Path.Combine(Application.StartupPath, "Resources", fileName);

            if (!File.Exists(path)) // Prüfen, ob Datei existiert
                throw new FileNotFoundException("Font-Datei nicht gefunden: " + path);

            return LadeAusDatei(path, size, style); // Font laden
        }

        // --- Gibt die Standardschriftart zurück ---
        // Standard ist: OpenSans-Light.ttf aus dem Resources-Ordner
        public static Font HoleStandardSchrift(float size)
        {
            return HoleSchrift("OpenSans-Light", size);
        }
    }
}