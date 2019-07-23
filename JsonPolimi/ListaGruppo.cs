using System.Collections;
using System.Collections.Generic;
using static System.String;

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

        public void Sort()
        {
            IComparer<Gruppo> cc = new CoordinatesBasedComparer();
            this._l.Sort(cc.Compare);
        }

        public class CoordinatesBasedComparer : IComparer<Gruppo>
        {
            public int Compare(Gruppo a, Gruppo b)
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                var i1 = CompareOrdinal(a.Year, b.Year);
                if (i1 != 0)
                    return i1;

                i1 = CompareOrdinal(a.Classe, b.Classe);
                return i1 != 0 ? i1 : CompareOrdinal(a.Platform, b.Platform);
            }
        }
    }
}