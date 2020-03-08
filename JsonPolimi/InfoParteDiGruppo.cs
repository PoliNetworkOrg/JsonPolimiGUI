using System.Collections.Generic;

namespace JsonPolimi
{
    internal class InfoParteDiGruppo
    {
        public string testo_selvaggio;
        public LinkGruppo link;
        public ImmagineGruppo immagine;
        public List<InfoParteDiGruppo> sottopezzi;

        public InfoParteDiGruppo(string testo_selvaggio)
        {
            this.testo_selvaggio = testo_selvaggio;
        }

        public InfoParteDiGruppo(LinkGruppo link)
        {
            this.link = link;
        }

        public InfoParteDiGruppo(ImmagineGruppo immagine)
        {
            this.immagine = immagine;
        }

        public InfoParteDiGruppo(List<InfoParteDiGruppo> sottopezzi)
        {
            this.sottopezzi = sottopezzi;
        }
    }
}