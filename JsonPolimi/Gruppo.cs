using System;
using System.Collections.Generic;

namespace JsonPolimi
{
    public class Gruppo
    {
        public string Classe;
        public string Degree;
        public string Id; // esempio: FB/2018/2019/LEONARDO/21432583243205
        public string IdLink; // esempio: 21432583243205
        public string Language;
        public string Office; // esempio: LEONARDO
        public string PermanentId; //per telegram, esempio -1000345953
        public string Platform; // esempio: FB
        public string School;
        public string Tipo;
        public string Year; // esempio: 2018/2019
        public DateTime? LastUpdateInviteLinkTime;
        public bool? isBot;

        internal void Aggiusta()
        {
            Classe = string.IsNullOrEmpty(Classe) ? "" : Classe.Replace('\n', ' ');

            if (string.IsNullOrEmpty(Tipo)) Tipo = "S";

            AggiustaAnno();

            if (!string.IsNullOrEmpty(Year))
                Tipo = "S";

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

            if (isBot == null)
                isBot = false;

            Id = CreaId();
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
            try
            {
                var r = Id.Split('/');
                return r[r.Length - 1];
            }
            catch
            {
                return null;
            }
        }

        private string CreaId()
        {
            return Platform + "/" + Year + "/" + Office + "/" + IdLink;
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

        internal string To_json()
        {
            var json = "{";

            json += "\"class\":\"";
            json += EscapeQuotes(Classe);
            json += "\",\"office\":\"";
            json += Office;
            json += "\",\"id\":\"";
            json += Id;
            json += "\",\"degree\":\"";
            json += Degree;
            json += "\",\"school\":\"";
            json += School;
            json += "\",\"id_link\":\"";
            json += IdLink;
            json += "\",\"language\":\"";
            json += Language;
            json += "\",\"type\":\"";
            json += Tipo;
            json += "\",\"year\":\"";
            json += Year;
            json += "\",\"permanentId\":\"";
            json += PermanentId;
            json += "\",\"LastUpdateInviteLinkTime\":\"";
            json += LastUpdateInviteLinkTime;
            json += "\",\"platform\":\"";
            json += Platform;
            json += "\"";

            json += "}";

            return json;
        }

        private static string EscapeQuotes(string s)
        {
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
            if (v == null)
                return;

            var vUpper = v.ToUpper();

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
            if (v == "<=") return;

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
            g.GruppoDiBase.Office = v;
            g.NomeOld.Office = v;
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
            return To_json() + " " + base.ToString();
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

            if (!string.IsNullOrEmpty(gruppo.Office) && string.IsNullOrEmpty(Office))
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

            isBot = gruppo.isBot;
        }
    }
}