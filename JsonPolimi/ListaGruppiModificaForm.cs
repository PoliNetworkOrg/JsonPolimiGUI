using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            foreach (var variable in Variabili.L)
            {
                listBox1.Items.Add(variable);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i < 0 || i >= listBox1.Items.Count)
            {
                MessageBox.Show("Devi selezionare un gruppo!");
                return;
            }

            var g = (Gruppo)listBox1.Items[i];
            g.Aggiusta();
            var x = new AggiungiForm(true, g);
            x.ShowDialog();
            listBox1.Items[i] = AggiungiForm.g;
            Variabili.L.SetElem(i, AggiungiForm.g);
        }
    }
}