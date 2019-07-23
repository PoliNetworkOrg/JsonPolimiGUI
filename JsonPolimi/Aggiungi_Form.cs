using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class AggiungiForm : Form
    {
        private readonly bool edit;
        public static Gruppo g;

        public AggiungiForm(bool edit, Gruppo g2 = null)
        {
            this.edit = edit;
            g = g2;

            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (t_classe.Text.Length <= 0 || t_degree.Text.Length <= 0 || t_id.Text.Length <= 0 ||
                t_lang.Text.Length <= 0 || t_office.Text.Length <= 0 || t_platform.Text.Length <= 0 ||
                t_school.Text.Length <= 0 || t_type.Text.Length <= 0 || t_year.Text.Length <= 0)
            {
                MessageBox.Show("Non hai compilato qualche campo!");
                return;
            }

            g = new Gruppo
            {
                Classe = t_classe.Text,
                Degree = t_degree.Text,
                Id = t_id.Text,
                Language = t_lang.Text,
                Office = t_office.Text,
                Platform = t_platform.Text,
                School = t_school.Text,
                Tipo = t_type.Text,
                Year = t_year.Text,
                IdLink = t_idlink.Text
            };

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            g.Aggiusta();

            if (edit == false) //new
            {
                Variabili.L.Add(g);
            }
            else
            {
                this.Close();
            }
        }

        private void Aggiungi_Form_Load(object sender, EventArgs e)
        {
            if (!edit || g == null) return;

            t_classe.Text = g.Classe;
            t_degree.Text = g.Degree;
            t_id.Text = g.Id;
            t_lang.Text = g.Language;
            t_office.Text = g.Office;
            t_platform.Text = g.Platform;
            t_school.Text = g.School;
            t_type.Text = g.Tipo;
            t_year.Text = g.Year;
            t_idlink.Text = g.IdLink;

            t_id.Enabled = false;
        }
    }
}