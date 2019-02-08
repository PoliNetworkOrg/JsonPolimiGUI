using System;

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
        public string type;
        public string year;

        internal void Aggiusta()
        {
            classe = classe.Replace('\n', ' ');

            if (type is null)
            {
                type = "S";
            }

            if (year is null)
            {
                year = "2018/2019";
            }
        }

        internal string to_json()
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
            json += type;
            json += "\",\"year\":\"";
            json += year;
            json += "\",\"platform\":\"";
            json += platform;
            json += "\"";

            json += "}";

            return json;
        }
    }
}