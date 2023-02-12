using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using JsonPolimi_Core_nf.Data;

namespace JsonPolimi.Forms;

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
        if (Variabili.L == null)
        {
            MessageBox.Show("Lista vuota!");
            return;
        }

        var html = "<html><body><table border=\"1\">";
        var n = Variabili.L.GetCount();

        if (n <= 0)
        {
            MessageBox.Show("Lista vuota!");
            return;
        }

        var done = 0;

        for (var i = 0; i < n; i++)
        {
            var elem = Variabili.L.GetElem(i);
            var html_elem = elem.GetHTML_DataRow(textBox_anno, textBox_piattaforma);
            html += html_elem;

            if (!string.IsNullOrEmpty(html_elem)) done++;
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
        Refresh_Tabella(textBox_anno.Text, textBox_piattaforma.Text);
    }
}