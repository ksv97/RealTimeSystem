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
		public enum RegulationType
		{
			PI_REGULATION, POLYNOMIAL_REGULATION
		}

		public bool autoRegulation = false;
		public RegulationType regulationType;
		public double DesiredValue = -1;
		public double input = 0;
		public double prevError = -1;
		public double prevRes = 0;
		public double kp = 0.9;
		public double tu = 1.0 / 19.0;
		public double T = 0.15;
		public int i = 0;
		Series mySeriesOfPoint;

		double h = 0.015;
		double y1 = 0.0, y1new = 0.0;
		double y2 = 0.0, y2new = 0.0;
		double y3 = 0.0, y3new = 0.0;
		double k1 = 0.0, k2 = 0.0, k3 = 0.0, k4 = 0.0;
		double delta = 0.0;
		double next = 0;

		public Form1()
		{
			InitializeComponent();

			chart1.ChartAreas.Add(new ChartArea("Test"));

			mySeriesOfPoint = new Series("Test");
			mySeriesOfPoint.ChartType = SeriesChartType.Spline;			
			mySeriesOfPoint.BorderWidth = 4;

			chart1.Series.Add(mySeriesOfPoint);
		}

		private void Calculate()
		{
			while (i >= 0)
			{
				if (autoRegulation)
				{
					if (i % 10 == 0)
					{
						if (regulationType == RegulationType.PI_REGULATION)
						{
							next = NextConsumptionPi(y1);
						}
						else if (regulationType == RegulationType.POLYNOMIAL_REGULATION)
						{
							next = NextConsumptionPolynomial(y1);
						}
						else throw new Exception("unknown regulation type!");
					}
				}
				else
				{
					next = EmptyStep();
				}

				inputCtrl.Invoke(new Action(() => inputCtrl.Value = (decimal)input));

				RungeKutt();				
				UpdateUi();
				i++;
			}
		}		

		private double EmptySteр() 
		{
			return input;
		}

		private void RungeKutt()
		{
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

			k1 = h * (1.45 * next - y1 - 140 * y3 - 25 * y2) / 322;
			k2 = h * (1.45 * next - y1 - 140 * (y3 + k1 / 2) - 25 * y2) / 322;
			k3 = h * (1.45 * next - y1 - 140 * (y3 + k2 / 2) - 25 * y2) / 322;
			k4 = h * (1.45 * next - y1 - 140 * (y3 + k3) - 25 * y2) / 322;
			delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
			y3new = y3 + delta;

			y1 = y1new;
			y2 = y2new;
			y3 = y3new;
		}

		private void UpdateUi()
		{
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

		public double NextConsumptionPolynomial(double y1)
		{
			double curError = DesiredValue - y1;
			double next = kp * curError + (kp * tu * T - kp) * prevError + prevRes;
			prevRes = next;
			prevError = curError;
			return next;
		}

		public double NextConsumptionPi(double y1)
		{
			double curError = DesiredValue - y1;
			double next = -1.056 * prevError + 1.066 * curError + prevRes;
			prevRes = next;
			prevError = curError;
			return next;
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
				regulationType = RegulationType.PI_REGULATION;
			}
			else if (polynomialRegulator.Checked)
			{
				regulationType = RegulationType.POLYNOMIAL_REGULATION;
			}

			DesiredValue = (double)desiredValueCtrl.Value;


			await Task.Run(() => Calculate());
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
			regulationType = RegulationType.PI_REGULATION;
		}

		private void polynomialRegulator_CheckedChanged(object sender, EventArgs e)
		{
			regulationType = RegulationType.POLYNOMIAL_REGULATION;
		}

		private void desiredValueCtrl_ValueChanged(object sender, EventArgs e)
		{
			DesiredValue = (double)desiredValueCtrl.Value;
		}

																																																																				private double EmptyStep()
																																																																				{
																																																																					return input * 0.986;
																																																																				}
	}
}
