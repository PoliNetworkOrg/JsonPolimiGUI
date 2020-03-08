using Independentsoft.Office.Odf;
using JsonPolimi.Tipi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Telegram.Bot.Types.Enums;
using Size = System.Drawing.Size;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace JsonPolimi
{
    public partial class Form1 : Form
    {
        public static FileSalvare FileSalvare;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            var ofd = new OpenFileDialog();
            var rDialog = ofd.ShowDialog();
            if (rDialog != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }

            string content;
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n" + e2.Message);
                ofd.Dispose();
                return;
            }

            if (content.Length < 1)
            {
                MessageBox.Show("Il file letto sembra vuoto!");
                ofd.Dispose();
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

            Variabili.L.Sort();

            ofd.Dispose();
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

            if (n <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

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

                html += "<td>";
                html += elem.PermanentId;
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

            try
            {
                g.PermanentId = i["permanentId"].ToString();
            }
            catch
            {
                g.PermanentId = null;
            }

            var data = i["LastUpdateInviteLinkTime"].ToString();
            try
            {
                g.LastUpdateInviteLinkTime = DataFromString(data);
            }
            catch (Exception e)
            {
                g.LastUpdateInviteLinkTime = null;
                throw e;
            }

            g.Aggiusta();

            Variabili.L.Add(g);
        }

        public static DateTime? DataFromString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            if (data == "null")
                return null;

            if (data.Contains("T"))
            {
                //2019-07-27T17:13:23.5409603+02:00
                var a1 = data.Split('T');

                //2019-07-27
                var a2 = a1[0].Split('-');

                //17:13:23.5409603+02:00
                var a3 = a1[1].Split('+');

                //17:13:23.5409603
                var a4 = a3[0].Split('.');

                //17:13:23
                var a5 = a4[0].Split(':');

                return new DateTime(Convert.ToInt32(a2[0]), Convert.ToInt32(a2[1]), Convert.ToInt32(a2[2]), Convert.ToInt32(a5[0]), Convert.ToInt32(a5[1]), Convert.ToInt32(a5[2]));
            }

            if (data.Contains("."))
            {
                //2019-07-29 18:26:55.034083
                data = data.Split('.')[0];

                //2019-07-29 18:26:55
                var b1 = data.Split(' ');

                //2019-07-29
                var b2 = b1[0].Split('-');

                //18:26:55
                var b3 = b1[1].Split(':');

                return new DateTime(Convert.ToInt32(b2[0]), Convert.ToInt32(b2[1]), Convert.ToInt32(b2[2]), Convert.ToInt32(b3[0]), Convert.ToInt32(b3[1]), Convert.ToInt32(b3[2]));
            }

            //27/07/2019 11:42:24
            var s1 = data.Split(' ');

            //27/07/2019
            var s2 = s1[0].Split('/');

            //11:42:24
            var s3 = s1[1].Split(':');

            return new DateTime(Convert.ToInt32(s2[2]), Convert.ToInt32(s2[1]), Convert.ToInt32(s2[0]), Convert.ToInt32(s3[0]), Convert.ToInt32(s3[1]), Convert.ToInt32(s3[2]));
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

            Variabili.L.Sort();
            Aggiusta();
            Variabili.L.Sort();

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

            Salva(json);
        }

        private static void Salva(string json)
        {
            var o = new SaveFileDialog();
            var r = o.ShowDialog();
            if (r != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            File.WriteAllText(o.FileName, json);
            o.Dispose();
        }

        private static void Aggiusta()
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

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

            Variabili.L.Sort();
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

            if (s.Contains("&lt;="))
            {
                var n = s.IndexOf("&lt;=", StringComparison.Ordinal);
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 5);
                return r;
            }

            s = s.Replace("&apos;", "'");
            s = s.Replace("&amp;", "&");

            return s;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var x = new AggiungiForm(false);
            x.ShowDialog();
            x.Dispose();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            var o = new OpenFileDialog();
            var r = o.ShowDialog();
            if (r != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            var (item1, item2) = ShowInputDialog("Anno");
            if (item1 != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            Apri_ODS(o.FileName, item2);

            o.Dispose();

            /*
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm3.ods", "2017/2018");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm4.ods", "2018/2019");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm5.ods", "2019/2020");
            */
        }

        private static Tuple<DialogResult, string> ShowInputDialog(string title)
        {
            var size = new Size(200, 70);
            var inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = title,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            var textBox = new TextBox
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = ""
            };
            inputBox.Controls.Add(textBox);

            var okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            var cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            var result = inputBox.ShowDialog();

            inputBox.Dispose();
            return new Tuple<DialogResult, string>(result, textBox.Text);
        }

        private static void Apri_ODS(string file, string year)
        {
            Spreadsheet x;
            try
            {
                x = new Spreadsheet();
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
                foreach (var y2 in y.Rows)
                {
                    //Console.WriteLine("----- NUOVA RIGA ------");

                    var g = new InsiemeDiGruppi { GruppoDiBase = { Year = year }, NomeOld = nomeOld };

                    foreach (var y3 in y2.Cells)
                        foreach (var y4 in y3.Content)
                            if (y4 is Paragraph y5)
                                foreach (var y6 in y5.Content)
                                    Gruppo.AggiungiInformazioneAmbigua(y6.ToString(), ref g);
                            else
                                Console.WriteLine(y4.ToString());

                    g.Aggiusta();

                    foreach (var g3 in g.L) Variabili.L.Add(g3);

                    if (!string.IsNullOrEmpty(g.NomeOld.Classe)) nomeOld.Classe = g.NomeOld.Classe;

                    if (!string.IsNullOrEmpty(g.NomeOld.Language)) nomeOld.Language = g.NomeOld.Language;

                    if (!string.IsNullOrEmpty(g.NomeOld.Degree)) nomeOld.Degree = g.NomeOld.Degree;

                    if (!string.IsNullOrEmpty(g.NomeOld.School)) nomeOld.School = g.NomeOld.School;

                    if (!string.IsNullOrEmpty(g.NomeOld.Office)) nomeOld.Office = g.NomeOld.Office;

                    if (!string.IsNullOrEmpty(g.NomeOld.Year)) nomeOld.Year = g.NomeOld.Year;
                }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Vuoi davvero eliminare la lista in RAM?", "Sicuro?",
                MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) Variabili.L = new ListaGruppo();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null) Variabili.L = new ListaGruppo();

            if (Variabili.L.GetCount() <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Variabili.L.Sort();

            var x = new ListaGruppiModificaForm();
            x.ShowDialog();

            x.Dispose();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            LoadGruppi();

            if (FileSalvare == null)
                FileSalvare = new FileSalvare();

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            foreach (var r in FileSalvare.Gruppi)
            {
                if (r.Chat.Type == ChatType.Private)
                    continue;

                if (r.we_are_admin == false)
                    continue;

                var g = new Gruppo
                {
                    Classe = r.Chat.Title,
                    PermanentId = r.Chat.Id.ToString(),
                    Platform = "TG",
                    IdLink = TelegramLinkLastPart(r.Chat.InviteLink),
                    Tipo = "C",
                    LastUpdateInviteLinkTime = r.LastUpdateInviteLinkTime,
                };

                g.Aggiusta();
                Variabili.L.Add(g);
            }

            Variabili.L.Sort();
        }

        private static string TelegramLinkLastPart(string chatInviteLink)
        {
            if (string.IsNullOrEmpty(chatInviteLink))
                return null;

            var r = chatInviteLink.Split('/');
            return r[r.Length - 1];
        }

        public static void LoadGruppi()
        {
            var ofd = new OpenFileDialog();
            var rDialog = ofd.ShowDialog();
            if (rDialog != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }

            string content;
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n" + e2.Message);
                ofd.Dispose();
                return;
            }

            try
            {
                FileSalvare = JsonConvert.DeserializeObject<FileSalvare>(content);
            }
            catch
            {
                ;
            }

            if (FileSalvare == null)
                FileSalvare = new FileSalvare();

            ofd.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            Variabili.L.ProvaAdUnire();
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Variabili.L.Sort();
            Aggiusta();
            Variabili.L.Sort();

            var json = "{\"Gruppi\": [";
            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                var j = elem.To_json_Tg();

                if (j == null)
                    continue;

                json += j;

                if (i != n - 1)
                    json += ",";
            }

            if (json[json.Length - 1] == ',')
                json = json.Remove(json.Length - 1);

            json += "]}";

            Salva(json);
        }

        private void Button8_Click2(object sender, EventArgs e)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            using (OpenFileDialog o = new OpenFileDialog())
            {
                var r = o.ShowDialog();
                if (r != DialogResult.OK)
                {
                    return;
                }

                doc.Load(o.FileName);
            }

            List<Gruppo> L2 = GetGruppiFromDocument(doc);

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.Importa(L2);
        }

        private List<Gruppo> GetGruppiFromDocument(HtmlAgilityPack.HtmlDocument doc)
        {
            List<HtmlNode> L = GetTables(doc.DocumentNode.ChildNodes);
            List<HtmlNode> L2 = new List<HtmlNode>();

            while (L.Count > 0)
            {
                if (L[0].Name == "tr")
                {
                    if (L[0].ChildNodes.Count > 0)
                    {
                        L2.Add(L[0]);
                    }
                }
                else
                {
                    L.AddRange(L[0].ChildNodes);
                }

                L.RemoveAt(0);
            }

            return GetGruppiFromDocument2(L2);
        }

        private List<HtmlNode> GetTables(HtmlNodeCollection childNodes)
        {
            List<HtmlNode> L = new List<HtmlNode>();
            List<HtmlNode> L2 = new List<HtmlNode>();
            L.AddRange(childNodes);

            while (L.Count > 0)
            {
                if (L[0].Name == "table")
                {
                    L2.Add(L[0]);                  
                }

                L.AddRange(L[0].ChildNodes);
                

                L.RemoveAt(0);
            }

            return L2;
        }

        private List<Gruppo> GetGruppiFromDocument2(List<HtmlNode> l2)
        {
            List<Gruppo> LG = new List<Gruppo>();
            foreach (var x in l2)
            {
                Gruppo x2 = GetGruppiFromDocument3(x);
                if (x2 != null)
                {
                    string x3 = x2.Classe.Trim();
                    if (!string.IsNullOrEmpty(x3))
                    {
                        if (x2.Id == null)
                        {
                            LG.Add(x2);
                        }
                        else if (!x2.Id.Contains("Area Servizi ICT") && !x2.Classe.Contains("Anno Corso") && !x2.Classe.Contains("Corso di Studi"))
                        { 
                            LG.Add(x2); 
                        }
                    }
                }
            }

            return LG;
        }

        private Gruppo GetGruppiFromDocument3(HtmlNode x)
        {
            List<HtmlNode> L = new List<HtmlNode>();
            for (int i = 0; i < x.ChildNodes.Count; i++)
            {
                if (x.ChildNodes[i].Name == "td")
                {
                    L.Add(x.ChildNodes[i]);
                }
            }

            List<InfoParteDiGruppo> infoParteDiGruppo_list = new List<InfoParteDiGruppo>();

            for (int i = 0; i < L.Count; i++)
            {
                infoParteDiGruppo_list.Add(GetGruppiFromDocument5(L[i]));
            }

            Gruppo g = Gruppo.FromInfoParteList(infoParteDiGruppo_list);
            if (g != null  && g.IsValido())
            {
                return g;
            }
            else
            {
                return null;
            }
        }
        
        private bool Contiene_table2(HtmlNode htmlNode)
        {
            foreach (var x in htmlNode.ChildNodes)
            {
                if (x.Name == "table")
                    return true;
            }

            return false;
        }

        private InfoParteDiGruppo GetGruppiFromDocument5(HtmlNode htmlNode)
        {
            bool contiene_table = Contiene_table2(htmlNode);
            if (contiene_table)
                return null;

            if (htmlNode.ChildNodes.Count == 3)
            {
                if (htmlNode.ChildNodes[0].Name == "#text" &&
                    htmlNode.ChildNodes[1].Name != "#text" &&
                    htmlNode.ChildNodes[2].Name == "#text" )
                {
                    string s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                    string s2 = htmlNode.ChildNodes[2].InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                    {
                        if (htmlNode.ChildNodes[1].Name == "div")
                        {
                            var x1 = htmlNode.ChildNodes[1];
                            if (x1.ChildNodes.Count == 1)
                            {
                                if (x1.ChildNodes[0].Name == "#text")
                                {
                                    return new InfoParteDiGruppo(testo_selvaggio: x1.ChildNodes[0].InnerHtml.Trim());
                                }
                                else
                                {
                                    ;
                                }
                            }
                            else
                            {
                                ;
                            }
                        }
                        else if (htmlNode.ChildNodes[1].Name == "a")
                        {
                            var x1 = htmlNode.ChildNodes[1];
                            if (x1.ChildNodes.Count == 3)
                            {
                                if (x1.ChildNodes[0].Name == "#text" &&
                                    x1.ChildNodes[1].Name != "#text" &&
                                    x1.ChildNodes[2].Name == "#text")
                                {
                                    if (x1.ChildNodes[1].Name == "img")
                                    {
                                        ImmagineGruppo immagine2 = new ImmagineGruppo(x1.ChildNodes[1]);
                                        return new InfoParteDiGruppo(immagine: immagine2);
                                    }
                                    else
                                    {
                                        ;
                                    }
                                }
                                else
                                {
                                    ;
                                }
                            }
                            else
                            {
                                string s = htmlNode.InnerHtml.Trim();
                                if (string.IsNullOrEmpty(s))
                                {
                                    return null;
                                }
                                else
                                {
                                    return new InfoParteDiGruppo(
                                        link: new LinkGruppo(
                                            htmlNode.ChildNodes[1].Attributes,
                                            htmlNode.ChildNodes[1].InnerHtml.Trim()
                                            )
                                        );
                                }
                            }
                        }
                        else
                        {
                            ;
                        }
                    }
                    else
                    {
                        string s3 = htmlNode.InnerHtml.Trim();
                        if (string.IsNullOrEmpty(s3))
                        {
                            return null;
                        }
                        else if (s3.StartsWith("Ingegneria Industriale e dell'Informazione"))
                        {
                            return null; //sicuro
                        }
                        else if (s3 == "1<sup><small>o</small></sup>Anno")
                        {
                            return null; //sicuro
                        }
                        else if (s3 == "2<sup><small>o</small></sup>Anno")
                        {
                            return null; //sicuro
                        }
                        else if (s3 == "3<sup><small>o</small></sup>Anno")
                        {
                            return null; //sicuro
                        }
                        else
                        {
                            string s4 = s1 + "<br>" + s2;
                            if (s4 == s3)
                            {
                                List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>
                                {
                                    new InfoParteDiGruppo(testo_selvaggio: s1),
                                    new InfoParteDiGruppo(testo_selvaggio: s2)
                                };
                                return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                else if (htmlNode.ChildNodes[0].Name == "img" &&
                        htmlNode.ChildNodes[1].Name == "#text" &&
                        htmlNode.ChildNodes[2].Name == "img")
                {
                        if (htmlNode.ChildNodes[0].Attributes["src"].Value.Contains("/it.png"))
                        {
                            if (htmlNode.ChildNodes[2].Attributes["src"].Value.Contains("/en.png"))
                            {
                                string s = htmlNode.ChildNodes[1].InnerHtml.Trim();
                                if (string.IsNullOrEmpty(s))
                                {
                                    ;
                                }
                                else if (s == "/")
                                {
                                    return null; //sicuro
                                }
                                else
                                {
                                    ;
                                }
                            }
                            else
                            {
                                ;
                            }
                        }
                        else
                        {
                            ;
                        }
                }
                else
                {
                    ;
                }

            }
            else if (htmlNode.ChildNodes.Count == 1)
            {
                string s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1))
                {
                    return null; //sono sicuro
                }
                else if (htmlNode.ChildNodes[0].Name == "a")
                {
                    var x1 = htmlNode.ChildNodes[0];
                    if (x1.ChildNodes.Count == 3)
                    {
                        if (x1.ChildNodes[0].Name == "#text" &&
                            x1.ChildNodes[1].Name == "div" &&
                            x1.ChildNodes[2].Name == "#text" )
                        {
                            string s11 = x1.ChildNodes[0].InnerHtml.Trim();
                            string s22 = x1.ChildNodes[2].InnerHtml.Trim();

                            if (string.IsNullOrEmpty(s11) && string.IsNullOrEmpty(s22))
                            {
                                var x2 = x1.ChildNodes[1];
                                if (x2.ChildNodes.Count == 3)
                                {
                                    ;
                                }
                                else if (x2.ChildNodes.Count == 5)
                                {
                                    return null;
                                }
                                else
                                {
                                    ;
                                }
                            }
                            else
                            {
                                ;
                            }
                        }
                        else
                        {
                            ;
                        }
                    }
                    else if (x1.ChildNodes.Count == 1)
                    {
                        var x2 = x1.ChildNodes[0];
                        if (x2.ChildNodes.Count == 0)
                        {
                            LinkGruppo link2 = new LinkGruppo(htmlNode.ChildNodes[0].Attributes, x2.InnerHtml.Trim());
                            return new InfoParteDiGruppo(link: link2);
                        }
                        else
                        {
                            ;
                        }
                    }
                    else
                    {
                        ;
                    }
                }
                else if(htmlNode.ChildNodes[0].Name == "#text")
                {
                    return new InfoParteDiGruppo(testo_selvaggio: s1);
                }
                else if (htmlNode.ChildNodes[0].Name == "select")
                {
                    return null;
                }
                else
                {
                    ;
                }
            }
            else if (htmlNode.ChildNodes.Count == 6)
            {
                string s = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    ;
                }
                else if (s.StartsWith("I CFU riportati a fianco a questo"))
                {
                    return null; //sicuro
                }
                else
                {
                    ;
                }
            }
            else if (htmlNode.ChildNodes.Count == 4)
            {
                return null;
            }
            else if (htmlNode.ChildNodes.Count == 0)
            {
                string s1 = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1))
                {
                    return null; //sicuro
                }
                else
                {
                    ;
                }
            }
            else if (htmlNode.ChildNodes.Count == 2)
            {
                if (htmlNode.ChildNodes[0].Name == "#text" && htmlNode.ChildNodes[1].Name == "div")
                {
                    string s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s1))
                    {
                        ;
                    }
                    else if (s1 == "10.0")
                    {
                        return null;
                    }
                    else
                    {
                        ;
                    }
                }
                else if (htmlNode.ChildNodes[0].Name == "#text" && htmlNode.ChildNodes[1].Name == "a")
                {
                    string s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s1))
                    {
                        var x1 = htmlNode.ChildNodes[1];
                        if (x1.ChildNodes.Count == 1)
                        {
                            if (x1.ChildNodes[0].Name == "#text")
                            {
                                LinkGruppo link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes, x1.ChildNodes[0].InnerHtml.Trim());
                                return new InfoParteDiGruppo(link: link2);
                            }
                            else
                            {
                                ;
                            }
                        }
                        else
                        {
                            ;
                        }

                    }
                    else
                    {
                        ;
                    }
                }
                else if (htmlNode.ChildNodes[0].Name == "a" && htmlNode.ChildNodes[1].Name == "a")
                {
                    string s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                    string s2 = htmlNode.ChildNodes[1].InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
                    {
                        LinkGruppo link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes, s2);
                        return new InfoParteDiGruppo(link: link2);
                    }
                    else if (!string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                    {
                        ;
                    }
                    else
                    {
                        ;
                    }
                }
                else
                {
                    ;
                }
            }
            else
            {
                ;
            }


            return null;
        }

        private HtmlNode GetGruppiFromDocument4(HtmlNode htmlNode)
        {
            if (htmlNode.ChildNodes.Count == 0)
                return htmlNode;

            List<char> LC = new List<char>
            {
                '\t',
                '\r'
            };

            foreach (var x in htmlNode.ChildNodes)
            {
                bool buono = true;
                foreach (var x2 in x.InnerText)
                {
                    if (LC.Contains(x2))
                    {
                        buono = false;
                        break;
                    }
                }

                if (buono)
                    return x;
            }

            return htmlNode;
        }

        private void Button12_Click(object sender, EventArgs e)
        {

        }
    }
}