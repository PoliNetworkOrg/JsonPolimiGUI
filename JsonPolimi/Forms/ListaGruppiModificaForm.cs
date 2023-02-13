using System;
using System.Windows.Forms;
using JsonPolimi_Core_nf.Data;
using JsonPolimi_Core_nf.Tipi;

namespace JsonPolimi.Forms;

public partial class ListaGruppiModificaForm : Form
{
    public ListaGruppiModificaForm()
    {
        InitializeComponent();
    }

    private void ListaGruppiModificaForm_Load(object sender, EventArgs e)
    {
        Variabili.L ??= new ListaGruppo();

        foreach (Gruppo variable in Variabili.L)
            if (string.IsNullOrEmpty(variable.Year))
                variable.AggiustaAnno();

        Filtra(null, 0, null, 0);
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

        r.G.Aggiusta(true, true);
        var x = new AggiungiForm(true, r.G);
        x.ShowDialog();

        r.G = AggiungiForm.g;

        listBox1.Items[i] = r;
        Variabili.L.SetElem(r.I, AggiungiForm.g);

        x.Dispose();
    }

    private void TextBox1_TextChanged(object sender, EventArgs e)
    {
        Filtra2();
    }

    private void Filtra(string? text, int selectedIndex, string? anno, int combobox_linkvalido)
    {
        listBox1.Items.Clear();

        text ??= "";

        anno ??= "";

        if (selectedIndex < 0)
            selectedIndex = 0;

        if (combobox_linkvalido < 0)
            combobox_linkvalido = 0;

        text = text.ToLower();
        anno = anno.ToLower();

        for (var i = 0; i < Variabili.L.GetCount(); i++)
        {
            var variable = Variabili.L.GetElem(i);

            if (!variable.Classe.ToLower().Contains(text)) continue;

            if (selectedIndex != 0 &&
                variable.Platform.ToUpper() != comboBox1.Items[selectedIndex].ToString()) continue;

            if (combobox_linkvalido > 0)
                switch (combobox_linkvalido)
                {
                    case 1 when variable.LinkFunzionante is null or false:
                    case 2 when variable.LinkFunzionante != null && variable.LinkFunzionante.Value:
                        continue;
                }

            var anno2 = variable.Year ?? "";

            if (anno2.Contains(anno))
                listBox1.Items.Add(new Riga(variable, i));
        }
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        var v1 = numericUpDown1.Value;
        var v2 = numericUpDown2.Value;
        if (v1 == -1 || v2 == -1)
        {
            MessageBox.Show("Devi selezionare dei valori validi!");
            return;
        }

        var dialogResult = MessageBox.Show("Sei sicuro di volerli unire?", "Sicuro?", MessageBoxButtons.YesNo);
        if (dialogResult != DialogResult.Yes) return;
        Variabili.L.MergeUnione(v1, v2);
        Filtra2();
    }

    private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        Filtra2();
    }

    private void TextBox2_TextChanged(object sender, EventArgs e)
    {
        Filtra2();
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        var v1 = numericUpDown1.Value;
        var v2 = numericUpDown2.Value;
        if (v1 == -1 || v2 == -1)
        {
            MessageBox.Show("Devi selezionare dei valori validi!");
            return;
        }

        var dialogResult =
            MessageBox.Show("Sei sicuro di voler far diventare il link del gruppo 1 quello del gruppo 2?", "Sicuro?",
                MessageBoxButtons.YesNo);
        if (dialogResult != DialogResult.Yes) return;
        Variabili.L.MergeLink((int)v1, (int)v2);
        Filtra2();
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
        {
            MessageBox.Show("Devi selezionare un gruppo da eliminare!");
            return;
        }

        var dialogResult = MessageBox.Show("Vuoi eliminare il gruppo? Sicuro?", "Sicuro?", MessageBoxButtons.YesNo);
        if (dialogResult != DialogResult.Yes) return;
        var g = (Riga)listBox1.Items[listBox1.SelectedIndex];
        Variabili.L.Remove(g.I);
        MessageBox.Show("Eliminato!");
        Filtra2();
    }

    private void Filtra2()
    {
        Filtra(textBox1.Text, comboBox1.SelectedIndex, textBox2.Text, comboBox2.SelectedIndex);
    }

    private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        Filtra2();
    }
}