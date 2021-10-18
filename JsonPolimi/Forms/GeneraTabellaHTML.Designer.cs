
namespace JsonPolimi.Forms
{
    partial class GeneraTabellaHTML
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_anno = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_piattaforma = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(382, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 116);
            this.button1.TabIndex = 0;
            this.button1.Text = "Genera";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox_anno
            // 
            this.textBox_anno.Location = new System.Drawing.Point(26, 28);
            this.textBox_anno.Name = "textBox_anno";
            this.textBox_anno.Size = new System.Drawing.Size(282, 20);
            this.textBox_anno.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Anno (esempio: \"2021/2022\")";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Piattaforma (TG,WA,FB)";
            // 
            // textBox_piattaforma
            // 
            this.textBox_piattaforma.Location = new System.Drawing.Point(26, 81);
            this.textBox_piattaforma.Name = "textBox_piattaforma";
            this.textBox_piattaforma.Size = new System.Drawing.Size(282, 20);
            this.textBox_piattaforma.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "campo vuoto = no filtro";
            // 
            // GeneraTabellaHTML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 158);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_piattaforma);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_anno);
            this.Controls.Add(this.button1);
            this.Name = "GeneraTabellaHTML";
            this.Text = "GeneraTabellaHTML";
            this.Load += new System.EventHandler(this.GeneraTabellaHTML_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_anno;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_piattaforma;
        private System.Windows.Forms.Label label3;
    }
}