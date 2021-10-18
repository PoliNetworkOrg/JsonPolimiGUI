using JsonPolimi_Core_nf.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace JsonPolimi_Core_nf.Tipi
{
    [Serializable]
    public class Gruppo
    {
        private string classe_hidden;

        public string Classe
        {
            get
            {
                return this.classe_hidden;
            }
            set
            {
                ;
                this.classe_hidden = value;
            }
        }

        public string IDCorsoPolimi { get; set; }
        public List<string> GruppoTabellaInsegnamenti { get; set; }
        public InfoManifesto Manifesto { get; set; }
        public int? AnnoCorsoStudio { get; set; }
        public ListaStringhePerJSON CCS { get; set; }
        public string PianoDiStudi { get; set; }
        public string NomeCorso { get; set; }

        public string Degree;
        public string Id; // esempio: FB/2018/2019/LEONARDO/21432583243205
        public string IdLink; // esempio: 21432583243205
        public string Language;
        public ListaStringhePerJSON Office; // esempio: LEONARDO

        public string GetHTML_DataRow(string textBox_anno, string textBox_piattaforma)
        {
            if (!string.IsNullOrEmpty(textBox_anno))
            {
                if (textBox_anno != Year)
                {
                    return "";
                }
            }

            if (!string.IsNullOrEmpty(textBox_piattaforma))
            {
                if (textBox_piattaforma != Platform)
                {
                    return "";
                }
            }

            string html = "";
            html += "<tr>";

            html += "<td>";
            html += this.Id;
            html += "</td>";

            html += "<td>";
            html += this.Platform;
            html += "</td>";

            html += "<td>";
            html += this.Classe;
            html += "</td>";

            html += "<td>";
            html += this.Degree;
            html += "</td>";

            html += "<td>";
            html += this.Language;
            html += "</td>";

            html += "<td>";
            html += this.Office;
            html += "</td>";

            html += "<td>";
            html += this.School;
            html += "</td>";

            html += "<td>";
            html += this.Tipo;
            html += "</td>";

            html += "<td>";
            html += this.Year;
            html += "</td>";

            html += "<td>";
            html += this.IdLink;
            html += "</td>";

            html += "<td>";
            html += this.PermanentId;
            html += "</td>";

            html += "<td>";
            html += this.NomeCorso;
            html += "</td>";

            html += "<td>";
            html += this.GetLink();
            html += "</td>";

            html += "<td>";
            html += this.LinkFunzionante;
            html += "</td>";

            html += "</tr>";
            return html;
        }

        public string PermanentId; //per telegram, esempio -1000345953
        public string Platform; // esempio: FB
        public string School;
        public string Tipo;
        public string Year; // esempio: 2018/2019
        public DateTime? LastUpdateInviteLinkTime;
        public bool? LinkFunzionante;

        public Gruppo()
        {
            ;
        }

        public void Aggiusta(bool aggiusta_anno, bool creaid)
        {
            Classe = string.IsNullOrEmpty(Classe) ? "" : Classe.Replace('\n', ' ');

            if (string.IsNullOrEmpty(Tipo)) Tipo = "S";

            if (aggiusta_anno)
                AggiustaAnno();

            if (Tipo != "G")
            {
                if (!string.IsNullOrEmpty(Year) && !string.IsNullOrEmpty(this.Classe) && !string.IsNullOrEmpty(this.Degree) &&
                    !string.IsNullOrEmpty(this.Id) && !string.IsNullOrEmpty(this.IdLink) && !string.IsNullOrEmpty(this.Language) && !IsEmpty(this.Office))
                {
                    if (string.IsNullOrEmpty(Tipo))
                    {
                        Tipo = "S";
                    }
                }
                else if (!string.IsNullOrEmpty(Year))
                {
                    Tipo = "S";
                }
            }

            if (string.IsNullOrEmpty(Language)) Language = IndovinaLaLinguaDalNome();

            if (string.IsNullOrEmpty(School))
                School = IndovinaLaSchool();

            if (string.IsNullOrEmpty(Degree))
                Degree = IndovinaIlDegree();

            if (string.IsNullOrEmpty(IdLink))
                IdLink = CreaIdLink();

            if (!string.IsNullOrEmpty(IdLink))
            {
                if (LastUpdateInviteLinkTime == null)
                    LastUpdateInviteLinkTime = DateTime.Now;
            }

            if (creaid)
                Id = CreaId();
        }

        public static bool IsEmpty(List<string> office)
        {
            if (office == null)
                return true;

            if (office.Count == 0)
                return true;

            foreach (var x in office)
            {
                if (string.IsNullOrEmpty(x))
                    return true;
            }

            return false;
        }

        public void AggiustaAnno()
        {
            if (!string.IsNullOrEmpty(Year)) return;

            var title = Classe.Replace("/", "-");
            title = title.Replace(" ", "-");
            var t2 = title.Split('-');

            var a = AnnoInTitolo(t2);
            if (a < 0) return;

            Year = t2[a] + "/" + t2[a + 1];
        }

        private static int AnnoInTitolo(IReadOnlyList<string> t)
        {
            if (t.Count <= 1) return -1;

            for (var i = 0; i < t.Count - 1; i++)
                try
                {
                    var a = Convert.ToInt32(t[i]);
                    var b = Convert.ToInt32(t[i + 1]);
                    if (a >= 2016 && b >= 2016)
                        return i;
                }
                catch
                {
                    ;
                }

            return -1;
        }

        private string CreaIdLink()
        {
            string r2;
            try
            {
                var r = Id.Split('/');
                r2 = r[3];
            }
            catch
            {
                return null;
            }

            return r2;
        }

        private string CreaId()
        {
            if (string.IsNullOrEmpty(PianoDiStudi))
            {
                ;
            }

            return Platform + "/" +
                Year + "/" +
                Office + "/" +
                IdLink + "/" +
                StringNotEmpty(IDCorsoPolimi) + "/" +
                CCS?.GetCCSCode() + "/" +
                PianoDiStudi;
        }

        private string StringNotEmpty(string a)
        {
            if (a == null)
                return "";

            return a;
        }

        private static string IndovinaIlDegree()
        {
            //throw new NotImplementedException();
            return null;
        }

        private static string IndovinaLaSchool()
        {
            //throw new NotImplementedException();
            return null;
        }

        private string IndovinaLaLinguaDalNome(string defaultLanguage = "ITA")
        {
            var c = Classe.ToLower();

            if (c.Contains("and"))
                return "ENG";
            return c.Contains("for") ? "ENG" : defaultLanguage;
        }

        public string To_json(CheckGruppo.E v)
        {
            var json = "{";

            switch (v)
            {
                case CheckGruppo.E.TUTTO:
                    {
                        json += "\"class\":";
                        json += StringCheckNull(EscapeQuotes(Classe));
                        json += ",\"office\":";
                        json += StringCheckNull(Office);
                        json += ",\"id\":";
                        json += StringCheckNull(Id);
                        json += ",\"degree\":";
                        json += StringCheckNull(Degree);
                        json += ",\"school\":";
                        json += StringCheckNull(School);
                        json += ",\"annocorso\":";
                        json += StringCheckNull(AnnoCorsoStudio);
                        json += ",\"nomecorso\":";
                        json += StringCheckNull(NomeCorso);
                        json += ",\"idcorso\":";
                        json += StringCheckNull(IDCorsoPolimi);
                        json += ",\"pianostudi\":";
                        json += StringCheckNull(PianoDiStudi);
                        json += ",\"id_link\":";
                        json += StringCheckNull(IdLink);
                        json += ",\"language\":";
                        json += StringCheckNull(Language);
                        json += ",\"type\":";
                        json += StringCheckNull(Tipo);
                        json += ",\"year\":";
                        json += StringCheckNull(Year);
                        json += ",\"ccs\":";
                        json += StringCheckNull(CCS);
                        json += ",\"permanentId\":";
                        json += StringCheckNull(PermanentId);
                        json += ",\"LastUpdateInviteLinkTime\":";
                        json += StringCheckNull(GetTelegramTime());
                        json += ",\"platform\":";
                        json += StringCheckNull(Platform);

                        break;
                    }

                case CheckGruppo.E.RICERCA_SITO_V3:
                case CheckGruppo.E.VECCHIA_RICERCA:
                    {
                        json += "\"class\":";
                        json += StringCheckNull(EscapeQuotes(Classe));
                        json += ",\"office\":";
                        json += StringCheckNull(Office);
                        json += ",\"id\":";
                        json += StringCheckNull(Id);
                        json += ",\"degree\":";
                        json += StringCheckNull(Degree);
                        json += ",\"school\":";
                        json += StringCheckNull(School);
                        json += ",\"id_link\":";
                        json += StringCheckNull(IdLink);
                        json += ",\"language\":";
                        json += StringCheckNull(Language);
                        json += ",\"type\":";
                        json += StringCheckNull(Tipo);
                        json += ",\"year\":";
                        json += StringCheckNull(Year);
                        json += ",\"platform\":";
                        json += StringCheckNull(Platform);
                        json += ",\"permanentId\":";
                        json += StringCheckNull(PermanentId);
                        json += ",\"LastUpdateInviteLinkTime\":";
                        json += StringCheckNull(GetTelegramTime());
                        json += ",\"linkfunzionante\":";
                        json += BoolCheckNotNull(LinkFunzionante);
                        break;
                    }
                case CheckGruppo.E.NUOVA_RICERCA:
                    {
                        json += "\"class\":";
                        json += StringCheckNull(EscapeQuotes(Classe));
                        json += ",\"office\":";
                        json += StringCheckNull(Office);
                        json += ",\"id\":";
                        json += StringCheckNull(Id);
                        json += ",\"degree\":";
                        json += StringCheckNull(Degree);
                        json += ",\"school\":";
                        json += StringCheckNull(School);
                        json += ",\"annocorso\":";
                        json += StringCheckNull(AnnoCorsoStudio);
                        json += ",\"nomecorso\":";
                        json += StringCheckNull(NomeCorso);
                        json += ",\"idcorso\":";
                        json += StringCheckNull(IDCorsoPolimi);
                        json += ",\"pianostudi\":";
                        json += StringCheckNull(PianoDiStudi);
                        json += ",\"id_link\":";
                        json += StringCheckNull(IdLink);
                        json += ",\"language\":";
                        json += StringCheckNull(Language);
                        json += ",\"type\":";
                        json += StringCheckNull(Tipo);
                        json += ",\"year\":";
                        json += StringCheckNull(Year);
                        json += ",\"ccs\":";
                        json += StringCheckNull(CCS);
                        json += ",\"platform\":";
                        json += StringCheckNull(Platform);
                        break;
                    }
            }

            json += "}";

            return json;
        }

        private string BoolCheckNotNull(bool? linkFunzionante)
        {
            return linkFunzionante == null ? "null" : linkFunzionante.Value ? '"' + "Y" + '"' : '"' + "N" + '"';
        }

        private string StringCheckNull(int? annoCorsoStudio)
        {
            if (annoCorsoStudio == null)
                return "null";

            return StringCheckNull(annoCorsoStudio.Value.ToString());
        }

        private string StringCheckNull(ListaStringhePerJSON office)
        {
            if (office == null)
                return "null";

            return '"' + office.StringNotNull() + '"';
        }

        private string StringCheckNull(string s)
        {
            if (String.IsNullOrEmpty(s))
                return "null";

            return '"' + s + '"';
        }

        private static string EscapeQuotes(string s)
        {
            if (s == null)
                return null;

            for (var i = 0; i < 3; i++)
            {
                s = UnEscapeQuotes(s);
            }

            var s2 = "";
            foreach (var t in s)
            {
                if (t == '"')
                {
                    s2 += '\\';
                    s2 += '"';
                    //  =>    \"
                }
                else
                {
                    s2 += t;
                }
            }

            return s2;
        }

        private static string UnEscapeQuotes(string s)
        {
            if (s == null)
                return null;

            var s2 = "";
            var i = 0;
            while (i < s.Length - 1)
            {
                if (s[i] == '\\' && s[i + 1] == '"')
                {
                    s2 += '"';
                    i += 2;
                }
                else
                {
                    s2 += s[i];
                    i++;
                }
            }

            while (i < s.Length)
            {
                s2 += s[i];
                i++;
            }

            return s2;
        }

        public static void AggiungiInformazioneAmbigua(string v, ref InsiemeDiGruppi g)
        {
            if (string.IsNullOrEmpty(v))
                return;

            var vUpper = v.ToUpper().Trim();

            //bisogna capire che tipo di informazione stiamo ricevendo
            if (v.StartsWith("https://", StringComparison.Ordinal) || v.StartsWith("http://", StringComparison.Ordinal))
            {
                AggiungiLink(v, ref g);
            }
            else if (IsSede(vUpper))
            {
                AggiungiSede(v, ref g);
            }
            else if (vUpper == "FACEBOOK" || vUpper == "TELEGRAM" || vUpper == "NON ANCORA CREATO" ||
                     vUpper == "CORSI" || vUpper == "LUOGO" || vUpper.StartsWith("LAUREE", StringComparison.Ordinal))
            {
                //è una cella inutile
                ;
            }
            else if (vUpper == "<=")
            {
                //è una cella inutile
                ;
            }
            else if (v.StartsWith("<text:a", StringComparison.Ordinal))
            {
                var n1 = v.IndexOf("xlink:href", StringComparison.Ordinal);
                var s1 = v.Substring(n1 + 12);
                var s2 = s1.Split('"');

                var s3 = s2[1].Split('>');
                var s4 = s3[1].Split('<');

                var nome = s4[0];

                if (nome.StartsWith("http", StringComparison.Ordinal))
                {
                    AggiungiLink(s2[0], ref g);
                }
                else
                {
                    AggiungiNome(nome, ref g);
                    AggiungiLink(s2[0], ref g);
                }
            }
            else
            {
                AggiungiAltro(ref vUpper, ref g, ref v);
            }
        }

        private static bool IsSede(string vUpper)
        {
            return vUpper == "LEONARDO" || vUpper == "MANTOVA" || vUpper == "BOVISA" || vUpper == "PIACENZA" ||
                   vUpper == "LECCO" || vUpper == "COMO" || vUpper == "CREMONA" || vUpper == "LEONARDO-CREMONA" ||
                   vUpper == "LEONARDO*";
        }

        private static void AggiungiAltro(ref string vUpper, ref InsiemeDiGruppi g, ref string v)
        {
            switch (vUpper)
            {
                case "LT":
                case "LM":
                case "LU":
                    AggiungiTriennaleMagistrale(vUpper, ref g);
                    break;

                case "3I":
                case "DES":
                case "AUIC":
                case "ICAT":
                case "3I+AUIC":
                case "ICAT+3I":
                case "DES+3I":
                    AggiungiScuola(vUpper, ref g);
                    break;

                case "ITA":
                case "ENG":
                case "ITA-ENG":
                    AggiungiLingua(vUpper, ref g);
                    break;

                default:
                    //altrimenti è il nome
                    AggiungiNome(v, ref g);
                    break;
            }
        }

        /*
            return (o1 == o2)  =>  0
            return (o1 >  o2)  => +1
            return (o1 <  o2)  => -1
        */

        public static int Confronta(ListaStringhePerJSON o1, ListaStringhePerJSON o2)
        {
            return ListaStringhePerJSON.Confronta(o1, o2);
        }

        private static void AggiungiLingua(string vUpper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.Language = vUpper;
            g.NomeOld.Language = vUpper;
        }

        private static void AggiungiScuola(string vUpper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.School = vUpper;
            g.NomeOld.School = vUpper;
        }

        private static void AggiungiTriennaleMagistrale(string vUpper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.Degree = vUpper;
            g.NomeOld.Degree = vUpper;
        }

        private static void AggiungiNome(string v, ref InsiemeDiGruppi g)
        {
            if (v == "<=")
                return;

            if (string.IsNullOrEmpty(v))
                return;

            if (v.Contains("<="))
                return;

            if (v.Contains("=>"))
                return;

            if (v.Contains(" vedasi PoliExtra"))
                return;

            if (string.IsNullOrEmpty(g.GruppoDiBase.Classe))
            {
                g.GruppoDiBase.Classe = v;
            }
            else
            {
                g.GruppoDiBase.Classe += " ";
                g.GruppoDiBase.Classe += v;
            }

            g.NomeOld.Classe = g.GruppoDiBase.Classe;
        }

        private static void AggiungiSede(string v, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.Office = new ListaStringhePerJSON(v);
            g.NomeOld.Office = new ListaStringhePerJSON(v);
        }

        private static void AggiungiLink(string v, ref InsiemeDiGruppi g)
        {
            var g2 = new Gruppo();

            var n1 = v.IndexOf("://", StringComparison.Ordinal);
            var s1 = v.Substring(n1 + 3);

            var n2 = s1.IndexOf("www.", StringComparison.Ordinal);
            if (n2 >= 0 && n2 < s1.Length) s1 = s1.Substring(4);

            if (s1[0] == 'f') // facebook
            {
                g2.Platform = "FB";

                var s2 = s1.Split('/');
                g2.IdLink = s2[1] == "groups" ? s2[2] : s2[1];
            }
            else if (s1[0] == 't') // telegram
            {
                g2.Platform = "TG";

                var s2 = s1.Split('/');
                g2.IdLink = s2[1] == "joinchat" ? s2[2] : s2[1];
            }
            else if (s1[0] == 'i') // instagram
            {
                g2.Platform = "IG";

                var s2 = s1.Split('/');

                g2.IdLink = s2[1];
            }
            else if (s1[0] == 'c') //whatsapp
            {
                g2.Platform = "WA";

                var s2 = s1.Split('/');

                g2.IdLink = s2[1];
            }
            else
            {
                ;
            }

            g.L.Add(g2);
        }

        public override string ToString()
        {
            return To_json(CheckGruppo.E.TUTTO) + " " + base.ToString();
        }

        public void Merge(Gruppo gruppo)
        {
            if (!string.IsNullOrEmpty(gruppo.Classe) && string.IsNullOrEmpty(Classe))
                Classe = gruppo.Classe;

            if (!string.IsNullOrEmpty(gruppo.Degree) && string.IsNullOrEmpty(Degree))
                Degree = gruppo.Degree;

            if (!string.IsNullOrEmpty(gruppo.Id) && string.IsNullOrEmpty(Id))
                Id = gruppo.Id;

            if (!string.IsNullOrEmpty(gruppo.IdLink))
            {
                if (string.IsNullOrEmpty(IdLink))
                {
                    IdLink = gruppo.IdLink;
                    LastUpdateInviteLinkTime = gruppo.LastUpdateInviteLinkTime;
                }
                else
                {
                    switch (LastUpdateInviteLinkTime)
                    {
                        case null when gruppo.LastUpdateInviteLinkTime == null:
                            IdLink = gruppo.IdLink;
                            LastUpdateInviteLinkTime = gruppo.LastUpdateInviteLinkTime;
                            break;

                        case null:
                            IdLink = gruppo.IdLink;
                            LastUpdateInviteLinkTime = gruppo.LastUpdateInviteLinkTime;
                            break;

                        default:
                            {
                                if (gruppo.LastUpdateInviteLinkTime != null)
                                {
                                    var r = DateTime.Compare(LastUpdateInviteLinkTime.Value,
                                        gruppo.LastUpdateInviteLinkTime.Value);
                                    if (r < 0)
                                    {
                                        IdLink = gruppo.IdLink;
                                        LastUpdateInviteLinkTime = gruppo.LastUpdateInviteLinkTime;
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            if (LastUpdateInviteLinkTime == null)
                LastUpdateInviteLinkTime = DateTime.Now;

            if (!string.IsNullOrEmpty(gruppo.Language) && string.IsNullOrEmpty(Language))
                Language = gruppo.Language;

            if (!IsEmpty(gruppo.Office) && IsEmpty(Office))
                Office = gruppo.Office;

            if (!string.IsNullOrEmpty(gruppo.PermanentId) && string.IsNullOrEmpty(PermanentId))
                PermanentId = gruppo.PermanentId;

            if (!string.IsNullOrEmpty(gruppo.Platform) && string.IsNullOrEmpty(Platform))
                Platform = gruppo.Platform;

            if (!string.IsNullOrEmpty(gruppo.School) && string.IsNullOrEmpty(School))
                School = gruppo.School;

            if (!string.IsNullOrEmpty(gruppo.Tipo) && string.IsNullOrEmpty(Tipo))
                Tipo = gruppo.Tipo;

            if (!string.IsNullOrEmpty(gruppo.Year) && string.IsNullOrEmpty(Year))
                Year = gruppo.Year;
        }

        public static bool IsEmpty(ListaStringhePerJSON office)
        {
            if (office == null)
                return true;

            return office.IsEmpty();
        }

        public string To_json_Tg()
        {
            /*
             {"Chat": {"id": -1001452418598, "type": "supergroup", "title": "Polimi Piacenza \ud83c\uddee\ud83c\uddf9\ud83d\udc48",
             "invite_link": "https://t.me/joinchat/LclXl1aSJiYbzl7wCW5WZg"}, "LastUpdateInviteLinkTime": "2019-08-20 08:47:55.368966", "we_are_admin": true}
            */

            if (String.IsNullOrEmpty(this.PermanentId))
                return null;

            if (String.IsNullOrEmpty(this.Classe))
                return null;

            if (String.IsNullOrEmpty(this.Platform))
                return null;

            if (this.Platform != "TG")
                return null;

            string json = "{" + '"' + "Chat" + '"' + ":{";

            json += '"' + "id" + '"' + ": ";
            json += this.PermanentId;
            json += ", " + '"' + "type" + '"' + ": \"supergroup\", \"title\": ";
            json += '"';
            json += Escape(this.Classe);
            json += '"';
            json += ", \"invite_link\": ";
            json += '"';
            json += this.GetLink();
            json += '"';
            json += "}, ";
            json += '"' + "LastUpdateInviteLinkTime" + '"';
            json += ": ";
            json += '"';
            json += this.GetTelegramTime();
            json += '"';
            json += ", ";
            json += '"' + "we_are_admin" + '"';
            json += ": true}";
            return json;
        }

        private string Escape(string classe)
        {
            string a = "" + '\\' + '"';
            string b = "" + '"';
            classe = classe.Replace(a, b);
            classe = classe.Replace(a, b);
            classe = classe.Replace(b, a);
            return classe;
        }

        private string GetTelegramTime()
        {
            if (this.LastUpdateInviteLinkTime == null)
                return null;

            //   2019-08-20 08:47:55.368966
            return this.LastUpdateInviteLinkTime.Value.Year.ToString().PadLeft(4, '0') + "-" +
                    this.LastUpdateInviteLinkTime.Value.Month.ToString().PadLeft(2, '0') + "-" +
                    this.LastUpdateInviteLinkTime.Value.Day.ToString().PadLeft(2, '0') + " " +
                    this.LastUpdateInviteLinkTime.Value.Hour.ToString().PadLeft(2, '0') + ":" +
                    this.LastUpdateInviteLinkTime.Value.Minute.ToString().PadLeft(2, '0') + ":" +
                    this.LastUpdateInviteLinkTime.Value.Second.ToString().PadLeft(2, '0') + "." +
                    this.LastUpdateInviteLinkTime.Value.Millisecond.ToString().PadLeft(3, '0');
        }

        private string GetLink()
        {
            if (string.IsNullOrEmpty(this.Platform))
                return "";

            switch (this.Platform)
            {
                case "TG":
                    {
                        return "https://t.me/joinchat/" + this.IdLink;
                    }

                case "WA":
                    {
                        return "https://chat.whatsapp.com/" + this.IdLink;
                    }

                case "FB":
                    {
                        return "https://www.facebook.com/groups/" + this.IdLink;
                    }
            }

            return "";
        }

        public static Gruppo FromInfoParteList(List<InfoParteDiGruppo> infoParteDiGruppo_list, string pLAT2)
        {
            if (infoParteDiGruppo_list == null)
                return null;

            if (infoParteDiGruppo_list.Count == 0)
                return null;

            if (infoParteDiGruppo_list.Count < 3)
            {
                if (infoParteDiGruppo_list.Count == 2)
                {
                    if (infoParteDiGruppo_list[0] == null && infoParteDiGruppo_list[1] == null)
                    {
                        return null; //sono sicuro
                    }
                    else if (infoParteDiGruppo_list[0] == null)
                    {
                        if (infoParteDiGruppo_list[1] != null)
                        {
                            if (string.IsNullOrEmpty(infoParteDiGruppo_list[1].testo_selvaggio))
                            {
                                return null;
                            }
                            else if (infoParteDiGruppo_list[1].testo_selvaggio == "Insegnamenti in Sequenza")
                            {
                                return null; //sicuro
                            }
                            else if (infoParteDiGruppo_list[1].testo_selvaggio == "Insegnamento completamente offerto in lingua italiana")
                            {
                                return null; //sicuro
                            }
                            else if (infoParteDiGruppo_list[1].testo_selvaggio == "Insegnamento completamente offerto in lingua inglese")
                            {
                                return null; //sicuro
                            }
                            else if (infoParteDiGruppo_list[1].testo_selvaggio == "Insegnamento offerto in lingua italiana e inglese")
                            {
                                return null; //sicuro
                            }
                            else if (infoParteDiGruppo_list[1].testo_selvaggio == "Visualizza offerta non diversificata (***)")
                            {
                                return null; //sicuro
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (string.IsNullOrEmpty(infoParteDiGruppo_list[0].testo_selvaggio) &&
                        string.IsNullOrEmpty(infoParteDiGruppo_list[1].testo_selvaggio))
                    {
                        return null;
                    }

                    if (infoParteDiGruppo_list[1] == null)
                    {
                        if (infoParteDiGruppo_list[0] == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(infoParteDiGruppo_list[0].testo_selvaggio))
                            {
                                return null;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Primo Semestre")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Secondo Semestre")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Insegnamento Annuale")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Corso Integrato")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Monodisciplinare")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Prova Finale")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Non definita")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Preside")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Livello")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Classe di Laurea")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Coordinatore CCS")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Lingua/e ufficiali")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Tirocinio")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Gli insegnamenti possono essere scelti nell'anno di corso precedente")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Gli insegnamenti NON possono essere scelti nell'anno di corso precedente")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Non significativo")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Ordine di scelta insegnamenti in fase di composizione piano")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio == "Scuola")
                    {
                        Variabili.ParametriCondivisiItem.infoManifesto.Scuola = infoParteDiGruppo_list[1].testo_selvaggio.Trim();
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Gruppo di insegnamenti a preferenza")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Italiano/Inglese")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Laboratorio")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio == "Mutuabile")
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio != null && infoParteDiGruppo_list[1].testo_selvaggio.StartsWith("Insegnamento"))
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("Classe di Laurea"))
                    {
                        return null; //sicuro
                    }

                    if (infoParteDiGruppo_list[1].testo_selvaggio.StartsWith("Workshop"))
                    {
                        return null; //sicuro
                    }

                    return null;
                }

                if (infoParteDiGruppo_list.Count == 1)
                {
                    if (infoParteDiGruppo_list[0] == null)
                    {
                        return null; //sono sicuro
                    }

                    if (infoParteDiGruppo_list[0].link != null)
                    {
                        return null; //sono sicuro
                    }

                    if (string.IsNullOrEmpty(infoParteDiGruppo_list[0].testo_selvaggio))
                    {
                        return null;
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio == "Legenda")
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio == "Insegnamenti del Gruppo  TABA")
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("(¹) Il corso di laurea offre "))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("Nessun insegnamento per"))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("4<sup>"))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("5<sup>"))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("(¹) Lo studente a"))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("Insegnamenti del Grupp"))
                    {
                        return null; //sicuro
                    }
                    else if (infoParteDiGruppo_list[0].testo_selvaggio.StartsWith("Corso di Studi"))
                    {
                        return null; //sicuro
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }

            if (infoParteDiGruppo_list[0] == null && infoParteDiGruppo_list[1] == null)
                return null;

            if (infoParteDiGruppo_list.Count == 4)
            {
                if (infoParteDiGruppo_list[0].testo_selvaggio == "Corso di Studio")
                {
                    string x1 = infoParteDiGruppo_list[1].testo_selvaggio.Trim();
                    string x2 = x1.Replace('\r', '\t');
                    x2 = x2.Replace('\n', '\t');
                    var x3 = x2.Split('\t');
                    List<string> x4 = new List<string>();
                    foreach (var x3a in x3)
                    {
                        if (!string.IsNullOrEmpty(x3a.Trim()))
                        {
                            x4.Add(x3a.Trim());
                        }
                    }

                    Variabili.ParametriCondivisiItem.infoManifesto.Corso_di_studio = x4;
                }

                if (infoParteDiGruppo_list[2].testo_selvaggio == "Sede del corso")
                {
                    string x1 = infoParteDiGruppo_list[3].testo_selvaggio.Trim();
                    Variabili.ParametriCondivisiItem.infoManifesto.Sede_del_corso = x1.Split(',');
                }

                if (infoParteDiGruppo_list[0].testo_selvaggio == "Anni di Corso Attivi")
                {
                    string x1 = infoParteDiGruppo_list[1].testo_selvaggio.Trim();
                    Variabili.ParametriCondivisiItem.infoManifesto.Anni_di_corso_attivi = x1.Split(',');
                }

                if (infoParteDiGruppo_list[0].testo_selvaggio == "Anno Accademico")
                {
                    string x1 = null;
                    try
                    {
                        x1 = infoParteDiGruppo_list[1].testo_selvaggio.Trim();
                    }
                    catch
                    {
                        ;
                    }

                    Variabili.ParametriCondivisiItem.infoManifesto.Anno_accademico = x1;
                }

                if (infoParteDiGruppo_list[2].testo_selvaggio == "Sede")
                {
                    string x1 = null;
                    try
                    {
                        x1 = infoParteDiGruppo_list[3].testo_selvaggio.Trim();
                    }
                    catch
                    {
                        ;
                    }

                    Variabili.ParametriCondivisiItem.infoManifesto.Sede = x1;
                }

                if (infoParteDiGruppo_list[2].testo_selvaggio == "Durata nominale del Corso")
                {
                    string x1 = infoParteDiGruppo_list[3].testo_selvaggio.Trim();
                    Variabili.ParametriCondivisiItem.infoManifesto.Durata_nominale_corso = x1;
                }

                return null; //info interessanti
            }

            if (infoParteDiGruppo_list.Count == 10 || infoParteDiGruppo_list.Count == 9 || infoParteDiGruppo_list.Count == 11)
            {
                if (infoParteDiGruppo_list[0].testo_selvaggio == "--" &&
                    infoParteDiGruppo_list[1].testo_selvaggio == "--" &&
                    infoParteDiGruppo_list[2].testo_selvaggio == "--" &&
                    infoParteDiGruppo_list[4].testo_selvaggio == "--" &&
                    infoParteDiGruppo_list[5].testo_selvaggio == "--" &&
                    infoParteDiGruppo_list[6].testo_selvaggio == "--")
                {
                    return null; //sicuro
                }

                string classe = null;
                int n1 = 0;
                if (infoParteDiGruppo_list[3] != null && infoParteDiGruppo_list[3].link != null && !string.IsNullOrEmpty(infoParteDiGruppo_list[3].link.v))
                {
                    classe = infoParteDiGruppo_list[3].link.v;
                    n1 = 3;
                }
                else if (infoParteDiGruppo_list[4] != null && infoParteDiGruppo_list[4].link != null && !string.IsNullOrEmpty(infoParteDiGruppo_list[4].link.v))
                {
                    classe = infoParteDiGruppo_list[4].link.v;
                    n1 = 4;
                }

                string lang;
                try
                {
                    lang = infoParteDiGruppo_list[n1 + 1].lingua.Value.ToString();
                }
                catch
                {
                    lang = "??";
                }

                if (!string.IsNullOrEmpty(classe))
                {
                    Gruppo g = new Gruppo
                    {
                        Classe = classe,
                        IDCorsoPolimi = infoParteDiGruppo_list[0].testo_selvaggio,
                        GruppoTabellaInsegnamenti = GetGruppoTabellaInsegnamenti(infoParteDiGruppo_list[1]),
                        Office = new ListaStringhePerJSON(GetSede(infoParteDiGruppo_list[5])),
                        Language = lang,
                        Tipo = "C",
                        AnnoCorsoStudio = Variabili.ParametriCondivisiItem.anno,
                        Platform = pLAT2,
                        PianoDiStudi = Variabili.ParametriCondivisiItem.pianostudi2,
                        NomeCorso = classe
                    };
                    g.IdLink = null;
                    g.Aggiusta(false, true);
                    if (g.IdLink != null)
                    {
                        ;
                    }
                    return g;
                }

                return null;
            }

            if (infoParteDiGruppo_list.Count == 3)
            {
                if (infoParteDiGruppo_list[1] == null && infoParteDiGruppo_list[2] == null)
                {
                    if (infoParteDiGruppo_list[0] == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(infoParteDiGruppo_list[0].testo_selvaggio))
                        {
                            return null; //sicuro
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else if (infoParteDiGruppo_list[1] != null && infoParteDiGruppo_list[1].testo_selvaggio == "Area Servizi ICT")
                {
                    return null; //sicuro
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public void AggiungiInfoDaManifesto(InfoManifesto infoManifesto)
        {
            this.Manifesto = infoManifesto;
        }

        public Gruppo Clone()
        {
            Gruppo g = new Gruppo
            {
                Classe = this.Classe,
                Degree = this.Degree,
                Id = this.Id,
                IDCorsoPolimi = this.IDCorsoPolimi,
                IdLink = this.IdLink,
                Language = this.Language,
                LastUpdateInviteLinkTime = this.LastUpdateInviteLinkTime,
                Office = this.Office,
                PermanentId = this.PermanentId,
                Platform = this.Platform,
                School = this.School,
                GruppoTabellaInsegnamenti = this.GruppoTabellaInsegnamenti,
                Tipo = this.Tipo,
                Year = this.Year,
                Manifesto = this.Manifesto,
                AnnoCorsoStudio = this.AnnoCorsoStudio,
                CCS = this.CCS,
                PianoDiStudi = this.PianoDiStudi,
                NomeCorso = this.NomeCorso
            };

            return g;
        }

        private static List<string> GetGruppoTabellaInsegnamenti(InfoParteDiGruppo infoParteDiGruppo)
        {
            if (infoParteDiGruppo == null)
                return null;

            List<string> L = new List<string>();
            if (string.IsNullOrEmpty(infoParteDiGruppo.testo_selvaggio) && infoParteDiGruppo.sottopezzi != null)
            {
                foreach (var x1 in infoParteDiGruppo.sottopezzi)
                {
                    L.Add(x1.testo_selvaggio);
                }
                return L;
            }

            L.Add(infoParteDiGruppo.testo_selvaggio);
            return L;
        }

        private static List<string> GetSede(InfoParteDiGruppo infoParteDiGruppo)
        {
            if (infoParteDiGruppo == null)
                return null;

            if (string.IsNullOrEmpty(infoParteDiGruppo.testo_selvaggio))
                return null;

            switch (infoParteDiGruppo.testo_selvaggio)
            {
                case "BV":
                    return new List<string>() { "Bovisa" };

                case "MI":
                    return new List<string>() { "Leonardo" };

                case "--":
                    return null;

                case "MN":
                    return new List<string>() { "Mantova" };

                case "PC":
                    return new List<string>() { "Piacenza" };

                case "LC":
                    return new List<string> { "Lecco" };

                case "CR":
                    return new List<string> { "Cremona" };

                case "CO":
                    return new List<string>() { "Como" };

                case "BV, MI":
                    return new List<string>() { "Bovisa", "Leonardo" };

                case "LC, MI":
                    return new List<string>() { "Lecco", "Leonardo" };
            }

            return null;
        }

        public bool IsValido()
        {
            if (string.IsNullOrEmpty(this.Classe))
                return false;

            //todo: fare altri controlli per vedere se il gruppo è valido

            return true;
        }

        public void RicreaId()
        {
            this.Id = CreaId();
        }

        public void AggiustaNomeDoppio()
        {
            if (string.IsNullOrEmpty(this.Classe))
                return;

            for (int k = 0; k < this.Classe.Length - 1; k++)
            {
                if (this.Classe[k] == ' ' && this.Classe[k + 1] == ' ')
                {
                    this.Classe = this.Classe.Remove(k, 1);
                    k--;
                }
            }

            string s2 = AggiustaNomeDoppio2(this.Classe);
            if (s2 != this.Classe)
            {
                this.Classe = s2;
            }

            //AggiustaNomeDoppio3();
        }

#pragma warning disable IDE0051 // Rimuovi i membri privati inutilizzati

        private void AggiustaNomeDoppio3()
#pragma warning restore IDE0051 // Rimuovi i membri privati inutilizzati
        {
            string uguale = "";

            float ugualeMax = this.Classe.Length / 3f;
            if (ugualeMax < 5)
                ugualeMax = 5;

            int i = 0;
            int j = 1;

            while (true)
            {
                if (i >= this.Classe.Length || j >= this.Classe.Length)
                {
                    ;

                    if (uguale.Length > ugualeMax)
                    {
                        this.Classe = uguale;
                        return;
                    }

                    return;
                }

                if (this.Classe[i].ToString().ToLower() == this.Classe[j].ToString().ToLower())
                {
                    uguale += this.Classe[i];
                    i++;
                    j++;
                }
                else
                {
                    ;

                    if (uguale.Length > ugualeMax)
                    {
                        this.Classe = uguale;
                        return;
                    }

                    uguale = "";
                    j++;
                    i = 0;
                }
            }
        }

        private string AggiustaNomeDoppio2(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            text = text.Trim();

            string[] s = text.Trim().Split(' ');
            ;

            List<string> s2 = new List<string>();
            foreach (var s3 in s)
            {
                s2.Add(s3.ToLower());
            }

            if (s2.Count == 0)
                return "";

            if (s2.Count == 1)
                return text;

            if (s2.Count == 2)
            {
                if (s2[0] == s2[1])
                    text = s[0];

                return text;
            }

            ;

            //s2.Count > 2
            for (int i = 0; i < s2.Count; i++)
            {
                for (int rip = 1; rip < s2.Count; rip++)
                {
                    if (i + rip < s2.Count)
                    {
                        bool uguali = FindSeUguali(s2, i, rip);
                        if (uguali)
                        {
                            ;

                            List<string> r = new List<string>();
                            int k = 0;
                            for (; k < (i + rip); k++)
                            {
                                r.Add(s[k]);
                            }
                            k += rip;
                            for (; k < s2.Count; k++)
                            {
                                r.Add(s[k]);
                            }

                            string text2 = "";
                            for (int l = 0; l < r.Count; l++)
                            {
                                text2 += r[l] + " ";
                            }

                            text2 = text2.Trim();

                            return AggiustaNomeDoppio2(text2);
                        }
                    }
                }
            }

            return text;
        }

        private bool FindSeUguali(List<string> s2, int i, int rip)
        {
            if (i >= s2.Count || rip >= s2.Count)
            {
                return false;
            }

            for (int k = 0; k < rip; k++)
            {
                int l1 = k + i;
                int l2 = k + i + rip;
                if (l1 < s2.Count && l2 < s2.Count)
                {
                    if (s2[l2] != s2[l1])
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void CheckSeIlLinkVa(bool saltaQuelliGiaFunzionanti)
        {
            switch (this.Platform)
            {
                case "TG":
                    {
                        this.LinkFunzionante = CheckSeIlLinkVa3_Telegram(saltaQuelliGiaFunzionanti);
                        break;
                    }
            }
        }

        private bool? CheckSeIlLinkVa3_Telegram(bool saltaQuelliGiaFunzionanti)
        {
            if (saltaQuelliGiaFunzionanti)
            {
                if (this.LinkFunzionante == true)
                    return true;
            }

            bool? works = null;
            for (int i = 0; i < 3; i++)
            {
                works = CheckSeIlLinkVa2_Telegram();
                if (works != null && works.Value == true)
                    return true;
            }

            return works;
        }

        private bool? CheckSeIlLinkVa2_Telegram()
        {
            string link = this.GetLink();
            string content = null;
            try
            {
                content = Download(link);
            }
            catch
            {
                ;
            }

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            else if (content.Contains("tg://") && content.Contains("Join Group") && (content.Contains("member")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Download(string uri)
        {
            WebClient client = new WebClient();

            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return s;
        }
    }
}