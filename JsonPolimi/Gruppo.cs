using System;

namespace JsonPolimi
{
    public class Gruppo
    {
        public string Classe;
        public string Office;   // esempio: LEONARDO
        public string IdLink;  // esempio: 21432583243205
        public string Id;       // esempio: FB/2018/2019/LEONARDO/21432583243205
        public string Degree;
        public string School;
        public string Language;
        public string Platform; // esempio: FB
        public string Tipo;
        public string Year;     // esempio: 2018/2019

        internal void Aggiusta()
        {
            Classe = string.IsNullOrEmpty(Classe) ? "" : Classe.Replace('\n', ' ');

            if (string.IsNullOrEmpty(Tipo))
            {
                Tipo = "S";
            }

            if (string.IsNullOrEmpty(Year))
            {
                Year = "2018/2019";
            }

            if (string.IsNullOrEmpty(Language))
            {
                Language = IndovinaLaLinguaDalNome();
            }

            if (string.IsNullOrEmpty(School))
                School = IndovinaLaSchool();

            if (string.IsNullOrEmpty(Degree))
                Degree = IndovinaIlDegree();

            if (string.IsNullOrEmpty(Id))
                Id = CreaId();
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

        private string IndovinaLaLinguaDalNome(string default_language = "ITA")
        {
            var c = Classe.ToLower();

            if (c.Contains("and"))
                return "ENG";
            return c.Contains("for") ? "ENG" : default_language;
        }

        internal string To_json()
        {
            var json = "{";

            json += "\"class\":\"";
            json += Classe;
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
            json += "\",\"platform\":\"";
            json += Platform;
            json += "\"";

            json += "}";

            return json;
        }

        public static void AggiungiInformazioneAmbigua(string v, ref InsiemeDiGruppi g)
        {
            if (v == null)
                return;

            var v_upper = v.ToUpper();

            //bisogna capire che tipo di informazione stiamo ricevendo
            if (v.StartsWith("https://") || v.StartsWith("http://"))
            {
                AggiungiLink(v, ref g);
            }
            else if (v_upper == "LEONARDO" || v_upper == "MANTOVA" || v_upper == "BOVISA" || v_upper == "PIACENZA" ||
                        v_upper == "LECCO" || v_upper == "COMO" || v_upper == "CREMONA" || v_upper == "LEONARDO-CREMONA" ||
                        v_upper == "LEONARDO*")
            {
                AggiungiSede(v, ref g);
            }
            else if (v_upper == "FACEBOOK" || v_upper == "TELEGRAM" || v_upper == "NON ANCORA CREATO" || v_upper == "CORSI" || v_upper == "LUOGO" || v_upper.StartsWith("LAUREE"))
            {
                //è una cella inutile
                ;
            }
            else if (v_upper == "<=")
            {
                //è una cella inutile
                ;
            }
            else if (v.StartsWith("<text:a"))
            {
                var n1 = v.IndexOf("xlink:href", StringComparison.Ordinal);
                var s1 = v.Substring(n1 + 12);
                var s2 = s1.Split('"');

                var s3 = s2[1].Split('>');
                var s4 = s3[1].Split('<');

                var nome = s4[0];

                if (nome.StartsWith("http"))
                    AggiungiLink(s2[0], ref g);
                else
                {
                    AggiungiNome(nome, ref g);
                    AggiungiLink(s2[0], ref g);
                }
            }
            else
            {
                switch (v_upper)
                {
                    case "LT":
                    case "LM":
                    case "LU":
                        AggiungiTriennaleMagistrale(v_upper, ref g);
                        break;

                    case "3I":
                    case "DES":
                    case "AUIC":
                    case "ICAT":
                    case "3I+AUIC":
                    case "ICAT+3I":
                    case "DES+3I":
                        AggiungiScuola(v_upper, ref g);
                        break;

                    case "ITA":
                    case "ENG":
                    case "ITA-ENG":
                        AggiungiLingua(v_upper, ref g);
                        break;

                    default:
                        //altrimenti è il nome
                        AggiungiNome(v, ref g);
                        break;
                }
            }
        }

        private static void AggiungiLingua(string v_upper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.Language = v_upper;
            g.NomeOld.Language = v_upper;
        }

        private static void AggiungiScuola(string v_upper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.School = v_upper;
            g.NomeOld.School = v_upper;
        }

        private static void AggiungiTriennaleMagistrale(string v_upper, ref InsiemeDiGruppi g)
        {
            g.GruppoDiBase.Degree = v_upper;
            g.NomeOld.Degree = v_upper;
        }

        private static void AggiungiNome(string v, ref InsiemeDiGruppi g)
        {
            if (v == "<=")
            {
                return;
            }

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
            if (n2 >= 0 && n2 < s1.Length)
            {
                s1 = s1.Substring(4);
            }

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
    }
}