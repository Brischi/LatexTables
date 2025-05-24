using System.Collections.Generic;

namespace LaTeXTables.Model
{
    // --- Modellklasse: Speichert alle Daten der Tabelle als verschachtelte Liste ---
    public class TableModel
    {
        // Haupt-Datenstruktur: Jede innere Liste ist eine Tabellenzeile
        public List<List<string>> TableData { get; private set; }

        // --- Konstruktor: Initialisiert Tabelle mit gegebener Zeilen- und Spaltenanzahl ---
        public TableModel(int rows, int columns)
        {
            TableData = new List<List<string>>();

            for (int i = 0; i < rows; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < columns; j++)
                    row.Add(""); // Leere Zellen einfügen
                TableData.Add(row);
            }
        }

        // Anzahl der Zeilen in der Tabelle
        public int RowCount => TableData.Count;

        // Anzahl der Spalten (anhand der ersten Zeile ermittelt)
        public int ColumnCount => TableData.Count > 0 ? TableData[0].Count : 0;

        // --- Fügt eine neue Zeile am Ende hinzu ---
        public void AddRow()
        {
            var newRow = new List<string>();

            for (int i = 0; i < ColumnCount; i++)
                newRow.Add(""); // Leere Spaltenzellen

            TableData.Add(newRow);
        }

        // --- Fügt eine neue Spalte zu jeder vorhandenen Zeile hinzu ---
        public void AddColumn()
        {
            foreach (var row in TableData)
                row.Add(""); // Leere Zelle ans Zeilenende anhängen
        }

        // --- Gibt den Inhalt einer bestimmten Zelle zurück ---
        public string GetValue(int row, int col)
        {
            if (row < RowCount && col < ColumnCount)
                return TableData[row][col];

            return ""; // Ungültiger Index → leeren String zurückgeben
        }

        // --- Setzt den Inhalt einer bestimmten Zelle ---
        public void SetValue(int row, int col, string value)
        {
            if (row < RowCount && col < ColumnCount)
                TableData[row][col] = value;
        }
    }
}
