using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VirusC
{
    public partial class Form2 : Form
    {
        string[] lstCountries = Form1.lstCountries;
        Tuple<string, double>[] population = Form1.population;
        string url = Form1.url[0];
                     //"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
                     //"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv";
        
        List<DateTime> lstDates;
        List<int>[] lstData;
        List<int>[] lstDataNew;
        string[] selCountries = new string[] { "China", "Italy", "US", "Spain", "Germany", 
                                               "Iran", "France", "Korea, South", "Switzerland", 
                                               "United Kingdom", "Netherlands", "Austria", 
                                               "Turkey", "Greece" };
        bool[] selCountriesShow = new bool[] { false, true, true, true, true, 
                                               true, true, false, true, 
                                               true, true, true, 
                                               true, true };
        List<Color> lineColor= new List<Color>() {Color.FromArgb(57,106,177),
                                                  Color.FromArgb(218,124,48),
                                                  Color.FromArgb(62,150,81),
                                                  Color.FromArgb(204,37,41),
                                                  Color.FromArgb(83,81,84),
                                                  Color.FromArgb(107,76,154),
                                                  Color.FromArgb(146,36,40),
                                                  Color.FromArgb(148,139,61),
                                                  Color.FromArgb(107,156,227),
                                                  Color.FromArgb(255,174,98),
                                                  Color.FromArgb(112,200,131),
                                                  Color.FromArgb(254,87,91),
                                                  Color.FromArgb(133,131,134),
                                                  Color.FromArgb(157,126,204),
                                                  Color.FromArgb(196,86,90),
                                                  Color.FromArgb(198,189,111),};

        bool bIsLoading = true;
        public Form2()
        {
            InitializeComponent();
            //KnownColor[] colors = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            //foreach (KnownColor col in colors) lineColor.Add(Color.FromKnownColor(col));
            ReadCountriesFile("countries.txt");
            bIsLoading = true;
            for(int i = 0; i < selCountries.Length; i++){
                CheckBox check = new CheckBox();
                check.Name = "checkBox" + i;
                check.Text = selCountries[i];
                check.CheckedChanged += checkBox_CheckedChanged;
                check.TextAlign = ContentAlignment.MiddleLeft;
                check.AutoSize = false;
                check.Top = 81 + (i - 1) * 23;
                check.Left = 1043;
                check.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                check.Checked = selCountriesShow[i];
                this.Controls.Add(check);
            }
            bIsLoading = false;
            this.WindowState = FormWindowState.Maximized;
        }

        void ReadCountriesFile(string fi)
        {
            if (!System.IO.File.Exists(fi)) return;
            List<string> lstCountries = new List<string>();
            List<bool> lstShow = new List<bool>();
            using (System.IO.StreamReader file = new System.IO.StreamReader(fi)){
                string line = file.ReadToEnd();
                string[] li = line.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach(string ll in li){
                    string[] lll = ll.Split(';');
                    if (lll.Length > 1){
                        lstCountries.Add(lll[0]);
                        if (lll[1] == "1") lstShow.Add(true); else lstShow.Add(false);
                    }
                }
            }
            selCountries = lstCountries.ToArray();
            selCountriesShow = lstShow.ToArray();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            button1_Click(button1, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string csv = GetCSV(url).Trim();
            if (csv.Length == 0) return;
            string[] lines = csv.Split(new string[] { "\n" }, StringSplitOptions.None);
            string[] dates = lines[0].Trim().Split(',');
            lstDates = new List<DateTime>();
            for (int i = 4; i < dates.Length; i++) {
                string[] compDates = dates[i].Split('/');
                lstDates.Add(new DateTime(int.Parse(compDates[2]), int.Parse(compDates[0]), int.Parse(compDates[1])));
            }
            lstData = new List<int>[lstCountries.Length];
            bool[] isSet = new bool[lstCountries.Length];
            for (int i = 0; i < lstCountries.Length; i++) { lstData[i] = new List<int>(); isSet[i] = false; }
            for(int i = 1; i < lines.Length; i++)
            {
                string[] li=null;
                string[] li1=null;
                if (lines[i].Trim().StartsWith("\"")){
                    li = lines[i].Trim().Trim().Split(new string[] { "\"," }, StringSplitOptions.None);
                    li1 = li[1].Split(',');
                    string[] newVal = new string[li1.Length + 1];
                    newVal[0] = li[0].Replace("\"", "");
                    Array.Copy(li1, 0, newVal, 1, li1.Length);
                    li1 = newVal;
                }else if (lines[i].Trim().Contains("\"")){
                    li = lines[i].Trim().Split(new string[] { "\"," }, StringSplitOptions.None);
                    li1 = li[1].Split(',');
                    string[] newVal = new string[li1.Length + 2];
                    newVal[1] = li[0].Replace(",\"", "");
                    newVal[0] = "";
                    Array.Copy(li1, 0, newVal, 2, li1.Length);
                    //Array.Copy(new string[] { "" }, 0, li1, 1, li1.Length);
                    li1 = newVal;
                }else{
                    li1=lines[i].Trim().Split(',');
                }
                string[] data = li1;
                string xora = data[1].Replace(";",",");
                int index = GetCountryIndex(xora);
                if (index >= 0)
                {
                    if (isSet[index]){
                        for (int j = 4; j < data.Length; j++)
                            if (data[j].Length > 0)
                                lstData[index][j - 4] += int.Parse(data[j]);
                    }else{
                        for (int j = 4; j < data.Length; j++) if (data[j].Length > 0) lstData[index].Add(int.Parse(data[j]));
                        isSet[index] = true;
                    }
                }
                //else { MessageBox.Show("Cannot find " + xora); }
            }
            
            lstDataNew = new List<int>[lstCountries.Length];
            //chart1.Series.Clear();
            foreach (string co in lstCountries){
                int index = GetCountryIndex(co);
                double pop = GetPopulation(co);
                lstDataNew[index] = new List<int>();
                for(int i = 0; i < lstData[index].Count; i++){
                    if ((double)lstData[index][i] * 10000000.0 / pop >= 10.0){
                        for (int j = i; j < lstData[index].Count; j++) lstDataNew[index].Add((int)((double)lstData[index][j] * 10000000.0 / pop));
                        break;
                    }
                }
            }
            SetChartSeries();
        }

        void SetChartSeries()
        {
            chart1.Series.Clear();
            int col = 0;
            foreach (string co in selCountries){
                int index = GetCountryIndex(co);
                double pop = GetPopulation(co);
                textBox1.Text += lstCountries[index] + ": ";
                for (int i = 0; i < lstDataNew[index].Count; i++){
                    textBox1.Text += (i>0?"-":"") + lstDataNew[index][i];
                }
                textBox1.Text += "\r\n";
                chart1.Series.Add(co);
                chart1.Series[co].ChartType = SeriesChartType.Line;
                for (int k = 0; k < lstDataNew[index].Count; k++)
                    chart1.Series[co].Points.AddXY((double)k, (double)lstDataNew[index][k]);
                chart1.Series[co].Color = lineColor[col];
                chart1.Series[co].Points[lstDataNew[index].Count - 1].Label = co;
                chart1.Series[co].Points[lstDataNew[index].Count - 1].LabelForeColor = lineColor[col];// chart1.Series[co].Color;
                chart1.Series[co].Points[lstDataNew[index].Count - 1].SetCustomProperty("LabelStyle", "Right");
                chart1.Series[co].Enabled = CheckBoxIsChecked(co);
                col++;
            }
        }

        bool CheckBoxIsChecked(string co)
        {
            foreach(Control chk in this.Controls){
                if (chk.GetType()==typeof(CheckBox)) 
                    if(chk.Text == co) 
                        return ((CheckBox)chk).Checked;
            }
            return false;
        }

        int GetCountryIndex(string xora)
        {
            for (int i = 0; i < lstCountries.Length; i++) 
                if (xora == lstCountries[i]) return i;
            return -1;
        }

        int GetSelectedCountryIndex(string xora)
        {
            for (int i = 0; i < selCountries.Length; i++)
                if (xora == selCountries[i]) return i;
            return -1;
        }

        double GetPopulation(string cou)
        {
            foreach (Tuple<string, double> item in population)
                if (item.Item1 == cou) return item.Item2;
            return 0.0;
        }

        public string GetCSV(string url)
        {
            try{
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }catch(Exception ex) {
                MessageBox.Show("Error in getting CSV file '" + url + "':\r\n" + ex.Message);
            }
            return "";
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (bIsLoading) return;
            string co = ((CheckBox)sender).Text;
            chart1.Series[co].Enabled = ((CheckBox)sender).Checked;
            selCountriesShow[GetSelectedCountryIndex(co)]= ((CheckBox)sender).Checked;
            chart1.ChartAreas["ChartArea1"].RecalculateAxesScale();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            tmpForm frm = new tmpForm(selCountries, selCountriesShow);
            frm.ShowDialog();
            bIsLoading = true;
            ReadCountriesFile("countries.txt");
            List<CheckBox> lst = new List<CheckBox>();
            foreach (Control ctrl in this.Controls.OfType<CheckBox>()) lst.Add((CheckBox)ctrl);
            foreach (CheckBox chk in lst) chk.Dispose();
            Application.DoEvents();
            for (int i = 0; i < selCountries.Length; i++){
                CheckBox check = new CheckBox();
                check.Name = "checkBox" + i;
                check.Text = selCountries[i];
                check.CheckedChanged += checkBox_CheckedChanged;
                check.TextAlign = ContentAlignment.MiddleLeft;
                check.AutoSize = false;
                check.Top = 81 + (i - 1) * 23;
                check.Left = button1.Left;
                check.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                check.Checked = selCountriesShow[i];
                this.Controls.Add(check);
            }
            bIsLoading = false;
            button1_Click(button1, EventArgs.Empty);
        }
    }
}
