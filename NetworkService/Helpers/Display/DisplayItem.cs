using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.Helpers.Display
{
    public class DisplayItem : BindableBase
    {
        private Entity _entity;
        private bool _isTaken;
        private bool _isSelected;
        private ImageBrush _background;
        private ObservableCollection<Entity> _connections;
        private double _x;
        private double _y;

        public DisplayItem()
        {
            _connections = new ObservableCollection<Entity>();
        }
        public Entity Entity
        {
            get { return _entity; }
            set
            {
                if (_entity != value)
                {
                    _entity = value;
                    if (_entity != null)
                    {
                        Background = SetImage(_entity.Type.Path);
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
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public double X
        {
            get { return _x; }
            set
            {
                if (_x != value)
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
        public ImageBrush Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        public ObservableCollection<Entity> Connections
        {
            get { return _connections; }
            set
            {
                if (_connections != value)
                {
                    _connections = value;
                    OnPropertyChanged(nameof(Connections));
                }
            }
        }
        public void AddConnection(Entity entity)
        {
            Connections.Add(entity);
        }
        public void Clear()
        {
            Background = null;
            Entity = null;
            IsTaken = false;
            IsSelected = false;
            Connections.Clear();
        }
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
