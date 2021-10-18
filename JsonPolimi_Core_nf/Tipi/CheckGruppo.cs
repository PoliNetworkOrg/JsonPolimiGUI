namespace JsonPolimi_Core_nf.Tipi
{
    public class CheckGruppo
    {
        //value
        public E n;

        public enum E
        {
            VECCHIA_RICERCA, NUOVA_RICERCA, TUTTO,
            RICERCA_SITO_V3
        }

        public CheckGruppo(E a)
        {
            this.n = a;
        }
    }
}