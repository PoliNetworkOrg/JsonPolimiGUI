namespace JsonPolimi_Core_nf.Tipi
{
    public class Riga
    {
        public Gruppo G;
        public int I;

        public Riga(Gruppo g, int i)
        {
            G = g;
            I = i;
        }

        public override string ToString()
        {
            return G + " " + I;
        }
    }
}