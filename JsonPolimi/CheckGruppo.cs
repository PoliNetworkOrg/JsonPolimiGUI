namespace JsonPolimi
{
    internal class CheckGruppo
    {

        //value
        public E n;

        public enum E { VECCHIA_RICERCA, NUOVA_RICERCA, TUTTO }

        public CheckGruppo(E a)
        {
            this.n = a;
        }
    }
}