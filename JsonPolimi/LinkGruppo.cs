using HtmlAgilityPack;

namespace JsonPolimi
{
    internal class LinkGruppo
    {
        private HtmlAttributeCollection attributes;
        private string v;

        public LinkGruppo(HtmlAttributeCollection attributes, string v)
        {
            this.attributes = attributes;
            this.v = v;
        }
    }
}