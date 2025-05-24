using LaTeXTables.Controller;
using LaTeXTables.Model;
using LaTeXTables.View;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LaTeXTables.View
{
    public partial class TableView : Form
    {
        // GUI-Komponenten
        private TableLayoutPanel tablePanel;
        private TextBox outputTextBox;
        private Button btnGenerateLatex;
        private ComboBox cbBorderStyle;
        private Panel panelSettings;
        private Panel panelTableContainer;

        // MVC-Komponenten
        private TableModel tableModel;
        private TableController tableController;

        public TableView()
        {
            InitializeComponent();
            this.Font = FontHelper.HoleStandardSchrift(12f); // Setzt Standardschrift
            InitialisiereBenutzeroberfläche();               // Initialisiert die Benutzeroberfläche
        }

        // --- Benutzeroberfläche aufbauen ---
        private void InitialisiereBenutzeroberfläche()
        {
            Font ralewayFont = FontHelper.HoleSchrift("Raleway-SemiBold", 16f);
            // --- Hauptlayout mit 3 vertikalen Bereichen ---
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                AutoSize = false
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10f)); // Einstellungen
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60f)); // Tabelle
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30f)); // Ausgabe

            // --- Einstellungen (Dropdown + Label) ---
            panelSettings = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Label über der ComboBox
            var lblRahmenart = new Label
            {
                Text = "Rahmenart:",
                AutoSize = true,
                Location = new Point(5, 5),
                Font = ralewayFont
            };
            panelSettings.Controls.Add(lblRahmenart);

            // ComboBox für die Auswahl des Rahmenstils
            cbBorderStyle = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300,
                Left = 5,
                Top = lblRahmenart.Bottom + 5  // unterhalb des Labels platzieren
            };
            cbBorderStyle.Items.AddRange(new string[]
            {
                "Kein Rahmen",
                "Nur äußerer Rahmen",
                "Zwischen Kopfzeile und erster Spalte",
                "Komplettes Gitter"
            });
            cbBorderStyle.SelectedIndex = 3;
            cbBorderStyle.SelectedIndexChanged += (s, e) => ErzeugeLatexUndAktualisiereAusgabe();

            panelSettings.Controls.Add(cbBorderStyle);

            // --- Tabellenbereich ---
            panelTableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            tablePanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            // Eingabefeld (0,0)
            var tb = new TableTextBox
            {
                BackColor = this.BackColor,
                ForeColor = Color.Black
            };
            tablePanel.Controls.Add(tb, 0, 0);

            // +Spalte Button (0,1)
            var btnCol = new Button
            {
                Text = "neue Spalte",
                Dock = DockStyle.Fill,
                Font = ralewayFont   // ← hier genau
            };
            btnCol.Click += (s, e) =>
            {
                foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                    if (ctrl.Text.Contains("Spalte"))
                        tablePanel.Controls.Remove(ctrl);

                tableController.AddColumn();
            };
            tablePanel.Controls.Add(btnCol, 1, 0);

            // +Zeile Button (1,0)
            var btnRow = new Button
            {
                Text = "neue Zeile",
                Dock = DockStyle.Fill,
                Font = ralewayFont   // ← hier genau
            };
            btnRow.Click += (s, e) =>
            {
                foreach (Control ctrl in tablePanel.Controls.OfType<Button>().ToList())
                    if (ctrl.Text.Contains("Zeile"))
                        tablePanel.Controls.Remove(ctrl);

                tableController.AddRow();
            };
            tablePanel.Controls.Add(btnRow, 0, 1);

            // LaTeX-Button (1,1)
            btnGenerateLatex = new Button
            {
                Text = "Tabelle erzeugen",
                Dock = DockStyle.Fill,
                Font = ralewayFont,  // ← hier genau
                TabStop = false
            };

            btnGenerateLatex.Click += BtnLatexErzeugen_Click;
            btnGenerateLatex.TabStop = false;
            tablePanel.Controls.Add(btnGenerateLatex, 1, 1);

            // --- Model und Controller initialisieren ---
            tableModel = new TableModel(2, 2);
            tableController = new TableController(tablePanel, this, tableModel, btnGenerateLatex);

            panelTableContainer.Controls.Add(tablePanel);

            // --- Ausgabe-Textbox ---
            outputTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = this.Font
            };

            // --- Layout zusammensetzen ---
            mainLayout.Controls.Add(panelSettings, 0, 0);
            mainLayout.Controls.Add(panelTableContainer, 0, 1);
            mainLayout.Controls.Add(outputTextBox, 0, 2);
            this.Controls.Add(mainLayout);
        }

        // --- Button: Tabelle erzeugen ---
        private void BtnLatexErzeugen_Click(object sender, EventArgs e)
        {
            ErzeugeLatexUndAktualisiereAusgabe();
        }

        // --- LaTeX-Code erzeugen und anzeigen ---
        private void ErzeugeLatexUndAktualisiereAusgabe()
        {
            tableController.UpdateModelFromView();
            string borderStyle = cbBorderStyle.SelectedItem?.ToString() ?? "Komplettes Gitter";
            string latexCode = LatexCode.ErzeugeTabelle(tableModel.TableData, borderStyle);
            outputTextBox.Text = latexCode;
        }
    }
}

