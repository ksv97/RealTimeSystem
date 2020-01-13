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

        public const int timerTickTime = 10;

        public const double h = (double)timerTickTime / 1000;//10 мс или 1/100 сек

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

            //то метод Эйлера
            var u = Uprev + h * (1.43 * input - Yprev - 43.9 * Zprev - 114.6 * Uprev) / 19.8;
            //var u = Uprev + h * (0.17 * input - Yprev - 21.48 * Zprev - 60.31 * Uprev) / 89.42;

            var z = Zprev + h * Uprev;
            var y = Yprev + h * Zprev;

            Yprev = y;
            Zprev = z;
            Uprev = u;

            return y;
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
