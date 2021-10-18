using JsonPolimi_Core_nf.Enums;
using System;
using System.Collections.Generic;

namespace JsonPolimi_Core_nf.Tipi
{
    public class ImportaReturn
    {
        public Enums.ActionDoneImport actionDoneImport;
        public List<Tuple<int, Tuple<SomiglianzaClasse, Gruppo>>> simili;

        public ImportaReturn(ActionDoneImport actionDoneImport)
        {
            this.actionDoneImport = actionDoneImport;
        }

        public ImportaReturn(ActionDoneImport actionDoneImport, List<Tuple<int, Tuple<SomiglianzaClasse, Gruppo>>> simili) : this(actionDoneImport)
        {
            this.simili = simili;
        }
    }
}