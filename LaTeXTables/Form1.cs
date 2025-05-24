// Namespace und Abhängigkeiten
using LaTeXTables.Controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LaTeXTables.Properties;

namespace LaTeXTables
{
    public partial class Form1 : Form
    {
        // GUI-Komponenten
        private TableLayoutPanel tablePanel;
        private TextBox outputTextBox;
        private Button btnGenerateLatex;
        private ComboBox cbBorderStyle;

        private Panel panelSettings;
        private Panel panelTableContainer;

        // Konstruktor der Form
        public Form1()
        {
            InitializeComponent();
            this.Font = FontHelper.GetDefault(12f); // Setzt Standardschrift
            SetupUI(); // Initialisiert die Benutzeroberfläche
        }

        private void SetupUI()
        {
            // Hauptlayout mit drei vertikalen Bereichen: oben Einstellungen, Mitte Tabelle, unten Ausgabe
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                AutoSize = false
            };

            // Prozentuale Aufteilung des vertikalen Layouts
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));  // 10 % oben (Einstellungen)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60f));  // 60 % Mitte (Tabelle)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30f));  // 30 % unten (LaTeX-Code-Ausgabe)

            // Panel für Einstellungsbereich (z. B. Dropdown)
            panelSettings = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Dropdown für Rahmen-Stil
            cbBorderStyle = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300,
                Left = 5,
                Top = 5
            };
            cbBorderStyle.Items.AddRange(new string[]
            {
                "Kein Rahmen",
                "Nur äußerer Rahmen",
                "Zwischen Kopfzeile und erster Spalte",
                "Komplettes Gitter"
            });
            cbBorderStyle.SelectedIndex = 3; // Standardwert
            cbBorderStyle.SelectedIndexChanged += (s, e) => GenerateLatexAndUpdateOutput();

            panelSettings.Controls.Add(cbBorderStyle);

            // Panel für Tabelle
            panelTableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Initiale Tabelle mit 2x2 Feldern
            tablePanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            // Spalten- und Zeilenformatierung
            for (int i = 0; i < 2; i++)
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            for (int i = 0; i < 2; i++)
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            // Erstes Eingabefeld
            var tb = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = this.BackColor,
                ForeColor = Color.Black,
                Multiline = true,
                AcceptsReturn = true,
                WordWrap = true
            };
            tablePanel.Controls.Add(tb, 0, 0);

            // Button zum Hinzufügen einer Spalte
            var btnCol = new Button { Text = "+ Spalte", Dock = DockStyle.Fill };
            btnCol.Click += btnAddColumn_Click;
            tablePanel.Controls.Add(btnCol, 1, 0);

            // Button zum Hinzufügen einer Zeile
            var btnRow = new Button { Text = "+ Zeile", Dock = DockStyle.Fill };
            btnRow.Click += btnAddRow_Click;
            tablePanel.Controls.Add(btnRow, 0, 1);

            // Button zur LaTeX-Ausgabe
            btnGenerateLatex = new Button { Text = "LaTeX-Tabelle erzeugen", Dock = DockStyle.Fill };
            btnGenerateLatex.Click += BtnGenerateLatex_Click;
            btnGenerateLatex.TabStop = false;
            tablePanel.Controls.Add(btnGenerateLatex, 1, 1); // Startposition

            panelTableContainer.Controls.Add(tablePanel);

            // Textfeld zur Ausgabe des LaTeX-Codes
            outputTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = this.Font
            };

            // Komponenten dem Layout hinzufügen
            mainLayout.Controls.Add(panelSettings, 0, 0);
            mainLayout.Controls.Add(panelTableContainer, 0, 1);
            mainLayout.Controls.Add(outputTextBox, 0, 2);
            this.Controls.Add(mainLayout);
        }

        // Fügt eine neue Spalte hinzu
        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            int neueSpalte = tablePanel.ColumnCount;
            int zeilen = tablePanel.RowCount;

            // Alten "+ Spalte"-Button entfernen
            foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                if (ctrl.Text == "+ Spalte")
                    tablePanel.Controls.Remove(ctrl);

            // Neue Spalte hinzufügen
            tablePanel.ColumnCount++;
            tablePanel.ColumnStyles.Clear();
            for (int i = 0; i < tablePanel.ColumnCount; i++)
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / tablePanel.ColumnCount));

            // Neuen "+ Spalte"-Button platzieren
            var btn = new Button { Text = "+ Spalte", Dock = DockStyle.Fill };
            btn.Click += btnAddColumn_Click;
            tablePanel.Controls.Add(btn, neueSpalte, 0);

            AddTextboxes();
            PlaceGenerateButton();
        }

        // Fügt eine neue Zeile hinzu
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            int neueZeile = tablePanel.RowCount;
            int spalten = tablePanel.ColumnCount;

            // Alten "+ Zeile"-Button entfernen
            foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                if (ctrl.Text == "+ Zeile")
                    tablePanel.Controls.Remove(ctrl);

            // Neue Zeile hinzufügen
            tablePanel.RowCount++;
            tablePanel.RowStyles.Clear();
            for (int i = 0; i < tablePanel.RowCount; i++)
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / tablePanel.RowCount));

            // Neuen "+ Zeile"-Button platzieren
            var btn = new Button { Text = "+ Zeile", Dock = DockStyle.Fill };
            btn.Click += btnAddRow_Click;
            tablePanel.Controls.Add(btn, 0, neueZeile);

            AddTextboxes();
            PlaceGenerateButton();
        }

        // Fügt alle fehlenden Textboxen ein
        private void AddTextboxes()
        {
            for (int row = 0; row < tablePanel.RowCount - 1; row++)
            {
                for (int col = 0; col < tablePanel.ColumnCount - 1; col++)
                {
                    if (tablePanel.GetControlFromPosition(col, row) == null)
                    {
                        var tb = new TextBox
                        {
                            Dock = DockStyle.Fill,
                            BorderStyle = BorderStyle.None,
                            BackColor = this.BackColor,
                            ForeColor = Color.Black,
                            Multiline = true,
                            AcceptsReturn = true,
                            WordWrap = true
                        };
                        tablePanel.Controls.Add(tb, col, row);
                    }
                }
            }
            UpdateTabOrder();
        }

        // Setzt die Tabulatorreihenfolge für das Springen per Tab
        private void UpdateTabOrder()
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

        // Platziert den Button zur LaTeX-Erzeugung unten rechts
        private void PlaceGenerateButton()
        {
            tablePanel.Controls.Remove(btnGenerateLatex);
            tablePanel.Controls.Add(btnGenerateLatex, tablePanel.ColumnCount - 1, tablePanel.RowCount - 1);
        }

        // Ereignis: Button zur LaTeX-Erzeugung wird geklickt
        private void BtnGenerateLatex_Click(object sender, EventArgs e)
        {
            GenerateLatexAndUpdateOutput();
        }

        // Erstellt LaTeX-Code und zeigt ihn an
        private void GenerateLatexAndUpdateOutput()
        {
            var tableData = new List<List<string>>();

            for (int row = 0; row < tablePanel.RowCount - 1; row++)
            {
                var rowData = new List<string>();
                for (int col = 0; col < tablePanel.ColumnCount - 1; col++)
                {
                    var control = tablePanel.GetControlFromPosition(col, row);
                    rowData.Add(control is TextBox tb ? tb.Text : "");
                }
                tableData.Add(rowData);
            }

            string borderStyle = cbBorderStyle.SelectedItem?.ToString() ?? "Komplettes Gitter";
            string latexCode = LatexCode.GenerateTable(tableData, borderStyle);
            outputTextBox.Text = latexCode;
        }
    }
}
