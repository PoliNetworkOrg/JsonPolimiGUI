using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
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
                bool bt2 = IsNullOrEmpty(_l[i].Year) || IsNullOrEmpty(g.Year);

                if (!IsNullOrEmpty(_l[i].Year) && !IsNullOrEmpty(g.Year))
                    if (_l[i].Year != g.Year)
                        bt2 = false;

                if (bt && bt2)
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

                i1 = CompareOrdinal2(a.Office, b.Office);
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

        public static int CompareOrdinal2(ListaStringhePerJSON office1, ListaStringhePerJSON office2)
        {
            if (office1 == null && office2 == null)
                return 0;

            if (office1 == null)
                return -1;

            if (office2 == null)
                return +1;

            return ListaGruppo.CompareOrdinal23(office1.o, office2.o);
        }

        private static int CompareOrdinal23(List<string> office1, List<string> office2)
        {
            if (office1 == null && office2 == null)
                return 0;

            if (office1 == null)
                return -1;

            if (office2 == null)
                return 1;

            office1.Sort();
            office2.Sort();

            if (office1.Count == office2.Count)
            {
                for (int i = 0; i < office1.Count; i++)
                {
                    int i1 = CompareOrdinal(office1[i].ToLower(), office2[i].ToLower());
                    if (i1 != 0)
                        return i1;
                }

                return 0;
            }
            else
            {
                if (office1.Count > office2.Count)
                {
                    for (int i = 0; i < office2.Count; i++)
                    {
                        int i1 = CompareOrdinal(office1[i], office2[i]);
                        if (i1 != 0)
                            return i1;
                    }

                    return 1;
                }
                else
                {
                    for (int i = 0; i < office1.Count; i++)
                    {
                        int i1 = CompareOrdinal(office1[i], office2[i]);
                        if (i1 != 0)
                            return i1;
                    }

                    return -1;
                }
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

            _l[v1].Aggiusta(true);
        }

        internal void ProvaAdUnire()
        {
            for (int i = 0; i < this._l.Count; i++)
            {
                this._l[i].Aggiusta(true);
                for (int j = this._l.Count - 1; j >= 0; j--)
                {
                    if (i != j)
                    {
                        Tuple<SomiglianzaClasse, Gruppo> r = Equivalenti(i, j);

                        bool do_that = false;
                        if (r.Item1.somiglianzaEnum == SomiglianzaEnum.IDENTITICI)
                        {
                            do_that = true;
                        }
                        else if (r.Item1.somiglianzaEnum == SomiglianzaEnum.DUBBIO)
                        {
                            //TODO: chiedere all'utente
                        }


                        if (do_that) 
                        { 
                            if (i > j)
                            {
                                this._l.RemoveAt(i);
                                this._l.RemoveAt(j);
                            }
                            else
                            {
                                this._l.RemoveAt(j);
                                this._l.RemoveAt(i);
                            }

                            this._l.Add(r.Item2);

                            i--;
                            j++;

                            if (i < 0)
                                i = 0;

                            if (j > this._l.Count - 1)
                                j = this._l.Count - 1;
                        }
                    }
                }
            }
        }

        private Tuple<SomiglianzaClasse, Gruppo> Equivalenti(int i, int j)
        {
            return Equivalenti2(i, this._l[j]);
            
        }

        private Tuple<SomiglianzaClasse, Gruppo> Equivalenti2(int i, Gruppo j)
        {
            Gruppo a1 = this._l[i];
            Gruppo a2 = j;

            SomiglianzaClasse eq = Equivalenti3(a1, a2);
            if (eq.somiglianzaEnum == SomiglianzaEnum.IDENTITICI || eq.somiglianzaEnum == SomiglianzaEnum.DUBBIO)
            {
                ;
                return new Tuple<SomiglianzaClasse, Gruppo>(eq, Unisci4(i, j));
            }

            return new Tuple<SomiglianzaClasse, Gruppo>(new SomiglianzaClasse(SomiglianzaEnum.DIVERSI), null);       
        }

        private SomiglianzaClasse Equivalenti3(Gruppo a1, Gruppo a2)
        {
            var r1 = Equivalenti5(a1, a2);

            if (r1.somiglianzaEnum == SomiglianzaEnum.IDENTITICI)
            {
                if (string.IsNullOrEmpty(a1.IDCorsoPolimi) && !string.IsNullOrEmpty(a2.IDCorsoPolimi))
                {
                    r1.somiglianzaEnum = SomiglianzaEnum.DUBBIO;
                }
                else if (!string.IsNullOrEmpty(a1.IDCorsoPolimi) && string.IsNullOrEmpty(a2.IDCorsoPolimi))
                {
                    r1.somiglianzaEnum = SomiglianzaEnum.DUBBIO;
                }
            }

            if (r1.somiglianzaEnum == SomiglianzaEnum.DUBBIO)
            {
                if (!string.IsNullOrEmpty(a1.Tipo) && !string.IsNullOrEmpty(a2.Tipo) &&
                    a1.Tipo.ToLower() != a2.Tipo.ToLower())
                {
                    r1.somiglianzaEnum = SomiglianzaEnum.DIVERSI;
                }
            }

            return r1;
        }

        private SomiglianzaClasse Equivalenti5(Gruppo a1, Gruppo a2)
        {
            if (!string.IsNullOrEmpty(a1.IDCorsoPolimi) && !string.IsNullOrEmpty(a2.IDCorsoPolimi) &&
                a1.IDCorsoPolimi == a2.IDCorsoPolimi)
            {
                return new SomiglianzaClasse(SomiglianzaEnum.IDENTITICI);
            }

            if (!string.IsNullOrEmpty(a1.IDCorsoPolimi) && !string.IsNullOrEmpty(a2.IDCorsoPolimi) &&
                a1.IDCorsoPolimi != a2.IDCorsoPolimi)
            {
                return new SomiglianzaClasse(SomiglianzaEnum.DIVERSI);
            }

            if (a1.PermanentId == a2.PermanentId && !String.IsNullOrEmpty(a1.PermanentId))
                return new SomiglianzaClasse(SomiglianzaEnum.IDENTITICI);
            else
            {
                if (!string.IsNullOrEmpty(a1.PermanentId))
                {
                    if (!string.IsNullOrEmpty(a2.PermanentId))
                    {
                        if (a1.PermanentId != a2.PermanentId)
                            return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                    }
                }
            }

            if (a1.Id == a2.Id && !String.IsNullOrEmpty(a1.Id))
                return new SomiglianzaClasse(SomiglianzaEnum.IDENTITICI);

            if (!String.IsNullOrEmpty(a1.Year))
            {
                if (!String.IsNullOrEmpty(a2.Year))
                {
                    if (a1.Year != a2.Year)
                        return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                }
            }

            if (!String.IsNullOrEmpty(a1.Platform))
            {
                if (!String.IsNullOrEmpty(a2.Platform))
                {
                    if (a1.Platform != a2.Platform)
                        return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                }
            }

            if (!Gruppo.IsEmpty(a1.Office))
            {
                if (!Gruppo.IsEmpty(a2.Office))
                {
                    int i1 = ListaGruppo.CompareOrdinal2(a1.Office, a2.Office);
                    if (i1 != 0)
                        return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI, a1, a2);
                }
            }

            if (!String.IsNullOrEmpty(a1.Degree))
            {
                if (!String.IsNullOrEmpty(a2.Degree))
                {
                    if (a1.Degree != a2.Degree)
                        return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                }
            }

            if (String.IsNullOrEmpty(a1.Classe))
                return new SomiglianzaClasse( SomiglianzaEnum.DUBBIO,a1,a2);

            if (String.IsNullOrEmpty(a2.Classe))
                return new SomiglianzaClasse( SomiglianzaEnum.DUBBIO,a1,a2);

            string[] s1 = a1.Classe.ToLower().Split(' ');
            string[] s2 = a2.Classe.ToLower().Split(' ');
            List<string> sa1 = new List<string>();
            List<string> sa2 = new List<string>();
            sa1.AddRange(s1);
            sa2.AddRange(s2);
            if (sa1.Contains("magistrale"))
            {
                if (String.IsNullOrEmpty(a2.Degree))
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                if (a2.Degree.ToLower() != "lm")
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
            }
            else if (sa1.Contains("triennale"))
            {
                if (String.IsNullOrEmpty(a2.Degree))
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                if (a2.Degree.ToLower() != "lt")
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
            }
            else if (sa2.Contains("magistrale"))
            {
                if (String.IsNullOrEmpty(a1.Degree))
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                if (a1.Degree.ToLower() != "lm")
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
            }
            else if (sa2.Contains("triennale"))
            {
                if (String.IsNullOrEmpty(a1.Degree))
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
                if (a1.Degree.ToLower() != "lt")
                    return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
            }

            var b1 = (NomiSimili(a1.Classe, a2.Classe));

            if (b1 == SomiglianzaEnum.DUBBIO)
            {
                return new SomiglianzaClasse( SomiglianzaEnum.DUBBIO, a1, a2);
            }
            else if (b1 == SomiglianzaEnum.IDENTITICI) 
            { 
                if (!string.IsNullOrEmpty(a1.IdLink) && string.IsNullOrEmpty(a2.IdLink) &&
                        string.IsNullOrEmpty(a1.IDCorsoPolimi) && !string.IsNullOrEmpty(a2.IDCorsoPolimi))
                {
                    return new SomiglianzaClasse( SomiglianzaEnum.DUBBIO,a1,a2);
                }

                return new SomiglianzaClasse( SomiglianzaEnum.IDENTITICI);
            }

            return new SomiglianzaClasse( SomiglianzaEnum.DIVERSI);
        }

        private SomiglianzaEnum NomiSimili(string n1, string n2)
        {
            if (n1.Length == 0 || n2.Length == 0)
                return SomiglianzaEnum.DIVERSI;

            if (n1 == n2)
                return SomiglianzaEnum.IDENTITICI;

            if (n1.ToLower() == n2.ToLower())
                return SomiglianzaEnum.DUBBIO;

            try
            {
                string[] s1 = n1.Split(' ');
                string[] s2 = n2.Split(' ');

                List<string> l1 = new List<string>();
                List<string> l2 = new List<string>();

                l1.AddRange(s1);
                l2.AddRange(s2);

                //lower words
                for (int i = 0; i < l1.Count; i++)
                {
                    l1[i] = l1[i].ToLower();
                }
                for (int i = 0; i < l2.Count; i++)
                {
                    l2[i] = l2[i].ToLower();
                }

                TryRename(ref l1, ref l2);

                //remove duplicate
                List<string> la1 = new List<string>();
                List<string> la2 = new List<string>();
                for (int i = 0; i < l1.Count; i++)
                {
                    if (!la1.Contains(l1[i]))
                        la1.Add(l1[i]);
                }
                for (int i = 0; i < l2.Count; i++)
                {
                    if (!la2.Contains(l2[i]))
                        la2.Add(l2[i]);
                }
                l1 = la1;
                l2 = la2;

                List<string> no_merge = new List<string>() { "analisi", "vehicles", "440" };
                foreach (string no_merge_s in no_merge)
                {
                    if (l1.Contains(no_merge_s) || l2.Contains(no_merge_s))
                        return SomiglianzaEnum.DIVERSI;
                }

                List<Tuple<string, string>> no_merge2 = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("chimica", "elettrica")
                };

                foreach (var no_merge_s2 in no_merge2)
                {
                    if (l1.Contains(no_merge_s2.Item1) && l1.Contains(no_merge_s2.Item2))
                        return SomiglianzaEnum.DIVERSI;
                    if (l2.Contains(no_merge_s2.Item1) && l2.Contains(no_merge_s2.Item2))
                        return SomiglianzaEnum.DIVERSI;
                }

                //remove useless words
                List<string> useless = new List<string>() {
                    "polimi", "politecnico", "and",
                    "engineering", "in",
                    "generale", "gruppo", "for", "the",
                    "e", "delle", "dei",
                    "ingegneria", "della", "-", "", "per", "le" };
                TryRemove(ref l1, ref l2, useless);

                bool? r2 = ManualCheck(n1, n2);
                if (r2 != null)
                {
                    return r2.Value ? SomiglianzaEnum.IDENTITICI : SomiglianzaEnum.DIVERSI;
                }

                //count how many words are in common
                List<string> quanti = new List<string>();
                for (int i = 0; i < l1.Count; i++)
                {
                    if (l2.Contains(l1[i]))
                        quanti.Add(l1[i]);
                }

                int minimo = 0;
                if (l1.Count + l2.Count > 14)
                {
                    minimo = 5;
                }
                else if (l1.Count + l2.Count > 11)
                {
                    minimo = 4;
                }
                else if (l1.Count + l2.Count > 5)
                {
                    minimo = 2;
                }
                else
                {
                    minimo = 1;
                }

                if (quanti.Count == 1)
                {
                    if (quanti[0] == "design")
                        return SomiglianzaEnum.DIVERSI;
                    if (quanti[0] == "architettura")
                        return SomiglianzaEnum.DIVERSI;
                }

                if (quanti.Count == minimo)
                    return SomiglianzaEnum.DUBBIO;
                if (quanti.Count == minimo + 1)
                    return SomiglianzaEnum.DUBBIO;
                if (quanti.Count >= minimo)
                    return SomiglianzaEnum.IDENTITICI;
            }
            catch
            {
                return SomiglianzaEnum.DUBBIO;
            }

            return SomiglianzaEnum.DIVERSI;
        }

        private void TryRename(ref List<string> l1, ref List<string> l2)
        {
            List<Tuple<string, string>> rename = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("informatica", "info")
            };

            foreach (var ren in rename)
            {
                for (int i = 0; i < l1.Count; i++)
                {
                    if (l1[i] == ren.Item1)
                        l1[i] = ren.Item2;
                }

                for (int i = 0; i < l2.Count; i++)
                {
                    if (l2[i] == ren.Item1)
                        l2[i] = ren.Item2;
                }
            }
        }

        private bool? ManualCheck(string n1, string n2)
        {
            if (n1[n1.Length - 1] == ' ')
                n1 = n1.Remove(n1.Length - 1);

            if (n2[n2.Length - 1] == ' ')
                n2 = n2.Remove(n2.Length - 1);

            n1 = n1.ToLower();
            n2 = n2.ToLower();

            if (n1 == "civil engineering magistrale" && n2 == "civil engineering for risk mitigation")
                return false;

            if (n2 == "civil engineering magistrale" && n1 == "civil engineering for risk mitigation")
                return false;

            if (n1 == "telecomunicazioni - informatica polimi" && n2 == "ingegneria informatica")
                return false;

            if (n2 == "telecomunicazioni - informatica polimi" && n1 == "ingegneria informatica")
                return false;

            if (n1 == "computer science and engineering - ingegneria informatica" && n2 == "telecomunicazioni - informatica polimi")
                return false;

            if (n2 == "computer science and engineering - ingegneria informatica" && n1 == "telecomunicazioni - informatica polimi")
                return false;

            if (n1 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale" && n2 == "design del prodotto industriale")
                return false;

            if (n2 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale" && n1 == "design del prodotto industriale")
                return false;

            if (n1 == "ingegneria per l'ambiente e il territorio - environmental and land planning engineering" && n2 == "urban planning and policy design - pianificazione urbana e politiche territoriali")
                return false;

            if (n2 == "ingegneria per l'ambiente e il territorio - environmental and land planning engineering" && n1 == "urban planning and policy design - pianificazione urbana e politiche territoriali")
                return false;

            if (n1 == "architettura e disegno urbano - architecture and urban design" && n2 == "urban planning and policy design - pianificazione urbana e politiche territoriali")
                return false;

            if (n2 == "architettura e disegno urbano - architecture and urban design" && n1 == "urban planning and policy design - pianificazione urbana e politiche territoriali")
                return false;

            if (n1 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale" && n2 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale" && n1 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "design for the fashion system - design per il sistema moda" &&
                n2 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "design for the fashion system - design per il sistema moda" &&
                n1 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "integrated product design" &&
                n2 == "design for the fashion system - design per il sistema moda design & engineering - progetto e ingegnerizzazione del prodotto industriale product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "integrated product design" &&
                n1 == "design for the fashion system - design per il sistema moda design & engineering - progetto e ingegnerizzazione del prodotto industriale product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "design for the fashion system - design per il sistema moda" && n2 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "design for the fashion system - design per il sistema moda" && n1 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "integrated product design" && n2 == "design for the fashion system - design per il sistema moda product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "integrated product design" && n1 == "design for the fashion system - design per il sistema moda product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "integrated product design" && n2 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "integrated product design" && n1 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "integrated product design design for the fashion system - design per il sistema moda product service system design - design per il sistema prodotto servizio" &&
                n2 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale")
                return false;

            if (n2 == "integrated product design design for the fashion system - design per il sistema moda product service system design - design per il sistema prodotto servizio" &&
                n1 == "design & engineering - progetto e ingegnerizzazione del prodotto industriale")
                return false;

            if (n1 == "computer science and engineering - ingegneria informatica" && n2 == "polimi info 2019/2020")
                return false;

            if (n2 == "computer science and engineering - ingegneria informatica" && n1 == "polimi info 2019/2020")
                return false;

            if (n1 == "ingegneria per l'ambiente e il territorio - polimi" && n2 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n2 == "ingegneria per l'ambiente e il territorio - polimi" && n1 == "product service system design - design per il sistema prodotto servizio")
                return false;

            if (n1 == "digital and interaction design")
            {
                if (n2.Contains("systems"))
                    return false;
            }

            if (n2 == "digital and interaction design")
            {
                if (n1.Contains("systems"))
                    return false;
            }

            if (n1 == "economia e organiz. aziendale b - elettrica")
                return false;

            if (n2 == "economia e organiz. aziendale b - elettrica")
                return false;

            if (n1 == "fisica 1" || n2 == "fisica 1")
                return false;

            if (n1 == "fisica tecnica" || n2 == "fisica tecnica")
                return false;

            if (n1 == "fisica tecnica e macchine" || n2 == "fisica tecnica e macchine")
                return false;

            if (n1 == "fondamenti di chimica - elettrotecnica" || n2 == "fondamenti di chimica - elettrotecnica")
                return false;

            if (n1 == "fondamenti di elettronica" || n2 == "fondamenti di elettronica")
                return false;

            if (n1 == "fondamenti di informatica" || n2 == "fondamenti di comunicazioni e internet")
                return false;

            if (n1 == "fondamenti di automatica" || n2 == "fondamenti di informatica")
                return false;

            if (n1 == "communication network design" || n2 == "communication network design")
                return false;

            if (n1 == "informatica online polimi" || n2 == "informatica online polimi")
                return false;

            if (n1 == "informatica e diritto" || n2 == "informatica e diritto")
                return false;

            if (n1 == "meccanica (per ing informatica)" || n2 == "meccanica (per ing informatica)")
                return false;

            if (n1 == "philosophical issues of computer science" || n2 == "philosophical issues of computer science")
                return false;

            if (n1 == "progetto di ingegneria del software" || n2 == "progetto di ingegneria del software")
                return false;

            if (n1 == "progetto di ingegneria informatica" || n2 == "progetto di ingegneria informatica")
                return false;

            if (n1 == "sistemi informatici" || n2 == "sistemi informatici")
                return false;

            if (n1 == "informatica" || n2 == "informatica")
                return false;

            if (n1 == "theoretical computer science" || n2 == "theoretical computer science")
                return false;

            if (n1 == "algebra and mathematical logic" || n2 == "algebra and mathematical logic")
                return false;

            if (n1 == "computer ethics" || n2 == "computer ethics")
                return false;

            if (n1 == "computer graphics" || n2 == "computer graphics")
                return false;

            if (n1 == "data management for the web" || n2 == "data management for the web")
                return false;

            if (n1 == "digital project management" || n2 == "digital project management")
                return false;

            if (n1 == "progetto sistemi informativi" || n2 == "progetto sistemi informativi")
                return false;

            if (n1 == "computer security" || n2 == "computer security")
                return false;

            return null;
        }

        internal void Importa(List<Gruppo> l2)
        {
            foreach (var l3 in l2)
            {
                Importa2(l3);
            }
        }

        private void Importa2(Gruppo l3)
        {
            for (int i = 0; i < _l.Count; i++)
            {
                Tuple<SomiglianzaClasse, Gruppo> r = Equivalenti2(i, l3);
                bool do_that = false;
                if (r.Item1.somiglianzaEnum == SomiglianzaEnum.IDENTITICI)
                {
                    do_that = true;
                }
                else if (r.Item1.somiglianzaEnum == SomiglianzaEnum.DUBBIO)
                {
                    string s = "Sono da unire?";
                    s += '\n';
                    s += '\n';

                    s += r.Item1.a1.To_json();

                    s += '\n';
                    s += '\n';

                    s += r.Item1.a2.To_json();

                    bool to_show = true;

                    //todo: E' TEMPORANEA QUESTA COSA
                    if (r.Item1.a2.CCS.Contains_In_Uno("Architettura"))
                    {
                        to_show = false;
                    }
                    else if (r.Item1.a2.CCS.Contains_In_Uno("Edile"))
                    {
                        to_show = false;
                    }
                    //FINE TEMP

                    if (to_show)
                    {
                        DialogResult dialogResult = MessageBox.Show(s, "Sono da unire?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            do_that = true;
                        }
                    }
                    else
                    {
                        do_that = false;
                    }

                }

                if (do_that)
                {
                    this._l[i] = r.Item2;
                    return;
                }
            }

            ;
            this._l.Add(l3);
        }

        private void TryRemove(ref List<string> l1, ref List<string> l2, List<string> to_remove)
        {
            foreach (string s in to_remove)
            {
                try
                {
                    l1.Remove(s);
                }
                catch
                {
                    ;
                }

                try
                {
                    l2.Remove(s);
                }
                catch
                {
                    ;
                }
            }
        }

        private Gruppo Unisci4(int i, Gruppo j)
        {
            Gruppo a1 = this._l[i];
            Gruppo a2 = j;

            Gruppo r = a1.Clone();

            if (String.IsNullOrEmpty(r.IDCorsoPolimi))
                r.IDCorsoPolimi = a2.IDCorsoPolimi;

            if (r.Manifesto == null)
                r.Manifesto = a2.Manifesto;

            if (r.AnnoCorsoStudio == null)
                r.AnnoCorsoStudio = a2.AnnoCorsoStudio;

            if (r.GruppoTabellaInsegnamenti == null)
                r.GruppoTabellaInsegnamenti = a2.GruppoTabellaInsegnamenti;

            if (String.IsNullOrEmpty(r.Classe))
                r.Classe = a2.Classe;
            else if (r.Classe.ToLower() == a2.Classe.ToLower())
            {
                ;
            }
            else if (r.Classe.ToLower().Contains(a2.Classe.ToLower()))
            {
                ;
            }
            else if (a2.Classe.ToLower().Contains(r.Classe.ToLower()))
            {
                r.Classe = a2.Classe;
            }
            else
            {
                if (!String.IsNullOrEmpty(a2.Classe))
                {
                    bool done = false;
                    if (String.IsNullOrEmpty(r.Year))
                    {
                        if (!String.IsNullOrEmpty(a2.Year))
                        {
                            if (a2.Classe.Length > r.Classe.Length)
                            {
                                r.Classe = a2.Classe;
                                done = true;
                            }
                        }
                    }
                    else if (String.IsNullOrEmpty(a2.Year))
                    {
                        if (!String.IsNullOrEmpty(r.Year))
                        {
                            if (r.Classe.Length > a2.Classe.Length)
                            {
                                done = true;
                            }
                        }
                    }

                    if (!done)
                        r.Classe += " " + a2.Classe;
                }
            }

            if (String.IsNullOrEmpty(r.Degree))
                r.Degree = a2.Degree;
            if (String.IsNullOrEmpty(r.Id))
                r.Id = a2.Id;
            if (String.IsNullOrEmpty(r.IdLink))
                r.IdLink = a2.IdLink;
            if (String.IsNullOrEmpty(r.Language))
                r.Language = a2.Language;
            if (Gruppo.IsEmpty(r.Office))
                r.Office = a2.Office;
            if (String.IsNullOrEmpty(r.PermanentId))
                r.PermanentId = a2.PermanentId;
            if (String.IsNullOrEmpty(r.Platform))
                r.Platform = a2.Platform;
            if (String.IsNullOrEmpty(r.School))
                r.School = a2.School;
            if (String.IsNullOrEmpty(r.Tipo))
                r.Tipo = a2.Tipo;
            if (String.IsNullOrEmpty(r.Year))
                r.Year = a2.Year;
            if (r.LastUpdateInviteLinkTime == null)
                r.LastUpdateInviteLinkTime = a2.LastUpdateInviteLinkTime;
            else
            {
                if (a2.LastUpdateInviteLinkTime != null)
                {
                    if (DateTime.Compare(r.LastUpdateInviteLinkTime.Value, a2.LastUpdateInviteLinkTime.Value) < 0)
                        r.LastUpdateInviteLinkTime = a2.LastUpdateInviteLinkTime;
                }
            }

            if (!String.IsNullOrEmpty(r.Year))
            {
                if (!String.IsNullOrEmpty(r.Tipo) && !String.IsNullOrEmpty(a2.Tipo))
                {
                    if (r.Tipo.ToLower() == "s" || a2.Tipo.ToLower() == "s")
                        r.Tipo = "S";
                }
            }

            r.Aggiusta(true);

            return r;
        }
    }
}