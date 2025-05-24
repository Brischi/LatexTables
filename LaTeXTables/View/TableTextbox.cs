using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaTeXTables.View
{

    public class TableTextBox : TextBox
    {
        public TableTextBox()
        {
            Multiline = true;
            AcceptsReturn = true;
            WordWrap = true;
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;
            // ggf. noch: Font = … oder BackColor = …
        }
    }
}

