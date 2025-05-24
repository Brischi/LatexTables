using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaTeXTables.Controller
{
    public static class LatexCode
    {
        // --- Erstellt den LaTeX-Code für die Tabelle basierend auf Daten und Stiloption ---
        public static string ErzeugeTabelle(List<List<string>> tableData, string borderStyle = "Komplettes Gitter")
        {
            // Wenn keine Daten vorhanden sind, abbrechen
            if (tableData == null || tableData.Count == 0)
                return "";

            // --- Spaltenanzahl bestimmen (für ungleichmäßige Zeilen) ---
            int columnCount = tableData.Max(row => row.Count);

            var sb = new StringBuilder();

            // --- Hinweise für LaTeX-Präambel ---
            sb.AppendLine("% In der Präambel einfügen:");
            sb.AppendLine("% \\usepackage{tabulary}");
            sb.AppendLine("% \\usepackage[table]{xcolor}");
            sb.AppendLine();

            // --- Spaltendefinition je nach Rahmenstil ---
            string columnDefs;
            switch (borderStyle)
            {
                case "Kein Rahmen":
                    columnDefs = string.Concat(Enumerable.Repeat("L", columnCount));
                    break;

                case "Nur äußerer Rahmen":
                    columnDefs = "|" + string.Concat(Enumerable.Repeat("L", columnCount)) + "|";
                    break;

                case "Zwischen Kopfzeile und erster Spalte":
                    columnDefs = "|" + string.Concat(Enumerable.Repeat("L|", columnCount));
                    break;

                default: // "Komplettes Gitter"
                    columnDefs = string.Concat(Enumerable.Repeat("|L", columnCount)) + "|";
                    break;
            }

            // --- LaTeX-Beginn der Tabelle ---
            sb.AppendLine("\\begin{tabulary}{\\textwidth}{" + columnDefs + "}");

            // --- Kopfzeile gestalten je nach Stil ---
            if (borderStyle != "Kein Rahmen")
            {
                sb.AppendLine("\\hline");
                if (borderStyle != "Nur äußerer Rahmen")
                    sb.AppendLine("\\rowcolor{gray!20}"); // Kopfzeile grau
            }

            // --- Datenzeilen erzeugen ---
            for (int i = 0; i < tableData.Count; i++)
            {
                var row = tableData[i];

                for (int j = 0; j < columnCount; j++)
                {
                    // Zelleninhalt lesen oder leeren String verwenden
                    string cell = j < row.Count ? ErsetzeSonderzeichen(row[j]) : "";

                    // Erste Spalte hervorheben (wenn Stil es vorsieht)
                    if (borderStyle != "Kein Rahmen" && borderStyle != "Nur äußerer Rahmen" && j == 0)
                        sb.Append("\\cellcolor{gray!20} ");

                    sb.Append(cell);

                    // Spaltentrenner (&), außer nach letzter Spalte
                    if (j < columnCount - 1)
                        sb.Append(" & ");
                }

                sb.Append(" \\\\"); // LaTeX-Zeilenumbruch
                sb.AppendLine();

                // Nach jeder Zeile eine Linie, je nach Stil
                if (borderStyle == "Nur äußerer Rahmen")
                {
                    if (i == tableData.Count - 1)
                        sb.AppendLine("\\hline"); // nur am Ende
                }
                else if (borderStyle != "Kein Rahmen")
                {
                    sb.AppendLine("\\hline");
                }
            }

            // --- LaTeX-Ende der Tabelle ---
            sb.AppendLine("\\end{tabulary}");

            return sb.ToString();
        }

        // --- Wandelt Sonderzeichen in LaTeX-kompatible Zeichen um ---
        private static string ErsetzeSonderzeichen(string input)
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
