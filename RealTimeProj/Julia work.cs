using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        static int controlMode = -1;
        static int regulatType = -1;
        static double goal = -1;
        static double signal = -1;
        static double prevError = goal;
        static double prevRes = 0;
        static double kp = 0.9;
        static double tu = 1.0/19.0;
        static double T = 0.15;
        static int i = 0;
        Series mySeriesOfPoint;

        public Form1()
        {
            InitializeComponent();

            if (chart1.InvokeRequired) chart1.Invoke(new Action(() => chart1.ChartAreas.Add(new ChartArea("Math functions"))));
            else chart1.ChartAreas.Add(new ChartArea("Math functions"));

            mySeriesOfPoint = new Series("Sirius");
            mySeriesOfPoint.ChartType = SeriesChartType.Spline;
            mySeriesOfPoint.ChartArea = "Math functions";
            mySeriesOfPoint.BorderWidth = 3;

            if (chart1.InvokeRequired) chart1.Invoke(new Action(() => chart1.Series.Add(mySeriesOfPoint)));
            else chart1.Series.Add(mySeriesOfPoint);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            i = 0;
            if (chart1.InvokeRequired) chart1.Invoke(new Action(() => mySeriesOfPoint.Points.Clear()));
            else mySeriesOfPoint.Points.Clear();

            if (controlMode == -1 || regulatType == - 1 || goal == -1 || signal == -1)
            {
                if (chart1.InvokeRequired) chart1.Invoke(new Action(() => label3.Text = "Задайте параметры для расчетов!"));
                else label3.Text = "Задайте параметры для расчетов!";
            }         
            else
            {
                button1.Enabled = false;
                button2.Enabled = true;
                await Task.Run(() => calculation());
            }
        }

        private void calculation()
        {
            double h = 0.015;
            double y1 = 0.0, y1new = 0.0;
            double y2 = 0.0, y2new = 0.0;
            double y3 = 0.0, y3new = 0.0;
            double k1 = 0.0, k2 = 0.0, k3 = 0.0, k4 = 0.0;
            double delta = 0.0;
            double temp = 0;         

            while (i >= 0)
            {
                if (controlMode == 1)
                {
                    if (i % 10 == 0)
                    {
                        if (regulatType == 1)
                        {
                            temp = helpBlock2(y1);
                        }
                        else
                        {
                            temp = helpBlock(y1);
                        }
                    }                        
                }
                else
                {
                    temp = signal;
                }

                k1 = h * y2;
                k2 = h * (y2 + k1/2);
                k3 = h * (y2 + k2/2);
                k4 = h * (y2 + k3);
                delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
                y1new = y1 + delta;

                k1 = h * y3;
                k2 = h * (y3 + k1 / 2);
                k3 = h * (y3 + k2 / 2);
                k4 = h * (y3 + k3);
                delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
                y2new = y2 + delta;

                k1 = h * (1.45 * temp - y1 - 140 * y3 - 25 * y2) / 322;
                k2 = h * (1.45 * temp - y1 - 140 * (y3 + k1/2) - 25 * y2) / 322;
                k3 = h * (1.45 * temp - y1 - 140 * (y3 + k2/2) - 25 * y2) / 322;
                k4 = h * (1.45 * temp - y1 - 140 * (y3 + k3) - 25 * y2) / 322;
                delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
                y3new = y3 + delta;

                y1 = y1new;
                y2 = y2new;
                y3 = y3new;
                i++;

                if (i % 20 == 0)
                {
                    if (chart1.InvokeRequired) chart1.Invoke(new Action(() =>
                    {
                        if (i >= 0)
                            mySeriesOfPoint.Points.AddXY(i, y1);
                    }
                    ));
                    else if (i >= 0)
                        mySeriesOfPoint.Points.AddXY(i, y1);


                    if (chart1.InvokeRequired) chart1.Invoke(new Action(() => textBox1.Text = y1.ToString()));
                    else textBox1.Text = y1.ToString();

                    if (chart1.InvokeRequired) chart1.Invoke(new Action(() =>
                    {
                        if (i >= 0)
                            chart1.Update();
                    }
                    ));
                    else if (i >= 0)
                        chart1.Update();
                }

            }
        }

        public double helpBlock(double y1)
        {
            double curError = goal - y1;
            //double temp = 0.904  * curError - 0.896 * prevError + prevRes;
            double temp = kp * curError + (kp*tu * T - kp) * prevError + prevRes;
            //double temp = (kp + kp*T*tu/2) * curError + (kp * T * tu / 2 - kp) * prevError + prevRes;
            //double temp = kp*(1+T*tu) * curError - kp * prevError + prevRes;
            prevRes = temp;
            prevError = curError;
            return temp;
        }

        public double helpBlock2(double y1)
        {
            double curError = goal - y1;
            double temp = - 1.056 * prevError + 1.066 * curError + prevRes;
            prevRes = temp;
            prevError = curError;
            return temp;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox2.Text, out signal))
                signal = 0;

            if (!double.TryParse(textBox3.Text, out goal))
                goal = 0;

            if (radioButton1.Checked)
                controlMode = 0;
            else controlMode = 1;

            if (radioButton4.Checked)
                regulatType = 0;
            else regulatType = 1;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            i = -100;
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            textBox3.Enabled = false;
            groupBox2.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = true;
            textBox2.Enabled = false;
            groupBox2.Enabled = true;
        }
    }
}
