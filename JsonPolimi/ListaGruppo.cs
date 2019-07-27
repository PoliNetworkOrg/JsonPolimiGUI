using System;
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

        public IEnumerator GetEnumerator()
        {
            return _l.GetEnumerator();
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
            var (item1, item2) = Contiene(g);

            if (!item1)
            {
                _l.Add(g);
                return;
            }

            Merge(item2, g);
        }

        private void Merge(int i, Gruppo g)
        {
            _l[i].Merge(g);
        }

        private Tuple<bool, int> Contiene(Gruppo g)
        {
            for (var i = 0; i < _l.Count; i++)
            {
                if (!IsNullOrEmpty(_l[i].PermanentId) && !IsNullOrEmpty(g.PermanentId))
                    if (_l[i].PermanentId == g.PermanentId && _l[i].Platform == g.Platform)
                        return new Tuple<bool, int>(true, i);

                if (_l[i].Id == g.Id)
                    return new Tuple<bool, int>(true, i);

                var bt = _l[i].Platform == "TG" && g.Platform == "TG" && _l[i].Classe == g.Classe;
                bt &= !IsNullOrEmpty(_l[i].PermanentId) || !IsNullOrEmpty(g.PermanentId);
                if (bt)
                    return new Tuple<bool, int>(true, i);
            }

            return new Tuple<bool, int>(false, 0);
        }

        internal void Remove(int i)
        {
            _l.RemoveAt(i);
        }

        internal void SetElem(int i, Gruppo elem)
        {
            _l[i] = elem;
        }

        public void Sort()
        {
            IComparer<Gruppo> cc = new CoordinatesBasedComparer();
            _l.Sort(cc.Compare);
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
                if (i1 != 0)
                    return i1;

                i1 = CompareOrdinal(a.Platform, b.Platform);
                if (i1 != 0)
                    return i1;

                i1 = CompareOrdinal(a.Office, b.Office);
                if (i1 != 0)
                    return i1;

                i1 = CompareOrdinal(a.Degree, b.Degree);
                if (i1 != 0)
                    return i1;

                i1 = CompareOrdinal(a.IdLink, b.IdLink);
                if (i1 != 0)
                    return i1;

                return CompareOrdinal(a.PermanentId, b.PermanentId);
            }
        }

        public void MergeUnione(decimal v1, decimal v2)
        {
            Merge((int)v1, this.GetElem((int)v2));
            _l.RemoveAt((int)v2);
        }

        public void MergeLink(int v1, int v2)
        {
            _l[v1].IdLink = _l[v2].IdLink;

            switch (_l[v1].LastUpdateInviteLinkTime)
            {
                case null when _l[v2].LastUpdateInviteLinkTime == null:
                    _l[v1].LastUpdateInviteLinkTime = _l[v2].LastUpdateInviteLinkTime;
                    break;

                case null:
                    _l[v1].LastUpdateInviteLinkTime = _l[v2].LastUpdateInviteLinkTime;
                    break;

                default:
                    {
                        if (_l[v2].LastUpdateInviteLinkTime != null)
                        {
                            var r = DateTime.Compare(_l[v1].LastUpdateInviteLinkTime.Value,
                                _l[v2].LastUpdateInviteLinkTime.Value);
                            if (r < 0)
                            {
                                _l[v1].LastUpdateInviteLinkTime = _l[v2].LastUpdateInviteLinkTime;
                            }
                        }

                        break;
                    }
            }

            if (_l[v1].LastUpdateInviteLinkTime == null)
                _l[v1].LastUpdateInviteLinkTime = DateTime.Now;

            _l[v1].Aggiusta();
        }
    }
}