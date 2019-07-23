using System.Collections;
using System.Collections.Generic;

namespace JsonPolimi
{
    public class ListaGruppo : IEnumerable
    {
        private readonly List<Gruppo> _l;

        public ListaGruppo()
        {
            _l = new List<Gruppo>();
        }

        internal int GetCount()
        {
            return _l.Count;
        }

        internal Gruppo GetElem(int i)
        {
            return _l[i];
        }

        internal void Add(Gruppo g)
        {
            var a = Contiene(g.Id);
            if (!a) _l.Add(g);
        }

        private bool Contiene(string id)
        {
            foreach (var i in _l)
                if (i.Id == id)
                    return true;
            return false;
        }

        internal void Remove(int i)
        {
            _l.RemoveAt(i);
        }

        internal void SetElem(int i, Gruppo elem)
        {
            _l[i] = elem;
        }

        public IEnumerator GetEnumerator()
        {
            return this._l.GetEnumerator();
        }
    }
}