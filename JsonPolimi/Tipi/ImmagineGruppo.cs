using HtmlAgilityPack;

namespace JsonPolimi.Tipi
{
    internal class ImmagineGruppo
    {
#pragma warning disable IDE0052 // Rimuovi i membri privati non letti
        private readonly HtmlNode htmlNode;
#pragma warning restore IDE0052 // Rimuovi i membri privati non letti

        public ImmagineGruppo(HtmlNode htmlNode)
        {
            this.htmlNode = htmlNode;
        }
    }
}