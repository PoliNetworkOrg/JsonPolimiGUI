using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using HtmlAgilityPack;
using JsonPolimi_Core_nf.Data;
using JsonPolimi_Core_nf.Enums;
using JsonPolimi_Core_nf.Tipi;
using JsonPolimi_Core_nf.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.Enums;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace JsonPolimi.Forms;

public partial class MainForm : Form
{
    private static FileSalvare? FileSalvare;

    private static ParametriCondivisi? parametriCondivisi;

    private List<string>? lastAdded;

    public MainForm(ParametriCondivisi? parametriCondivisiParam)
    {
        parametriCondivisi = parametriCondivisiParam;
        InitializeComponent();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

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
        var count = 0;

        JEnumerable<JToken>? i;
        if (infoData == null)
        {
            ;
            var i4 = stuff.First;
            ;
            var i5 = i4?.First;
            ;
            i = i5?.Children();
            if (i != null)
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

    private static void Aggiungi(JToken? i, bool aggiusta_Anno, bool merge)
    {
        var g = new Gruppo
        {
            Classe = i?["class"]?.ToString(),
            Degree = i?["degree"]?.ToString()
        };

        try
        {
            try
            {
                g.Platform = i?["group_type"]?.ToString();
            }
            catch
            {
                g.Platform = i?["platform"]?.ToString();
            }
        }
        catch
        {
            g.Platform = i?["platform"]?.ToString();
        }

        g.Id = i?["id"]?.ToString();
        g.Language = i?["language"]?.ToString();
        g.Office = new ListaStringhePerJSON(i?["office"]?.ToString());
        g.School = i?["school"]?.ToString();
        g.IdLink = i?["id_link"]?.ToString();

        try
        {
            g.Tipo = i?["type"]?.ToString();
        }
        catch
        {
            g.Tipo = null;
        }

        try
        {
            g.Year = i?["year"]?.ToString();
        }
        catch
        {
            g.Year = null;
        }

        try
        {
            g.PermanentId = i?["permanentId"]?.ToString();
        }
        catch
        {
            g.PermanentId = null;
        }

        try
        {
            g.CCS = new ListaStringhePerJSON(i?["ccs"]?.ToString());
        }
        catch
        {
            g.CCS = null;
        }

        try
        {
            var s = i?["annocorso"]?.ToString();
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
            g.IDCorsoPolimi = i?["idcorso"]?.ToString();
        }
        catch
        {
            g.IDCorsoPolimi = null;
        }

        try
        {
            g.PianoDiStudi = i?["pianostudi"]?.ToString();
        }
        catch
        {
            g.PianoDiStudi = null;
        }

        object? data2 = i?["LastUpdateInviteLinkTime"];

        string? data = null;
        try
        {
            data = data2?.ToString();
        }
        catch
        {
            ;
        }

        try
        {
            g.LastUpdateInviteLinkTime = Dates.DataFromString(data);
        }
        catch (Exception e)
        {
            g.LastUpdateInviteLinkTime = null;
            throw e;
        }

        g.LinkFunzionante = BoolFromString(i?["linkfunzionante"]);

        g.Aggiusta(aggiusta_Anno, false);

        Variabili.L.Add(g, merge);
    }

    private static bool? BoolFromString(JToken? jTokens)
    {
        try
        {
            var s = jTokens?.ToString();
            switch (s)
            {
                case "Y":
                    return true;
                case "N":
                    return false;
            }
        }
        catch
        {
            ;
        }

        return null;
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        Aggiusta();
        var generaTabellaHtml = new GeneraTabellaHTML();
        generaTabellaHtml.Show();
    }

    private static void Aggiusta()
    {
        if (Variabili.L == null)
            return;

        var list = Variabili.L.GetGroups();

        foreach (var i in list) i.Aggiusta(true, true);
    }

    private static void Button3_Click(CheckGruppo.E i, bool? entrambiIndex)
    {
        Salva_Generico(new CheckGruppo(i), entrambiIndex);
    }

    private static void Salva_Generico(CheckGruppo v, bool? entrambiIndex)
    {
        if (Variabili.L == null)
        {
            MessageBox.Show("Lista vuota!");
            return;
        }

        var index = entrambiIndex ?? false;
        var json = JsonBuilder.GetJson(v, index);

        Salva(json);
    }


    private static void Salva(string json)
    {
        var o = new SaveFileDialog();
        o.Filter = "Json files (*.json)|*.json";
        var r = o.ShowDialog();
        if (r != DialogResult.OK)
        {
            o.Dispose();
            return;
        }

        File.WriteAllText(o.FileName, json);
        o.Dispose();
    }


    private void Button4_Click(object sender, EventArgs e)
    {
        var x = new AggiungiForm(false);
        x.ShowDialog();
        x.Dispose();
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

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

        var n2 = Variabili.L.GetCount();

        foreach (var y2 in ods_read)
        {
            //Console.WriteLine("----- NUOVA RIGA ------");

            var g = new InsiemeDiGruppi { GruppoDiBase = { Year = year }, NomeOld = nomeOld };

            foreach (var y3 in y2) Gruppo.AggiungiInformazioneAmbigua(y3.Trim(), ref g);

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
        Variabili.L ??= new ListaGruppo();

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

        FileSalvare ??= new FileSalvare();

        Variabili.L ??= new ListaGruppo();

        var n2 = Variabili.L.GetCount();

        foreach (var g in from r in FileSalvare.Gruppi
                 where r.Chat.Type != ChatType.Private
                 where r.we_are_admin != false
                 select new Gruppo
                 {
                     Classe = r.Chat.Title,
                     PermanentId = r.Chat.Id.ToString(),
                     Platform = "TG",
                     IdLink = TelegramLinkLastPart(r.Chat.InviteLink),
                     Tipo = "C",
                     LastUpdateInviteLinkTime = r.LastUpdateInviteLinkTime
                 })
        {
            g.Aggiusta(true, true);
            Variabili.L.Add(g, n2 != 0);
        }

        Variabili.L.Sort();
    }

    private static string? TelegramLinkLastPart(string? chatInviteLink)
    {
        if (string.IsNullOrEmpty(chatInviteLink))
            return null;

        string?[] r = chatInviteLink.Split('/');
        return r[^1];
    }

    public static void BinarySerializeObject(string path, object obj)
    {
        using var streamWriter = new StreamWriter(path);
        var binaryFormatter = new BinaryFormatter();
        try
        {
            binaryFormatter.Serialize(streamWriter.BaseStream, obj);
        }
        catch (SerializationException ex)
        {
            throw new SerializationException(ex + "\n" + ex.Source);
        }
    }

    public static object BinaryDeserializeObject(string path)
    {
        using var streamReader = new StreamReader(path);
        var binaryFormatter = new BinaryFormatter();
        object obj;
        try
        {
            obj = binaryFormatter.Deserialize(streamReader.BaseStream);
        }
        catch (SerializationException ex)
        {
            throw new SerializationException(ex + "\n" + ex.Source);
        }

        return obj;
    }

    private static void LoadGruppi()
    {
        var ofd = new OpenFileDialog();
        var rDialog = ofd.ShowDialog();
        if (rDialog != DialogResult.OK)
        {
            ofd.Dispose();
            return;
        }

        string? content = null;
        Exception? ex = null;
        try
        {
            content = File.ReadAllText(ofd.FileName);
        }
        catch (Exception? e2)
        {
            ex = e2;
        }

        try
        {
            if (content != null) 
                FileSalvare = JsonConvert.DeserializeObject<FileSalvare?>(content);
        }
        catch
        {
            ;
        }

        if (content == null)
        {
            MessageBox.Show("Lettura fallita! \n\n" + ex?.Message);
            return;
        }

        FileSalvare ??= new FileSalvare();

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

    private static void Button10_Click(object? sender, EventArgs? e)
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

        if (json[^1] == ',')
            json = json.Remove(json.Length - 1);

        json += "]}";

        Salva(json);
    }

    private void Button8_Click2(object sender, EventArgs e)
    {
        var doc = new HtmlDocument();
        using (var o = new OpenFileDialog())
        {
            var r = o.ShowDialog();
            if (r != DialogResult.OK) return;

            doc.Load(o.FileName);
        }

        var x1 = LoadManifesto(doc, "TG");
        Importa4(x1, Chiedi.SI);
    }

    private static void Importa4(IReadOnlyList<Tuple<Gruppo>>? x1, Chiedi sI)
    {
        var r2 = Variabili.L.Importa(x1, false, sI);
        for (var i = 0; i < r2.Count; i++)
        {
            var r3 = r2[i];
            switch (r3.actionDoneImport)
            {
                case ActionDoneImport.IMPORTED:
                    break;

                case ActionDoneImport.ADDED:
                    break;

                case ActionDoneImport.SIMILARITIES_FOUND:
                {
                    var importato = false;
                    foreach (var r4 in r3.simili)
                    {
                        var askToUnifyForm = new AskToUnifyForm(r4.Item2, r3.simili.Count)
                        {
                            StartPosition = FormStartPosition.CenterScreen
                        };
                        askToUnifyForm.ShowDialog();
                        if (askToUnifyForm.r == null) continue;
#pragma warning disable CS1690 // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione
                        if (!askToUnifyForm.r.Value) continue;
                        // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione
                        Variabili.L.Importa3(r4.Item1, r4.Item2);
                        importato = true;
                        break;
                    }

                    if (importato == false) Variabili.L.Add(x1?[i].Item1, false);
                    break;
                }
            }
        }
    }

    private static List<Tuple<Gruppo>> LoadManifesto(HtmlDocument doc, string plat2)
    {
        parametriCondivisi ??= new ParametriCondivisi();
        parametriCondivisi.infoManifesto = new InfoManifesto();
        parametriCondivisi.anno = null;
        parametriCondivisi.pianostudi2 = null;

        var l2 = GetGruppiFromDocument(doc, plat2);

        if (string.IsNullOrEmpty(parametriCondivisi.pianostudi2) || parametriCondivisi.pianostudi2.Length < 5) ;

        foreach (var t in l2)
        {
            t.Item1.AggiungiInfoDaManifesto(parametriCondivisi.infoManifesto);
            t.Item1.CCS = new ListaStringhePerJSON(parametriCondivisi.infoManifesto.Corso_di_studio);

            t.Item1.PianoDiStudi = parametriCondivisi.pianostudi2;
        }

        return l2;
    }

    private static List<Tuple<Gruppo>> GetGruppiFromDocument(HtmlDocument doc, string pLAT2)
    {
        var L = GetTables(doc.DocumentNode.ChildNodes);
        var L2 = new List<HtmlNode>();

        while (L.Count > 0)
        {
            if (L[0].Name == "tr")
            {
                if (L[0].ChildNodes.Count > 0) L2.Add(L[0]);
            }
            else
            {
                L.AddRange(L[0].ChildNodes);
            }

            L.RemoveAt(0);
        }

        return GetGruppiFromDocument2(L2, pLAT2);
    }

    private static List<HtmlNode> GetTables(HtmlNodeCollection childNodes)
    {
        var L = new List<HtmlNode>();
        var L2 = new List<HtmlNode>();
        L.AddRange(childNodes);

        while (L.Count > 0)
        {
            if (L[0].Name == "table") L2.Add(L[0]);

            L.AddRange(L[0].ChildNodes);

            L.RemoveAt(0);
        }

        return L2;
    }

    private static List<Tuple<Gruppo>> GetGruppiFromDocument2(IEnumerable<HtmlNode> l2, string pLat2)
    {
        return (
            from x in l2
            select GetGruppiFromDocument3(x, pLat2)
            into x2
            where x2 != null
            let x3 = x2.Classe.Trim()
            where !string.IsNullOrEmpty(x3)
            select new Tuple<Gruppo>(x2)
        ).ToList();
    }

    private static Gruppo? GetGruppiFromDocument3(HtmlNode x, string pLat2)
    {
        var l = x.ChildNodes.Where(t => t.Name == "td").ToList();

        var infoParteDiGruppoList = l.Select(GetGruppiFromDocument5).ToList();

        var g = Gruppo.FromInfoParteList(infoParteDiGruppoList, pLat2);
        if (g != null && g.IsValido())
            return g;
        return null;
    }

    private static bool Contiene_table2(HtmlNode htmlNode)
    {
        return htmlNode.ChildNodes.Any(x => x.Name == "table");
    }

    private static InfoParteDiGruppo? GetGruppiFromDocument5(HtmlNode htmlNode)
    {
        var contieneTable = Contiene_table2(htmlNode);
        if (contieneTable)
            return null;

        var classes = htmlNode.GetClasses();
        var enumerable = classes.ToList();
        var ce = enumerable?.Any(c2 => c2 == "TitleInfoCard");

        parametriCondivisi ??= new ParametriCondivisi();
        if (ce ?? false)
        {
            var s = htmlNode.InnerHtml.Trim();
            if (string.IsNullOrEmpty(s))
                ;
            else if (s == "Legenda")
                return null; //sicuro
            else if (s.StartsWith("<span id="))
                ;
            else if (s.StartsWith("1<sup>"))
                parametriCondivisi.anno = 1;
            else if (s.StartsWith("2<sup>"))
                parametriCondivisi.anno = 2;
            else if (s.StartsWith("3<sup>"))
                parametriCondivisi.anno = 3;
            else if (s.StartsWith("4<sup>"))
                parametriCondivisi.anno = 4;
            else if (s.StartsWith("5<sup>"))
                parametriCondivisi.anno = 5;
            else if (s.StartsWith("Insegnamenti"))
                return null; //sicuro
            else
                ;
        }

        var ce2 = enumerable?.Count(c2 => c2 is "ElementInfoCard2" or "left");

        if (ce2 == 2)
        {
            if (htmlNode.ChildNodes.Count > 0)
            {
                var x1 = htmlNode.ChildNodes[0];
                parametriCondivisi ??= new ParametriCondivisi();
                
                if (x1.Name == "select")
                {
                    var x2 = GetPianoStudi(x1);
                    if (x2?.Item1 ?? false)
                    {
                        parametriCondivisi.pianostudi2 = x2.Item2;
                        return null; //sicuro
                    }
                }
                else if (htmlNode.InnerHtml.StartsWith("*** - "))
                {
                    var s3 = htmlNode.InnerHtml.Trim().Split('<');
                    var s4 = s3[0].Trim();
                    var s5 = s4[5..].Trim();
                    parametriCondivisi.pianostudi2 = s5;
                    return null; //sicuro
                }
                else
                {
                    parametriCondivisi.pianostudi2 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                    return null;
                }
            }
            else
            {
                ;
            }
        }

        switch (htmlNode.ChildNodes.Count)
        {
            case 3:
                switch (htmlNode.ChildNodes[0].Name)
                {
                    case "#text" when
                        htmlNode.ChildNodes[1].Name != "#text" &&
                        htmlNode.ChildNodes[2].Name == "#text":
                    {
                        var s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                        var s2 = htmlNode.ChildNodes[2].InnerHtml.Trim();
                        if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                        {
                            switch (htmlNode.ChildNodes[1].Name)
                            {
                                case "div":
                                {
                                    var x1 = htmlNode.ChildNodes[1];
                                    if (x1.ChildNodes.Count == 1)
                                    {
                                        if (x1.ChildNodes[0].Name == "#text")
                                            return new InfoParteDiGruppo(x1.ChildNodes[0].InnerHtml.Trim());
                                        ;
                                    }
                                    else
                                    {
                                        ;
                                    }

                                    break;
                                }
                                case "a":
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
                                                var immagine2 = new ImmagineGruppo(x1.ChildNodes[1]);
                                                return new InfoParteDiGruppo(immagine2);
                                            }

                                            ;
                                        }
                                        else
                                        {
                                            ;
                                        }
                                    }
                                    else
                                    {
                                        var s = htmlNode.InnerHtml.Trim();
                                        if (string.IsNullOrEmpty(s))
                                            return null;
                                        return new InfoParteDiGruppo(
                                            new LinkGruppo(
                                                htmlNode.ChildNodes[1].Attributes,
                                                htmlNode.ChildNodes[1].InnerHtml.Trim()
                                            )
                                        );
                                    }

                                    break;
                                }
                                default:
                                    ;
                                    break;
                            }
                        }
                        else
                        {
                            var s3 = htmlNode.InnerHtml.Trim();
                            if (string.IsNullOrEmpty(s3)) return null;

                            if (s3.StartsWith("Ingegneria Industriale e dell'Informazione")) return null; //sicuro

                            switch (s3)
                            {
                                case "1<sup><small>o</small></sup>Anno":
                                //sicuro
                                case "2<sup><small>o</small></sup>Anno":
                                //sicuro
                                case "3<sup><small>o</small></sup>Anno":
                                    return null; //sicuro
                            }

                            var s4 = s1 + "<br>" + s2;
                            if (s4 != s3) return new InfoParteDiGruppo(htmlNode.InnerHtml.Trim());
                            var sottopezzi2 = new List<InfoParteDiGruppo>
                            {
                                new(s1),
                                new(s2)
                            };
                            return new InfoParteDiGruppo(sottopezzi2);
                        }

                        break;
                    }
                    case "img" when
                        htmlNode.ChildNodes[1].Name == "#text" &&
                        htmlNode.ChildNodes[2].Name == "img":
                    {
                        if (htmlNode.ChildNodes[0].Attributes["src"].Value.Contains("/it.png"))
                        {
                            if (htmlNode.ChildNodes[2].Attributes["src"].Value.Contains("/en.png"))
                            {
                                var s = htmlNode.ChildNodes[1].InnerHtml.Trim();
                                if (string.IsNullOrEmpty(s))
                                    ;
                                else if (s == "/")
                                    return null; //sicuro
                                else
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

                        break;
                    }
                    default:
                    {
                        var s = htmlNode.InnerHtml.Trim();
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
                            var link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes,
                                htmlNode.ChildNodes[1].InnerHtml.Trim());
                            return new InfoParteDiGruppo(link2);
                        }
                        else
                        {
                            ;
                        }

                        break;
                    }
                }

                break;
            case 1:
            {
                if (htmlNode.ChildNodes[0].Name == "img")
                {
                    var src = htmlNode.ChildNodes[0].Attributes["src"].Value;
                    if (src.EndsWith("it.png"))
                        return new InfoParteDiGruppo(Lingua.IT);
                    if (src.EndsWith("en.png"))
                        return new InfoParteDiGruppo(Lingua.EN);
                    if (src.EndsWith("innovativa.png"))
                        return null;
                    if (src.EndsWith("sequenza.png"))
                        return null;
                    ;
                }

                var s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1)) return null; //sono sicuro

