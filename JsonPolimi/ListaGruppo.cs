using System.Collections.Generic;

namespace JsonPolimi
{
    public class ListaGruppo
    {
        private readonly List<Gruppo> _l = null;

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
            var a = this.Contiene(g.id);
            if (!a)
            {
                _l.Add(g);
            }
        }

        private bool Contiene(string id)
        {
            foreach (var i in _l)
            {
                if (i.id == id)
                    return true;
            }
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
    }
}