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
    }
}