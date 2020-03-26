using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirusC
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
            cbCountry.Items.AddRange(Form1.lstCountries);
            txtRate.Text = "" + Form1.p1;
            txtMax.Text = "" + Form1.p3;
            txtPythonScript.Text = Form1.pythonScript;
            cbCountry.SelectedIndex = Form1.GetIndexByCountryName(Form1.startCountry);
            comboBox1.SelectedIndex = (Form1.calcMethod == "python" ? 0 : 1);
            txtURLconfirmed.Text = Form1.url[0];
            txtURLdeaths.Text = Form1.url[1];
            txtURLrecovered.Text = Form1.url[2];
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Form1.startCountry = cbCountry.Text;
            Form1.p1 = int.Parse(txtRate.Text);
            Form1.p3 = int.Parse(txtMax.Text);
            Form1.pythonScript = txtPythonScript.Text;
            Form1.calcMethod = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            Form1.url[0] = txtURLconfirmed.Text;
            Form1.url[1] = txtURLdeaths.Text;
            Form1.url[2] = txtURLrecovered.Text;
            SaveIniFile();
            this.Close();
        }

        void SaveIniFile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Form1.iniFile)){
                file.WriteLine("# The solving method (python or NelderMead) during the startup");
                file.WriteLine("CalcMethod={0}", comboBox1.Items[comboBox1.SelectedIndex].ToString());
                file.WriteLine("# The name of python script that downloads data and calculates the curve fit");
                file.WriteLine("PythonScript={0}", txtPythonScript.Text);
                file.WriteLine("# The name of the country with which the application starts");
                file.WriteLine("StartingCountry={0}",cbCountry.Text);
                file.WriteLine("# The initial value of infection rate to be used in optimization process");
                file.WriteLine("StartingRate={0}", txtRate.Text);
                file.WriteLine("# The initial value of infected number of people at the end of infection to be used in optimization process");
                file.WriteLine("StartingInfectedAtEnd={0}",txtMax.Text);
                file.WriteLine("# URL CSSEGISandData-COVID-19 for confirmed-infected");
                file.WriteLine("URLconfirmed={0}", txtURLconfirmed.Text);
                file.WriteLine("# URL CSSEGISandData-COVID-19 for deaths");
                file.WriteLine("URLdeaths={0}", txtURLdeaths.Text);
                file.WriteLine("# URL CSSEGISandData-COVID-19 for recovered");
                file.WriteLine("URLrecovered={0}", txtURLrecovered.Text);
            }
        }
    }
}
