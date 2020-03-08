using System.Collections.Generic;

namespace JsonPolimi
{
    public class InfoManifesto
    {
        public List<string> corso_di_studio { get; internal set; }
        public string[] anni_di_corso_attivi { get; internal set; }
        public string anno_accademico { get; internal set; }
        public string[] sede_del_corso { get; internal set; }
        public string durata_nominale_corso { get; internal set; }
        public string sede { get; internal set; }
        public string scuola { get; internal set; }
    }
}