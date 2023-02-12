using System;
using System.Windows.Forms;
using JsonPolimi_Core_nf.Tipi;
using JsonPolimi.Forms;

namespace JsonPolimi;

public static class Program
{
    /// <summary>
    ///     Punto di ingresso principale dell'applicazione.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm(new ParametriCondivisi()));
    }
}