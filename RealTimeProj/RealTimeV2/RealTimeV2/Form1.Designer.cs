namespace RealTimeV2
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.piRegulation = new System.Windows.Forms.RadioButton();
			this.polynomialRegulator = new System.Windows.Forms.RadioButton();
			this.desiredValueCtrl = new System.Windows.Forms.NumericUpDown();
			this.inputCtrl = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.desiredValueCtrl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.inputCtrl)).BeginInit();
			this.SuspendLayout();
			// 
			// chart1
			// 
			chartArea1.Name = "ChartArea1";
			this.chart1.ChartAreas.Add(chartArea1);
			this.chart1.Location = new System.Drawing.Point(12, 12);
			this.chart1.Name = "chart1";
			this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
			series1.ChartArea = "ChartArea1";
			series1.Name = "Series1";
			this.chart1.Series.Add(series1);
			this.chart1.Size = new System.Drawing.Size(514, 389);
			this.chart1.TabIndex = 0;
			this.chart1.Text = "chart1";
			this.chart1.Click += new System.EventHandler(this.chart1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(532, 163);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(660, 119);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(136, 70);
			this.button1.TabIndex = 2;
			this.button1.Text = "Start";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(531, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Input";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(669, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Desired value";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(532, 147);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Output";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(535, 65);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(48, 17);
			this.checkBox1.TabIndex = 8;
			this.checkBox1.Text = "Auto";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// piRegulation
			// 
			this.piRegulation.AutoSize = true;
			this.piRegulation.Checked = true;
			this.piRegulation.Enabled = false;
			this.piRegulation.Location = new System.Drawing.Point(536, 95);
			this.piRegulation.Name = "piRegulation";
			this.piRegulation.Size = new System.Drawing.Size(79, 17);
			this.piRegulation.TabIndex = 9;
			this.piRegulation.TabStop = true;
			this.piRegulation.Text = "PI-regulator";
			this.piRegulation.UseVisualStyleBackColor = true;
			this.piRegulation.CheckedChanged += new System.EventHandler(this.piRegulation_CheckedChanged);
			// 
			// polynomialRegulator
			// 
			this.polynomialRegulator.AutoSize = true;
			this.polynomialRegulator.Enabled = false;
			this.polynomialRegulator.Location = new System.Drawing.Point(535, 119);
			this.polynomialRegulator.Name = "polynomialRegulator";
			this.polynomialRegulator.Size = new System.Drawing.Size(119, 17);
			this.polynomialRegulator.TabIndex = 10;
			this.polynomialRegulator.Text = "Polynomial regulator";
			this.polynomialRegulator.UseVisualStyleBackColor = true;
			this.polynomialRegulator.CheckedChanged += new System.EventHandler(this.polynomialRegulator_CheckedChanged);
			// 
			// desiredValueCtrl
			// 
			this.desiredValueCtrl.Location = new System.Drawing.Point(668, 34);
			this.desiredValueCtrl.Name = "desiredValueCtrl";
			this.desiredValueCtrl.Size = new System.Drawing.Size(120, 20);
			this.desiredValueCtrl.TabIndex = 12;
			this.desiredValueCtrl.ValueChanged += new System.EventHandler(this.desiredValueCtrl_ValueChanged);
			// 
			// inputCtrl
			// 
			this.inputCtrl.DecimalPlaces = 10;
			this.inputCtrl.Location = new System.Drawing.Point(532, 34);
			this.inputCtrl.Name = "inputCtrl";
			this.inputCtrl.Size = new System.Drawing.Size(120, 20);
			this.inputCtrl.TabIndex = 11;
			this.inputCtrl.ValueChanged += new System.EventHandler(this.inputCtrl_ValueChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 413);
			this.Controls.Add(this.desiredValueCtrl);
			this.Controls.Add(this.inputCtrl);
			this.Controls.Add(this.polynomialRegulator);
			this.Controls.Add(this.piRegulation);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.chart1);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.desiredValueCtrl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.inputCtrl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.RadioButton piRegulation;
		private System.Windows.Forms.RadioButton polynomialRegulator;
		private System.Windows.Forms.NumericUpDown desiredValueCtrl;
		private System.Windows.Forms.NumericUpDown inputCtrl;
	}
}

