using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class AggiungiForm : Form
    {
        public static Gruppo g;
        private readonly bool _edit;

        public AggiungiForm(bool edit, Gruppo g2 = null)
        {
            _edit = edit;
            g = g2;

            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (t_classe.Text.Length <= 0 || t_degree.Text.Length <= 0  ||
                t_lang.Text.Length <= 0 || t_office.Text.Length <= 0 || t_platform.Text.Length <= 0 ||
                t_school.Text.Length <= 0 || t_type.Text.Length <= 0 || t_year.Text.Length <= 0)
            {
                MessageBox.Show("Non hai compilato qualche campo!");
                return;
            }

            if (t_year.Text == "-")
                t_year.Text = "";

            if (t_degree.Text == "-")
                t_degree.Text = "";

            if (t_school.Text == "-")
                t_school.Text = "";

            if (t_office.Text == "-")
                t_office.Text = "";

            g = new Gruppo
            {
                Classe = t_classe.Text,
                Degree = t_degree.Text,
                Id = t_id.Text,
                Language = t_lang.Text,
                Office = new OfficeSede( t_office.Text ),
                Platform = t_platform.Text,
                School = t_school.Text,
                Tipo = t_type.Text,
                Year = t_year.Text,
                IdLink = t_idlink.Text
            };

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            g.Aggiusta();

            if (_edit == false) //new
                Variabili.L.Add(g);
            else
                Close();
        }

        private void Aggiungi_Form_Load(object sender, EventArgs e)
        {
            t_id.Enabled = false;

            if (!_edit || g == null) return;

            t_classe.Text = g.Classe;
            t_degree.Text = g.Degree;
            t_id.Text = g.Id;
            t_lang.Text = g.Language;
            t_office.Text =  StringCheckNull( g.Office);
            t_platform.Text = g.Platform;
            t_school.Text = g.School;
            t_type.Text = g.Tipo;
            t_year.Text = g.Year;
            t_idlink.Text = g.IdLink;

            button1.Text = "Modifica";
        }

        private string StringCheckNull(OfficeSede office)
        {
            if (office == null)
                return null;

            return office.StringNotNull();
        }

    }
}