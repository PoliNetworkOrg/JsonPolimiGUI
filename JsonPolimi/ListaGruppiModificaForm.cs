using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class ListaGruppiModificaForm : Form
    {
        public ListaGruppiModificaForm()
        {
            InitializeComponent();
        }

        private void ListaGruppiModificaForm_Load(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Filtra(null);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var i = listBox1.SelectedIndex;
            if (i < 0 || i >= listBox1.Items.Count)
            {
                MessageBox.Show("Devi selezionare un gruppo!");
                return;
            }

            var r = (Riga)listBox1.Items[i];

            r.G.Aggiusta();
            var x = new AggiungiForm(true, r.G);
            x.ShowDialog();

            r.G = AggiungiForm.g;

            listBox1.Items[i] = r;
            Variabili.L.SetElem(r.I, AggiungiForm.g);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Filtra(textBox1.Text);
        }

        private void Filtra(string text)
        {
            listBox1.Items.Clear();

            if (text == null)
                text = "";

            text = text.ToLower();

            for (var i = 0; i < Variabili.L.GetCount(); i++)
            {
                var variable = Variabili.L.GetElem(i);

                if (variable.Classe.ToLower().Contains(text)) listBox1.Items.Add(new Riga(variable, i));
            }
        }
    }
}