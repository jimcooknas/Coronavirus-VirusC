using Accord.Math;
using Accord.Math.Optimization;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirusC
{
    /// <summary>
    /// Idea taken from Gianluca Malato 
    /// (at https://towardsdatascience.com/covid-19-infection-in-italy-mathematical-models-and-predictions-7784b4d7dd8d)
    /// and generalized, using the Novel Coronavirus (COVID-19) Cases provided by JHU CSSE
    /// (at https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv)
    /// </summary>
    public partial class Form1 : Form
    {
        public static int p1 = 4;
        public static int p3 = 1000;
        public static string startCountry = "Greece";
        public static string pythonScript = "virus_script.py";
        public static string iniFile = "virus.ini";
        // old adresses
        // "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv",
        // "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Deaths.csv",
        // "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Recovered.csv"
        public static string[] url = new string[] {"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv",
                                                   "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv",
                                                   "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv"
                                                  };

        string pythonVersion = "";
        double[] valX;
        double[] valY;
        double[] perdayY;
        double[] valY_d;
        double[] valY_r;
        double[] calcX;
        double[] calcY;
        double[] expX;
        double[] expY;
        double a = 2.0;
        double b = 10.0;
        double c = 1000.0;
        double err_a = 0.0;
        double err_b = 0.0;
        double err_c = 0.0;
        double sol = 0;
        double exp_a = 0.0;
        double exp_b = 0.0;
        double exp_c = 0.0;
        DateTime startDate = new DateTime(2020, 1, 1);
        public static string calcMethod = "python";
        List<DateTime> lstDates;
        List<int>[] lstData;

        public static string[] lstCountries = new string[] { 
            "World", "Afghanistan", "Albania", "Algeria", "Andorra", "Antigua and Barbuda", 
            "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", 
            "Bahrain", "Bangladesh", "Belarus", "Belgium", "Bhutan", "Bolivia", 
            "Bosnia and Herzegovina", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Cambodia", 
            "Cameroon", "Canada", "Cayman Islands", "Central African Republic", "Chile", "China", 
            "Colombia", "Congo (Brazzaville)", "Congo (Kinshasa)", "Costa Rica", "Cote d'Ivoire", 
            "Croatia", "Cruise Ship", "Cuba", "Curacao", "Cyprus", "Czechia", "Denmark", 
            "Dominican Republic", "Ecuador", "Egypt", "Equatorial Guinea", "Estonia", "Eswatini", 
            "Ethiopia", "Finland", "France", "Gabon", "Georgia", "Germany", "Ghana", "Greece", 
            "Guadeloupe", "Guatemala", "Guernsey", "Guinea", "Guyana", "Holy See", "Honduras", 
            "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", 
            "Italy",  "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Korea, South", 
            "Kosovo", "Kuwait", "Latvia", "Lebanon", "Liechtenstein", "Lithuania", "Luxembourg", 
            "Malaysia", "Maldives", "Malta", "Martinique", "Mauritania", "Mexico", "Moldova", 
            "Monaco", "Mongolia", "Morocco", "Namibia", "Nepal", "Netherlands", "New Zealand", 
            "Nigeria", "North Macedonia", "Norway", "Oman", "Pakistan", "Panama", "Paraguay", 
            "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Reunion", "Romania", "Russia", 
            "Rwanda", "Saint Lucia", "Saint Vincent and the Grenadines", "San Marino", 
            "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Singapore", "Slovakia", "Slovenia", 
            "South Africa", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", 
            "Taiwan*", "Thailand", "Togo", "Trinidad and Tobago", "Tunisia", "Turkey", "Ukraine", 
            "United Arab Emirates", "United Kingdom", "Uruguay", "US", "Uzbekistan", "Venezuela", 
            "Vietnam" };

        public static Tuple<string, double>[] population = {
                Tuple.Create("Afghanistan",34656032.0), Tuple.Create("Albania",2876101.0),
                Tuple.Create("Afghanistan",34656032.0), Tuple.Create("Albania",2876101.0),
                Tuple.Create("Afghanistan",34656032.0), Tuple.Create("Albania",2876101.0),
                Tuple.Create("Algeria",40606052.0), Tuple.Create("Andorra",77281.0),
                Tuple.Create("Antigua and Barbuda",100963.0), Tuple.Create("Argentina",43847430.0),
                Tuple.Create("Armenia",2924816.0), Tuple.Create("Aruba",104822.0),
                Tuple.Create("Australia",24127159.0), Tuple.Create("Austria",8747358.0),
                Tuple.Create("Azerbaijan",9762274.0), Tuple.Create("Bahrain",1425171.0),
                Tuple.Create("Bangladesh",162951560.0), Tuple.Create("Belarus",9507120.0),
                Tuple.Create("Belgium",11348159.0), Tuple.Create("Bhutan",797765.0),
                Tuple.Create("Bolivia",10887882.0), Tuple.Create("Bosnia and Herzegovina",3516816.0),
                Tuple.Create("Brazil",207652865.0), Tuple.Create("Brunei",423196.0),
                Tuple.Create("Bulgaria",7127822.0), Tuple.Create("Burkina Faso",18646433.0),
                Tuple.Create("Cambodia",15762370.0), Tuple.Create("Cameroon",23439189.0),
                Tuple.Create("Canada",36286425.0), Tuple.Create("Cayman Islands",60765.0),
                Tuple.Create("Chile",17909754.0), Tuple.Create("China",1378665000.0),
                Tuple.Create("Colombia",48653419.0), Tuple.Create("Congo (Kinshasa)",5125821.0),
                Tuple.Create("Costa Rica",4857274.0), Tuple.Create("Cote d'Ivoire",23695919.0),
                Tuple.Create("Croatia",4170600.0), Tuple.Create("Cruise Ship",1000.0),
                Tuple.Create("Cuba",11475982.0), Tuple.Create("Cyprus",1170125.0),
                Tuple.Create("Czechia",10561633.0), Tuple.Create("Denmark",5731118.0),
                Tuple.Create("Dominican Republic",10648791.0), Tuple.Create("Ecuador",16385068.0),
                Tuple.Create("Egypt",95688681.0), Tuple.Create("Estonia",1316481.0),
                Tuple.Create("Ethiopia",102403196.0), Tuple.Create("Finland",5495096.0),
                Tuple.Create("France",66896109.0), Tuple.Create("French Guiana",50000.0),
                Tuple.Create("Georgia",3719300.0), Tuple.Create("Germany",82667685.0),
                Tuple.Create("Greece",10746740.0), Tuple.Create("Guadeloupe",200000.0),
                Tuple.Create("Guinea",12395924.0), Tuple.Create("Guyana",773303.0),
                Tuple.Create("Holy See",1000.0), Tuple.Create("Honduras",9112867.0),
                Tuple.Create("Hungary",9817958.0), Tuple.Create("Iceland",334252.0),
                Tuple.Create("India",1324171354.0), Tuple.Create("Indonesia",261115456.0),
                Tuple.Create("Iran",80277428.0), Tuple.Create("Iraq",37202572.0),
                Tuple.Create("Ireland",4773095.0), Tuple.Create("Israel",8547100.0),
                Tuple.Create("Italy",60600590.0), Tuple.Create("Jamaica",2881355.0),
                Tuple.Create("Japan",126994511.0), Tuple.Create("Jordan",9455802.0),
                Tuple.Create("Kazakhstan",17797032.0), Tuple.Create("Kenya",48461567.0),
                Tuple.Create("Korea, South",51470000.0), Tuple.Create("Kuwait",4052584.0),
                Tuple.Create("Latvia",1960424.0), Tuple.Create("Lebanon",6006668.0),
                Tuple.Create("Liechtenstein",37666.0), Tuple.Create("Lithuania",2872298.0),
                Tuple.Create("Luxembourg",582972.0), Tuple.Create("Malaysia",31187265.0),
                Tuple.Create("Maldives",417492.0), Tuple.Create("Malta",436947.0),
                Tuple.Create("Martinique",1000.0), Tuple.Create("Mexico",127540423.0),
                Tuple.Create("Moldova",3552000.0), Tuple.Create("Monaco",38499.0),
                Tuple.Create("Mongolia",3027398.0), Tuple.Create("Morocco",35276786.0),
                Tuple.Create("Nepal",28982771.0), Tuple.Create("Netherlands",17018408.0),
                Tuple.Create("New Zealand",4692700.0), Tuple.Create("Nigeria",185989640.0),
                Tuple.Create("North Macedonia",500000.0), Tuple.Create("Norway",5232929.0),
                Tuple.Create("Oman",4424762.0), Tuple.Create("Pakistan",193203476.0),
                Tuple.Create("Panama",4034119.0), Tuple.Create("Paraguay",6725308.0),
                Tuple.Create("Peru",31773839.0), Tuple.Create("Philippines",103320222.0),
                Tuple.Create("Poland",37948016.0), Tuple.Create("Portugal",10324611.0),
                Tuple.Create("Qatar",2569804.0), Tuple.Create("Reunion",1000.0),
                Tuple.Create("Romania",19705301.0), Tuple.Create("Russia",51470000.0),
                Tuple.Create("San Marino",33203.0), Tuple.Create("Saudi Arabia",32275687.0),
                Tuple.Create("Senegal",15411614.0), Tuple.Create("Serbia",7057412.0),
                Tuple.Create("Singapore",5607283.0), Tuple.Create("Slovakia",51470000.0),
                Tuple.Create("Slovenia",2064845.0), Tuple.Create("South Africa",55908865.0),
                Tuple.Create("Spain",46443959.0), Tuple.Create("Sri Lanka",21203000.0),
                Tuple.Create("Sudan",39578828.0), Tuple.Create("Sweden",9903122.0),
                Tuple.Create("Switzerland",8372098.0), Tuple.Create("Taiwan*",23780000.0),
                Tuple.Create("Thailand",68863514.0), Tuple.Create("Togo",7606374.0),
                Tuple.Create("Tunisia",11403248.0), Tuple.Create("Turkey",79512426.0),
                Tuple.Create("US",327200000.0), Tuple.Create("Ukraine",45004645.0),
                Tuple.Create("United Arab Emirates",9269612.0), Tuple.Create("United Kingdom",65637239.0),
                Tuple.Create("Vietnam",92701100.0)
            };




        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(lstCountries);
            ReadIniFile();
            comboBox1.SelectedIndex = GetIndexByCountryName(startCountry);
            chkShowDeaths.Checked = false;
            chkShowRecovered.Checked = false;
            lstMethod.SelectedIndex = 0;
            //cbCalcMethod.SelectedIndex = 0;
        }

        void ReadIniFile()
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(iniFile)){
                while (!file.EndOfStream){
                    string line = file.ReadLine();
                    if (!line.StartsWith("#")){
                        string[] li = line.Split('=');
                        switch (li[0]){
                            case "CalcMethod":
                                calcMethod = li[1];
                                if(calcMethod=="python") cbCalcMethod.SelectedIndex = 0;
                                else cbCalcMethod.SelectedIndex = 1;
                                break;
                            case "PythonScript":
                                pythonScript = li[1];
                                break;
                            case "StartingCountry":
                                startCountry = li[1];
                                break;
                            case "StartingRate":
                                p1 = int.Parse(li[1]);
                                break;
                            case "StartingInfectedAtEnd":
                                p3 = int.Parse(li[1]);
                                break;
                            case "URLconfirmed":
                                url[0] = li[1];
                                break;
                            case "URLdeaths":
                                url[1] = li[1];
                                break;
                            case "URLrecovered":
                                url[2] = li[1];
                                break;
                        }
                    }
                }
            }
        }

        public static int GetIndexByCountryName(string country)
        {
            for(int i = 0; i < lstCountries.Length; i++){
                if (country == lstCountries[i]) return i;
            }
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunPython(comboBox1.Text);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            chkShowErrors.Checked = false;
            RunPython(comboBox1.Text);
        }

        private void chkShowErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowErrors.Checked){
                lblPythonErrors.Visible = true;
                textBox2.Visible = true;
                textBox1.Width = textBox2.Left - textBox1.Left - 20;
            }else{
                lblPythonErrors.Visible = false;
                textBox2.Visible = false;
                textBox1.Width = textBox2.Left + textBox2.Width - textBox1.Left;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings stg = new frmSettings();
            stg.ShowDialog();
        }

        private void chkShowDeaths_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series["Deaths"].Enabled = chkShowDeaths.Checked;
        }

        private void chkShowRecovered_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series["Recovered"].Enabled = chkShowRecovered.Checked;
        }

        void LogEntry(string txt)
        {
            if (txt.Trim().Length > 0){
                textBox1.Text += txt + "\r\n";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
        }

        void LogError(string country, string txt)
        {
            if (txt.Trim().Length > 0){
                textBox2.Text += "Error in " + country + ":\r\n*****" + txt + "\r\n";
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
            }
        }

        double SigmoidFunction(double x, double a1, double b1, double c1)
        {
            return c1 / (1 + Math.Exp(-(x - b1) / a1));
        }

        double ExponentialFunction(double x, double a1, double b1, double c1)
        {
            return a1 * Math.Exp(b1 * (x - c1));
        }

        double GetPopulation(string cou)
        {
            foreach (Tuple<string, double> item in population)
                if (item.Item1 == cou) return item.Item2;
            return 0.0;
        }

        void RunPython(string country="World")
        {
            Cursor = Cursors.WaitCursor;
            lblCountry.Text = country;
            string expon = "";
            double maxY = 0.0;
            txtRate.Text = "";
            txtMax.Text = "";
            txtTotal.Text = "";
            lblDates.Text = "";
            lblLastTotal.Text = "";
            lblLastDeaths.Text = "";
            lblLastRecovered.Text = "";
            lblTotalCases.Text = "";
            txtDateEnd.Text = "";
            lblMSE.Text = "";
            lblPopulationIndex.Text = "";
            lblPopulation.Text = String.Format("{0:#,###}", GetPopulation(country));
            Application.DoEvents();
            switch (calcMethod) {
                case "python":
                    string progToRun = pythonScript;
                    char[] splitter = { '\r' };
                    Process proc = new Process();
                    proc.StartInfo.FileName = "python.exe";
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    LogEntry("***** Running Python script '" + pythonScript + "' for " + country + " *****");
                    proc.StartInfo.Arguments = string.Concat(progToRun, " \"", country,"\" ", p1, " ", p3, " ", lstMethod.Items[lstMethod.SelectedIndex]);
                    proc.Start();

                    StreamReader sReader = proc.StandardOutput;
                    StreamReader eReader = proc.StandardError;
                    string[] output = sReader.ReadToEnd().Split(splitter);
                    string[] error = eReader.ReadToEnd().Split(splitter);
                    if (output.Length > 1){
                        //LogEntry("output: ");
                        foreach (string ss in output) if(ss.Trim().Length>0)LogEntry(ss.Trim());
                    }
                    if (error.Length > 1){
                        //LogEntry("error: ");
                        foreach (string ss in error) LogError(country, ss);
                        //LogEntry("**** End of Running Python script 'virus_script.py' for " + country + " ****" + "\r\n");
                        //return;
                    }
                    LogEntry("**** End of Running Python script '" + pythonScript + "' for " + country + " ****" + "\r\n");
                    string outX = "";
                    string outY = "";
                    string outY_d = "";
                    string outY_r = "";
                    string coeff = "";
                    string errors = "";
                    for(int i = 0; i < output.Length; i++){
                        if (pythonVersion.Length == 0 && output[i].Trim().StartsWith("Running in Python ver.:")){
                            pythonVersion = output[i].Trim().Substring("Running in Python ver.:".Length + 1);
                            this.Text = String.Format("COVID-19  ver.: {0}    -------    (Python version: {1})", this.ProductVersion,pythonVersion);
                        }
                        if (output[i].Trim() == "Results") { coeff = output[i + 1].Trim(); errors = output[i + 2].Trim(); }
                        if (output[i].Trim() == "X") outX = output[i + 1].Trim().Replace("[","").Replace("]","");
                        if (output[i].Trim() == "Y") outY = output[i + 1].Trim().Replace("[", "").Replace("]", "");
                        if (output[i].Trim() == "Y_d") outY_d = output[i + 1].Trim().Replace("[", "").Replace("]", "");
                        if (output[i].Trim() == "Y_r") outY_r = output[i + 1].Trim().Replace("[", "").Replace("]", "");
                        if (output[i].Trim() == "Exponential") expon = output[i + 1].Trim().Replace("[", "").Replace("]", "");
                    }
                    if (outX.Length > 0 && outY.Length > 0 & coeff.Length > 0){
                        //LogEntry("Plotting chart");
                        if (coeff.Length > 0){
                            string[] vals = coeff.Split(' ');
                            if (IsNumeric(vals[0].Replace('.', ','))) a = double.Parse(vals[0].Replace('.', ',')); else a = 0.0;
                            if (IsNumeric(vals[1].Replace('.', ','))) b = double.Parse(vals[1].Replace('.', ',')); else b = 0.0;
                            if (IsNumeric(vals[2].Replace('.', ','))) c = double.Parse(vals[2].Replace('.', ',')); else c = 0.0;
                            if (IsNumeric(vals[3].Replace('.', ','))) sol = double.Parse(vals[3].Replace('.', ',')); else sol = 0;
                            string[] vals1 = errors.Split(' ');
                            if (IsNumeric(vals1[0].Replace('.', ','))) err_a = double.Parse(vals1[0].Replace('.', ',')); else err_a = 0.0;
                            if (IsNumeric(vals1[1].Replace('.', ','))) err_b = double.Parse(vals1[1].Replace('.', ',')); else err_b = 0.0;
                            if (IsNumeric(vals1[2].Replace('.', ','))) err_c = double.Parse(vals1[2].Replace('.', ',')); else err_c = 0.0;
                            txtRate.Text = String.Format("{0:F2} ± {1:F2}", a, err_a);

                            txtMax.Text = String.Format("{0:F0} ± {1:F0}", b, err_b);
                            if (err_b > 100.0 && err_b < 1000.0 && b < 1000.0){
                                lblDates.Text = String.Format("(? - {0:d/M/yy} - ?)", startDate.AddDays((int)b));
                            }else if (err_b < 100.0 && b < 1000.0){
                                lblDates.Text = String.Format("({0:d/M/yy}-{1:d/M/yy})", startDate.AddDays((int)(b - err_b)), startDate.AddDays((int)(b + err_b)));
                            }else { lblDates.Text = "Non Applicable"; }
                            txtTotal.Text = String.Format("{0:#,##0} ± {1:#,##0}", c, err_c);
                            if (sol < 1000.0) txtDateEnd.Text = String.Format("{0:d/M/yy}", startDate.AddDays((int)(sol)));
                            else txtDateEnd.Text = "Non Applicable";
                        }
                        if (outX.Length > 0){
                            string[] vals = outX.Split(',');
                            valX = new double[vals.Length];
                            for (int i = 0; i < vals.Length; i++){
                                if (IsNumeric(vals[i].Trim())) valX[i] = double.Parse(vals[i].Trim());
                                else valX[i] = 0.0;
                            }
                        }
                        if (outY.Length > 0){
                            string[] vals = outY.Split(',');
                            valY = new double[vals.Length];
                            perdayY = new double[vals.Length];
                            for (int i = 0; i < vals.Length; i++){
                                if (IsNumeric(vals[i].Trim())){
                                    valY[i] = double.Parse(vals[i].Trim());
                                    if (i > 0) perdayY[i] = valY[i] - valY[i - 1];
                                    else perdayY[i] = 0.0;
                                }else { valY[i] = 0.0; perdayY[i] = 0.0; }
                            }
                        }
                        if (outY_d.Length > 0){
                            string[] vals = outY_d.Split(',');
                            valY_d = new double[vals.Length];
                            for (int i = 0; i < vals.Length; i++){
                                if (IsNumeric(vals[i].Trim())) valY_d[i] = double.Parse(vals[i].Trim());
                                else valY_d[i] = 0.0;
                            }
                        }
                        if (outY_r.Length > 0){
                            string[] vals = outY_r.Split(',');
                            valY_r = new double[vals.Length];
                            for (int i = 0; i < vals.Length; i++){
                                if (IsNumeric(vals[i].Trim())) valY_r[i] = double.Parse(vals[i].Trim());
                                else valY_r[i] = 0.0;
                            }
                        }
                    }else{
                        MessageBox.Show("Could not get results with python!!!");
                    }
                    break;
                case "NelderMead":
                    this.Text = String.Format("COVID-19  ver.: {0}", this.ProductVersion);
                    ExtractDataFromURLs(country);
                    int coun = 0;
                    while (a < 1.0 && coun < 10) { button1.Text = "Run script " + coun; Application.DoEvents(); ExtractDataFromURLs(country); coun++; }
                    button1.Text = "Run script";
                    if (calcMethod=="python")txtRate.Text = String.Format("{0:F2} ± {1:F2}", a, err_a);
                    else txtRate.Text = String.Format("{0:F2}", a);
                    if (calcMethod == "python") txtMax.Text = String.Format("{0:F0} ± {1:F0}", b, err_b);
                    else txtMax.Text = String.Format("{0:F0}", b);
                    if (err_b > 100.0 && err_b < 1000.0 && b < 1000.0){
                        lblDates.Text = String.Format("(? - {0:d/M/yy} - ?)", startDate.AddDays((int)b));
                    } else if (err_b < 100.0 && b < 1000.0){
                        lblDates.Text = String.Format("({0:d/M/yy}-{1:d/M/yy})", startDate.AddDays((int)(b - err_b)), startDate.AddDays((int)(b + err_b)));
                    } else { lblDates.Text = "Non Applicable"; }
                    if (calcMethod == "python") txtTotal.Text = String.Format("{0:#,##0} ± {1:#,##0}", c, err_c);
                    else txtTotal.Text = String.Format("{0:#,##0}", c);
                    if (sol < 1000.0) txtDateEnd.Text = String.Format("{0:d/M/yy}", startDate.AddDays((int)(sol)));
                    else txtDateEnd.Text = "Non Applicable";
                    break;
                }
                lblLastTotal.Text = String.Format("{0:#,##0}", valY[valY.Length - 1]);
                lblLastDeaths.Text = String.Format("{0:#,##0}", valY_d[valY_d.Length - 1]);
                lblLastRecovered.Text = String.Format("{0:#,##0}", valY_r[valY_r.Length - 1]);
                lblTotalCases.Text= String.Format("{0:#,##0}", (valY_d[valY_d.Length - 1]/0.0087)*Math.Pow(2, 17.3/6.18));
                lblPopulationIndex.Text = String.Format("{0:F2}", 1000*c/GetPopulation(country));
                if (valX != null && valY != null){
                    calcX = new double[2 * valX.Length];
                    calcY = new double[2 * valX.Length];
                    for (int i = 0; i < 2 * valX.Length; i++){
                        calcX[i] = i;
                        calcY[i] = SigmoidFunction(calcX[i], a, b, c);
                        if (calcY[i] > maxY) maxY = calcY[i];
                    }
                    chart1.Series["Calculated Infection Incidents"].Points.Clear();
                    for (int i = 0; i < calcX.Length; i++){
                        chart1.Series["Calculated Infection Incidents"].Points.AddXY(calcX[i], calcY[i]);
                    }
                    chart1.Series["Observed Infected People"].Points.Clear();
                    for (int i = 0; i < valX.Length; i++){
                        if (valY[i] > 0){
                            chart1.Series["Observed Infected People"].Points.AddXY(valX[i], valY[i]);
                            chart1.Series["Observed Infected People"].Points[chart1.Series["Observed Infected People"].Points.Count - 1].ToolTip = String.Format("{0:dd/MM/yy} ({1},{2})",startDate.AddDays(valX[i]),valX[i],valY[i]);
                        }
                    }
                    chart1.Series["Deaths"].Points.Clear();
                    for (int i = 0; i < valX.Length; i++){
                        if (valY_d.Length>i && valY_d[i] > 0) chart1.Series["Deaths"].Points.AddXY(valX[i], valY_d[i]);
                    }
                    chart1.Series["Recovered"].Points.Clear();
                    for (int i = 0; i < valX.Length; i++){
                        if (valY_r.Length > i && valY_r[i] > 0) chart1.Series["Recovered"].Points.AddXY(valX[i], valY_r[i]);
                    }
                    chart1.Series["InfectedPerDay"].Points.Clear();
                    for (int i = 0; i < valX.Length; i++){
                        if (perdayY[i] > 0) chart1.Series["InfectedPerDay"].Points.AddXY(valX[i], perdayY[i]);
                    }
                    List<double> expLX = new List<double>();
                    List<double> expLY = new List<double>();
                    expX = null;
                    expY = null;
                    if (expon.Length > 0 && calcMethod.ToLower()=="python"){
                        string[] exp = expon.Split(' ');
                        if (IsNumeric(exp[0].Replace('.', ','))) exp_a = double.Parse(exp[0].Replace('.', ','));
                        if (IsNumeric(exp[1].Replace('.', ','))) exp_b = double.Parse(exp[1].Replace('.', ','));
                        if (IsNumeric(exp[2].Replace('.', ','))) exp_c = double.Parse(exp[2].Replace('.', ','));
                        foreach (double x in calcX){
                            double yval = ExponentialFunction(x, exp_a, exp_b, exp_c);
                            if (yval <= c){
                                expLX.Add(x);
                                expLY.Add(yval);
                            }
                        }
                        expX = expLX.ToArray();
                        expY = expLY.ToArray();
                    }
                    chart1.Titles[0].Text = country;
                    chart1.Series["Inflation day"].Points.Clear();
                    if (b < 1000.0){
                        chart1.Series["Inflation day"].Points.AddXY(b, 0);
                        chart1.Series["Inflation day"].Points.AddXY(b, c);
                        chart1.Series["Inflation day"].Points[1].Label = String.Format("{0:d/M/yy}", startDate.AddDays((int)b));
                        chart1.Series["Inflation day"].Points[1].LabelForeColor = Color.Red;
                    }
                    if (sol < 1000.0){
                        chart1.Series["End of Infection"].Points.Clear();
                        chart1.Series["End of Infection"].Points.AddXY(sol, 0);
                        chart1.Series["End of Infection"].Points.AddXY(sol, c);
                        chart1.Series["End of Infection"].Points[1].Label = String.Format("{0:d/M/yy}", startDate.AddDays((int)sol));
                        chart1.Series["End of Infection"].Points[1].LabelForeColor = Color.Green;
                    }
                    chart1.Series["Exponential Infection"].Points.Clear();
                    if (expX != null && expY != null){
                        for (int i = 0; i < expX.Length; i++){
                            chart1.Series["Exponential Infection"].Points.AddXY(expX[i], expY[i]);
                        }
                    }
                    double[] calcData = new double[valY.Length];
                    for(int i=0;i<valX.Length;i++)calcData[i]= SigmoidFunction(valX[i], a, b, c);
                    try{
                        lblMSE.Text = String.Format("{0:F0}", MeanStandardError(valY, calcData));
                    }catch(Exception ex) { LogEntry("Error in MSE: "+ex.Message); }
                    cbScale.SelectedIndex = 0;

                }
                else { MessageBox.Show("valX or ValY is null. No calculation is possible"); }
            //}else{
            //    MessageBox.Show("Could not get results!!!");
            //}
            Cursor = Cursors.Default;
        }


        void ExtractDataFromURLs(string country)
        {
            for(int type = 0; type < 3; type++) { //0=confirmed 1=deaths 2=recovered
                string csv = GetCSV(url[type]).Trim();
                if (csv.Length == 0) return;
                string[] lines = csv.Split(new string[] { "\n" }, StringSplitOptions.None);
                string[] dates = lines[0].Trim().Split(',');
                lstDates = new List<DateTime>();
                for (int i = 4; i < dates.Length; i++){
                    string[] compDates = dates[i].Split('/');
                    lstDates.Add(new DateTime(int.Parse(compDates[2]), int.Parse(compDates[0]), int.Parse(compDates[1])));
                }
                lstData = new List<int>[lstCountries.Length];
                bool[] isSet = new bool[lstCountries.Length];
                for (int i = 0; i < lstCountries.Length; i++) { lstData[i] = new List<int>(); isSet[i] = false; }
                for (int i = 1; i < lines.Length; i++){
                    string[] li = null;
                    string[] li1 = null;
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
                        li1 = lines[i].Trim().Split(',');
                    }
                    string[] data = li1;
                    string xora = data[1].Replace(";", ",");
                    int index1 = GetCountryIndex(xora);
                    if (country == "World"){
                        if (index1 >= 0){
                            if (isSet[0]){
                                for (int j = 4; j < data.Length; j++)
                                    if (data[j].Length > 0)
                                        lstData[0][j - 4] += int.Parse(data[j]);
                            }else{
                                for (int j = 4; j < data.Length; j++) if (data[j].Length > 0) lstData[0].Add(int.Parse(data[j]));
                                isSet[0] = true;
                            }
                        }
                    }else{
                        if (index1 >= 0){
                            if (isSet[index1]){
                                for (int j = 4; j < data.Length; j++)
                                    if (data[j].Length > 0)
                                        lstData[index1][j - 4] += int.Parse(data[j]);
                            }else{
                                for (int j = 4; j < data.Length; j++) if (data[j].Length > 0) lstData[index1].Add(int.Parse(data[j]));
                                isSet[index1] = true;
                            }
                        }
                    }
                    //else { MessageBox.Show("Cannot find " + xora); }
                }
                if (type == 0){
                    valX = new double[lstDates.Count];
                    for (int k = 0; k < lstDates.Count; k++) valX[k] = k + 21;// (lstDates[0].Date - startDate).Days;// (double)((lstDates[k]-startDate).Days);
                }
                int index = GetCountryIndex(country);
                if (type == 0){
                    valY = (lstData[index].ConvertAll(x => (double)x)).ToArray<double>();
                    perdayY = new double[valY.Length];
                    for (int k = 0; k < valY.Length; k++){
                        if (k > 0) perdayY[k] = valY[k] - valY[k - 1];
                        else perdayY[k] = 0.0;
                    }
                }
                if (type == 1) valY_d = (lstData[index].ConvertAll(x => (double)x)).ToArray<double>();
                if (type == 2) valY_r = (lstData[index].ConvertAll(x => (double)x)).ToArray<double>();
            }
            double[] nmRes = new double[] { 0.0, 0.0, 0.0 };
            while (!(nmRes[0]>0.0 && nmRes[1]>0.0 && nmRes[2] > 0.0)){
                double[][] res = NelderMead.optimizer(NelderMead.LogisticCurve, 3, valX, valY);
                //for (int j = 0; j < 4; j++) Console.WriteLine("{0}: {1} {2} {3}", j, res[j][0], res[j][1], res[j][2]);
                nmRes = res[0];
            }
            a = nmRes[0];
            b = nmRes[1];
            c = nmRes[2];
            err_a = 0.0;
            err_b = 0.0;
            err_c = 0.0;
            sol = b + 3.1 * a * Math.Log(10);
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

        int GetCountryIndex(string xora)
        {
            if (xora == "World") return 0;
            for (int i = 0; i < lstCountries.Length; i++)
                if (xora == lstCountries[i]) return i;
            return -1;
        }

        public static bool IsNumeric(object Expression)
        {
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return isNum;
        }

        private double MeanStandardError(double[] real_data, double[] calc_data)
        {
            if (real_data.Length != calc_data.Length) throw new InvalidDataException("The two arrays must have the same number of elements");
            if (real_data.Length == 0) throw new DivideByZeroException("Arrays cannot be null");
            double sum = 0.0;
            for(int i=0;i<real_data.Length;i++){
                double difference = real_data[i] - calc_data[i];
                sum += difference * difference;
            }
            double mse = sum / real_data.Length;
            Console.WriteLine("The mean squared error is {0}", mse);
            return mse;
        }

        private void chkShowPerDay_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series["InfectedPerDay"].Enabled = chkShowPerDay.Checked;
        }

        private void cbScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.UseWaitCursor=true;
            Application.DoEvents();
            if (cbScale.SelectedIndex == 0){
                chart1.ChartAreas["ChartArea1"].AxisY.Maximum = Double.NaN;
                chart1.ChartAreas["ChartArea1"].RecalculateAxesScale();
            }else{
                chart1.ChartAreas["ChartArea1"].AxisY.Maximum = double.Parse(cbScale.Items[cbScale.SelectedIndex].ToString());
                chart1.ChartAreas["ChartArea1"].RecalculateAxesScale();
            }
            Cursor = Cursors.Default;
            Application.UseWaitCursor = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }
 
        private void cbCalcMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCalcMethod.SelectedIndex == 0){
                calcMethod = "python";
                lstMethod.Visible = true;
                label15.Visible = true;
            } else { 
                calcMethod = "NelderMead";
                lstMethod.Visible = false;
                label15.Visible = false;
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            frmInfo inf = new frmInfo();
            inf.ShowDialog();
        }
    }
}
