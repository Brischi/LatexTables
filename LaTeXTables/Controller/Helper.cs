
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace LaTeXTables.Controller
{
    public static class FontHelper
    {
        // Lädt eine Schriftart direkt von einer .ttf-Datei vom angegebenen Pfad
        // size = Schriftgröße in Punkten
        // style = z. B. Regular, Bold, Italic
        public static Font LadeAusDatei(string path, float size, FontStyle style = FontStyle.Regular)
        {
            var fonts = new PrivateFontCollection();          // Font-Speichercontainer anlegen
            fonts.AddFontFile(path);                          // Schriftdatei laden
            return new Font(fonts.Families[0], size, style);  // Font erzeugen und zurückgeben
        }

        // Lädt eine Schriftart mit dem Dateinamen (ohne .ttf-Endung) aus dem Ausgabeverzeichnis
        // z. B. "OpenSans-Light" wird zu "OpenSans-Light.ttf"
        public static Font HoleSchrift(string fontNameWithoutExtension, float size, FontStyle style = FontStyle.Regular)
        {
            string fileName = fontNameWithoutExtension + ".ttf";                          // Endung ergänzen
            string path = Path.Combine(Application.StartupPath, "Resources", fileName);

            if (!File.Exists(path))                                                      // Datei vorhanden?
                throw new FileNotFoundException("Font-Datei nicht gefunden: " + path);   // sonst Fehler

            return LadeAusDatei(path, size, style);                                       // Font laden
        }

        // Liefert standardmäßig die Schriftart "OpenSans-Light.ttf"
        public static Font HoleStandardSchrift(float size)
        {
            return HoleSchrift("OpenSans-Light", size);  // Standard-Schriftart laden
        }
    }
}


//using System;
//using System.Drawing;
//using System.Drawing.Text;
//using System.IO;
//using System.Windows.Forms;

//namespace LaTeXTables.Controller
//{
//    public static class FontHelper
//    {
//        // Lädt eine Schriftart direkt von einer .ttf-Datei vom angegebenen Pfad
//        // size = Schriftgröße in Punkten
//        // style = z. B. Regular, Bold, Italic
//        public static Font LadeAusDatei(string path, float size, FontStyle style = FontStyle.Regular)
//        {
//            var fonts = new PrivateFontCollection();          // Font-Speichercontainer anlegen
//            fonts.AddFontFile(path);                          // Schriftdatei laden
//            return new Font(fonts.Families[0], size, style);  // Font erzeugen und zurückgeben
//        }

//        // Lädt eine Schriftart mit dem Dateinamen (ohne .ttf-Endung) aus dem Ausgabeverzeichnis
//        // z. B. "OpenSans-Light" wird zu "OpenSans-Light.ttf"
//        public static Font HoleSchrift(string fontNameWithoutExtension, float size, FontStyle style = FontStyle.Regular)
//        {
//            string fileName = fontNameWithoutExtension + ".ttf";                          // Endung ergänzen
//            string path = Path.Combine(Application.StartupPath, "Resources", fileName);

//            if (!File.Exists(path))                                                      // Datei vorhanden?
//                throw new FileNotFoundException("Font-Datei nicht gefunden: " + path);   // sonst Fehler

//            return LadeAusDatei(path, size, style);                                       // Font laden
//        }

//        // Liefert standardmäßig die Schriftart "OpenSans-Light.ttf"
//        public static Font HoleStandardSchrift(float size)
//        {
//            return HoleSchrift("OpenSans-Light", size);  // Standard-Schriftart laden
//        }
//    }
//}
