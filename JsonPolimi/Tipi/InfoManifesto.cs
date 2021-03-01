using System;
using System.Collections.Generic;

namespace JsonPolimi.Tipi
{
    [Serializable]
    public class InfoManifesto
    {
        public List<string> Corso_di_studio { get; internal set; }
        public string[] Anni_di_corso_attivi { get; internal set; }
        public string Anno_accademico { get; internal set; }
        public string[] Sede_del_corso { get; internal set; }
        public string Durata_nominale_corso { get; internal set; }
        public string Sede { get; internal set; }
        public string Scuola { get; internal set; }
    }
}