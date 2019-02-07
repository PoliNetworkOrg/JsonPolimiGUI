﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace JsonPolimi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Variabili.L = new List<Gruppo>();

            string content = File.ReadAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups.json");
            var stuff = JObject.Parse(content);

            var InfoData = stuff["info_data"];
            var i = InfoData.Children();

            foreach (var i2 in i)
            {
                var i3 = i2.First;

                Aggiungi(i3);
            }


        }

        private void Refresh_Tabella()
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            string html = "<html><body><table>";
            for (int i = 0; i< Variabili.L.Count; i++)
            {
                html += "<tr>";

                html += "<td>";
                html += Variabili.L[i].id;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].platform;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].classe;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].degree;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].language;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].office;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].school;
                html += "</td>";

                html += "<td>";
                html += Variabili.L[i].type;
                html += "</td>";

                html += "</tr>";
            }
            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private void Aggiungi(JToken i)
        {
            Gruppo G = new Gruppo
            {
                classe = i["class"].ToString(),
                degree = i["degree"].ToString(),
                platform = i["group_type"].ToString(),
                id = i["id"].ToString(),
                language = i["language"].ToString(),
                office = i["office"].ToString(),
                school = i["school"].ToString()
            };

            try
            {
                G.type = i["type"].ToString();
            }
            catch
            {
                G.type = null;
            }

            G.Aggiusta();

            Variabili.L.Add(G);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Refresh_Tabella();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            string json = "{\"info_data\":{";
            for (int i=0; i< Variabili.L.Count; i++)
            {
                /*
                    "FB/230920497487655":{"class":"Ingegneria Aerospaziale","office":"BOVISA","id":"FB/230920497487655","degree":"LT","school":"3I","language":"ITA","group_type":"FB"},
                */
                json += '"';
                json += Variabili.L[i].id;
                json += '"' + ":{";

                json += "\"class\":\"";
                json += Variabili.L[i].classe;
                json += "\",\"office\":\"";
                json += Variabili.L[i].office;
                json += "\",\"id\":\"";
                json += Variabili.L[i].id;
                json += "\",\"degree\":\"";
                json += Variabili.L[i].degree;
                json += "\",\"school\":\"";
                json += Variabili.L[i].school;
                json += "\",\"language\":\"";
                json += Variabili.L[i].language;
                json += "\",\"type\":\"";
                json += Variabili.L[i].type;
                json += "\",\"platform\":\"";
                json += Variabili.L[i].platform;
                json += "\"";

                json += "}";

                if (i != Variabili.L.Count - 1)
                    json += ",";

            }

            json += "},\"index_data\":[";
            for (int i = 0; i < Variabili.L.Count; i++)
            {
                //{"class":"Ingegneria Aerospaziale","office":"BOVISA","id":"FB/230920497487655","degree":"LT","school":"3I","language":"ITA","group_type":"FB"},

                json += "{";

                json += "\"class\":\"";
                json += Variabili.L[i].classe;
                json += "\",\"office\":\"";
                json += Variabili.L[i].office;
                json += "\",\"id\":\"";
                json += Variabili.L[i].id;
                json += "\",\"degree\":\"";
                json += Variabili.L[i].degree;
                json += "\",\"school\":\"";
                json += Variabili.L[i].school;
                json += "\",\"language\":\"";
                json += Variabili.L[i].language;
                json += "\",\"type\":\"";
                json += Variabili.L[i].type;
                json += "\",\"platform\":\"";
                json += Variabili.L[i].platform;
                json += "\"";

                json += "}";

                if (i != Variabili.L.Count - 1)
                    json += ",";
            }

            json += "]}";

            File.WriteAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups2.json", json);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Aggiungi_Form x = new Aggiungi_Form();
            x.ShowDialog();
        }
    }
}