                switch (htmlNode.ChildNodes[0].Name)
                {
                    case "a":
                    {
                        var x1 = htmlNode.ChildNodes[0];
                        switch (x1.ChildNodes.Count)
                        {
                            case 3 when x1.ChildNodes[0].Name == "#text" &&
                                        x1.ChildNodes[1].Name == "div" &&
                                        x1.ChildNodes[2].Name == "#text":
                            {
                                var s11 = x1.ChildNodes[0].InnerHtml.Trim();
                                var s22 = x1.ChildNodes[2].InnerHtml.Trim();

                                if (string.IsNullOrEmpty(s11) && string.IsNullOrEmpty(s22))
                                {
                                    var x2 = x1.ChildNodes[1];
                                    switch (x2.ChildNodes.Count)
                                    {
                                        case 3:
                                            ;
                                            break;
                                        case 5:
                                            return null;
                                        default:
                                            ;
                                            break;
                                    }
                                }
                                else
                                {
                                    ;
                                }

                                break;
                            }
                            case 3:
                                ;
                                break;
                            case 1:
                            {
                                var x2 = x1.ChildNodes[0];
                                if (x2.ChildNodes.Count == 0)
                                {
                                    var link2 = new LinkGruppo(htmlNode.ChildNodes[0].Attributes, x2.InnerHtml.Trim());
                                    return new InfoParteDiGruppo(link2);
                                }

                                ;
                                break;
                            }
                            default:
                                ;
                                break;
                        }

                        break;
                    }
                    case "#text":
                        return new InfoParteDiGruppo(s1);
                    case "select":
                        return null;
                    default:
                    {
                        var s3 = htmlNode.InnerHtml.Trim();
                        if (string.IsNullOrEmpty(s3))
                            ;
                        else if (s3.StartsWith("<span "))
                            return null;
                        else
                            ;
                        break;
                    }
                }

