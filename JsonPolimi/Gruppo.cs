using System;
using System.Collections.Generic;

namespace JsonPolimi
{
    public class Gruppo
    {
        public string classe;
        public string office;
        public string id;
        public string degree;
        public string school;
        public string language;
        public string platform;
        public string tipo;
        public string year;

        internal void Aggiusta()
        {
            if (classe == null)
                classe = "";

            classe = classe.Replace('\n', ' ');

            if (tipo is null)
            {
                tipo = "S";
            }

            if (year is null)
            {
                year = "2018/2019";
            }
        }

        internal string To_json()
        {
            string json = "{";

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
            string v_upper = v.ToUpper();

            //bisogna capire che tipo di informazione stiamo ricevendo
            if (v.StartsWith("https://") || v.StartsWith("http://"))
            {
                AggiungiLink(v, ref g);
            }
            else if (v_upper == "LEONARDO" || v_upper == "MANTOVA" || v_upper == "BOVISA" || v_upper == "PIACENZA" || v_upper == "LECCO")
            {
                AggiungiSede(v, ref g);
                    
            }
            else if (v_upper == "FACEBOOK" || v_upper == "TELEGRAM" || v_upper == "NON ANCORA CREATO")
            {
                //è una cella inutile
                ;
            }
            else if (v.StartsWith("<text:a"))
            {
                int n1 = v.IndexOf("xlink:href");
                string s1 = v.Substring(n1 + 12);
                string[] s2 = s1.Split('"');
                AggiungiLink(s2[0], ref g);
            }
            else
            {
                //altrimenti è il nome
                AggiungiNome(v, ref g);
            }
        }

        private static void AggiungiNome(string v, ref InsiemeDiGruppi g)
        {          
            g.gruppo_di_base.classe = v;
        }

        private static void AggiungiSede(string v, ref InsiemeDiGruppi g)
        {
            g.gruppo_di_base.office = v;
        }

        private static void AggiungiLink(string v, ref InsiemeDiGruppi g)
        {
            Gruppo g2 = new Gruppo();

            int n1 = v.IndexOf("://");
            string s1 = v.Substring(n1 + 3);

            int n2 = s1.IndexOf("www.");
            if (n2>=0 && n2<s1.Length)
            {
                s1 = s1.Substring(4);
            }

            if (s1[0] == 'f') // facebook
            {
                g2.platform = "FB";

                string[] s2 = s1.Split('/');
                if (s2[1] == "groups")
                {
                    g2.id = s2[2];
                }
                else
                {
                    g2.id = s2[1];
                }
            }
            else if (s1[0] == 't')
            {
                g2.platform = "TG";

                string[] s2 = s1.Split('/');
                if (s2[1] == "joinchat")
                {
                    g2.id = s2[2];
                }
                else
                {
                    g2.id = s2[1];
                }
            }

            g.L.Add(g2);
        }
    }
}