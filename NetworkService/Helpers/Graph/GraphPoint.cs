using System;

namespace NetworkService.Helpers.Graph
{
    public class GraphPoint : BindableBase
    {
        private int _value;
        private double _x;
        private double _y;
        private DateTime _time;
        public GraphPoint() { }

        public int Value
        {
            get { return _value; }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        public double X
        {
            get { return _x; }
            set
            {
                if (this._x != value)
                {
                    this._x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        public double Y
        {
            get { return _y; }
            set
            {
                if (this._y != value)
                {
                    this._y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }
        public DateTime Time
        {
            get { return _time; }
            set
            {
                if (this._time != value)
                {
                    _time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }
    }
}
