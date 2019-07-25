using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPolimi
{
    public class Riga
    {
        public Gruppo g;
        public int i;

        public Riga(Gruppo g, int i)
        {
            this.g = g;
            this.i = i;
        }

        public override string ToString()
        {
            return this.g.ToString() + " " + this.i.ToString();
        }
    }
}