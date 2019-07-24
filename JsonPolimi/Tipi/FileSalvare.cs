using System;
using System.Collections.Generic;

namespace JsonPolimi.Tipi
{
    [Serializable]
    public class FileSalvare
    {
        public List<GruppoTelegram> Gruppi;

        public FileSalvare()
        {
            Gruppi = new List<GruppoTelegram>();
        }
    }
}