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

        Sensor sensor = new Sensor();
        WorkingObject wo = new WorkingObject(0);
        Timer timer;

        public PlotModel PlotModel { get; private set; }

		private double _previousError = 0;
		private double _previousConsumption;
        private double _KP, _Tu, _T;

        private int step = 0;		

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

            this.DesiredValue = 2;

            //_KP = 17.053;// 20; //1 / 20.037;
            _KP = 0.9;
            _Tu = 1.0/19.0; //2; //6.005;
            _T = 0.15; //0.05; max = 0.087

            // DEBUG PI REGULATOR
            this.Regulator = true;
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
			var next = _KP * currentError + (_KP * _Tu * _T - _KP) * _previousError + _previousConsumption;

			_previousError = currentError;
			_previousConsumption = next;

			return next;
		}

		private double NextConsumptionPolinom(double currentObjectOutput)
		{
			var currentError = desiredValue - currentObjectOutput;
			double next = -1.056 * _previousError + 1.066 * currentError + _previousConsumption;

			_previousConsumption = next;
			_previousError = currentError;

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

		public event PropertyChangedEventHandler PropertyChanged;

		public CommandHandler DecreaseCommand { get { return decreaseCommand ?? (decreaseCommand = new CommandHandler(obj => { Input--; })); } }

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}


		private void EmptyStep(double currentObjectOutput)
        {
            _previousError = desiredValue - currentObjectOutput;
            _previousConsumption = Input;
        }
    }
}
