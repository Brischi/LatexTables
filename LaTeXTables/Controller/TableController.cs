using LaTeXTables.Model;
using LaTeXTables.View;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LaTeXTables.Controller
    {
        // --- Controller: Steuert Logik zwischen UI (View) und Datenmodell (Model) ---
        public class TableController
        {
        // --- Referenzen auf GUI-Komponenten und Datenmodell ---
            private readonly Font ralewayFont;
            private readonly TableLayoutPanel tablePanel;       // Tabelle in der Oberfläche
            private readonly Form parentForm;                   // Hauptformular (für Farben etc.)
            private readonly TableModel tableModel;             // Datenmodell
            private readonly Button btnGenerateLatex;           // Button zur LaTeX-Erzeugung

        // --- Konstruktor: Verbindet View, Model und Button ---
        public TableController(TableLayoutPanel panel, Form form, TableModel model, Button generateButton)
        {
            tablePanel = panel;
            parentForm = form;
            tableModel = model;
            btnGenerateLatex = generateButton;

            // Raleway-Schriftart vorbereiten
            ralewayFont = FontHelper.HoleSchrift("Raleway-SemiBold", 14f);
        }

        // --- Fügt eine neue Spalte zur Tabelle und zum Model hinzu ---
        public void AddColumn()
            {
                int neueSpalte = tablePanel.ColumnCount;

                tableModel.AddColumn(); // Datenmodell aktualisieren

                // Alten "+ Spalte"-Button entfernen
                foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                    if (ctrl.Text == "+ Spalte")
                        tablePanel.Controls.Remove(ctrl);

                tablePanel.ColumnCount++;

                // Spalten gleichmäßig verteilen
                tablePanel.ColumnStyles.Clear();
                for (int i = 0; i < tablePanel.ColumnCount; i++)
                    tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / tablePanel.ColumnCount));

                // Neuen "+ Spalte"-Button hinzufügen
                var btn = new Button
                {
                    Text = "+ Spalte",
                    Dock = DockStyle.Fill,
                    Font = ralewayFont
                };
                btn.Click += (s, e) => AddColumn();
                    tablePanel.Controls.Add(btn, neueSpalte, 0);

                    AddMissingTextBoxes();
                    UpdateTabOrder();
                    PlaceGenerateButton();
            }

            // --- Fügt eine neue Zeile zur Tabelle und zum Model hinzu ---
            public void AddRow()
            {
                int neueZeile = tablePanel.RowCount;

                tableModel.AddRow(); // Datenmodell aktualisieren

                // Alten "+ Zeile"-Button entfernen
                foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                    if (ctrl.Text == "+ Zeile")
                        tablePanel.Controls.Remove(ctrl);

                tablePanel.RowCount++;

                // Zeilen gleichmäßig verteilen
                tablePanel.RowStyles.Clear();
                for (int i = 0; i < tablePanel.RowCount; i++)
                    tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / tablePanel.RowCount));

                // Neuen "+ Zeile"-Button hinzufügen
                var btn = new Button
                {
                    Text = "+ Zeile",
                    Dock = DockStyle.Fill,
                    Font = ralewayFont
                };
                btn.Click += (s, e) => AddRow();
                    tablePanel.Controls.Add(btn, 0, neueZeile);

                    AddMissingTextBoxes();
                    UpdateTabOrder();
                    PlaceGenerateButton();
            }

            // --- Fügt alle noch fehlenden Textboxen an leere Stellen in der Tabelle ein ---
            public void AddMissingTextBoxes()
            {
                for (int row = 0; row < tablePanel.RowCount - 1; row++)     // letzte Zeile = Buttons
                {
                    for (int col = 0; col < tablePanel.ColumnCount - 1; col++) // letzte Spalte = Buttons
                    {
                        if (tablePanel.GetControlFromPosition(col, row) == null)
                        {
                            var tb = new TableTextBox
                            {
                                BackColor = parentForm.BackColor,
                                ForeColor = Color.Black
                            };
                            tablePanel.Controls.Add(tb, col, row);
                        }
                    }
                }
            }

            // --- Aktualisiert die Reihenfolge der Tastennavigation (Tabulator-Sprungreihenfolge) ---
            public void UpdateTabOrder()
            {
                int tabIndex = 0;

                var controls = tablePanel.Controls.Cast<Control>()
                    .Where(c => c.TabStop && c.CanFocus)
                    .Select(c => new
                    {
                        Control = c,
                        Position = tablePanel.GetPositionFromControl(c)
                    })
                    .OrderBy(x => x.Position.Row)
                    .ThenBy(x => x.Position.Column)
                    .ToList();

                foreach (var c in controls)
                    c.Control.TabIndex = tabIndex++;
            }

            // --- Platziert den LaTeX-Erzeugen-Button in die unterste rechte Ecke ---
            public void PlaceGenerateButton()
            {
                tablePanel.Controls.Remove(btnGenerateLatex);
                tablePanel.Controls.Add(btnGenerateLatex, tablePanel.ColumnCount - 1, tablePanel.RowCount - 1);
            }

            // --- Überträgt aktuelle Eingaben aus den Textboxen ins Datenmodell ---
            public void UpdateModelFromView()
            {
                for (int row = 0; row < tableModel.RowCount; row++)
                {
                    for (int col = 0; col < tableModel.ColumnCount; col++)
                    {
                        var control = tablePanel.GetControlFromPosition(col, row);
                        if (control is TextBox tb)
                            tableModel.SetValue(row, col, tb.Text);
                    }
                }
            }
        }
    }
