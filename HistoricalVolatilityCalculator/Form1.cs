using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace HistoricalVolatilityCalculator
{
    public partial class Form1 : Form
    {
        double annualStDev;
        List<Price> prices = new List<Price>();
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboAboveBelow.SelectedIndex = 0;
            dateTimePicker1.MinDate = DateTime.Now;
            dateTimePicker2.MinDate = DateTime.Now;
            reset();
        }

        private void reset()
        {
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            lbl1StDev.Text = "";
            lbl2StDev.Text = "";
            lblProbability.Text = "";
            numericUpDownTarget.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Comma-Delimited Files (*.csv) |*.csv|All Files (*.*)|*.*";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.FileName = "";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                button1.Enabled = true;
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;

                //clear labels & reset dates
                reset();

                //Clears previous rows
                dataGridView1.Rows.Clear();

                //Populates prices List
                prices = VolatilityCalculator.parseCSV(openFileDialog1.FileName, (int)numericUpDownSampSize.Value);

                //populates datagridview
                foreach (Price price in prices)
                {
                    dataGridView1.Rows.Add(price.Date, price.Close, price.Diff, price.Ln, price.Square);
                }

                //lnSum is the sum of the ln column in the dataGrid
                double lnSum = 0;

                //lnSquaredSum is the sum of the ln^2 column in the dataGrid
                double lnSquaredSum = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    lnSum += Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                    lnSquaredSum += Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
                }

                //Subtract by 1 to account for not being able to make calculations for the most recent sample
                int sampleSize = (int)numericUpDownSampSize.Value - 1;

                //calculates standard deviation of the sample
                double sampleStDev = VolatilityCalculator.calculateSampleStDev(lnSum, lnSquaredSum, sampleSize);
                
                //calculates annual standard deviation
                annualStDev = VolatilityCalculator.calculateAnnualStDev(sampleStDev, VolatilityCalculator.CalculateTimeFactor(comboBox1.SelectedIndex));

                //Populate labels
                lblMean.Text = lnSum.ToString("P", CultureInfo.InvariantCulture);
                lblSampStDev.Text = sampleStDev.ToString("P", CultureInfo.InvariantCulture);
                lblAnnStDev.Text = annualStDev.ToString("P", CultureInfo.InvariantCulture);
            }
            else
            {
                //do nothing
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //Calculate number of days between today and the target date
            DateTime today = DateTime.Now;
            DateTime pickerDate = dateTimePicker1.Value;
            TimeSpan tspan = pickerDate - today;
            double numDays = tspan.Days;

            //convert annual volatility to chosen time period
            double vol = (Math.Sqrt(numDays / 365)) * annualStDev;
            //Get current price of stock
            double currentPrice = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            //Calculate +1 Standard Deviation
            double plusOneStDev = currentPrice * (Math.Pow(Math.E, (1 * vol)));
            //Calculate -1 Standard Deviation
            double minusOneStDev = currentPrice * (Math.Pow(Math.E, (-1 * vol)));
            //Calculate +2 Standard Deviation
            double plusTwoStDev = currentPrice * (Math.Pow(Math.E, (2 * vol)));
            //Calculate -2 Standard Deviation
            double minusTwoStDev = currentPrice * (Math.Pow(Math.E, (-2 * vol)));

            //Populate labels
            lbl1StDev.Text = minusOneStDev.ToString("F") + " - " + plusOneStDev.ToString("F");
            lbl2StDev.Text = minusTwoStDev.ToString("F") + " - " + plusTwoStDev.ToString("F");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Calculate number of days between today and the target date
            DateTime today = DateTime.Now;
            DateTime pickerDate = dateTimePicker2.Value;
            TimeSpan tspan = pickerDate - today;
            double numDays = tspan.Days;
            double targetPrice = (double)numericUpDownTarget.Value;
            
            //convert annual volatility to chosen time period
            double vol = (Math.Sqrt(numDays / 365)) * annualStDev;
            
            //Get current price of stock
            double currentPrice = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            
            //Calculate z value
            double z = (Math.Log(targetPrice / currentPrice) / vol);
            if (comboAboveBelow.SelectedIndex == 0)
            {
                //Calculate probability stock is ABOVE price by target date
                double probability = (VolatilityCalculator.N(3) - VolatilityCalculator.N(z));
                lblProbability.Text = (probability * 100).ToString("F") + "%";
            }
            else
            {
                //Calculate probability stock is BELOW price by target date
                double probability = (1 - (VolatilityCalculator.N(3) - VolatilityCalculator.N(z)));
                lblProbability.Text = (probability * 100).ToString("F") + "%";
            }
        }
    }
}