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
using Independentsoft.Office.Odf;
using Newtonsoft.Json.Linq;

namespace JsonPolimi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Variabili.L = new List<Gruppo>();

            string content = "";
            try
            {
                content = File.ReadAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups.json");
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n"+e2.Message.ToString());
                return;
            }

            if (content.Length<1)
            {
                MessageBox.Show("Il file letto sembra vuoto!");
                return;
            }

            var stuff = JObject.Parse(content);

            var InfoData = stuff["info_data"];
            var i = InfoData.Children();

            foreach (var i2 in i)
            {
                var i3 = i2.First;

                Aggiungi(i3);
            }


        }

        private void Refresh_Tabella()
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            string html = "<html><body><table>";
            for (int i = 0; i< Variabili.L.Count; i++)
            {
                html += "<tr>";

                html += "<td>";
                html += Variabili.L[i].id;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].platform;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].classe;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].degree;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].language;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].office;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].school;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].tipo;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].year;
                html += "</td>";

                html += "</tr>";
            }
            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private void Aggiungi(JToken i)
        {
            Gruppo G = new Gruppo
            {
                classe = i["class"].ToString(),
                degree = i["degree"].ToString(),
                platform = i["group_type"].ToString(),
                id = i["id"].ToString(),
                language = i["language"].ToString(),
                office = i["office"].ToString(),
                school = i["school"].ToString()
            };

            try
            {
                G.tipo = i["type"].ToString();
            }
            catch
            {
                G.tipo = null;
            }

            try
            {
                G.year = i["year"].ToString();
            }
            catch
            {
                G.year = null;
            }

            G.Aggiusta();

            Variabili.L.Add(G);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Refresh_Tabella();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            string json = "{\"info_data\":{";
            for (int i=0; i< Variabili.L.Count; i++)
            {
                json += '"';
                json += Variabili.L[i].id;
                json += '"' + ":";

                json += Variabili.L[i].To_json();
                
                if (i != Variabili.L.Count - 1)
                    json += ",";
            }
            json += "},\"index_data\":[";
            for (int i = 0; i < Variabili.L.Count; i++)
            {
                json += Variabili.L[i].To_json();

                if (i != Variabili.L.Count - 1)
                    json += ",";
            }
            json += "]}";

            File.WriteAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups2.json", json);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Aggiungi_Form x = new Aggiungi_Form();
            x.ShowDialog();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new List<Gruppo>();

            Independentsoft.Office.Odf.Spreadsheet x = new Independentsoft.Office.Odf.Spreadsheet();
            try
            {
                x.Open("C:\\Users\\Arme\\Downloads\\pm.ods");
            }
            catch
            {
                MessageBox.Show("Non sono riuscito ad aprire il file!");
                return;
            }

            foreach (var y in x.Tables)
            {
                foreach (var y2 in y.Rows)
                {
                    Console.WriteLine("----- NUOVA RIGA ------");
                    InsiemeDiGruppi g = new InsiemeDiGruppi();
                    g.gruppo_di_base.year = "2017/2018";

                    foreach (var y3 in y2.Cells)
                    {
                        foreach (var y4 in y3.Content)
                        {
                            if (y4 is Paragraph y5)
                            {
                                foreach (var y6 in y5.Content)
                                {
                                    Gruppo.AggiungiInformazioneAmbigua(y6.ToString(), ref g);
                                }
                            }
                            else
                            {
                                Console.WriteLine(y4.ToString());
                            }
                        }
                    }

                    g.Aggiusta();

                    foreach (Gruppo g3 in g.L)
                    {
                        Variabili.L.Add(g3);
                    }
                }
            }

            ;
        }
    }
}
