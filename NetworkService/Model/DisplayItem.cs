using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.Model
{
    public class DisplayItem : BindableBase
    {
        private Entity _entity;
        private bool _isTaken;
        private bool _isConnected;
        private ImageBrush _backround;
        private double _x;
        private double _y;

        public DisplayItem()
        {
        }
        public Entity Entity
        {
            get { return _entity; }
            set
            {
                if (_entity != value)
                {
                    _entity = value;
                    if (value != null)
                    {
                        Backround = SetImage(_entity.Type.Path);
                    }
                    OnPropertyChanged(nameof(Entity));
                }
            }
        }
        
        public bool IsTaken
        {
            get { return _isTaken; }
            set
            {
                if (value != _isTaken)
                {
                    _isTaken = value;
                    OnPropertyChanged(nameof(IsTaken));
                }
            }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if(value != _isConnected)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }
        public double X
        {
            get { return _x; }
            set
            {
                if(_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        public double Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }
        public ImageBrush Backround
        {
            get { return _backround; }
            set
            {
                if(_backround != value)
                {
                    _backround = value;
                    OnPropertyChanged(nameof(Backround));
                }
            }
        }
        public void Clear()
        {
            Backround = null;
            Entity = null;
            IsTaken = false;
            IsConnected = false;
        }
        //private void CheckValue(object sender, PropertyChangedEventArgs e)
        //{
        //    if(e.PropertyName == nameof(Entity.Value))
        //    {
        //        ValueToImage();
        //    }
        //}
        //private void ValueToImage()
        //{
        //    if (Entity.Value > 350 || Entity.Value < 250)
        //    {
        //        Backround = SetImage("../../Data/Images/Warning.png");
        //    }
        //    else
        //    {
        //        Backround = SetImage(Entity.Type.Path);
        //    }
        //}
        private ImageBrush SetImage(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bmp.EndInit();
            return new ImageBrush(bmp);
        }
    }
}
