using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class Aggiungi_Form : Form
    {
        public Aggiungi_Form()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (t_classe.Text.Length > 0 && t_degree.Text.Length > 0 && t_id.Text.Length > 0 &&
                t_lang.Text.Length > 0 && t_office.Text.Length > 0 && t_platform.Text.Length > 0 &&
                t_school.Text.Length > 0 && t_type.Text.Length > 0 && t_year.Text.Length > 0)
            {
                ;
            }
            else
            {
                MessageBox.Show("Non hai compilato qualche campo!");
                return;
            }

            var G = new Gruppo
            {
                classe = t_classe.Text,
                degree = t_degree.Text,
                id = t_id.Text,
                language = t_lang.Text,
                office = t_office.Text,
                platform = t_platform.Text,
                school = t_school.Text,
                tipo = t_type.Text,
                year = t_year.Text
            };

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            G.Aggiusta();

            Variabili.L.Add(G);
        }

        private void Aggiungi_Form_Load(object sender, EventArgs e)
        {
        }
    }
}