                break;
            }
            case 6:
            {
                var s = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s))
                    ;
                else if (s.StartsWith("I CFU riportati a fianco a questo"))
                    return null; //sicuro
                else if (s.StartsWith("*** - Non diversificato"))
                    return null;
                else if (s.StartsWith("*** - offerta comune"))
                    return null;
                else if (s.StartsWith("*** - URBAN PLANNING AND POLICY DESIGN"))
                    return null;
                else if (s.StartsWith("N1L"))
                    return null;
                else if (s.StartsWith("R1O"))
                    return null;
                else if (s.StartsWith("E1A"))
                    return null;
                else if (s.StartsWith("I1A"))
                    return null;
                else if (s.StartsWith("U1L"))
                    return null;
                else if (s.StartsWith("A1A"))
                    return null;
                else if (s.StartsWith("XEN"))
                    return null;
                else if (s.StartsWith("PSS"))
                    return null;
                else if (s.StartsWith("FOE"))
                    return null;
                else if (s.StartsWith("MOB"))
                    return null;
                else if (s.StartsWith("NDE"))
                    return null;
                else if (s.StartsWith("OA1"))
                    return null;
                else if (s.StartsWith("NDF"))
                    return null;
                else
                    ;
                break;
            }
            case 4:
            {
                var s = htmlNode.InnerHtml.Trim();
                if (s.StartsWith("<span id=\"infocard"))
                    return null; //sicuro
                if (htmlNode.ChildNodes[0].Name == "select")
                    return null; //sicuro
                return null;
            }
            case 0:
            {
                var s1 = htmlNode.InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1))
                    return null; //sicuro
                ;
                break;
            }
            case 2 when htmlNode.ChildNodes[0].Name == "#text" && htmlNode.ChildNodes[1].Name == "div":
            {
                var s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1))
                    ;
                else if (s1 == "10.0")
                    return null;
                else if (s1.StartsWith("1.0"))
                    return null;
                else if (s1.StartsWith("9.0"))
                    return null;
                else if (s1.StartsWith("12.0"))
                    return null;
                else if (s1.StartsWith("6.0"))
                    return null;
                else if (s1.StartsWith("4.0"))
                    return null;
                else if (s1.StartsWith("3.0"))
                    return null;
                else if (s1.StartsWith("8.0"))
                    return null;
                else if (s1.StartsWith("16.0"))
                    return null;
                else if (s1.StartsWith("14.0"))
                    return null;
                else if (s1.StartsWith("5.0"))
                    return null;
                else if (s1.StartsWith("18.0"))
                    return null;
                else if (s1.StartsWith("15.0"))
                    return null;
                else if (s1.StartsWith("2.0"))
                    return null;
                else if (s1.StartsWith("7.0"))
                    return null;
                else
                    ;
                break;
            }
            case 2 when htmlNode.ChildNodes[0].Name == "#text" && htmlNode.ChildNodes[1].Name == "a":
            {
                var s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1))
                {
                    var x1 = htmlNode.ChildNodes[1];
                    if (x1.ChildNodes.Count == 1)
                    {
                        if (x1.ChildNodes[0].Name == "#text")
                        {
                            var link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes,
                                x1.ChildNodes[0].InnerHtml.Trim());
                            return new InfoParteDiGruppo(link2);
                        }

                        ;
                    }
                    else
                    {
                        ;
                    }
                }
                else
                {
                    var s = htmlNode.InnerHtml.Trim();
                    if (string.IsNullOrEmpty(s))
                        ;
                    else if (s.StartsWith("Insegnamenti a scelta dal"))
                        return null; //sicuro
                    else
                        ;
                }

                break;
            }
            case 2 when htmlNode.ChildNodes[0].Name == "a" && htmlNode.ChildNodes[1].Name == "a":
            {
                var s1 = htmlNode.ChildNodes[0].InnerHtml.Trim();
                var s2 = htmlNode.ChildNodes[1].InnerHtml.Trim();
                if (string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
                {
                    var link2 = new LinkGruppo(htmlNode.ChildNodes[1].Attributes, s2);
                    return new InfoParteDiGruppo(link2);
                }

                if (!string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                    ;
                else
                    ;
                break;
            }
            case 2 when htmlNode.ChildNodes[0].Name == "a" && htmlNode.ChildNodes[1].Name == "span":
            {
                var link2 = new LinkGruppo(htmlNode.ChildNodes[0].Attributes, htmlNode.ChildNodes[0].InnerHtml.Trim());
                return new InfoParteDiGruppo(link2);
            }
            case 2:
            {
                var s = htmlNode.InnerHtml.Trim();
                var s2 = s.Split('<');
                parametriCondivisi.infoManifesto.Scuola = s2[0].Trim();
                return null;
            }
            default:
            {
                var s = htmlNode.InnerHtml.Trim();
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
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
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
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
                }
                else if (s.StartsWith("Architettura Urbanistica Ingegneria delle Costruzioni"))
                {
                    return null;
                }
                else if (s.StartsWith("BIO"))
                {
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
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
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
                }
                else if (s.StartsWith("Ingegneria Civile"))
                {
                    return null; //sicuro
                }
                else if (s.StartsWith("ING-INF"))
                {
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
                }
                else if (s.StartsWith("GEO"))
                {
                    var sottopezzi2 = new List<InfoParteDiGruppo>();
                    var s2 = s.Split('<');
                    foreach (var s3 in s2)
                    {
                        var s4 = s3;
                        if (s3.StartsWith("br>")) s4 = s3[3..];

                        sottopezzi2.Add(new InfoParteDiGruppo(s4));
                    }

                    return new InfoParteDiGruppo(sottopezzi2);
                }
                else
                {
                    ;
                }

                break;
            }
        }

        return null;
    }

    private static Tuple<bool, string?>? GetPianoStudi(HtmlNode x1)
    {
        if (x1.ChildNodes.Count == 0)
            return new Tuple<bool, string?>(false, null);

        var ce = true;
        foreach (var x2 in x1.ChildNodes)
        {
            if (x2.Name != "option")
                ce = false;

            if (x2.InnerHtml is "2019/2020" or "Qualunque sede"
                or "Scuola di Architettura Urbanistica Ingegneria delle Costruzioni (Arc. Urb. Ing. Cos.)" or "1")
                return new Tuple<bool, string?>(false, null);
        }

        if (!ce) return new Tuple<bool, string?>(false, null);

        return (from x3 in x1.ChildNodes
            where x3.Name == "option"
            where x3.Attributes.Any(x4 => x4.Name == "selected" && x4.Value == "selected")
            select new Tuple<bool, string>(true, x3.InnerHtml.Trim())).FirstOrDefault();
    }

