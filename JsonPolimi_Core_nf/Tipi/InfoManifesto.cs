using System;
using System.Collections.Generic;

namespace JsonPolimi_Core_nf.Tipi
{
    [Serializable]
    public class InfoManifesto
    {
        public List<string> Corso_di_studio { get; set; }
        public string[] Anni_di_corso_attivi { get; set; }
        public string Anno_accademico { get; set; }
        public string[] Sede_del_corso { get; set; }
        public string Durata_nominale_corso { get; set; }
        public string Sede { get; set; }
        public string Scuola { get; set; }
    }
}