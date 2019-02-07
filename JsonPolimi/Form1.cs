using System;
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

        List<Gruppo> L = null;

        private void Button1_Click(object sender, EventArgs e)
        {
            L = new List<Gruppo>();

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
            string html = "<html><body><table>";
            for (int i = 0; i<L.Count; i++)
            {
                html += "<tr>";

                html += "<td>";
                html += L[i].id;
                html += "</td>";

                html += "<td>";
                html += L[i].platform;
                html += "</td>";

                html += "<td>";
                html += L[i].classe;
                html += "</td>";

                html += "<td>";
                html += L[i].degree;
                html += "</td>";

                html += "<td>";
                html += L[i].language;
                html += "</td>";

                html += "<td>";
                html += L[i].office;
                html += "</td>";

                html += "<td>";
                html += L[i].school;
                html += "</td>";

                html += "<td>";
                html += L[i].type;
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

            L.Add(G);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Refresh_Tabella();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string json = "{\"info_data\":{";
            for (int i=0; i<L.Count; i++)
            {
                /*
                    "FB/230920497487655":{"class":"Ingegneria Aerospaziale","office":"BOVISA","id":"FB/230920497487655","degree":"LT","school":"3I","language":"ITA","group_type":"FB"},
                */
                json += '"';
                json += L[i].id;
                json += '"' + ":{";

                json += "\"class\":\"";
                json += L[i].classe;
                json += "\",\"office\":\"";
                json += L[i].office;
                json += "\",\"id\":\"";
                json += L[i].id;
                json += "\",\"degree\":\"";
                json += L[i].degree;
                json += "\",\"school\":\"";
                json += L[i].school;
                json += "\",\"language\":\"";
                json += L[i].language;
                json += "\",\"type\":\"";
                json += L[i].type;
                json += "\",\"platform\":\"";
                json += L[i].platform;
                json += "\"";

                json += "}";

                if (i != L.Count - 1)
                    json += ",";

            }

            json += "},\"index_data\":[";
            for (int i = 0; i < L.Count; i++)
            {
                //{"class":"Ingegneria Aerospaziale","office":"BOVISA","id":"FB/230920497487655","degree":"LT","school":"3I","language":"ITA","group_type":"FB"},

                json += "{";

                json += "\"class\":\"";
                json += L[i].classe;
                json += "\",\"office\":\"";
                json += L[i].office;
                json += "\",\"id\":\"";
                json += L[i].id;
                json += "\",\"degree\":\"";
                json += L[i].degree;
                json += "\",\"school\":\"";
                json += L[i].school;
                json += "\",\"language\":\"";
                json += L[i].language;
                json += "\",\"type\":\"";
                json += L[i].type;
                json += "\",\"platform\":\"";
                json += L[i].platform;
                json += "\"";

                json += "}";

                if (i != L.Count - 1)
                    json += ",";
            }

            json += "]}";

            File.WriteAllText("C:\\git\\polinetwork.github.io\\data\\search\\groups2.json", json);
        }
    }
}
