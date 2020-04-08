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
    public partial class tmpForm : Form
    {
        string[] lstCountries = Form1.lstCountries;
        string[] countries;
        bool[] show;
        int colorCount = 0;

        public tmpForm(string[] cntr, bool[] show)
        {
            InitializeComponent();
            this.countries = cntr;
            this.show = show;
            foreach (string cnt in lstCountries){
                listBox1.Items.Add(cnt);
            }
            colorCount = countries.Length;
            lblColorCount.Text = ""+colorCount;
            for(int i=0;i<countries.Length;i++){
                checkedListBox1.Items.Add(countries[i]);
                checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, show[i]);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0){//we cannot add world
                if (!checkedListBox1.Items.Contains(listBox1.Items[listBox1.SelectedIndex])){
                    if (colorCount < 16){
                        checkedListBox1.Items.Add(listBox1.Items[listBox1.SelectedIndex]);
                        colorCount++;
                        lblColorCount.Text = "" + colorCount;
                    }else MessageBox.Show("Already selected 16 countries. You cannot add more...");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (colorCount > 0){
                checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
                colorCount--;
                lblColorCount.Text = "" + colorCount;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try{
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("countries.txt")){
                    for (int i = 0; i < checkedListBox1.Items.Count; i++){
                        file.WriteLine("{0};{1}", checkedListBox1.Items[i].ToString(), (checkedListBox1.GetItemChecked(i) ? 1 : 0));
                    }
                }
            }catch(Exception ex) { 
                MessageBox.Show("Error in saving 'countries.txt' file: " + ex.Message); 
            }
            this.Close();
        }
    }
}
