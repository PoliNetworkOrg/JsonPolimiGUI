using JsonPolimi.Forms;
 
using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public static class Program
    {
        /// <summary>
        ///     Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new JsonPolimi_Core_nf.Tipi.ParametriCondivisi()));
        }
    }
}