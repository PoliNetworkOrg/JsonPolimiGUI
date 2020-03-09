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
    public partial class AskToUnifyForm : Form
    {
        private Tuple<SomiglianzaClasse, Gruppo> d;
        public bool? r = null;

        public AskToUnifyForm()
        {
            InitializeComponent();
        }

        public AskToUnifyForm(Tuple<SomiglianzaClasse, Gruppo> d)
        {
            this.d = d;
        }

        private void AskToUnifyForm_Load(object sender, EventArgs e)
        {
            textBox2.Text = d.Item1.a1.Classe;
            textBox3.Text = d.Item1.a2.Classe;

            textBox1.Text = d.Item1.a1.To_json();
            textBox4.Text = d.Item1.a2.To_json();

            linkLabel1.Text = "https://t.me/joinchat/" + d.Item1.a1.IdLink;
            linkLabel2.Text = "https://t.me/joinchat/" + d.Item1.a2.IdLink;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            r = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            r = false;
            Close();
        }
    }
}
