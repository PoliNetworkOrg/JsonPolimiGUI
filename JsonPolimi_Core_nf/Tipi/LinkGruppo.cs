using HtmlAgilityPack;

namespace JsonPolimi_Core_nf.Tipi
{
    public class LinkGruppo
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