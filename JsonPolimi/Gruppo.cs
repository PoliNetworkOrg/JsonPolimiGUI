namespace JsonPolimi
{
    public class Gruppo
    {
        public string classe;
        public string office;   // esempio: LEONARDO
        public string id_link;  // esempio: 21432583243205
        public string id;       // esempio: FB/2018/2019/LEONARDO/21432583243205
        public string degree;
        public string school;
        public string language;
        public string platform; // esempio: FB
        public string tipo;
        public string year;     // esempio: 2018/2019

        internal void Aggiusta()
        {
            classe = string.IsNullOrEmpty(classe) ? "" : classe.Replace('\n', ' ');

            if (string.IsNullOrEmpty(tipo))
            {
                tipo = "S";
            }

            if (string.IsNullOrEmpty(year))
            {
                year = "2018/2019";
            }

            if (string.IsNullOrEmpty(language))
            {
                language = IndovinaLaLinguaDalNome();
            }

            if (string.IsNullOrEmpty(school))
                school = IndovinaLaSchool();

            if (string.IsNullOrEmpty(degree))
                degree = IndovinaIlDegree();

            if (string.IsNullOrEmpty(id))
                id = CreaId();
        }

        private string CreaId()
        {
            return platform + "/" + year + "/" + office + "/" + id_link;
        }

        private string IndovinaIlDegree()
        {
            //throw new NotImplementedException();
            return null;
        }

        private string IndovinaLaSchool()
        {
            //throw new NotImplementedException();
            return null;
        }

        private string IndovinaLaLinguaDalNome(string default_language = "ITA")
        {
            var c = classe.ToLower();

            if (c.Contains("and"))
                return "ENG";
            return c.Contains("for") ? "ENG" : default_language;
        }

        internal string To_json()
        {
            var json = "{";

            json += "\"class\":\"";
            json += classe;
            json += "\",\"office\":\"";
            json += office;
            json += "\",\"id\":\"";
            json += id;
            json += "\",\"degree\":\"";
            json += degree;
            json += "\",\"school\":\"";
            json += school;
            json += "\",\"id_link\":\"";
            json += id_link;
            json += "\",\"language\":\"";
            json += language;
            json += "\",\"type\":\"";
            json += tipo;
            json += "\",\"year\":\"";
            json += year;
            json += "\",\"platform\":\"";
            json += platform;
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
                var n1 = v.IndexOf("xlink:href");
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
            g.gruppo_di_base.language = v_upper;
            g.nome_old.language = v_upper;
        }

        private static void AggiungiScuola(string v_upper, ref InsiemeDiGruppi g)
        {
            g.gruppo_di_base.school = v_upper;
            g.nome_old.school = v_upper;
        }

        private static void AggiungiTriennaleMagistrale(string v_upper, ref InsiemeDiGruppi g)
        {
            g.gruppo_di_base.degree = v_upper;
            g.nome_old.degree = v_upper;
        }

        private static void AggiungiNome(string v, ref InsiemeDiGruppi g)
        {
            if (v == "<=")
            {
                return;
            }

            if (string.IsNullOrEmpty(g.gruppo_di_base.classe))
            {
                g.gruppo_di_base.classe = v;
            }
            else
            {
                g.gruppo_di_base.classe += " ";
                g.gruppo_di_base.classe += v;
            }
            g.nome_old.classe = g.gruppo_di_base.classe;
        }

        private static void AggiungiSede(string v, ref InsiemeDiGruppi g)
        {
            g.gruppo_di_base.office = v;
            g.nome_old.office = v;
        }

        private static void AggiungiLink(string v, ref InsiemeDiGruppi g)
        {
            var g2 = new Gruppo();

            var n1 = v.IndexOf("://");
            var s1 = v.Substring(n1 + 3);

            var n2 = s1.IndexOf("www.");
            if (n2 >= 0 && n2 < s1.Length)
            {
                s1 = s1.Substring(4);
            }

            if (s1[0] == 'f') // facebook
            {
                g2.platform = "FB";

                var s2 = s1.Split('/');
                g2.id_link = s2[1] == "groups" ? s2[2] : s2[1];
            }
            else if (s1[0] == 't') // telegram
            {
                g2.platform = "TG";

                var s2 = s1.Split('/');
                g2.id_link = s2[1] == "joinchat" ? s2[2] : s2[1];
            }
            else if (s1[0] == 'i') // instagram
            {
                g2.platform = "IG";

                var s2 = s1.Split('/');

                g2.id_link = s2[1];
            }
            else if (s1[0] == 'c') //whatsapp
            {
                g2.platform = "WA";

                var s2 = s1.Split('/');

                g2.id_link = s2[1];
            }
            else
            {
                ;
            }

            g.L.Add(g2);
        }
    }
}