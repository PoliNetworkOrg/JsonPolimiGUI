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

        private static void Refresh_Tabella(string textBox_anno, string textBox_piattaforma)
        {
            if (JsonPolimi_Core_nf.Data.Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            var html = "<html><body><table border=\"1\">";
            var n = JsonPolimi_Core_nf.Data.Variabili.L.GetCount();

            if (n <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            int done = 0;

            for (var i = 0; i < n; i++)
            {
                var elem = JsonPolimi_Core_nf.Data.Variabili.L.GetElem(i);
                var html_elem =  elem.GetHTML_DataRow(textBox_anno,textBox_piattaforma);
                html += html_elem;

                if (!string.IsNullOrEmpty(html_elem))
                {
                    done++;
                }
            }

            if (done <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Refresh_Tabella(textBox_anno: textBox_anno.Text,  textBox_piattaforma: textBox_piattaforma.Text);
        }
    }
}
