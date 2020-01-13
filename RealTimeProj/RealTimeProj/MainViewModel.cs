using OxyPlot;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RealTimeProj
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event InputEventHandler StoredValueAsked;
        public event OutputEventHandler InputChanged;

        private double input;
        public double Input
        {
            get { return input; }
            set
            {
                input = value;
                wo.OnInputChanged(value, this);
                OnPropertyChanged(nameof(Input));
            }
        }

        private double desiredValue;
        public double DesiredValue
        {
            get { return desiredValue; }
            set
            {
                desiredValue = value;
                OnPropertyChanged(nameof(DesiredValue));
            }
        }


        private bool autoRegulated;
        public bool AutoRegulated
        {
            get { return autoRegulated; }
            set
            {
                autoRegulated = value;
                OnPropertyChanged(nameof(AutoRegulated));
            }
        }

        private bool regulator;
        public bool Regulator
        {
            get { return regulator; }
            set
            {
                regulator = value;
                OnPropertyChanged(nameof(Regulator));
            }
        }

        private bool regulatorPoli;
        public bool RegulatorPoli
        {
            get { return regulatorPoli; }
            set
            {
                regulatorPoli = value;
                OnPropertyChanged(nameof(RegulatorPoli));
            }
        }

		#region Koefficient Properties
		public double CurrentErrorKoef
		{
			get { return this.currentErrorKoef; }
			set
			{
				this.currentErrorKoef = value;
				OnPropertyChanged(nameof(CurrentErrorKoef));
			}
		}

		public double PrevError1Koef
		{
			get { return this.prevError1Koef; }
			set
			{
				this.prevError1Koef = value;
				OnPropertyChanged(nameof(PrevError1Koef));
			}
		}

		public double PrevError2Koef
		{
			get { return this.prevError2Koef; }
			set
			{
				this.prevError2Koef = value;
				OnPropertyChanged(nameof(PrevError2Koef));
			}
		}

		public double PrevError3Koef
		{
			get { return this.prevError3Koef; }
			set
			{
				this.prevError3Koef = value;
				OnPropertyChanged(nameof(PrevError3Koef));
			}
		}

		public double PrevConsumption1Koef
		{
			get { return this.prevConsumption1Koef; }
			set
			{
				this.prevConsumption1Koef = value;
				OnPropertyChanged(nameof(PrevConsumption1Koef));
			}
		}

		public double PrevConsumption2Koef
		{
			get { return this.prevConsumption2Koef; }
			set
			{
				this.prevConsumption2Koef = value;
				OnPropertyChanged(nameof(PrevConsumption2Koef));
			}
		}

		public double PrevConsumption3Koef
		{
			get { return this.prevConsumption3Koef; }
			set
			{
				this.prevConsumption3Koef = value;
				OnPropertyChanged(nameof(PrevConsumption3Koef));
			}
		}

		public double Denominator
		{
			get { return this.denominator; }
			set
			{
				this.denominator = value;
				OnPropertyChanged(nameof(Denominator));
			}
		}
		#endregion

		Sensor sensor = new Sensor();
        WorkingObject wo = new WorkingObject(1.2);
        Timer timer;

        public PlotModel PlotModel { get; private set; }

        private double _previousError=0, _previousError1=0, _previousError2=0, _previousError3=0;
        private double _previousConsumption, _previousConsumption1, _previousConsumption2, _previousConsumption3;
        private double _KP, _Tu, _T;

        private int step = 0;		

		private double currentErrorKoef = 2.75403;
		private double prevError1Koef = -8.0317597;
		private double prevError2Koef = -7.80274;
		private double prevError3Koef = -2.525015;
		private double prevConsumption1Koef = -1.2364 * Math.Pow(10, 12);
		private double prevConsumption2Koef = -9.12279 * Math.Pow(10, 12);
		private double prevConsumption3Koef = 3.241412;		
		private double denominator = -1.75403;

		/* double next = (2.75403 * currentError 
		 * - 8.0317597 * _previousError1 
		 * + 7.80274 * _previousError2 
		 * - 2.525015 * _previousError3 
		 * + (1.2364) * Math.Pow(10,13) * _previousConsumption1 
		 - 9.12279 * Math.Pow(10,12) * _previousConsumption2 
		 - 3.241412 * _previousConsumption3) / (-1.75403);
		 */

		public MainViewModel()
        {
            PlotModel = new PlotModel() { Title = "Function visualisation" };
            this.PlotModel.Series.Add(new LineSeries());

            //wo.StepCalculated += SetSeries;
            wo.StepCalculated += sensor.OnStepCalculated;

            var timerCallback = new TimerCallback(Tick);
            timer = new Timer(timerCallback, new object(), 50, 1000);

            StoredValueAsked += sensor.OnStoredValueAsked;
            InputChanged += wo.OnInputChanged;

            this.DesiredValue = 10;

            //_KP = 17.053;// 20; //1 / 20.037;
            _KP = 17;
            _Tu = 30; //2; //6.005;
            _T = 0.015; //0.05; max = 0.087

            // DEBUG PI REGULATOR
            this.RegulatorPoli = true;
            this.AutoRegulated = true;
            
        }

        public void SetSeries(double t, double y)
        {
            (this.PlotModel.Series[0] as LineSeries).Points.Add(new DataPoint(t, y));
            this.PlotModel.InvalidatePlot(true);
        }

		private double NextConsumption(double currentObjectOutput)
		{
			var currentError = desiredValue - currentObjectOutput;
			var next = _KP * currentError + (_Tu * _T - _KP) * _previousError + _previousConsumption;

			_previousError = currentError;
			_previousConsumption = next;

			return next;
		}

		private double NextConsumptionPolinom(double currentObjectOutput)
		{
			var currentError = desiredValue - currentObjectOutput;
			
			double next = (currentErrorKoef * currentError 
				+ prevError1Koef * _previousError1 
				+ prevError2Koef * _previousError2 
				+ prevError3Koef * _previousError3 
				+ prevConsumption1Koef * _previousConsumption1 
				+ prevConsumption2Koef * _previousConsumption2 
				+ prevConsumption3Koef * _previousConsumption3) / denominator;

			_previousConsumption3 = _previousConsumption2;
			_previousError3 = _previousError2;

			_previousError2 = _previousError1;
			_previousConsumption2 = _previousConsumption1;

			_previousError1 = currentError;
			_previousConsumption1 = next;


			
			//var next = _previousConsumption/100 + x * currentError - x2 * _previousError;
			//0.0000000000000389
			//double next = 0.0000000000000000389 * (currentError - (1.66062 * _previousError1) + (0.6714666 * _previousError2) - (0.0031989 * _previousError3)) - 2 * _previousConsumption1 + 2.861209 * _previousConsumption2 + 0.138791 * _previousConsumption3;
			//double next = 0.1 * (currentError - (1.66062 * _previousError1) + (0.6714666 * _previousError2) - (0.0031989 * _previousError3)) - 2 * _previousConsumption1 + 2.861209 * _previousConsumption2 + 0.138791 * _previousConsumption3;

			//double next = (currentError - 3289.6606 * _previousError + 5460.7899 * _previousError1 - 2207.3385 * _previousError2 + 10.0817 * _previousError3 + 6.4024 * _previousConsumption - 1.7135 * _previousConsumption1 + 0.0361 * _previousConsumption2)/ 4.725;
			//double next = (1.2583 * currentError - 0.6022 * _previousError1 - 0.656 * _previousError2 + 4.8317 * _previousConsumption1 - 4.6949 * _previousConsumption2 + 1.5195 * _previousConsumption3) / 1.2583;
			//double next = (-3.239 * currentError + 4.8179 * _previousError1 - 0.3047 * _previousError2 - 1.2764 * _previousError3 + 12.3652 * _previousConsumption1 - 12.01498 * _previousConsumption2 + 3.8888 * _previousConsumption3) / 4.239;
			//double next = (1.506 * currentError - 2.384 * _previousError1 + 0.4174 * _previousError2 + 0.4615 * _previousError3 - 1.473 * _previousConsumption1 + 1.4284 * _previousConsumption2 - 0.4615 * _previousConsumption3) / (-0.506);
			//double next = (166.112957 * currentError - 484.49169 * _previousError1 + 470.7262 * _previousError2 - 152.34746 * _previousError3 - 480.5629 * _previousConsumption1 + 465.951556 * _previousConsumption2 - 150.5016 * _previousConsumption3) / (-165.112957);
			//_previousError3 = _previousError2;
			//_previousConsumption2 = _previousConsumption1;

			//_previousError2 = _previousError1;
			//_previousConsumption1 = _previousConsumption;

			//_previousError1 = _previousError;

			//_previousError = currentError;
			//_previousConsumption = next;

			return next;
		}
		
        private void Tick(object obj)
        {
            double objOutput = 0;
            StoredValueAsked?.Invoke(ref objOutput, this);

            SetSeries(step, objOutput);
            //_tbOutput.Invoke(new Action(() =>
            //{
            //    _tbOutput.Text = objOutput.ToString();
            //}));
            if (autoRegulated)
            {
                if (regulator)
                {
                    var nextCons = NextConsumption(objOutput);
                    Input = nextCons;
                }
                else if(regulatorPoli)
                {
                    var nextCons = NextConsumptionPolinom(objOutput);
                    Input = nextCons;
                }
            }
            else
            {
                EmptyStep(objOutput);
            }

            step++;
        }

		private CommandHandler addCommand;
		public CommandHandler AddCommand { get { return addCommand ?? (addCommand = new CommandHandler(obj => { Input++; })); } }

		private CommandHandler addDesiredValue;
		public CommandHandler AddDesiredValue { get { return addDesiredValue ?? (addDesiredValue = new CommandHandler(obj => { DesiredValue++; })); } }

		private CommandHandler decreaseDesiredValue;
		public CommandHandler DecreaseDesiredValue { get { return decreaseDesiredValue ?? (decreaseDesiredValue = new CommandHandler(obj => { DesiredValue--; })); } }

		private CommandHandler decreaseCommand;
		public CommandHandler DecreaseCommand { get { return decreaseCommand ?? (decreaseCommand = new CommandHandler(obj => { Input--; })); } }

		private CommandHandler restartCommand;
		public CommandHandler RestartCommand { get { return restartCommand ?? (restartCommand = new CommandHandler(obj => this.Restart())); } }

		public event PropertyChangedEventHandler PropertyChanged;

		

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public void Restart()
		{
			this.Input = 0;

			sensor = new Sensor();
			wo = new WorkingObject(1.2);
			PlotModel = new PlotModel() { Title = "Function visualisation" };
			PlotModel.InvalidatePlot(true);
			this.PlotModel.Series.Add(new LineSeries());

			//wo.StepCalculated += SetSeries;
			wo.StepCalculated += sensor.OnStepCalculated;

			var timerCallback = new TimerCallback(Tick);
			timer.Dispose();
			timer = new Timer(timerCallback, new object(), 50, 1000);

			StoredValueAsked += sensor.OnStoredValueAsked;
			InputChanged += wo.OnInputChanged;
		}

		private void EmptyStep(double currentObjectOutput)
        {
            _previousError = desiredValue - currentObjectOutput;
            _previousConsumption = Input;
        }
    }
}
