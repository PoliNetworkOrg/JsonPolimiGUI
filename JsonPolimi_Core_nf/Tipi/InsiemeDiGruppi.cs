using System.Collections.Generic;

// ReSharper disable ForCanBeConvertedToForeach

namespace JsonPolimi_Core_nf.Tipi
{
    public class InsiemeDiGruppi
    {
        public Gruppo GruppoDiBase;
        public List<Gruppo> L;
        public Gruppo NomeOld;

        public InsiemeDiGruppi()
        {
            GruppoDiBase = new Gruppo();
            L = new List<Gruppo>();
            NomeOld = new Gruppo();
        }

        public void Aggiusta()
        {
            for (var i = 0; i < L.Count; i++)
            {
                L[i].Classe = GruppoDiBase.Classe;
                L[i].Degree = GruppoDiBase.Degree;
                L[i].Language = GruppoDiBase.Language;
                L[i].Office = GruppoDiBase.Office;
                L[i].School = GruppoDiBase.School;
                L[i].Tipo = GruppoDiBase.Tipo;
                L[i].Year = GruppoDiBase.Year;

                if (string.IsNullOrEmpty(L[i].Classe))
                    L[i].Classe = NomeOld.Classe;

                if (string.IsNullOrEmpty(L[i].Language))
                    L[i].Language = NomeOld.Language;

                if (string.IsNullOrEmpty(L[i].Degree))
                    L[i].Degree = NomeOld.Degree;

                if (string.IsNullOrEmpty(L[i].School))
                    L[i].School = NomeOld.School;

                if (Gruppo.IsEmpty(L[i].Office))
                    L[i].Office = NomeOld.Office;

                if (string.IsNullOrEmpty(L[i].Year))
                    L[i].Year = NomeOld.Year;

                L[i].Aggiusta(true, true);
            }
        }
    }
}