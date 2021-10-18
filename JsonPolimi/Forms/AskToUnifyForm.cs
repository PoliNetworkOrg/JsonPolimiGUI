 
using JsonPolimi_Core_nf.Tipi;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace JsonPolimi.Forms
{
    public partial class AskToUnifyForm : Form
    {
        private readonly Tuple<SomiglianzaClasse, Gruppo> d;
        public bool? r = null;
        int count;

        public AskToUnifyForm()
        {
            InitializeComponent();
        }

        public AskToUnifyForm(Tuple<SomiglianzaClasse, Gruppo> d, int count)
        {
            this.d = d;
            this.count = count;
            InitializeComponent();
        }

        private void AskToUnifyForm_Load(object sender, EventArgs e)
        {
            textBox2.Text = d.Item1.a1.Classe;
            textBox3.Text = d.Item1.a2.Classe;

            textBox1.Text = d.Item1.a1.To_json(CheckGruppo.E.TUTTO);
            textBox4.Text = d.Item1.a2.To_json(CheckGruppo.E.TUTTO);

            linkLabel1.Text = "https://t.me/joinchat/" + d.Item1.a1.IdLink;
            linkLabel2.Text = "https://t.me/joinchat/" + d.Item1.a2.IdLink;

            this.Text += " n=" + count.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            r = true;
            Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            r = false;
            Close();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel1.Text);
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel2.Text);
        }
    }
}