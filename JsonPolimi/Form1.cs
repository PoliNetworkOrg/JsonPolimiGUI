﻿using System;
using System.Diagnostics;
using System.IO;
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

            string content;
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
                html += elem.Id;
                html += "</td>";

                html += "<td>";
                html += elem.Platform;
                html += "</td>";

                html += "<td>";
                html += elem.Classe;
                html += "</td>";

                html += "<td>";
                html += elem.Degree;
                html += "</td>";

                html += "<td>";
                html += elem.Language;
                html += "</td>";

                html += "<td>";
                html += elem.Office;
                html += "</td>";

                html += "<td>";
                html += elem.School;
                html += "</td>";

                html += "<td>";
                html += elem.Tipo;
                html += "</td>";

                html += "<td>";
                html += elem.Year;
                html += "</td>";

                html += "<td>";
                html += elem.IdLink;
                html += "</td>";

                html += "</tr>";
            }
            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private static void Aggiungi(JToken i)
        {
            var g = new Gruppo
            {
                Classe = i["class"].ToString(),
                Degree = i["degree"].ToString()
            };
            try
            {
                g.Platform = i["group_type"].ToString();
            }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            catch (Exception)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
            {
                g.Platform = i["platform"].ToString();
            }

            g.Id = i["id"].ToString();
            g.Language = i["language"].ToString();
            g.Office = i["office"].ToString();
            g.School = i["school"].ToString();
            g.IdLink = i["id_link"].ToString();

            try
            {
                g.Tipo = i["type"].ToString();
            }
            catch
            {
                g.Tipo = null;
            }

            try
            {
                g.Year = i["year"].ToString();
            }
            catch
            {
                g.Year = null;
            }

            g.Aggiusta();

            Variabili.L.Add(g);
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
                json += elem.Id;
                json += '"' + ":";

                json += elem.To_json();

                if (i != n - 1)
                    json += ",";
            }
            json += "},";
            json += '\n';
            json += "\"index_data\":[";
            for (var i = 0; i < n; i++)
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
                if (!string.IsNullOrEmpty(elem.IdLink)) continue;
                Variabili.L.Remove(i);

                i--;
                n = Variabili.L.GetCount();
            }

            n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                var nome = AggiustaNome(elem.Classe);
                elem.Classe = nome;

                Variabili.L.SetElem(i, elem);
            }
        }

        private static string AggiustaNome(string s)
        {
            if (s.Contains("<="))
            {
                var n = s.IndexOf("<=", StringComparison.Ordinal);
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 2);
                return r;
            }
            else if (s.Contains("&lt;="))
            {
                var n = s.IndexOf("&lt;=", StringComparison.Ordinal);
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
            var x = new AggiungiForm();
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
            Independentsoft.Office.Odf.Spreadsheet x;
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

            var nomeOld = new Gruppo();

            foreach (var y in x.Tables)
            {
                foreach (var y2 in y.Rows)
                {
                    //Console.WriteLine("----- NUOVA RIGA ------");

                    var g = new InsiemeDiGruppi { GruppoDiBase = { Year = year }, NomeOld = nomeOld };

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

                    if (!string.IsNullOrEmpty(g.NomeOld.Classe))
                    {
                        nomeOld.Classe = g.NomeOld.Classe;
                    }

                    if (!string.IsNullOrEmpty(g.NomeOld.Language))
                    {
                        nomeOld.Language = g.NomeOld.Language;
                    }

                    if (!string.IsNullOrEmpty(g.NomeOld.Degree))
                    {
                        nomeOld.Degree = g.NomeOld.Degree;
                    }

                    if (!string.IsNullOrEmpty(g.NomeOld.School))
                    {
                        nomeOld.School = g.NomeOld.School;
                    }

                    if (!string.IsNullOrEmpty(g.NomeOld.Office))
                    {
                        nomeOld.Office = g.NomeOld.Office;
                    }

                    if (!string.IsNullOrEmpty(g.NomeOld.Year))
                    {
                        nomeOld.Year = g.NomeOld.Year;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}