using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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