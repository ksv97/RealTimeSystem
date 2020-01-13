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
        WorkingObject wo = new WorkingObject(1.2);
        Timer timer;

        public PlotModel PlotModel { get; private set; }

        private double _previousError=0, _previousError1=0, _previousError2=0, _previousError3=0;
        private double _previousConsumption, _previousConsumption1, _previousConsumption2, _previousConsumption3;
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

            //var x = 6;
            //var x2 = 0.02;

            //var next = _previousConsumption/100 + x * currentError - x2 * _previousError;
            //0.0000000000000389
            //double next = 0.0000000000000000389 * (currentError - (1.66062 * _previousError1) + (0.6714666 * _previousError2) - (0.0031989 * _previousError3)) - 2 * _previousConsumption1 + 2.861209 * _previousConsumption2 + 0.138791 * _previousConsumption3;
            //double next = 0.1 * (currentError - (1.66062 * _previousError1) + (0.6714666 * _previousError2) - (0.0031989 * _previousError3)) - 2 * _previousConsumption1 + 2.861209 * _previousConsumption2 + 0.138791 * _previousConsumption3;

            //double next = (currentError - 3289.6606 * _previousError + 5460.7899 * _previousError1 - 2207.3385 * _previousError2 + 10.0817 * _previousError3 + 6.4024 * _previousConsumption - 1.7135 * _previousConsumption1 + 0.0361 * _previousConsumption2)/ 4.725;
            //double next = (1.2583 * currentError - 0.6022 * _previousError1 - 0.656 * _previousError2 + 4.8317 * _previousConsumption1 - 4.6949 * _previousConsumption2 + 1.5195 * _previousConsumption3) / 1.2583;
            //double next = (-3.239 * currentError + 4.8179 * _previousError1 - 0.3047 * _previousError2 - 1.2764 * _previousError3 + 12.3652 * _previousConsumption1 - 12.01498 * _previousConsumption2 + 3.8888 * _previousConsumption3) / 4.239;
            //double next = (1.506 * currentError - 2.384 * _previousError1 + 0.4174 * _previousError2 + 0.4615 * _previousError3 - 1.473 * _previousConsumption1 + 1.4284 * _previousConsumption2 - 0.4615 * _previousConsumption3) / (-0.506);
            //double next = (166.112957 * currentError - 484.49169 * _previousError1 + 470.7262 * _previousError2 - 152.34746 * _previousError3 - 480.5629 * _previousConsumption1 + 465.951556 * _previousConsumption2 - 150.5016 * _previousConsumption3) / (-165.112957);
            double next = ((-0.249) * currentError + 0.72624 * _previousError1 - 0.7056 * _previousError2 + 0.228366 * _previousError3 + 3.63523 * _previousConsumption1 - 3.524699 * _previousConsumption2 + 1.138472 * _previousConsumption3) / 1.249;

            _previousConsumption3 = _previousConsumption2;
            _previousError3 = _previousError2;

            _previousError2 = _previousError1;
            _previousConsumption2 = _previousConsumption1;

            _previousError1 = currentError;
            _previousConsumption1 = next;
            //_previousError3 = _previousError2;
            //_previousConsumption2 = _previousConsumption1;

            //_previousError2 = _previousError1;
            //_previousConsumption1 = _previousConsumption;

            //_previousError1 = _previousError;

            //_previousError = currentError;
            //_previousConsumption = next;

            return next;
        }

        private void EmptyStep(double currentObjectOutput)
        {
            _previousError = desiredValue - currentObjectOutput;
            _previousConsumption = Input;
        }
    }
}
