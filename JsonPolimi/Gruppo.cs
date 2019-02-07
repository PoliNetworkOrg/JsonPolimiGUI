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

        internal void Aggiusta()
        {
            classe = classe.Replace('\n', ' ');

            if (type is null || type.Length == 0)
            {
                type = "S";
            }
        }
    }
}