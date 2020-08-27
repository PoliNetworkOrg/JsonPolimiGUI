﻿using System;
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

        internal void ProvaAdUnire()
        {
            for (int i = 0; i < this._l.Count; i++)
            {
                this._l[i].Aggiusta();
                for (int j = this._l.Count - 1; j >= 0; j--)
                {
                    if (i != j)
                    {
                        Tuple<bool, Gruppo> r = Equivalenti(i, j);
                        if (r.Item1)
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

        private Tuple<bool, Gruppo> Equivalenti(int i, int j)
        {
            Gruppo a1 = this._l[i];
            Gruppo a2 = this._l[j];

            if (a1.PermanentId == a2.PermanentId && !String.IsNullOrEmpty(a1.PermanentId))
                return Unisci(i, j);
            else
            {
                if (!String.IsNullOrEmpty(a1.PermanentId))
                {
                    if (!String.IsNullOrEmpty(a2.PermanentId))
                    {
                        if (a1.PermanentId != a2.PermanentId)
                            return new Tuple<bool, Gruppo>(false, null);
                    }
                }
            }

            if (a1.Id == a2.Id && !String.IsNullOrEmpty(a1.Id))
            {
                if (a1.Classe.ToLower().Contains("cartografia") && !a2.Classe.ToLower().Contains("cartografia"))
                {
                    return new Tuple<bool, Gruppo>(false, null);
                }
                if (a2.Classe.ToLower().Contains("cartografia") && !a1.Classe.ToLower().Contains("cartografia"))
                {
                    return new Tuple<bool, Gruppo>(false, null);
                }

                return Unisci(i, j);
            }

            if (!String.IsNullOrEmpty(a1.Year))
            {
                if (!String.IsNullOrEmpty(a2.Year))
                {
                    if (a1.Year != a2.Year)
                        return new Tuple<bool, Gruppo>(false, null);
                }
            }

            if (!String.IsNullOrEmpty(a1.Platform))
            {
                if (!String.IsNullOrEmpty(a2.Platform))
                {
                    if (a1.Platform != a2.Platform)
                        return new Tuple<bool, Gruppo>(false, null);
                }
            }

            if (!String.IsNullOrEmpty(a1.Office))
            {
                if (!String.IsNullOrEmpty(a2.Office))
                {
                    if (a1.Office != a2.Office)
                        return new Tuple<bool, Gruppo>(false, null);
                }
            }

            if (!String.IsNullOrEmpty(a1.Degree))
            {
                if (!String.IsNullOrEmpty(a2.Degree))
                {
                    if (a1.Degree != a2.Degree)
                        return new Tuple<bool, Gruppo>(false, null);
                }
            }

            if (String.IsNullOrEmpty(a1.Classe))
                return new Tuple<bool, Gruppo>(false, null);

            if (String.IsNullOrEmpty(a2.Classe))
                return new Tuple<bool, Gruppo>(false, null);

            string[] s1 = a1.Classe.ToLower().Split(' ');
            string[] s2 = a2.Classe.ToLower().Split(' ');
            List<string> sa1 = new List<string>();
            List<string> sa2 = new List<string>();
            sa1.AddRange(s1);
            sa2.AddRange(s2);
            if (sa1.Contains("magistrale"))
            {
                if (String.IsNullOrEmpty(a2.Degree))
                    return new Tuple<bool, Gruppo>(false, null);
                if (a2.Degree.ToLower() != "lm")
                    return new Tuple<bool, Gruppo>(false, null);
            }
            else if (sa1.Contains("triennale"))
            {
                if (String.IsNullOrEmpty(a2.Degree))
                    return new Tuple<bool, Gruppo>(false, null);
                if (a2.Degree.ToLower() != "lt")
                    return new Tuple<bool, Gruppo>(false, null);
            }
            else if (sa2.Contains("magistrale"))
            {
                if (String.IsNullOrEmpty(a1.Degree))
                    return new Tuple<bool, Gruppo>(false, null);
                if (a1.Degree.ToLower() != "lm")
                    return new Tuple<bool, Gruppo>(false, null);
            }
            else if (sa2.Contains("triennale"))
            {
                if (String.IsNullOrEmpty(a1.Degree))
                    return new Tuple<bool, Gruppo>(false, null);
                if (a1.Degree.ToLower() != "lt")
                    return new Tuple<bool, Gruppo>(false, null);
            }

            if (NomiSimili(a1.Classe, a2.Classe))
                return Unisci(i, j);

            return new Tuple<bool, Gruppo>(false, null);
        }

        private bool NomiSimili(string n1, string n2)
        {
            if (n1.Length == 0 || n2.Length == 0)
                return false;

            if (n1 == n2)
                return true;

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
                        return false;
                }

                List<Tuple<string, string>> no_merge2 = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("chimica", "elettrica")
                };

                foreach (var no_merge_s2 in no_merge2)
                {
                    if (l1.Contains(no_merge_s2.Item1) && l1.Contains(no_merge_s2.Item2))
                        return false;
                    if (l2.Contains(no_merge_s2.Item1) && l2.Contains(no_merge_s2.Item2))
                        return false;
                }

                //remove useless words
                List<string> useless = new List<string>() {
                    "polimi", "politecnico", "and",
                    "engineering", "in",
                    "generale", "gruppo", "for", "the",
                    "e", "delle", "dei",
                    "ingegneria", "della", "-", "" };
                TryRemove(ref l1, ref l2, useless);

                bool? r2 = ManualCheck(n1, n2);
                if (r2 != null)
                {
                    return r2.Value;
                }

                //count how many words are in common
                List<string> quanti = new List<string>();
                for (int i = 0; i < l1.Count; i++)
                {
                    if (l2.Contains(l1[i]))
                        quanti.Add(l1[i]);
                }

                int minimo = 0;
                if (l1.Count + l2.Count > 5)
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
                        return false;
                    if (quanti[0] == "architettura")
                        return false;
                }

                if (quanti.Count >= minimo)
                    return true;
            }
            catch
            {
                return false;
            }

            return false;
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

        private Tuple<bool, Gruppo> Unisci(int i, int j)
        {
            Gruppo g = Unisci2(i, j);
            if (g == null)
                return new Tuple<bool, Gruppo>(false, null);

            return new Tuple<bool, Gruppo>(true, g);
        }

        private Gruppo Unisci2(int i, int j)
        {
            Gruppo a1 = this._l[i];
            Gruppo a2 = this._l[j];

            if (!string.IsNullOrEmpty(a1.Classe) && !string.IsNullOrEmpty(a2.Classe))
            {
                if (a1.Classe.ToLower().Contains("cazzeggio") && !a2.Classe.ToLower().Contains("cazzeggio"))
                    return null;

                if (a2.Classe.ToLower().Contains("cazzeggio") && !a1.Classe.ToLower().Contains("cazzeggio"))
                    return null;

                if (a1.Classe.ToLower().Contains("theory") && a2.Classe.ToLower().Contains("theory"))
                {
                    if (a1.Classe.ToLower() == "information theory" && a2.Classe.ToLower() == "advanced circuit theory")
                    {
                        return null;
                    }


                    if (a2.Classe.ToLower() == "information theory" && a1.Classe.ToLower() == "advanced circuit theory")
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("mobility") && a2.Classe.ToLower().Contains("mobility")  )
                {
                    if (a1.Classe.ToLower().Contains("safety") && !a2.Classe.ToLower().Contains("safety"))
                    {
                        return null;
                    }
                    if (a2.Classe.ToLower().Contains("safety") && !a1.Classe.ToLower().Contains("safety"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("data") && !a1.Classe.ToLower().Contains("data"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("data") && !a2.Classe.ToLower().Contains("data"))
                    {
                        return null;
                    }
                }
                   
                if (a1.Classe.ToLower().Contains("meccanica") && a2.Classe.ToLower().Contains("meccanica"))
                {
                    if (a2.Classe.ToLower().Contains("tecnologia") && !a1.Classe.ToLower().Contains("tecnologia"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("tecnologia") && !a2.Classe.ToLower().Contains("tecnologia"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("scaglione") && !a2.Classe.ToLower().Contains("scaglione"))
                    return null;

                if (a2.Classe.ToLower().Contains("scaglione") && !a1.Classe.ToLower().Contains("scaglione"))
                    return null;

                if (a1.Classe.ToLower().Contains("design") && a2.Classe.ToLower().Contains("design"))
                {
                    if (a2.Classe.ToLower().Contains("accessory") && !a1.Classe.ToLower().Contains("accessory"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("accessory") && !a2.Classe.ToLower().Contains("accessory"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("structural") && !a1.Classe.ToLower().Contains("structural"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("structural") && !a2.Classe.ToLower().Contains("structural"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("ambientale") && a2.Classe.ToLower().Contains("ambientale"))
                {
                    if (a2.Classe.ToLower().Contains("acustica") && !a1.Classe.ToLower().Contains("acustica"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("acustica") && !a2.Classe.ToLower().Contains("acustica"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("polimi") && !a1.Classe.ToLower().Contains("polimi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("polimi") && !a2.Classe.ToLower().Contains("polimi"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("acustica") && a2.Classe.ToLower().Contains("acustica"))
                {
                    if (a2.Classe.ToLower().Contains("applicata") && !a1.Classe.ToLower().Contains("applicata"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("applicata") && !a2.Classe.ToLower().Contains("applicata"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("illuminotecnica") && !a1.Classe.ToLower().Contains("illuminotecnica"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("illuminotecnica") && !a2.Classe.ToLower().Contains("illuminotecnica"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("edifici") && a2.Classe.ToLower().Contains("edifici"))
                {
                    if (a2.Classe.ToLower().Contains("caratteri") && !a1.Classe.ToLower().Contains("caratteri"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("caratteri") && !a2.Classe.ToLower().Contains("caratteri"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("tirocinio") && a2.Classe.ToLower().Contains("tirocinio"))
                {
                    if (a2.Classe.ToLower().Contains("avviamento") && !a1.Classe.ToLower().Contains("avviamento"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("avviamento") && !a2.Classe.ToLower().Contains("avviamento"))
                    {
                        return null;
                    }
                }


                if (a1.Classe.ToLower().Contains("game") && a2.Classe.ToLower().Contains("game"))
                {
                    if (a2.Classe.ToLower().Contains("business") && !a1.Classe.ToLower().Contains("business"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("business") && !a2.Classe.ToLower().Contains("business"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("business") && a2.Classe.ToLower().Contains("business"))
                {
                    if (a2.Classe.ToLower().Contains("game") && !a1.Classe.ToLower().Contains("game"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("game") && !a2.Classe.ToLower().Contains("game"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("systems") && a2.Classe.ToLower().Contains("systems"))
                {
                    if (a2.Classe.ToLower().Contains("business") && !a1.Classe.ToLower().Contains("business"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("business") && !a2.Classe.ToLower().Contains("business"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("strutture") && a2.Classe.ToLower().Contains("strutture"))
                {
                    if (a2.Classe.ToLower().Contains("prova") && !a1.Classe.ToLower().Contains("prova"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("prova") && !a2.Classe.ToLower().Contains("prova"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("methods") && a2.Classe.ToLower().Contains("methods"))
                {
                    if (a2.Classe.ToLower().Contains("random") && !a1.Classe.ToLower().Contains("random"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("random") && !a2.Classe.ToLower().Contains("random"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("aziendale") && a2.Classe.ToLower().Contains("aziendale"))
                {
                    if (a2.Classe.ToLower().Contains("economia") && !a1.Classe.ToLower().Contains("economia"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("economia") && !a2.Classe.ToLower().Contains("economia"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("fondamenti") && a2.Classe.ToLower().Contains("fondamenti"))
                {
                    if (a2.Classe.ToLower().Contains("automatica") && !a1.Classe.ToLower().Contains("automatica"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("automatica") && !a2.Classe.ToLower().Contains("automatica"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("internet") && !a1.Classe.ToLower().Contains("internet"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("internet") && !a2.Classe.ToLower().Contains("internet"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("segnali") && !a1.Classe.ToLower().Contains("segnali"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("segnali") && !a2.Classe.ToLower().Contains("segnali"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("laboratorio") && a2.Classe.ToLower().Contains("laboratorio"))
                {
                    if (a2.Classe.ToLower().Contains("finale") && !a1.Classe.ToLower().Contains("finale"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("finale") && !a2.Classe.ToLower().Contains("finale"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("fisica") && !a1.Classe.ToLower().Contains("fisica"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("fisica") && !a2.Classe.ToLower().Contains("fisica"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("communication") && a2.Classe.ToLower().Contains("communication"))
                {
                    if (a2.Classe.ToLower().Contains("and") && !a1.Classe.ToLower().Contains("and"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("and") && !a2.Classe.ToLower().Contains("and"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("branding") && !a1.Classe.ToLower().Contains("branding"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("branding") && !a2.Classe.ToLower().Contains("branding"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("advanced") && a2.Classe.ToLower().Contains("advanced"))
                {
                    if (a2.Classe.ToLower().Contains("multivariable") && !a1.Classe.ToLower().Contains("multivariable"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("multivariable") && !a2.Classe.ToLower().Contains("multivariable"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("topic") && !a1.Classe.ToLower().Contains("topic"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("topic") && !a2.Classe.ToLower().Contains("topic"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("random") && !a1.Classe.ToLower().Contains("random"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("random") && !a2.Classe.ToLower().Contains("random"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("materials") && !a1.Classe.ToLower().Contains("materials"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("materials") && !a2.Classe.ToLower().Contains("materials"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("circuit") && !a1.Classe.ToLower().Contains("circuit"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("circuit") && !a2.Classe.ToLower().Contains("circuit"))
                    {
                        return null;
                    }


                    if (a2.Classe.ToLower().Contains("computer") && !a1.Classe.ToLower().Contains("computer"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("computer") && !a2.Classe.ToLower().Contains("computer"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("manufacturing") && a2.Classe.ToLower().Contains("manufacturing"))
                {
                    if (a2.Classe.ToLower().Contains("advanced") && !a1.Classe.ToLower().Contains("advanced"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("advanced") && !a2.Classe.ToLower().Contains("advanced"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().Contains("space") && !a1.Classe.ToLower().Contains("space"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("space") && !a2.Classe.ToLower().Contains("space"))
                    {
                        return null;
                    }

                    if (a2.Classe.ToLower().EndsWith(" b") && !a1.Classe.ToLower().EndsWith(" b"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().EndsWith(" b") && !a2.Classe.ToLower().EndsWith(" b"))
                    {
                        return null;
                    }


                }

                if (a1.Classe.ToLower().Contains("food") && a2.Classe.ToLower().Contains("food"))
                {
                    if (a2.Classe.ToLower().Contains("materials") && !a1.Classe.ToLower().Contains("materials"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("materials") && !a2.Classe.ToLower().Contains("materials"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("materials") && a2.Classe.ToLower().Contains("materials"))
                {
                    if (a2.Classe.ToLower().Contains("nuclear") && !a1.Classe.ToLower().Contains("nuclear"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("nuclear") && !a2.Classe.ToLower().Contains("nuclear"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("fisica") && a2.Classe.ToLower().Contains("fisica"))
                {
                    if (a2.Classe.ToLower().Contains("sperimentale") && !a1.Classe.ToLower().Contains("sperimentale"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("sperimentale") && !a2.Classe.ToLower().Contains("sperimentale"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("per") && a2.Classe.ToLower().Contains("per"))
                {
                    if (a2.Classe.ToLower().Contains("gruppo") && !a1.Classe.ToLower().Contains("gruppo"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("gruppo") && !a2.Classe.ToLower().Contains("gruppo"))
                    {
                        return null;
                    }
                }


                if (a1.Classe.ToLower().Contains("metodi") && a2.Classe.ToLower().Contains("metodi"))
                {
                    if (a2.Classe.ToLower().Contains("dispositivi") && !a1.Classe.ToLower().Contains("dispositivi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("dispositivi") && !a2.Classe.ToLower().Contains("dispositivi"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("sicurezza") && a2.Classe.ToLower().Contains("sicurezza"))
                {
                    if (a2.Classe.ToLower().Contains("processo") && !a1.Classe.ToLower().Contains("processo"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("processo") && !a2.Classe.ToLower().Contains("processo"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("rischio") && a2.Classe.ToLower().Contains("rischio"))
                {
                    if (a2.Classe.ToLower().Contains("mitigazione") && !a1.Classe.ToLower().Contains("mitigazione"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("mitigazione") && !a2.Classe.ToLower().Contains("mitigazione"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("final") && a2.Classe.ToLower().Contains("final"))
                {
                    if (a2.Classe.ToLower().Contains("work") && !a1.Classe.ToLower().Contains("work"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("work") && !a2.Classe.ToLower().Contains("work"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("reti") && a2.Classe.ToLower().Contains("reti"))
                {
                    if (a2.Classe.ToLower().Contains("logiche") && !a1.Classe.ToLower().Contains("logiche"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("logiche") && !a2.Classe.ToLower().Contains("logiche"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("safety") && a2.Classe.ToLower().Contains("safety"))
                {
                    if (a2.Classe.ToLower().Contains("mobility") && !a1.Classe.ToLower().Contains("mobility"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("mobility") && !a2.Classe.ToLower().Contains("mobility"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("sistemi") && a2.Classe.ToLower().Contains("sistemi"))
                {
                    if (a2.Classe.ToLower().Contains("edilizi") && !a1.Classe.ToLower().Contains("edilizi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("edilizi") && !a2.Classe.ToLower().Contains("edilizi"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("comunicazione") && a2.Classe.ToLower().Contains("comunicazione"))
                {
                    if (a2.Classe.ToLower().Contains("design") && !a1.Classe.ToLower().Contains("design"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("design") && !a2.Classe.ToLower().Contains("design"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains("magistrale") && a2.Classe.ToLower().Contains("magistrale"))
                {
                    if (a2.Classe.ToLower().Contains("tesi") && !a1.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("tesi") && !a2.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                }


                if (a1.Classe.ToLower().Contains("tesi") && a2.Classe.ToLower().Contains("tesi"))
                {
                    if (a2.Classe.ToLower().Contains("tesi") && !a1.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("tesi") && !a2.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                }

                if (a1.Classe.ToLower().Contains(" e ") && a2.Classe.ToLower().Contains(" e "))
                {
                    if (a2.Classe.ToLower().Contains("tesi") && !a1.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                    if (a1.Classe.ToLower().Contains("tesi") && !a2.Classe.ToLower().Contains("tesi"))
                    {
                        return null;
                    }
                }

                if (a2.Classe.ToLower().EndsWith(" a") && !a1.Classe.ToLower().EndsWith(" a"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" a") && !a2.Classe.ToLower().EndsWith(" a"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().EndsWith(" b") && !a1.Classe.ToLower().EndsWith(" b"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" b") && !a2.Classe.ToLower().EndsWith(" b"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().EndsWith(" c") && !a1.Classe.ToLower().EndsWith(" c"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" c") && !a2.Classe.ToLower().EndsWith(" c"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().EndsWith(" 1") && !a1.Classe.ToLower().EndsWith(" 1"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" 1") && !a2.Classe.ToLower().EndsWith(" 1"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().EndsWith(" 2") && !a1.Classe.ToLower().EndsWith(" 2"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" 2") && !a2.Classe.ToLower().EndsWith(" 2"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().EndsWith(" 3") && !a1.Classe.ToLower().EndsWith(" 3"))
                {
                    return null;
                }
                if (a1.Classe.ToLower().EndsWith(" 3") && !a2.Classe.ToLower().EndsWith(" 3"))
                {
                    return null;
                }

                if (a2.Classe.ToLower().StartsWith("al ") && !a1.Classe.ToLower().StartsWith("al "))
                {
                    return null;
                }
                if (a1.Classe.ToLower().StartsWith("al ") && !a2.Classe.ToLower().StartsWith("al "))
                {
                    return null;
                }

                if (a1.Classe.ToLower().Contains("~") && !a2.Classe.ToLower().Contains("~"))
                    return null;

                if (a2.Classe.ToLower().Contains("~") && !a1.Classe.ToLower().Contains("~"))
                    return null;
            }

            if (!string.IsNullOrEmpty(a1.Classe) && !string.IsNullOrEmpty(a2.Classe) && a1.Classe.ToLower() == a2.Classe.ToLower())
            {
                //good
            }
            else
            {
                ;
            }

            ;

            if (String.IsNullOrEmpty(a1.Classe))
                a1.Classe = a2.Classe;
            else
            {
                if (!String.IsNullOrEmpty(a2.Classe))
                {
                    bool done = false;
                    if (String.IsNullOrEmpty(a1.Year))
                    {
                        if (!String.IsNullOrEmpty(a2.Year))
                        {
                            if (a2.Classe.Length > a1.Classe.Length)
                            {
                                a1.Classe = a2.Classe;
                                done = true;
                            }
                        }
                    }
                    else if (String.IsNullOrEmpty(a2.Year))
                    {
                        if (!String.IsNullOrEmpty(a1.Year))
                        {
                            if (a1.Classe.Length > a2.Classe.Length)
                            {
                                done = true;
                            }
                        }
                    }

                    if (!done)
                        a1.Classe += " " + a2.Classe;
                }
            }

            if (String.IsNullOrEmpty(a1.Degree))
                a1.Degree = a2.Degree;
            if (String.IsNullOrEmpty(a1.Id))
                a1.Id = a2.Id;
            if (String.IsNullOrEmpty(a1.IdLink))
                a1.IdLink = a2.IdLink;
            if (String.IsNullOrEmpty(a1.Language))
                a1.Language = a2.Language;
            if (String.IsNullOrEmpty(a1.Office))
                a1.Office = a2.Office;
            if (String.IsNullOrEmpty(a1.PermanentId))
                a1.PermanentId = a2.PermanentId;
            if (String.IsNullOrEmpty(a1.Platform))
                a1.Platform = a2.Platform;
            if (String.IsNullOrEmpty(a1.School))
                a1.School = a2.School;
            if (String.IsNullOrEmpty(a1.Tipo))
                a1.Tipo = a2.Tipo;
            if (String.IsNullOrEmpty(a1.Year))
                a1.Year = a2.Year;
            if (a1.LastUpdateInviteLinkTime == null)
                a1.LastUpdateInviteLinkTime = a2.LastUpdateInviteLinkTime;
            else
            {
                if (a2.LastUpdateInviteLinkTime != null)
                {
                    if (DateTime.Compare(a1.LastUpdateInviteLinkTime.Value, a2.LastUpdateInviteLinkTime.Value) < 0)
                        a1.LastUpdateInviteLinkTime = a2.LastUpdateInviteLinkTime;
                }
            }

            if (!String.IsNullOrEmpty(a1.Year))
            {
                if (!String.IsNullOrEmpty(a1.Tipo) && !String.IsNullOrEmpty(a2.Tipo))
                {
                    if (a1.Tipo.ToLower() == "s" || a2.Tipo.ToLower() == "s")
                        a1.Tipo = "S";
                }
            }

            a1.Aggiusta();

            return a1;
        }
    }
}