using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPolimi
{
    public class SomiglianzaClasse
    {
        public SomiglianzaEnum somiglianzaEnum;
        public Gruppo a1;
        public Gruppo a2;

        public SomiglianzaClasse(SomiglianzaEnum dIVERSI, Gruppo a1 = null, Gruppo a2 = null)
        {
            this.somiglianzaEnum = dIVERSI;
            this.a1 = a1;
            this.a2 = a2;
        }
    }
}
