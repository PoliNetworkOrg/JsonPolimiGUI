using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace JsonPolimi.Forms
{
    public partial class GeneraTabellaHTML : Form
    {
        public GeneraTabellaHTML()
        {
            InitializeComponent();
        }

        private void GeneraTabellaHTML_Load(object sender, EventArgs e)
        {

        }

        private static void Refresh_Tabella()
        {
            if (Data.Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            var html = "<html><body><table>";
            var n = Data.Variabili.L.GetCount();

            if (n <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            for (var i = 0; i < n; i++)
            {
                var elem = Data.Variabili.L.GetElem(i);
                html += elem.GetHTML_DataRow();
            }

            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Refresh_Tabella();
        }
    }
}
