using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPolimi
{
    public class InsiemeDiGruppi
    {
        public Gruppo gruppo_di_base;
        public List<Gruppo> L;
        public Gruppo nome_old;

        public InsiemeDiGruppi()
        {
            gruppo_di_base = new Gruppo();
            L = new List<Gruppo>();
            nome_old = new Gruppo();
        }

        internal void Aggiusta()
        {
            for (var i = 0; i < L.Count; i++)
            {
                L[i].classe = gruppo_di_base.classe;
                L[i].degree = gruppo_di_base.degree;
                L[i].language = gruppo_di_base.language;
                L[i].office = gruppo_di_base.office;
                L[i].school = gruppo_di_base.school;
                L[i].tipo = gruppo_di_base.tipo;
                L[i].year = gruppo_di_base.year;

                if (string.IsNullOrEmpty(L[i].classe))
                    L[i].classe = nome_old.classe;

                if (string.IsNullOrEmpty(L[i].language))
                    L[i].language = nome_old.language;

                if (string.IsNullOrEmpty(L[i].degree))
                    L[i].degree = nome_old.degree;

                if (string.IsNullOrEmpty(L[i].school))
                    L[i].school = nome_old.school;

                if (string.IsNullOrEmpty(L[i].office))
                    L[i].office = nome_old.office;

                if (string.IsNullOrEmpty(L[i].year))
                    L[i].year = nome_old.year;

                L[i].Aggiusta();
            }
        }
    }
}