#pragma warning disable IDE0051 // Rimuovi i membri privati inutilizzati

    private static HtmlNode GetGruppiFromDocument4(HtmlNode htmlNode)
#pragma warning restore IDE0051 // Rimuovi i membri privati inutilizzati
    {
        if (htmlNode.ChildNodes.Count == 0)
            return htmlNode;

        var LC = new List<char>
        {
            '\t',
            '\r'
        };

        foreach (var x in htmlNode.ChildNodes)
        {
            var buono = x.InnerText.All(x2 => !LC.Contains(x2));

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
        string[]? f = null;
        using (var fbd = new FolderBrowserDialog())
        {
            var result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                f = Directory.GetFiles(fbd.SelectedPath);
        }

        if (f == null)
            return;

        var l = new List<Gruppo>();
        foreach (var f2 in f)
            if (f2.EndsWith(".htm"))
            {
                var doc = new HtmlDocument();
                doc.Load(f2);
                var l2 = LoadManifesto(doc, "TG");

                l.AddRange(l2.Select(t1 => t1.Item1));
            }

        var saveFileDialog = new SaveFileDialog();
        var r = saveFileDialog.ShowDialog();
        if (r != DialogResult.OK) return;
        var b2 = ObjectToByteArray(l);
        if (b2 != null) 
            File.WriteAllBytes(saveFileDialog.FileName, b2);
    }

    private static byte[]? ObjectToByteArray(object? obj)
    {
        if (obj == null)
            return null;

        var bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf?.Serialize(ms, obj);
        return ms.ToArray();
    }

    private static T? FromByteArray<T>(byte[]? data)
    {
        if (data == null)
#pragma warning disable IDE0034 // Semplifica l'espressione 'default'
            return default;
#pragma warning restore IDE0034 // Semplifica l'espressione 'default'
        var bf = new BinaryFormatter();
        using var ms = new MemoryStream(data);
        var obj = bf.Deserialize(ms);
        return (T)obj;
    }

    private void Button14_Click(object sender, EventArgs e)
    {
        Importa2(Chiedi.SI);
    }

    private static void Importa2(Chiedi chiedi2)
    {
        var openFileDialog = new OpenFileDialog();
        var r = openFileDialog.ShowDialog();
        if (r != DialogResult.OK) return;
        var o2 = File.ReadAllBytes(openFileDialog.FileName);
        var L2 = FromByteArray<List<Gruppo>>(o2);

        Variabili.L ??= new ListaGruppo();

        var l3 = L2?.Select(item => new Tuple<Gruppo>(item)).ToList();

        Importa4(l3, chiedi2);
        MessageBox.Show("Fatto!");
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
        Variabili.L ??= new ListaGruppo();

        Variabili.L.Fix_link_IDCorsi_se_ce_uno_che_ha_il_link_con_id_corso_uguale();
    }

    private void Button18_Click(object sender, EventArgs e)
    {
        Variabili.L.FixPianoStudi();
    }

    private void Button19_Click(object sender, EventArgs e)
    {
        var L = new List<string>
        {
            "Bot telegram",
            "Sito, ricerca vecchia",
            "Sito, ricerca nuova",
            "TUTTO",
            "Sito, ricerca v3 (2021-02)"
        };

        var askFromList = new AskFromList(L);
        askFromList.ShowDialog();

        if (askFromList.r == null) return;

#pragma warning disable CS1690 // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione
        var i = askFromList.r.Value;
#pragma warning restore CS1690 // L'accesso a un membro in un campo di una classe con marshalling per riferimento potrebbe causare un'eccezione in fase di esecuzione

        bool? entrambiIndex = null;
        switch (i)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            {
                var dialogResult = MessageBox.Show("Vuoi entrambi gli index (si) o solo uno (no)?", "Scegli",
                    MessageBoxButtons.YesNo);
                entrambiIndex = dialogResult switch
                {
                    DialogResult.Yes => true,
                    DialogResult.No => false,
                    _ => null
                };

                if (entrambiIndex == null)
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
                Button3_Click(CheckGruppo.E.VECCHIA_RICERCA, entrambiIndex);
                return;
            }

            case 2:
            {
                Button3_Click(CheckGruppo.E.NUOVA_RICERCA, entrambiIndex);
                return;
            }

            case 3:
            {
                Button3_Click(CheckGruppo.E.TUTTO, entrambiIndex);
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
        Variabili.L ??= new ListaGruppo();

        var openFileDialog = new OpenFileDialog();
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

        var acc = "";

        foreach (var t in r)
        {
            if (string.IsNullOrEmpty(t)) acc = "";

            if (string.IsNullOrEmpty(acc) && lastAdded is { Count: > 0 })
                try
                {
                    var aggiunto2 = AggiungiTesto("", t?.Trim(), true);
                    if (aggiunto2)
                        continue;
                }
                catch
                {
                    ;
                }

            acc += t?.Trim() + " ";

            var aggiunto = ValutaSeDaAggiungereENelCasoAggiungi(acc);
            if (aggiunto)
                acc = "";
            else
                lastAdded = null;
        }
    }

    private bool ValutaSeDaAggiungereENelCasoAggiungi(string acc)
    {
        if (string.IsNullOrEmpty(acc))
            return false;

        var accSplitted = acc.Contains(' ') == false ? new List<string> { acc } : acc.Split(' ').ToList();

        var indexofwebsite = ControllaSeCeUnSito(accSplitted);
        if (indexofwebsite?.Item1 == null || indexofwebsite.Item2 == null)
            return false;

        var nome = accSplitted.Where((t, i) => i != indexofwebsite.Item1.Value)
            .Aggregate("", (current, t) => current + t.Trim() + " ");

        var url = "";
        if (indexofwebsite.Item2.Value == 0)
            url = accSplitted[indexofwebsite.Item1.Value];

        return AggiungiTesto(nome, url, false);
    }

    private bool AggiungiTesto(string nome, string? url, bool gruppoAdded)
    {
        var r2 = ControllaSeCeUnSito2(url);
        if (r2 is not 0)
            return false;

        if (gruppoAdded && lastAdded is { Count: > 0 })
        {
            var g2 = new Gruppo
            {
                NomeCorso = lastAdded[^1].Trim(),
                IdLink = url?.Trim(),
                Id = url?.Trim()
            };
            g2.RicreaId();
            var ceGia2 = VediSeCeGiaDaUrl(url);
            if (ceGia2) return false;
            lastAdded ??= new List<string>();

            lastAdded.Add(g2.NomeCorso);
            Variabili.L.Add(g2, false);
            return true;
        }

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(url))
            return false;

        var g = new Gruppo
        {
            NomeCorso = nome.Trim(),
            IdLink = url.Trim(),
            Id = url.Trim()
        };
        g.RicreaId();

        var ceGia = VediSeCeGiaDaUrl(url);
        if (ceGia) return false;
        lastAdded ??= new List<string>();

        lastAdded.Add(g.NomeCorso);
        Variabili.L.Add(g, false);
        return true;
    }

    private static bool VediSeCeGiaDaUrl(string? url)
    {
        return Variabili.L.VediSeCeGiaDaURL(url);
    }

    private static Tuple<int?, int?>? ControllaSeCeUnSito(IReadOnlyList<string>? acc)
    {
        if (acc == null)
            return null;

        for (var i = 0; i < acc.Count; i++)
        {
            var r = ControllaSeCeUnSito2(acc[i]);
            if (r != null)
                return new Tuple<int?, int?>(i, r);
        }

        return null;
    }

    private static int? ControllaSeCeUnSito2(string? v)
    {
        if (string.IsNullOrEmpty(v))
            return null;
        
        var result = Uri.TryCreate(v, UriKind.Absolute, out var uriResult)
                     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        if (result)
            return 0;

        var i = v.IndexOf("http://", StringComparison.Ordinal);
        if (i >= 0)
            return i;

        i = v.IndexOf("https://", StringComparison.Ordinal);
        if (i >= 0)
            return i;

        return null;
    }

    private void Button10_Click_1(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        var openFileDialog = new OpenFileDialog();
        var r = openFileDialog.ShowDialog();
        if (r != DialogResult.OK) return;

        var read = File.ReadAllText(openFileDialog.FileName);

        try
        {
            ImportaSql(read);
        }
        catch
        {
            ;
        }
    }

    private static void ImportaSql(string read)
    {
        var s = read.Split(new[] { "INSERT INTO \"Groups\" " }, StringSplitOptions.None);

        ;

        foreach (var s2 in s)
            try
            {
                ImportaSql2(s2);
            }
#pragma warning disable IDE0059
            // Assegnazione non necessaria di un valore
#pragma warning disable CS0168
            // La variabile è dichiarata, ma non viene mai usata
            catch (Exception e2)
#pragma warning restore CS0168
                // La variabile è dichiarata, ma non viene mai usata
#pragma warning restore IDE0059
                // Assegnazione non necessaria di un valore
            {
                ;
            }
    }

    private static void ImportaSql2(string s2)
    {
        if (string.IsNullOrEmpty(s2))
            return;

        if (s2.StartsWith("(") == false)
            return;

        ;

        var s3 = s2.Split(new[] { "VALUES" }, StringSplitOptions.None);

        var s4 = s3[1];

        ;

        s4 = s4.Trim();

        ;

        var lastSemiColomn = s4.LastIndexOf(";", StringComparison.Ordinal);
        if (lastSemiColomn < 0 || lastSemiColomn >= s4.Length)
            return;

        var s5 = s4[..lastSemiColomn].Trim();

        ;

        ImportaSql3(s5);
    }

    private static void ImportaSql3(string s5)
    {
        ;

        var s6 = s5.Split(',');

        ;

        if (s6.Length == 7)
        {
            ImportaSql4(s6);
            return;
        }

        ;

        var s7 = new List<string>();
        for (var i = 0; i < 6; i++) s7.Add(s6[i]);

        ;

        var nome = "";
        for (var i = 6; i < s6.Length; i++) nome += s6[i] + ",";
        if (nome[^1] == ',') nome = nome.Remove(nome.Length - 1);

        s7.Add(nome);

        ImportaSql4(s7.ToArray());
    }

    private static void ImportaSql4(IReadOnlyList<string> s6)
    {
        var id = Convert.ToInt64(s6[0][1..].Trim());
        var idInRam = FindInRamSql(id);
        if (idInRam == null)
            ImportaSql5(s6);
        else
            ImportaSql6(s6, idInRam.Value);
    }

    private static void ImportaSql6(IReadOnlyList<string> s6, int groupId)
    {
        var g = OttieniGruppoSql(s6);

        Variabili.L.AddAndMerge(g, groupId);
    }

    private static void ImportaSql5(IReadOnlyList<string> s6)
    {
        ;

        var g = OttieniGruppoSql(s6);

        //this group is not in ram, we have to add it
        Variabili.L.Add(g, false);
    }

    private static Gruppo OttieniGruppoSql(IReadOnlyList<string> s6)
    {
        var s7 = s6[3].Split('/');

        var g = new Gruppo
        {
            PermanentId = s6[0][1..].Trim(),
            Platform = "TG",
            IdLink = s7[^1].Trim()
        };

        if (g.IdLink[^1] == '\'') g.IdLink = g.IdLink.Remove(g.IdLink.Length - 1);
        g.Classe = s6[6].Trim();
        if (g.Classe[^1] == ')') g.Classe = g.Classe.Remove(g.Classe.Length - 1);
        if (g.Classe[^1] == '\'') g.Classe = g.Classe.Remove(g.Classe.Length - 1);
        if (g.Classe[0] == '\'') g.Classe = g.Classe[1..].Trim();

        var semicolomn = g.Classe.IndexOf(");", StringComparison.Ordinal);
        if (semicolomn >= 0 && semicolomn < g.Classe.Length) g.Classe = g.Classe[..semicolomn].Trim();

        g.LastUpdateInviteLinkTime = ToDateTime(s6[4]);

        g.Aggiusta(false, true);
        return g;
    }

    private static DateTime? ToDateTime(string v)
    {
        var v2 = v.Trim().Split(' ');

        if (v2.Length < 2)
            return null;

        var v3 = v2[0].Split('-');
        var v4 = v2[1].Split('.');
        var v5 = v4[0].Trim().Split(':');

        ;

        if (v3[0].StartsWith("'")) v3[0] = v3[0][1..];

        var anno = Convert.ToInt32(v3[0]);
        var mese = Convert.ToInt32(v3[1]);
        var giorno = Convert.ToInt32(v3[2]);
        var ora = Convert.ToInt32(v5[0]);
        var minuto = Convert.ToInt32(v5[1]);

        if (v5[2][v5[2].Length - 1] == '\'') v5[2] = v5[2].Remove(v5[2].Length - 1);

        var secondo = Convert.ToInt32(v5[2]);

        var millisec = 0;
        if (v4.Length <= 1)
            return new DateTime(anno,
                mese,
                giorno,
                ora,
                minuto,
                secondo,
                millisec);

        if (v4[1][v4[1].Length - 1] == '\'') v4[1] = v4[1].Remove(v4[1].Length - 1);

        if (v4[1].Length > 3) v4[1] = v4[1][..3];

        millisec = Convert.ToInt32(v4[1]);

        return new DateTime(anno,
            mese,
            giorno,
            ora,
            minuto,
            secondo,
            millisec);
    }

    private static int? FindInRamSql(long id)
    {
        return Variabili.L.FindInRamSQL(id);
    }

    private void Button20_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.AggiustaNomiDoppi();
    }

    private void Button21_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        var parameters = new ParametriFunzione();
        parameters.AddParam(true, "laPrimaVoltaControllaDaCapo"); //bool laPrimaVoltaControllaDaCapo
        parameters.AddParam(2, "volteCheCiRiprova"); //int volteCheCiRiprova
        parameters.AddParam(100, "waitOgniVoltaCheCiRiprova"); //int waitOgniVoltaCheCiRiprova

        //saltaQuelliGiaFunzionanti: false
        var r1 = Variabili.L.CheckSeILinkVanno(parameters);
        r1.action(null, null);


        MessageBox.Show("Finito il check dei link!");
    }

    private void Button22_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.SalvaTelegramIdDeiGruppiLinkCheNonVanno(textBox1.Text);
    }

    private void Button23_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.ImportaGruppiDalComandoDelBotTelegram_UpdateLinkFromJson();
    }

    private void Button24_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.ImportaGruppiDaTabellaTelegramGruppiBot_PuntoBin();
        MessageBox.Show("Gruppi importati dalla tabella telegram del bot!");
    }

    private void Button25_Click(object sender, EventArgs e)
    {
        //"Controlla se i link vanno (solo quelli che già non vanno) e flaggali di conseguenza (tutto in ram)"

        Variabili.L ??= new ListaGruppo();

        var parameters = new ParametriFunzione();
        parameters.AddParam(false, "laPrimaVoltaControllaDaCapo"); //bool laPrimaVoltaControllaDaCapo
        parameters.AddParam(2, "volteCheCiRiprova"); //int volteCheCiRiprova
        parameters.AddParam(100, "waitOgniVoltaCheCiRiprova"); //int waitOgniVoltaCheCiRiprova

        //saltaQuelliGiaFunzionanti: true
        var r1 = Variabili.L.CheckSeILinkVanno(parameters);
        r1.action(null, null);

        MessageBox.Show("Finito il check dei link!");
    }

    private void Button26_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.StampaWhatsapp();
    }

    private void Button27_Click(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        Variabili.L.AggiustaGruppiDoppiIDTelegramUguale();
    }
}