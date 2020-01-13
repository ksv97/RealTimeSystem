using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RealTimeProj
{
    public delegate void OutputEventHnd(double time, double y); 
    public class WorkingObject
    {
        public int TickCount = 0;
        private double _input;
        private object _inputLocker;

        private Timer _timer;

        public double Yprev = 0, Zprev = 0, Uprev = 0;

        public const int timerTickTime = 13;

		double h = 0.015;
		double y1 = 0.0, y1new = 0.0;
		double y2 = 0.0, y2new = 0.0;
		double y3 = 0.0, y3new = 0.0;
		double k1 = 0.0, k2 = 0.0, k3 = 0.0, k4 = 0.0;
		double delta = 0.0;

		//public const double h = 0.015;//10 мс или 1/100 сек

		public event OutputEventHnd StepCalculated;


        public WorkingObject(double input)
        {
            _inputLocker = new object();

            _input = input;
            var state = new object();
            var timerCallback = new TimerCallback(Tick);
            _timer = new Timer(timerCallback, state, 50, timerTickTime);
        }

        //На каждом тике таймера высчитываем значение, возвращаемое объектом, и отправляем его в сенсор.
        private void Tick(object obj)
        {

            var current = Diffur();
            //Ошибка:
            StepCalculated?.Invoke(current, 0);
            TickCount++;
        }
        //подсчет функции на текущем шагу.
        public double Diffur()
        {
            double input;
            lock (_inputLocker)
            {
                input = _input;
            }



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

			k1 = h * (1.45 * input - y1 - 140 * y3 - 25 * y2) / 322;
			k2 = h * (1.45 * input - y1 - 140 * (y3 + k1 / 2) - 25 * y2) / 322;
			k3 = h * (1.45 * input - y1 - 140 * (y3 + k2 / 2) - 25 * y2) / 322;
			k4 = h * (1.45 * input - y1 - 140 * (y3 + k3) - 25 * y2) / 322;
			delta = 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
			y3new = y3 + delta;

			y1 = y1new;
			y2 = y2new;
			y3 = y3new;

			////то метод Эйлера
			//var u = Uprev + h * (1.43 * input - Yprev - 43.9 * Zprev - 114.6 * Uprev) / 19.8;
   //         //var u = Uprev + h * (0.17 * input - Yprev - 21.48 * Zprev - 60.31 * Uprev) / 89.42;

   //         var z = Zprev + h * Uprev;
   //         var y = Yprev + h * Zprev;

            Yprev = y1;
            Zprev = y2;
            Uprev = y3;

            return y1;
        }

        //Когда приходит откуда-либо новое значение входа, безопасно его изменяем.
        public void OnInputChanged(double value, object sender)
        {
            lock (_inputLocker)
            {
                _input = value;
            }
        }
    }
}
