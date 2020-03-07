namespace JsonPolimi
{
    internal class InfoParteDiGruppo
    {
        public string testo_selvaggio;
        public LinkGruppo link;
        public ImmagineGruppo immagine;

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
    }
}