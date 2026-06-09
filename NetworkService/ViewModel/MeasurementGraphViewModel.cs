using NetworkService.Helpers;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public ObservableCollection<Entity> Entities { get; }
        private Entity currentEntity;

        private readonly double _typeCount = 0;
        private readonly double _grapMaxHight = 450;
        private int[] _lastReadings = new int[5];

        public int PointOffset { get; } = 150;
        public int EdgeOffset { get; } = 170;
        public int PointLabelOffset { get; } = 160;



        public MeasurementGraphViewModel(ObservableCollection<Entity> entities)
        {
            Entities = entities;
            if (Entities.Count > 0)
            {
                foreach (var entity in Entities)
                {
                    if (entity.Type.Name == "RTD")
                    {
                        _typeCount++;
                    }
                }
                _typeCount /= Entities.Count;
                _typeCount *= 200;
            }
            CurrentEntity = entities[0];
            loadReadings();
            CurrentEntity.PropertyChanged += SelectionChanged;
        }

        public MeasurementGraphViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            if (Entities.Count > 0)
            {
                foreach (var entity in Entities)
                {
                    if (entity.Type.Name == "RTD")
                    {
                        _typeCount++;
                    }
                }
                _typeCount /= Entities.Count;
                _typeCount *= 200;
            }
            CurrentEntity = new Entity();
            CurrentEntity.PropertyChanged += SelectionChanged;
            PropertyChanged += SelectionChanged;
        }
        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                if (currentEntity != value)
                {
                    currentEntity = value;
                    OnPropertyChanged("CurrentEntity");
                }
            }
        }
        public double TypeCount
        {
            get { return _typeCount; }
        }
        public int[] LastReading
        {
            get { return _lastReadings; }
        }
        public void SelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "CurrentEntity")
            {
                loadReadings();
            }
        }
        private void loadReadings()
        {
            int i = 0;
            using (TextReader tr = File.OpenText("../../Data/log.txt"))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { '_', ':' });
                    if (int.Parse(parts[3]) + 1 == currentEntity.ID)
                    {
                        _lastReadings[i] = int.Parse(parts[4]);
                        i++;
                    }
                    if (i >= _lastReadings.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}
