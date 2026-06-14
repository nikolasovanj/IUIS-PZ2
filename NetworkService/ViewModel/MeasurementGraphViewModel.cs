using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public ObservableCollection<Entity> Entities { get; }
        private Entity currentEntity;

        private readonly double _typeCount = 0;

        private ObservableCollection<GraphPoint> _points;
        private ObservableCollection<GraphEdge> _edges;


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
            }
            CurrentEntity = entities[0];
            CurrentEntity.PropertyChanged += ValueChanged;
            PropertyChanged += EntityChanged;
            PropertyChanged += ValueChanged;
            InitializePoints();
            LoadReading(currentEntity.LastValues, currentEntity.LastTimeStamps);
            InitializeEdges();
            CreateEdges();
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
            }
            CurrentEntity = new Entity();
            CurrentEntity.PropertyChanged += ValueChanged;
            PropertyChanged += EntityChanged;
            PropertyChanged += ValueChanged;
            InitializePoints();
            LoadReading(currentEntity.LastValues, currentEntity.LastTimeStamps);
            InitializeEdges();
            CreateEdges();

        }



        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                if (currentEntity != value)
                {
                    if(CurrentEntity != null)
                    {
                        CurrentEntity.PropertyChanged -= ValueChanged;
                    }
                    currentEntity = value;
                    OnPropertyChanged(nameof(CurrentEntity));
                }
            }
        }
        public double TypeCount
        {
            get { return _typeCount; }
        }
        public ObservableCollection<GraphPoint> Points
        {
            get { return _points; }
            set
            {
                if (value != _points)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }
        public ObservableCollection<GraphEdge> Edges
        { 
            get { return _edges; }
            set
            {
                if(value != _edges)
                {
                    _edges = value;
                    OnPropertyChanged(nameof(Edges));
                }
            }
        }
        private void EntityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentEntity) && CurrentEntity != null)
            {
                CurrentEntity.PropertyChanged += ValueChanged;
            }
        }
        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Entity.Value) || e.PropertyName == nameof(Entity.TimeStamp) || e.PropertyName == nameof(CurrentEntity))
            {
                LoadReading(currentEntity.LastValues, currentEntity.LastTimeStamps);
            }
        }
        private void InitializePoints()
        {
            Points = new ObservableCollection<GraphPoint>
            {
                new GraphPoint(),
                new GraphPoint(),
                new GraphPoint(),
                new GraphPoint(),
                new GraphPoint()
            };
        }
        private void LoadReading(int[] readings, DateTime[] times)
        {
            if(readings.Length > 0 && times.Length > 0)
            {
                for (int i = 0; i < readings.Length; i++)
                {
                    Points[i].Value = readings[i]; 
                    Points[i].X = i * 100 + 20;
                    Points[i].Y = 250 - ((readings[i] - 150) * 5 / 6);
                    Points[i].Time = times[i];
                }
            }
        }
        private void InitializeEdges()
        {
            Edges = new ObservableCollection<GraphEdge>
            {
                new GraphEdge(),
                new GraphEdge(),
                new GraphEdge(),
                new GraphEdge()
            };
        }
        private void CreateEdges()
        {
            if(Points.Count > 1)
            {
                for(int i = 0; i < Points.Count-1; i++)
                {
                    Edges[i].Point1 = Points[i];
                    Edges[i].Point2 = Points[i+1];
                }
            }
        }
    }
}
