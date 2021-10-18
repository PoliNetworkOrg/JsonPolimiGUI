﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JsonPolimi.Forms
{
    public partial class AskFromList : Form
    {
        public int? r = null;

        public AskFromList(List<string> s)
        {
            InitializeComponent();

            foreach (var s2 in s)
            {
                listBox1.Items.Add(s2);
            }
        }

        private void AskFromList_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Non hai selezionato nulla!");
                return;
            }

            this.r = listBox1.SelectedIndex;
            this.Close();
        }
    }
}