using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class GraphPoint : BindableBase
    {
        private int _value;
        private double _x;
        private double _y;

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
    }
}
