using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class TableFilter : BindableBase
    {
        private EntityType _type;
        private int? _id;
        private IDFilter? _idFilter;
        private ValueFilter? _valueFilter;
        public TableFilter() 
        {
            _type = null;
            _id = null;
            _idFilter = Model.IDFilter.Equal;
            _valueFilter = Model.ValueFilter.All;
        }
        public EntityType Type 
        { 
            get { return _type; }
            set 
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            } 
        }
        public int? ID 
        {
            get { return _id; }
            set 
            {
                if (_id != value)
                { 
                    _id = value;
                    OnPropertyChanged(nameof(ID));
                }
            } 
        }
        public IDFilter? IDFilter 
        { 
            get { return  _idFilter; }
            set
            {
                if( _idFilter != value)
                {
                    _idFilter= value;
                    OnPropertyChanged(nameof(IDFilter));
                }
            }
        }
        public ValueFilter? ValueFilter 
        { 
            get { return _valueFilter; }
            set
            {
                if( value != _valueFilter)
                {
                    _valueFilter = value;
                    OnPropertyChanged(nameof(ValueFilter));
                }
            }
        }
    }
    public enum IDFilter
    {
        Lower,
        Higher,
        Equal
    }
    public enum ValueFilter
    {
        All,
        OutOfBounds,
        InsideBounds
    }
}
