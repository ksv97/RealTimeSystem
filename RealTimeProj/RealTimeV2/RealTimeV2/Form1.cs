using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RealTimeV2
{
	public partial class Form1 : Form
	{
		static bool autoRegulation = false;
		static int regulatType = -1;
		static double goal = -1;
		static double input = 0;
		static double prevError = goal;
		static double prevRes = 0;
		static double kp = 0.9;
		static double tu = 1.0 / 19.0;
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
				if (autoRegulation)
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
					temp = input;
				}

				inputCtrl.Invoke(new Action(() => inputCtrl.Value = (decimal)input));
				

				k1 = h * y2;
				k2 = h * (y2 + k1 / 2);
				k3 = h * (y2 + k2 / 2);
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
				k2 = h * (1.45 * temp - y1 - 140 * (y3 + k1 / 2) - 25 * y2) / 322;
				k3 = h * (1.45 * temp - y1 - 140 * (y3 + k2 / 2) - 25 * y2) / 322;
				k4 = h * (1.45 * temp - y1 - 140 * (y3 + k3) - 25 * y2) / 322;
				delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
				y3new = y3 + delta;

				y1 = y1new;
				y2 = y2new;
				y3 = y3new;
				i++;
				//System.Threading.Thread.Sleep(100);

				if (i % 100 == 0)
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
			double temp = kp * curError + (kp * tu * T - kp) * prevError + prevRes;
			//double temp = (kp + kp*T*tu/2) * curError + (kp * T * tu / 2 - kp) * prevError + prevRes;
			//double temp = kp*(1+T*tu) * curError - kp * prevError + prevRes;
			prevRes = temp;
			prevError = curError;
			return temp;
		}

		public double helpBlock2(double y1)
		{
			double curError = goal - y1;
			double temp = -1.056 * prevError + 1.066 * curError + prevRes;
			prevRes = temp;
			prevError = curError;
			return temp;
		}

		private void chart1_Click(object sender, EventArgs e)
		{
			
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			i = 0;
			if (chart1.InvokeRequired) chart1.Invoke(new Action(() => mySeriesOfPoint.Points.Clear()));
			else mySeriesOfPoint.Points.Clear();

			autoRegulation = checkBox1.Checked;
			if (piRegulation.Checked)
			{
				regulatType = 1;
			}
			else if (polynomialRegulator.Checked)
			{
				regulatType = 2;
			}

			goal = (double)desiredValueCtrl.Value;


			await Task.Run(() => calculation());
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void inputCtrl_ValueChanged(object sender, EventArgs e)
		{
			input = (double)inputCtrl.Value;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			autoRegulation = checkBox1.Checked;
			piRegulation.Enabled = checkBox1.Checked;
			polynomialRegulator.Enabled = checkBox1.Checked;
			inputCtrl.Enabled = !checkBox1.Checked;
		}

		private void piRegulation_CheckedChanged(object sender, EventArgs e)
		{
			regulatType = 1;
		}

		private void polynomialRegulator_CheckedChanged(object sender, EventArgs e)
		{
			regulatType = 2;
		}

		private void desiredValueCtrl_ValueChanged(object sender, EventArgs e)
		{
			goal = (double)desiredValueCtrl.Value;
		}
	}
}
