using HtmlAgilityPack;

namespace JsonPolimi
{
    internal class ImmagineGruppo
    {
        private HtmlNode htmlNode;

        public ImmagineGruppo(HtmlNode htmlNode)
        {
            this.htmlNode = htmlNode;
        }
    }
}