using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaTeXTables.Controller
{
    public static class LatexCode
    {
        // Erzeugt aus einer Tabelle (Liste von Listen mit Strings) den LaTeX-Code als Text
        // Mit tabulary für dynamische Spaltenbreiten und automatische Zeilenumbrüche
        public static string GenerateTable(List<List<string>> tableData, string borderStyle = "Komplettes Gitter")
        {
            if (tableData == null || tableData.Count == 0)
                return "";

            int columnCount = 0;
            // Maximalanzahl der Spalten bestimmen (kann je Zeile variieren)
            foreach (var row in tableData)
            {
                if (row.Count > columnCount)
                    columnCount = row.Count;
            }

            var sb = new StringBuilder();

            // Paket-Hinweise für LaTeX-Dokument (sollte in Präambel stehen)
            sb.AppendLine("% Bitte in der LaTeX-Präambel einfügen:");
            sb.AppendLine("% \\usepackage{tabulary}");
            sb.AppendLine("% \\usepackage[table]{xcolor}");
            sb.AppendLine();

            string columnDefs;

            switch (borderStyle)
            {
                case "Kein Rahmen":
                    // Alle Spalten linksbündig, keine Linien
                    columnDefs = string.Concat(Enumerable.Repeat("L", columnCount));
                    break;

                case "Nur äußerer Rahmen":
                    // Außen Linien, innen keine
                    columnDefs = "|" + string.Concat(Enumerable.Repeat("L", columnCount)) + "|";
                    break;

                case "Zwischen Kopfzeile und erster Spalte":
                    // Außen Linien + Linien nach jeder Spalte
                    columnDefs = "|" + string.Concat(Enumerable.Repeat("L|", columnCount));
                    break;

                default: // Komplettes Gitter
                    // Linien innen + außen
                    columnDefs = string.Concat(Enumerable.Repeat("|L", columnCount)) + "|";
                    break;
            }

            // Beginn der tabulary-Umgebung mit Spaltendefinitionen
            sb.AppendLine("\\begin{tabulary}{\\textwidth}{" + columnDefs + "}"); // Tabellenbreite auf \textwidth begrenzt

            if (borderStyle == "Kein Rahmen")
            {
                // keine Linien
            }
            else if (borderStyle == "Nur äußerer Rahmen")
            {
                sb.AppendLine("\\hline");
            }
            else
            {
                sb.AppendLine("\\hline");
                sb.AppendLine("\\rowcolor{gray!20}"); // erste Zeile grau hinterlegt
            }

            // Jede Tabellenzeile durchlaufen
            for (int i = 0; i < tableData.Count; i++)
            {
                var row = tableData[i];
                for (int j = 0; j < columnCount; j++)
                {
                    // Inhalt der Zelle (evtl. leer)
                    string cell = j < row.Count ? EscapeLatex(row[j]) : ""; // EscapeLatex macht Sonderzeichen LaTeX-kompatibel

                    // Erste Spalte farbig bei Kopfzellen-Hervorhebung (außer bei "Kein Rahmen" oder "Nur äußerer Rahmen")
                    if (borderStyle != "Kein Rahmen" && borderStyle != "Nur äußerer Rahmen" && j == 0)
                        sb.Append("\\cellcolor{gray!20} ");

                    sb.Append(cell);

                    // & als Zellentrenner, außer nach der letzten Spalte
                    if (j < columnCount - 1)
                        sb.Append(" & ");
                }

                // Zeilenende in LaTeX
                sb.Append(" \\\\");
                sb.AppendLine();

                // Horizontale Linie unter jeder Zeile, je nach Rahmentyp
                if (borderStyle == "Kein Rahmen")
                {
                    // keine Linie
                }
                else if (borderStyle == "Nur äußerer Rahmen")
                {
                    if (i == tableData.Count - 1)
                        sb.AppendLine("\\hline"); // nur untere Linie
                }
                else
                {
                    sb.AppendLine("\\hline");
                }
            }

            // Ende der tabulary-Umgebung
            sb.AppendLine("\\end{tabulary}");

            return sb.ToString();
        }


        // Ersetzt Sonderzeichen latex-kompatibel (aus & --> \&)
        private static string EscapeLatex(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input
                .Replace(@"\", @"\textbackslash ")
                .Replace("&", @"\&")
                .Replace("%", @"\%")
                .Replace("$", @"\$")
                .Replace("#", @"\#")
                .Replace("_", @"\_")
                .Replace("{", @"\{")
                .Replace("}", @"\}")
                .Replace("~", @"\textasciitilde ")
                .Replace("^", @"\textasciicircum ");
        }
    }
}
