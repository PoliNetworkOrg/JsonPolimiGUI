using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class AggiungiForm : Form
    {
        public AggiungiForm()
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
                Classe = t_classe.Text,
                Degree = t_degree.Text,
                Id = t_id.Text,
                Language = t_lang.Text,
                Office = t_office.Text,
                Platform = t_platform.Text,
                School = t_school.Text,
                Tipo = t_type.Text,
                Year = t_year.Text
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