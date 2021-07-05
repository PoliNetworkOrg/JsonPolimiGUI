using HtmlAgilityPack;
using JsonPolimi.Data;
using JsonPolimi.Enums;
using JsonPolimi.Tipi;
using JsonPolimi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Telegram.Bot.Types.Enums;
using Size = System.Drawing.Size;

namespace JsonPolimi.Forms
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
            int count = 0;

            JEnumerable<JToken> i;
            if (infoData == null)
            {
                ;
                var i4 = stuff.First;
                ;
                var i5 = i4.First;
                ;
                i = i5.Children();
                foreach (var i2 in i)
                {
                    Aggiungi(i2, false, false);
                    count++;
                }
            }
            else
            {
                i = infoData.Children();
                foreach (var i2 in i)
                {
                    var i3 = i2.First;

                    Aggiungi(i3, false, false);
                    count++;
                }
            }

            Console.WriteLine("Added " + count + " groups");

            Variabili.L.Sort();

            ofd.Dispose();

            MessageBox.Show("Finito!");
        }

       

        private static void Aggiungi(JToken i, bool aggiusta_Anno, bool merge)
        {
            var g = new Gruppo
            {
                Classe = i["class"].ToString(),
                Degree = i["degree"].ToString()
            };

            try
            {
                try
                {
                    g.Platform = i["group_type"].ToString();
                }
                catch
                {
                    g.Platform = i["platform"].ToString();
                }
            }
            catch
            {
                g.Platform = i["platform"].ToString();
            }

            g.Id = i["id"].ToString();
            g.Language = i["language"].ToString();
            g.Office = new ListaStringhePerJSON(i["office"].ToString());
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

            try
            {
                g.CCS = new ListaStringhePerJSON(i["ccs"].ToString());
            }
            catch
            {
                g.CCS = null;
            }

            try
            {
                string s = i["annocorso"].ToString();
                if (string.IsNullOrEmpty(s))
                    g.AnnoCorsoStudio = null;
                else
                    g.AnnoCorsoStudio = Convert.ToInt32(s);
            }
            catch
            {
                g.AnnoCorsoStudio = null;
            }

            try
            {
                g.IDCorsoPolimi = i["idcorso"].ToString();
            }
            catch
            {
                g.IDCorsoPolimi = null;
            }

            try
            {
                g.PianoDiStudi = i["pianostudi"].ToString();
            }
            catch
            {
                g.PianoDiStudi = null;
            }

            object data2 = i["LastUpdateInviteLinkTime"];

            string data = null;
            try
            {
                data = data2.ToString();
            }
            catch
            {
                ;
            }

            try
            {
                g.LastUpdateInviteLinkTime = DataFromString(data);
            }
            catch (Exception e)
            {
                g.LastUpdateInviteLinkTime = null;
                throw e;
            }

            g.LinkFunzionante = BoolFromString(i["linkfunzionante"]);

            g.Aggiusta(aggiusta_Anno, creaid: false);

            Variabili.L.Add(g, merge);
        }

        private static bool? BoolFromString(JToken jTokens)
        {
            try
            {
                string s = jTokens.ToString();
                if (s == "Y")
                    return true;
                else if (s == "N")
                    return false;
            }
            catch
            {
                ;
            }

            return null;
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
            GeneraTabellaHTML generaTabellaHTML = new GeneraTabellaHTML();
            generaTabellaHTML.Show();
        }

        private void Button3_Click(CheckGruppo.E i, bool entrambi_index)
        {
            Salva_Generico(new CheckGruppo(i), entrambi_index);
        }

        private void Salva_Generico(CheckGruppo v, bool entrambi_index)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Variabili.L.Sort();
            Aggiusta();
            Variabili.L.Sort();

            var n = Variabili.L.GetCount();

            var json = "{";

            if (entrambi_index)
            {
                json += "\"info_data\":{";

                for (var i = 0; i < n; i++)
                {
                    var elem = Variabili.L.GetElem(i);

                    bool tenere = DoCheckGruppo(v, elem);
                    if (tenere)
                    {
                        json += '\n';
                        json += '"';
                        json += elem.Id;
                        json += '"' + ":";
                        json += elem.To_json(v.n);
                        json += ',';
                    }
                }

                if (json.EndsWith(","))
                {
                    json = json.Substring(0, json.Length - 1);
                }

                json += "}";
                json += ",";
            }

            if (true)
            {
                json += '\n';
                json += "\"index_data\":[";
                for (var i = 0; i < n; i++)
                {
                    var elem = Variabili.L.GetElem(i);
                    bool tenere = DoCheckGruppo(v, elem);
                    if (tenere)
                    {
                        json += '\n';
                        json += elem.To_json(v.n);
                        json += ',';
                    }
                }

                if (json.EndsWith(","))
                {
                    json = json.Substring(0, json.Length - 1);
                }

                json += "]";
            }
            json += "}";

            Salva(json);
        }

        private bool DoCheckGruppo(CheckGruppo v, Gruppo elem)
        {
            switch (v.n)
            {
                case CheckGruppo.E.RICERCA_SITO_V3:
                case CheckGruppo.E.VECCHIA_RICERCA:
                    {
                        if (string.IsNullOrEmpty(elem.Classe))
                            return false;
                        if (string.IsNullOrEmpty(elem.IdLink))
                            return false;
                        break;
                    }
                case CheckGruppo.E.NUOVA_RICERCA:
                    {
                        if (Empty(elem.CCS))
                            return false;

                        break;
                    }
                case CheckGruppo.E.TUTTO:
                    {
                        break;
                    }
            }

            return true;
        }

        private bool Empty(ListaStringhePerJSON cCS)
        {
            if (cCS == null)
                return true;

            return cCS.IsEmpty();
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
                if (!string.IsNullOrEmpty(elem.Id)) continue;
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
            if (s == null)
                return null;

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
            var ods_read = ODS_Reader.Read2(file);

            var nomeOld = new Gruppo();

            int n2 = Variabili.L.GetCount();

            foreach (List<string> y2 in ods_read)
            {
                //Console.WriteLine("----- NUOVA RIGA ------");

                var g = new InsiemeDiGruppi { GruppoDiBase = { Year = year }, NomeOld = nomeOld };

                foreach (string y3 in y2)
                {
                    Gruppo.AggiungiInformazioneAmbigua(y3.ToString().Trim(), ref g);
                }

                g.Aggiusta();

                foreach (var g3 in g.L) Variabili.L.Add(g3, n2 != 0);

                if (!string.IsNullOrEmpty(g.NomeOld.Classe)) nomeOld.Classe = g.NomeOld.Classe;

                if (!string.IsNullOrEmpty(g.NomeOld.Language)) nomeOld.Language = g.NomeOld.Language;

                if (!string.IsNullOrEmpty(g.NomeOld.Degree)) nomeOld.Degree = g.NomeOld.Degree;

                if (!string.IsNullOrEmpty(g.NomeOld.School)) nomeOld.School = g.NomeOld.School;

                if (!Gruppo.IsEmpty(g.NomeOld.Office)) nomeOld.Office = g.NomeOld.Office;

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

            int n2 = Variabili.L.GetCount();

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

                g.Aggiusta(true, true);
                Variabili.L.Add(g, n2 != 0);
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

        public static void BinarySerializeObject(string path, object obj)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                try
                {
                    binaryFormatter.Serialize(streamWriter.BaseStream, obj);
                }
                catch (SerializationException ex)
                {
                    throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
                }
            }
        }

        public static object BinaryDeserializeObject(string path)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object obj;
                try
                {
                    obj = binaryFormatter.Deserialize(streamReader.BaseStream);
                }
                catch (SerializationException ex)
                {
                    throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
                }
                return obj;
            }
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

            string content = null;
            Exception ex = null;
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                ex = e2;
            }

            try
            {
                FileSalvare = JsonConvert.DeserializeObject<FileSalvare>(content);
            }
            catch
            {
                ;
            }

            if (content == null)
            {
                MessageBox.Show("Lettura fallita! \n\n" + ex.Message);
                return;
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
            MessageBox.Show("Prova ad unire terminato!");
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

        public static InfoManifesto infoManifesto = null;

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

            var x1 = LoadManifesto(doc, "TG");
            Variabili.L.Importa(x1, false, Chiedi.SI);
        }

        private List<Gruppo> LoadManifesto(HtmlAgilityPack.HtmlDocument doc, string PLAT2)
        {
            infoManifesto = new InfoManifesto();
            Form1.anno = null;
            Form1.pianostudi2 = null;

            List<Gruppo> L2 = GetGruppiFromDocument(doc, PLAT2);

            if (string.IsNullOrEmpty(pianostudi2) || pianostudi2.Length < 5)
            {
                ;
            }

            for (int i = 0; i < L2.Count; i++)
            {
                L2[i].AggiungiInfoDaManifesto(infoManifesto);
                L2[i].CCS = new ListaStringhePerJSON(infoManifesto.Corso_di_studio);

                L2[i].PianoDiStudi = pianostudi2;
            }

            return L2;
        }

        private List<Gruppo> GetGruppiFromDocument(HtmlAgilityPack.HtmlDocument doc, string pLAT2)
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

            return GetGruppiFromDocument2(L2, pLAT2);
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

        private List<Gruppo> GetGruppiFromDocument2(List<HtmlNode> l2, string pLAT2)
        {
            List<Gruppo> LG = new List<Gruppo>();
            foreach (var x in l2)
            {
                Gruppo x2 = GetGruppiFromDocument3(x, pLAT2);
                if (x2 != null)
                {
                    string x3 = x2.Classe.Trim();
                    if (!string.IsNullOrEmpty(x3))
                    {
                        LG.Add(x2);
                    }
                }
            }

            return LG;
        }

        private Gruppo GetGruppiFromDocument3(HtmlNode x, string pLAT2)
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

            Gruppo g = Gruppo.FromInfoParteList(infoParteDiGruppo_list, pLAT2);
            if (g != null && g.IsValido())
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

        public static int? anno = null;
        public static string pianostudi2 = null;

        private InfoParteDiGruppo GetGruppiFromDocument5(HtmlNode htmlNode)
        {
            bool contiene_table = Contiene_table2(htmlNode);
            if (contiene_table)
                return null;

            IEnumerable<string> classes = htmlNode.GetClasses();
            bool ce = false;
            foreach (var c2 in classes)
            {
                if (c2 == "TitleInfoCard")
                {
                    ce = true;
                    break;
                }
            }

            if (ce)
            {
                string s = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    ;
                }
                else if (s == "Legenda")
                {
                    return null; //sicuro
                }
                else if (s.StartsWith("<span id="))
                {
                    ;
                }
                else if (s.StartsWith("1<sup>"))
                {
                    Form1.anno = 1;
                }
                else if (s.StartsWith("2<sup>"))
                {
                    Form1.anno = 2;
                }
                else if (s.StartsWith("3<sup>"))
                {
                    Form1.anno = 3;
                }
                else if (s.StartsWith("4<sup>"))
                {
                    Form1.anno = 4;
                }
                else if (s.StartsWith("5<sup>"))
                {
                    Form1.anno = 5;
                }
                else if (s.StartsWith("Insegnamenti"))
                {
                    return null; //sicuro
                }
                else
                {
                    ;
                }
            }

            int ce2 = 0;
            foreach (var c2 in classes)
            {
                if (c2 == "ElementInfoCard2" || c2 == "left")
                {
                    ce2++;
                }
            }

            if (ce2 == 2)
            {
                if (htmlNode.ChildNodes.Count > 0)
                {
                    var x1 = htmlNode.ChildNodes[0];
                    ;
                    if (x1.Name == "select")
                    {
                        var x2 = GetPianoStudi(x1);
                        if (x2.Item1)
                        {
                            pianostudi2 = x2.Item2;
                            return null; //sicuro
                        }
                    }
                    else if (htmlNode.InnerHtml.StartsWith("*** - "))
                    {
                        var s3 = htmlNode.InnerHtml.Trim().Split('<');
                        var s4 = s3[0].Trim();
                        var s5 = s4.Substring(5).Trim();
                        pianostudi2 = s5;
                        return null; //sicuro
                    }
                    else
                    {
                        pianostudi2 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                        return null;
                    }
                }
                else
                {
                    ;
                }
            }

            if (htmlNode.ChildNodes.Count == 3)
            {
                if (htmlNode.ChildNodes[0].Name == "#text" &&
                    htmlNode.ChildNodes[1].Name != "#text" &&
                    htmlNode.ChildNodes[2].Name == "#text")
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
                                return new InfoParteDiGruppo(testo_selvaggio: htmlNode.InnerHtml.Trim());
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
                    string s = htmlNode.InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s))
                    {
                        ;
                    }
                    else if (s.StartsWith("Insegnamenti a scelta dal"))
                    {
                        return null;
                    }
                    else if (htmlNode.ChildNodes[0].Name == "a" && htmlNode.ChildNodes[1].Name == "a" &&
                        htmlNode.ChildNodes[2].Name == "span")
                    {
                        LinkGruppo link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes, htmlNode.ChildNodes[1].InnerHtml.Trim());
                        return new InfoParteDiGruppo(link: link2);
                    }
                    else
                    {
                        ;
                    }
                }
            }
            else if (htmlNode.ChildNodes.Count == 1)
            {
                if (htmlNode.ChildNodes[0].Name == "img")
                {
                    string src = htmlNode.ChildNodes[0].Attributes["src"].Value.ToString();
                    if (src.EndsWith("it.png"))
                        return new InfoParteDiGruppo(lingua: Lingua.IT);
                    else if (src.EndsWith("en.png"))
                        return new InfoParteDiGruppo(lingua: Lingua.EN);
                    else if (src.EndsWith("innovativa.png"))
                        return null;
                    else if (src.EndsWith("sequenza.png"))
                        return null;
                    else
                    {
                        ;
                    }
                }

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
                            x1.ChildNodes[2].Name == "#text")
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
                else if (htmlNode.ChildNodes[0].Name == "#text")
                {
                    return new InfoParteDiGruppo(testo_selvaggio: s1);
                }
                else if (htmlNode.ChildNodes[0].Name == "select")
                {
                    return null;
                }
                else
                {
                    string s3 = htmlNode.InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s3))
                    {
                        ;
                    }
                    else if (s3.StartsWith("<span "))
                    {
                        return null;
                    }
                    else
                    {
                        ;
                    }
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
                else if (s.StartsWith("*** - Non diversificato"))
                {
                    return null;
                }
                else if (s.StartsWith("*** - offerta comune"))
                {
                    return null;
                }
                else if (s.StartsWith("*** - URBAN PLANNING AND POLICY DESIGN"))
                {
                    return null;
                }
                else if (s.StartsWith("N1L"))
                {
                    return null;
                }
                else if (s.StartsWith("R1O"))
                {
                    return null;
                }
                else if (s.StartsWith("E1A"))
                {
                    return null;
                }
                else if (s.StartsWith("I1A"))
                {
                    return null;
                }
                else if (s.StartsWith("U1L"))
                {
                    return null;
                }
                else if (s.StartsWith("A1A"))
                {
                    return null;
                }
                else if (s.StartsWith("XEN"))
                {
                    return null;
                }
                else if (s.StartsWith("PSS"))
                {
                    return null;
                }
                else if (s.StartsWith("FOE"))
                {
                    return null;
                }
                else if (s.StartsWith("MOB"))
                {
                    return null;
                }
                else if (s.StartsWith("NDE"))
                {
                    return null;
                }
                else if (s.StartsWith("OA1"))
                {
                    return null;
                }
                else if (s.StartsWith("NDF"))
                {
                    return null;
                }
                else
                {
                    ;
                }
            }
            else if (htmlNode.ChildNodes.Count == 4)
            {
                string s = htmlNode.InnerHtml.Trim();
                if (s.StartsWith("<span id=\"infocard"))
                {
                    return null; //sicuro
                }
                else if (htmlNode.ChildNodes[0].Name == "select")
                {
                    return null; //sicuro
                }
                else
                {
                    return null;
                }
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
                    else if (s1.StartsWith("1.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("9.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("12.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("6.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("4.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("3.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("8.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("16.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("14.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("5.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("18.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("15.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("2.0"))
                    {
                        return null;
                    }
                    else if (s1.StartsWith("7.0"))
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
                        string s = htmlNode.InnerHtml.Trim();
                        if (string.IsNullOrEmpty(s))
                        {
                            ;
                        }
                        else if (s.StartsWith("Insegnamenti a scelta dal"))
                        {
                            return null; //sicuro
                        }
                        else
                        {
                            ;
                        }
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
                else if (htmlNode.ChildNodes[0].Name == "a" && htmlNode.ChildNodes[1].Name == "span")
                {
                    LinkGruppo link2 = new LinkGruppo(htmlNode.ChildNodes[0].Attributes, htmlNode.ChildNodes[0].InnerHtml.Trim());
                    return new InfoParteDiGruppo(link: link2);
                }
                else
                {
                    string s = htmlNode.InnerHtml.Trim();
                    var s2 = s.Split('<');
                    Form1.infoManifesto.Scuola = s2[0].Trim();
                    return null;
                }
            }
            else
            {
                string s = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    ;
                }
                else if (s.StartsWith("4.0"))
                {
                    return null;
                }
                else if (s.StartsWith("ICAR"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else if (s.StartsWith("12.0"))
                {
                    return null;
                }
                else if (s.StartsWith("8.0"))
                {
                    return null;
                }
                else if (s.StartsWith("AGR"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else if (s.StartsWith("Architettura Urbanistica Ingegneria delle Costruzioni"))
                {
                    return null;
                }
                else if (s.StartsWith("BIO"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else if (s.StartsWith("15.0"))
                {
                    return null;
                }
                else if (s.StartsWith("6.0"))
                {
                    return null;
                }
                else if (s.StartsWith("18.0"))
                {
                    return null;
                }
                else if (s.StartsWith("22.0"))
                {
                    return null;
                }
                else if (s.StartsWith("Design"))
                {
                    return null;
                }
                else if (s.StartsWith("ING-IND"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else if (s.StartsWith("Ingegneria Civile"))
                {
                    return null; //sicuro
                }
                else if (s.StartsWith("ING-INF"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else if (s.StartsWith("GEO"))
                {
                    List<InfoParteDiGruppo> sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        string s4 = s3;
                        if (s3.StartsWith("br>"))
                        {
                            s4 = s3.Substring(3);
                        }

                        sottopezzi2.Add(new InfoParteDiGruppo(testo_selvaggio: s4));
                    }
                    return new InfoParteDiGruppo(sottopezzi: sottopezzi2);
                }
                else
                {
                    ;
                }
            }

            return null;
        }

        private Tuple<bool, string> GetPianoStudi(HtmlNode x1)
        {
            if (x1.ChildNodes.Count == 0)
                return new Tuple<bool, string>(false, null);

            bool ce = true;
            foreach (var x2 in x1.ChildNodes)
            {
                if (x2.Name != "option")
                    ce = false;

                if (x2.InnerHtml == "2019/2020" ||
                    x2.InnerHtml == "Qualunque sede" ||
                    x2.InnerHtml == "Scuola di Architettura Urbanistica Ingegneria delle Costruzioni (Arc. Urb. Ing. Cos.)" ||
                    x2.InnerHtml == "1")
                    return new Tuple<bool, string>(false, null);
            }

            if (!ce)
            {
                return new Tuple<bool, string>(false, null);
            }

            ;

            foreach (var x3 in x1.ChildNodes)
            {
                if (x3.Name == "option")
                {
                    foreach (var x4 in x3.Attributes)
                    {
                        if (x4.Name == "selected" && x4.Value == "selected")
                        {
                            return new Tuple<bool, string>(true, x3.InnerHtml.Trim());
                        }
                    }
                }
            }

            return null;
        }

#pragma warning disable IDE0051 // Rimuovi i membri privati inutilizzati

        private HtmlNode GetGruppiFromDocument4(HtmlNode htmlNode)
#pragma warning restore IDE0051 // Rimuovi i membri privati inutilizzati
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
            //todo: fare questo tasto
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            string[] f = null;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    f = Directory.GetFiles(fbd.SelectedPath);
                }
            }

            if (f == null)
                return;

            List<Gruppo> L = new List<Gruppo>();
            foreach (var f2 in f)
            {
                if (f2.EndsWith(".htm"))
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.Load(f2);
                    var L2 = LoadManifesto(doc, "TG");
                    L.AddRange(L2);
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            var r = saveFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                var b2 = ObjectToByteArray(L);
                File.WriteAllBytes(path: saveFileDialog.FileName, bytes: b2);
            }
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
#pragma warning disable IDE0034 // Semplifica l'espressione 'default'
                return default(T);
#pragma warning restore IDE0034 // Semplifica l'espressione 'default'
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            Importa2(Chiedi.SI);
        }

        private void Importa2(Chiedi chiedi2)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var r = openFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                byte[] o2 = File.ReadAllBytes(openFileDialog.FileName);
                List<Gruppo> L2 = FromByteArray<List<Gruppo>>(o2);

                if (Variabili.L == null)
                    Variabili.L = new ListaGruppo();

                Variabili.L.Importa(L2, false, chiedi2);
                MessageBox.Show("Fatto!");
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            Variabili.L.RicreaID();
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            Importa2(Chiedi.NO_DIVERSI);
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.Fix_link_IDCorsi_se_ce_uno_che_ha_il_link_con_id_corso_uguale();
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            Variabili.L.FixPianoStudi();
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            List<string> L = new List<string>
            {
                "Bot telegram",
                "Sito, ricerca vecchia",
                "Sito, ricerca nuova",
                "TUTTO",
                "Sito, ricerca v3 (2021-02)"
            };

            AskFromList askFromList = new AskFromList(L);
            askFromList.ShowDialog();

            if (askFromList.r == null)
            {
                return;
            }

#pragma warning disable CS1690 // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione
            int i = askFromList.r.Value;
#pragma warning restore CS1690 // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione

            bool? entrambi_index = null;
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    {
                        DialogResult dialogResult = MessageBox.Show("Vuoi entrambi gli index (si) o solo uno (no)?", "Scegli", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            entrambi_index = true;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            entrambi_index = false;
                        }

                        if (entrambi_index == null)
                        {
                            MessageBox.Show("Non hai scelto nulla!");
                            return;
                        }

                        break;
                    }
            }

            switch (i)
            {
                case 0:
                    {
                        Button10_Click(null, null);
                        return;
                    }

                case 1:
                    {
                        Button3_Click(CheckGruppo.E.VECCHIA_RICERCA, entrambi_index.Value);
                        return;
                    }

                case 2:
                    {
                        Button3_Click(CheckGruppo.E.NUOVA_RICERCA, entrambi_index.Value);
                        return;
                    }

                case 3:
                    {
                        Button3_Click(CheckGruppo.E.TUTTO, entrambi_index.Value);
                        return;
                    }
                case 4:
                    {
                        Button3_Click(CheckGruppo.E.RICERCA_SITO_V3, false);
                        return;
                    }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            var r = openFileDialog.ShowDialog();
            if (r != DialogResult.OK)
                return;

            ImportaTesto(openFileDialog.FileName);
        }

        private void ImportaTesto(string fileName)
        {
            try
            {
                var r = File.ReadAllLines(fileName);
                ImportaTesto2(r);
            }
            catch
            {
                ;
            }
        }

        private void ImportaTesto2(string[] r)
        {
            if (r == null)
                return;

            string acc = "";

            for (int i = 0; i < r.Length; i++)
            {
                if (string.IsNullOrEmpty(r[i]))
                {
                    acc = "";
                }

                if (string.IsNullOrEmpty(acc) && lastAdded != null && lastAdded.Count > 0)
                {
                    try
                    {
                        bool aggiunto2 = AggiungiTesto("", r[i].Trim(), true);
                        if (aggiunto2)
                            continue;
                    }
                    catch
                    {
                        ;
                    }
                }

                acc += r[i].Trim() + " ";

                bool aggiunto = ValutaSeDaAggiungereENelCasoAggiungi(acc);
                if (aggiunto)
                    acc = "";
                else
                {
                    lastAdded = null;
                }
            }
        }

        private bool ValutaSeDaAggiungereENelCasoAggiungi(string acc)
        {
            if (string.IsNullOrEmpty(acc))
                return false;

            List<string> acc_splitted;
            if (acc.Contains(" ") == false)
            {
                acc_splitted = new List<string>() { acc };
            }
            else
            {
                acc_splitted = acc.Split(' ').ToList();
            }

            Tuple<int?, int?> indexofwebsite = ControllaSeCeUnSito(acc_splitted);
            if (indexofwebsite == null || indexofwebsite.Item1 == null || indexofwebsite.Item2 == null)
                return false;

            string nome = "";
            for (int i = 0; i < acc_splitted.Count; i++)
            {
                if (i != indexofwebsite.Item1.Value)
                {
                    nome += acc_splitted[i].Trim() + " ";
                }
            }

            string url = "";
            if (indexofwebsite.Item2.Value == 0)
            {
                url = acc_splitted[indexofwebsite.Item1.Value];
            }
            else
            {
                ;
            }

            return AggiungiTesto(nome, url, false);
        }

        private List<string> lastAdded = null;

        private bool AggiungiTesto(string nome, string url, bool gruppoAdded)
        {
            int? r2 = ControllaSeCeUnSito2(url);
            if (r2 == null || r2 != 0)
                return false;

            if (gruppoAdded && (lastAdded != null && lastAdded.Count > 0))
            {
                Gruppo g2 = new Gruppo
                {
                    NomeCorso = lastAdded[lastAdded.Count - 1].Trim(),
                    IdLink = url.Trim(),
                    Id = url.Trim()
                };
                g2.RicreaId();
                bool ce_gia2 = VediSeCeGiaDaURL(url);
                if (ce_gia2 == false)
                {
                    if (lastAdded == null)
                        lastAdded = new List<string>();

                    lastAdded.Add(g2.NomeCorso);
                    Variabili.L.Add(g2, false);
                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(url))
                return false;

            Gruppo g = new Gruppo
            {
                NomeCorso = nome.Trim(),
                IdLink = url.Trim(),
                Id = url.Trim()
            };
            g.RicreaId();

            bool ce_gia = VediSeCeGiaDaURL(url);
            if (ce_gia == false)
            {
                if (lastAdded == null)
                    lastAdded = new List<string>();

                lastAdded.Add(g.NomeCorso);
                Variabili.L.Add(g, false);
                return true;
            }

            return false;
        }

        private bool VediSeCeGiaDaURL(string url)
        {
            return Variabili.L.VediSeCeGiaDaURL(url);
        }

        private Tuple<int?, int?> ControllaSeCeUnSito(List<string> acc)
        {
            if (acc == null)
                return null;

            for (int i = 0; i < acc.Count; i++)
            {
                int? r = ControllaSeCeUnSito2(acc[i]);
                if (r != null)
                    return new Tuple<int?, int?>(i, r);
            }

            return null;
        }

        private int? ControllaSeCeUnSito2(string v)
        {
            bool result = Uri.TryCreate(v, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
                return 0;

            int i = v.IndexOf("http://");
            if (i >= 0)
                return i;

            i = v.IndexOf("https://");
            if (i >= 0)
                return i;

            return null;
        }

        private void Button10_Click_1(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            var r = openFileDialog.ShowDialog();
            if (r != DialogResult.OK)
            {
                return;
            }

            var read = File.ReadAllText(openFileDialog.FileName);

            try
            {
                ImportaSQL(read);
            }
            catch
            {
                ;
            }
        }

        private void ImportaSQL(string read)
        {
            string[] s = read.Split(new string[] { "INSERT INTO \"Groups\" " }, StringSplitOptions.None);

            ;

            foreach (var s2 in s)
            {
                try
                {
                    ImportaSQL2(s2);
                }
#pragma warning disable IDE0059 // Assegnazione non necessaria di un valore
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
                catch (Exception e2)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
#pragma warning restore IDE0059 // Assegnazione non necessaria di un valore
                {
                    ;
                }
            }
        }

        private void ImportaSQL2(string s2)
        {
            if (string.IsNullOrEmpty(s2))
                return;

            if (s2.StartsWith("(") == false)
                return;

            ;

            var s3 = s2.Split(new string[] { "VALUES" }, StringSplitOptions.None);

            var s4 = s3[1];

            ;

            s4 = s4.Trim();

            ;

            int lastSemiColomn = s4.LastIndexOf(";");
            if (lastSemiColomn < 0 || lastSemiColomn >= s4.Length)
                return;

            string s5 = s4.Substring(0, lastSemiColomn).Trim();

            ;

            ImportaSQL3(s5);
        }

        private void ImportaSQL3(string s5)
        {
            ;

            var s6 = s5.Split(',');

            ;

            if (s6.Length == 7)
            {
                ImportaSQL4(s6);
                return;
            }

            ;

            List<string> s7 = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                s7.Add(s6[i]);
            }

            ;

            string nome = "";
            for (int i = 6; i < s6.Length; i++)
            {
                nome += s6[i] + ",";
            }
            if (nome[nome.Length - 1] == ',')
            {
                nome = nome.Remove(nome.Length - 1);
            }

            s7.Add(nome);

            ImportaSQL4(s7.ToArray());
        }

        private void ImportaSQL4(string[] s6)
        {
            long id = Convert.ToInt64(s6[0].Substring(1).Trim());
            int? id_in_ram = FindInRamSQL(id);
            if (id_in_ram == null)
            {
                ImportaSQL5(s6);
            }
            else
            {
                ImportaSQL6(s6, id_in_ram.Value);
            }
        }

        private void ImportaSQL6(string[] s6, int GroupId)
        {
            Gruppo g = OttieniGruppoSQL(s6);

            Variabili.L.AddAndMerge(g, GroupId);
        }

        private void ImportaSQL5(string[] s6)
        {
            ;

            Gruppo g = OttieniGruppoSQL(s6);

            //this group is not in ram, we have to add it
            Variabili.L.Add(g, false);
        }

        private Gruppo OttieniGruppoSQL(string[] s6)
        {
            var s7 = s6[3].Split('/');

            Gruppo g = new Gruppo
            {
                PermanentId = s6[0].Substring(1).Trim(),
                Platform = "TG",
                IdLink = s7[s7.Length - 1].Trim()
            };

            if (g.IdLink[g.IdLink.Length - 1] == '\'')
            {
                g.IdLink = g.IdLink.Remove(g.IdLink.Length - 1);
            }
            g.Classe = s6[6].Trim();
            if (g.Classe[g.Classe.Length - 1] == ')')
            {
                g.Classe = g.Classe.Remove(g.Classe.Length - 1);
            }
            if (g.Classe[g.Classe.Length - 1] == '\'')
            {
                g.Classe = g.Classe.Remove(g.Classe.Length - 1);
            }
            if (g.Classe[0] == '\'')
            {
                g.Classe = g.Classe.Substring(1).Trim();
            }

            int semicolomn = g.Classe.IndexOf(");");
            if (semicolomn >= 0 && semicolomn < g.Classe.Length)
            {
                g.Classe = g.Classe.Substring(0, semicolomn).Trim();
            }

            g.LastUpdateInviteLinkTime = ToDateTime(s6[4]);

            g.Aggiusta(false, true);
            return g;
        }

        private DateTime? ToDateTime(string v)
        {
            var v2 = v.Trim().Split(' ');

            if (v2.Length < 2)
                return null;

            var v3 = v2[0].Split('-');
            var v4 = v2[1].Split('.');
            var v5 = v4[0].Trim().Split(':');

            ;

            if (v3[0].StartsWith("'"))
            {
                v3[0] = v3[0].Substring(1);
            }

            int anno = Convert.ToInt32(v3[0]);
            int mese = Convert.ToInt32(v3[1]);
            int giorno = Convert.ToInt32(v3[2]);
            int ora = Convert.ToInt32(v5[0]);
            int minuto = Convert.ToInt32(v5[1]);

            if (v5[2][v5[2].Length - 1] == '\'')
            {
                v5[2] = v5[2].Remove(v5[2].Length - 1);
            }

            int secondo = Convert.ToInt32(v5[2]);

            int millisec = 0;
            if (v4.Length > 1)
            {
                if (v4[1][v4[1].Length - 1] == '\'')
                {
                    v4[1] = v4[1].Remove(v4[1].Length - 1);
                }

                if (v4[1].Length > 3)
                {
                    v4[1] = v4[1].Substring(0, 3);
                }

                millisec = Convert.ToInt32(v4[1]);
            }

            return new DateTime(anno,
                mese,
                giorno,
                ora,
                minuto,
                secondo,
                millisec);
        }

        private int? FindInRamSQL(long id)
        {
            ;

            return Variabili.L.FindInRamSQL(id);
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                Variabili.L = new ListaGruppo();
            }

            Variabili.L.AggiustaNomiDoppi();
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.CheckSeILinkVanno(saltaQuelliGiaFunzionanti: false);

            MessageBox.Show("Finito il check dei link!");
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                Variabili.L = new ListaGruppo();
            }

            Variabili.L.SalvaTelegramIdDeiGruppiLinkCheNonVanno(textBox1.Text);
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                Variabili.L = new ListaGruppo();
            }

            Variabili.L.ImportaGruppiDalComandoDelBotTelegram_UpdateLinkFromJson();
        }

        private void Button24_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                Variabili.L = new ListaGruppo();
            }

            Variabili.L.ImportaGruppiDaTabellaTelegramGruppiBot_PuntoBin();
            MessageBox.Show("Gruppi importati dalla tabella telegram del bot!");
        }

        private void Button25_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.CheckSeILinkVanno(saltaQuelliGiaFunzionanti: true);

            MessageBox.Show("Finito il check dei link!");
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.StampaWhatsapp();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Variabili.L.AggiustaGruppiDoppiIDTelegramUguale();
        }
    }
}