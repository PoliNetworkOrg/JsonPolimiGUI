using System;
using System.Collections.Generic;

namespace JsonPolimi
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

        internal string StringNotNull()
        {
            if (o == null)
                    return null;

            return this.ToString();
        }

        internal bool Contains_In_Uno(string v)
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
    }
}