using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public MyICommand AddNewEntity { get; set; }
        public MyICommand DeleteEntity { get; set; }
        public MyICommand ResetFilter { get; set; }
        private Entity currentEntity = new Entity();
        private Entity selectedEntity = new Entity();
        public ObservableCollection<EntityType> Types { get; }
        public ObservableCollection<Entity> Entities { get; }
        private ObservableCollection<Entity> _filteredEntities;
        
        private readonly int HighLimit = 350;
        private readonly int LowLimit = 250;
        private TableFilter _filter = new TableFilter();

        public NetworkEntitiesViewModel()
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            ResetFilter = new MyICommand(OnResetFilter);
            Entities = new ObservableCollection<Entity>();
            Types = MainWindowViewModel.Types;
            FilteredEntities = Entities;
            _filter.PropertyChanged += FilterChanged;
        }
        public NetworkEntitiesViewModel(ObservableCollection<Entity> entities)
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            ResetFilter = new MyICommand(OnResetFilter);
            Types = MainWindowViewModel.Types;
            Entities = entities;
            FilteredEntities = entities;
            _filter.PropertyChanged += FilterChanged;
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
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (selectedEntity != value)
                {
                    selectedEntity = value;
                    OnPropertyChanged("SelectedEntity");
                }
            }
        }
        public TableFilter Filter
        {
            get { return _filter; }
            set
            {
                if(_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged("Filter");
                }
            }
        }
        public ObservableCollection<Entity> FilteredEntities
        {
            get { return _filteredEntities; }
            set
            {
                if (_filteredEntities != value)
                {
                    _filteredEntities = value;
                    OnPropertyChanged(nameof(FilteredEntities));
                }
            }
        }

        private void OnAdd()
        {
            currentEntity.Validate();
            if (currentEntity.IsValid)
            {
                Entity entity = new Entity()
                {
                    ID = currentEntity.ID,
                    Name = currentEntity.Name,
                    Type = currentEntity.Type
                };
                Messenger.Default.Send<Entity>(entity, MainWindowViewModel.AddToken);
                CurrentEntity.ID = 0;
                CurrentEntity.Name = string.Empty;
                CurrentEntity.Type = null;
            }
        }
        private void OnDelete()
        {
            Messenger.Default.Send<Entity>(selectedEntity, MainWindowViewModel.RemoveToken);
        }
        private void OnResetFilter()
        {
            Filter.IDFilter = IDFilter.Equal;
            Filter.ID = null;
            Filter.ValueFilter = ValueFilter.All;
            Filter.Type = null;
            FilteredEntities = Entities;
        }
        private void FilterChanged(object sender, PropertyChangedEventArgs args)
        {   
            FilteredEntities = Entities;
            if(Filter.Type != null)
            {
                FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.Type.Name.Equals(Filter.Type.Name)));
            }
            if (Filter.ID != null && Filter.ID > 0 && Filter.IDFilter != null)
            { 
                Trace.WriteLine(Filter.IDFilter.ToString());
                switch(Filter.IDFilter)
                {
                    case IDFilter.Lower:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID < Filter.ID));
                        break;
                    case IDFilter.Higher:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID > Filter.ID));
                        break;
                    case IDFilter.Equal:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID == Filter.ID));
                        break;
                    
                }
            }
            if (Filter.ValueFilter != null)
            {
                switch (Filter.ValueFilter)
                {
                    case ValueFilter.OutOfBounds:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.Value >= HighLimit || e.Value <= LowLimit));
                        break;
                    case ValueFilter.InsideBounds:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.Value < HighLimit && e.Value > LowLimit));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
