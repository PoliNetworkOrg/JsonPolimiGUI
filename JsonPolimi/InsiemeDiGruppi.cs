﻿using System;
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
        public string nome_old;

        public InsiemeDiGruppi()
        {
            gruppo_di_base = new Gruppo();
            L = new List<Gruppo>();
        }

        internal void Aggiusta()
        {

            for (int i = 0; i < L.Count; i++)
            {
                L[i].classe = gruppo_di_base.classe;
                L[i].degree = gruppo_di_base.degree;
                L[i].language = gruppo_di_base.language;
                L[i].office = gruppo_di_base.office;
                L[i].school = gruppo_di_base.school;
                L[i].tipo = gruppo_di_base.tipo;
                L[i].year = gruppo_di_base.year;

                if (String.IsNullOrEmpty(L[i].classe))
                    L[i].classe = nome_old;
                

                L[i].Aggiusta();
            }
        }
    }
}
