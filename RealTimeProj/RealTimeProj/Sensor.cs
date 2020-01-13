using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProj
{
    public class Sensor
    {
        private double _storedValue;
        private object _locker;

        public Sensor()
        {
            _locker = new object();
            _storedValue = 0;
        }

        public void OnStepCalculated(double value, double something = 0)
        {
            lock (_locker)
            {
                _storedValue = value;
            }
        }

        public void OnStoredValueAsked(ref double value, object sender)
        {
            lock (_locker)
            {
                value = _storedValue;
            }
        }
    }
}
