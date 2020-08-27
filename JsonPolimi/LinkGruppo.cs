using HtmlAgilityPack;

namespace JsonPolimi
{
    internal class LinkGruppo
    {
        public HtmlAttributeCollection attributes;
        public string v;

        public LinkGruppo(HtmlAttributeCollection attributes, string v)
        {
            this.attributes = attributes;
            this.v = v;
        }
    }
}