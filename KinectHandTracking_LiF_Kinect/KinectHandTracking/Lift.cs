using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KinectHandTracking
{
    internal class Lift
    {
        private readonly double _amplitude;

        private readonly int _integrationTime;
        private readonly double _learnRate;
        private readonly double _omega;

        private int _tlif;

        private List<double[]> _values;
        private double _x0;
        private double _xlif;
        private double _ylif;

        public Lift(int integrationTimeIn, double x0In, double amplitudeIn, double learnRateIn, double omegaIn)
        {
            Debug.Assert(integrationTimeIn > 0);
            _integrationTime = integrationTimeIn;
            _x0 = x0In;
            _amplitude = amplitudeIn;
            _learnRate = learnRateIn;
            _omega = omegaIn;

            _tlif = 0;

            _values = new List<double[]>();
            double[] localFill = {0, 0, 0, 0};
            _values = Enumerable.Repeat(localFill, _integrationTime).ToList();
        }

        public double GetLifX()
        {
            _tlif++;

            _xlif = _x0 + _amplitude * Math.Cos(_omega * _tlif);

            return _xlif;
        }

        public double[] DoLifY(double obs)
        {
            _ylif = obs * _amplitude * Math.Cos(_omega * _tlif);

            double[] rowToAdd = {_tlif, _xlif, _ylif, _x0};
            _values = MatrixPush(_values, rowToAdd);

            if (_tlif > _integrationTime)
                _x0 = _x0 + _learnRate * SumOverColumn(_values, 2) / _integrationTime;

            return new[] {_tlif, _xlif, _ylif, _x0, obs};
        }

        private double SumOverColumn(List<double[]> m, int column)
        {
            double sum = 0;
            foreach (var i in m)
                sum = sum + i[column];
            return sum;
        }

        private List<double[]> MatrixPush(List<double[]> m, double[] row)
        {
            m.RemoveAt(0);
            m.Add(row);
            return m;
        }
    }
}