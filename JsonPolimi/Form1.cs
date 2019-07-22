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
            Variabili.L = new ListaGruppo();

            var ofd = new OpenFileDialog();
            var rDialog = ofd.ShowDialog();
            if (rDialog != DialogResult.OK)
            {
                return;
            }

            var content = "";
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n" + e2.Message.ToString());
                return;
            }

            if (content.Length < 1)
            {
                MessageBox.Show("Il file letto sembra vuoto!");
                return;
            }

            var stuff = JObject.Parse(content);

            var infoData = stuff["info_data"];
            var i = infoData.Children();

            foreach (var i2 in i)
            {
                var i3 = i2.First;

                Aggiungi(i3);
            }
        }

        private static void Refresh_Tabella()
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            var html = "<html><body><table>";
            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                html += "<tr>";

                html += "<td>";
                html += elem.id;
                html += "</td>";

                html += "<td>";
                html += elem.platform;
                html += "</td>";

                html += "<td>";
                html += elem.classe;
                html += "</td>";

                html += "<td>";
                html += elem.degree;
                html += "</td>";

                html += "<td>";
                html += elem.language;
                html += "</td>";

                html += "<td>";
                html += elem.office;
                html += "</td>";

                html += "<td>";
                html += elem.school;
                html += "</td>";

                html += "<td>";
                html += elem.tipo;
                html += "</td>";

                html += "<td>";
                html += elem.year;
                html += "</td>";

                html += "<td>";
                html += elem.id_link;
                html += "</td>";

                html += "</tr>";
            }
            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private static void Aggiungi(JToken i)
        {
            var G = new Gruppo
            {
                classe = i["class"].ToString(),
                degree = i["degree"].ToString()
            };

            try
            {
                G.platform = i["group_type"].ToString();
            }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
            {
                G.platform = i["platform"].ToString();
            }

            G.id = i["id"].ToString();
            G.language = i["language"].ToString();
            G.office = i["office"].ToString();
            G.school = i["school"].ToString();
            G.id_link = i["id_link"].ToString();

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
            Aggiusta();
            Refresh_Tabella();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Aggiusta();

            var json = "{\"info_data\":{";
            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                json += '\n';
                json += '"';
                json += elem.id;
                json += '"' + ":";

                json += elem.To_json();

                if (i != n - 1)
                    json += ",";
            }
            json += "},";
            json += '\n';
            json += "\"index_data\":[";
            for (int i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                json += '\n';
                json += elem.To_json();

                if (i != n - 1)
                    json += ",";
            }
            json += "]}";

            File.WriteAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups2.json", json);
        }

        private void Aggiusta()
        {
            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);
                if (String.IsNullOrEmpty(elem.id_link))
                {
                    Variabili.L.Remove(i);

                    i--;
                    n = Variabili.L.GetCount();
                }
            }

            n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                string nome = AggiustaNome(elem.classe);
                elem.classe = nome;

                Variabili.L.SetElem(i, elem);
            }
        }

        private string AggiustaNome(string s)
        {
            if (s.Contains("<="))
            {
                var n = s.IndexOf("<=");
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 2);
                return r;
            }
            else if (s.Contains("&lt;="))
            {
                var n = s.IndexOf("&lt;=");
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 5);
                return r;
            }
            else
            {
                return s;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var x = new Aggiungi_Form();
            x.ShowDialog();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm3.ods", "2017/2018");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm4.ods", "2018/2019");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm5.ods", "2019/2020");

            ;
        }

        private static void Apri_ODS(string file, string year)
        {
            Independentsoft.Office.Odf.Spreadsheet x = null;
            try
            {
                x = new Independentsoft.Office.Odf.Spreadsheet();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            try
            {
                x.Open(file);
            }
            catch
            {
                MessageBox.Show("Non sono riuscito ad aprire il file!");
                return;
            }

            var nome_old = new Gruppo();

            foreach (var y in x.Tables)
            {
                foreach (var y2 in y.Rows)
                {
                    //Console.WriteLine("----- NUOVA RIGA ------");

                    var g = new InsiemeDiGruppi { gruppo_di_base = { year = year }, nome_old = nome_old };

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

                    foreach (var g3 in g.L)
                    {
                        Variabili.L.Add(g3);
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.classe))
                    {
                        nome_old.classe = g.nome_old.classe;
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.language))
                    {
                        nome_old.language = g.nome_old.language;
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.degree))
                    {
                        nome_old.degree = g.nome_old.degree;
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.school))
                    {
                        nome_old.school = g.nome_old.school;
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.office))
                    {
                        nome_old.office = g.nome_old.office;
                    }

                    if (!string.IsNullOrEmpty(g.nome_old.year))
                    {
                        nome_old.year = g.nome_old.year;
                    }
                }
            }
        }
    }
}