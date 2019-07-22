using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPolimi
{
    public class ListaGruppo
    {
        private List<Gruppo> L = null;

        public ListaGruppo()
        {
            L = new List<Gruppo>();
        }

        internal int GetCount()
        {
            return L.Count;
        }

        internal Gruppo GetElem(int i)
        {
            return L[i];
        }

        internal void Add(Gruppo g)
        {
            var a = this.Contiene(g.id);
            if (!a)
            {
                L.Add(g);
            }
        }

        private bool Contiene(string id)
        {
            foreach (var i in L)
            {
                if (i.id == id)
                    return true;
            }
            return false;
        }

        internal void Remove(int i)
        {
            L.RemoveAt(i);
        }

        internal void SetElem(int i, Gruppo elem)
        {
            L[i] = elem;
        }
    }
}