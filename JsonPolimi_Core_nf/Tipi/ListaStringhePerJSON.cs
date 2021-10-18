using System;
using System.Collections.Generic;

namespace JsonPolimi_Core_nf.Tipi
{
    [Serializable]
    public class ListaStringhePerJSON
    {
        public List<string> o;

        public ListaStringhePerJSON(List<string> office)
        {
            this.o = office;
        }

        public ListaStringhePerJSON(string v)
        {
            this.o = new List<string>() { v };
        }

        public override string ToString()
        {
            if (o == null)
                return null;

            string r = "";
            foreach (string v in o)
            {
                r += v;
                r += ", ";
            }
            r = r.Substring(0, r.Length - 2);
            return r;
        }

        public bool IsEmpty()
        {
            if (o == null)
                return true;

            if (o.Count == 0)
                return true;

            foreach (var x in o)
            {
                if (string.IsNullOrEmpty(x))
                    return true;
            }

            return false;
        }

        public string StringNotNull()
        {
            if (o == null)
                return null;

            return this.ToString();
        }

        public bool Contains_In_Uno(string v)
        {
            if (o == null)
                return false;

            foreach (var s2 in o)
            {
                if (s2.Contains(v))
                    return true;
            }

            return false;
        }

        public string GetCCSCode()
        {
            if (this.o == null)
                return null;

            if (o.Count < 2)
            {
                var s2 = o[0].Split(' ');

                foreach (var x1 in s2)
                {
                    if (x1.StartsWith("(") && x1.EndsWith("),"))
                    {
                        string s3 = x1.Substring(0, x1.Length - 1);
                        return GetCCSCode2(s3);
                    }
                    else if (x1.StartsWith("(") && x1.EndsWith(")"))
                        return GetCCSCode2(x1);
                }

                return GetCCSCode2(s2[s2.Length - 1]);
            }

            foreach (var x1 in o)
            {
                if (x1.StartsWith("(") && x1.EndsWith("),"))
                {
                    string s3 = x1.Substring(0, x1.Length - 1);
                    return GetCCSCode2(s3);
                }
                else if (x1.StartsWith("(") && x1.EndsWith(")"))
                    return GetCCSCode2(x1);
            }

            return null;
        }

        private string GetCCSCode2(string v)
        {
            string s = v.Trim();
            if (s.StartsWith("(") && s.EndsWith(")"))
            {
                s = s.Substring(1);
                s = s.Substring(0, s.Length - 1);

                return s;
            }

            return null;
        }

        /*
            return (o1 == o2)  =>  0
            return (o1 >  o2)  => +1
            return (o1 <  o2)  => -1
        */

        public static int Confronta(ListaStringhePerJSON o1, ListaStringhePerJSON o2)
        {
            if (o1 == null && o2 == null)
                return 0;

            if (o1 == null)
                return -1;

            if (o2 == null)
                return 1;

            bool contained = true;
            foreach (var i1 in o1.o)
            {
                bool contains = o2.o.Contains(i1);
                if (!contains)
                {
                    contained = false;
                }
            }

            if (contained)
            {
                if (o1.o.Count == o2.o.Count)
                    return 0;

                return -1;
            }

            foreach (var i2 in o2.o)
            {
                bool contains = o1.o.Contains(i2);
                if (!contains)
                {
                    contained = false;
                }
            }

            if (contained)
            {
                if (o1.o.Count == o2.o.Count)
                    return 0;

                return 1;
            }

            ;

            return 1;
        }

        public static bool IsEmpty(ListaStringhePerJSON o)
        {
            if (o == null)
                return true;

            return o.IsEmpty();
        }
    }
}