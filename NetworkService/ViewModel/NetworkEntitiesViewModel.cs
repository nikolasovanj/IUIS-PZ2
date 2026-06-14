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
        public MyICommand Undo { get; set; }
        public MyICommand Redo { get; set; }
        public MyICommand ResetFilter { get; set; }
        public MyICommand SetFilter { get; set; }
        private Entity currentEntity = new Entity();
        private Entity selectedEntity = new Entity();
        public ObservableCollection<EntityType> Types { get; }
        public ObservableCollection<Entity> Entities { get; }
        private ObservableCollection<Entity> _filteredEntities;
        
        private readonly int HighLimit = 350;
        private readonly int LowLimit = 250;
        private TableFilter _filter = new TableFilter();

        private CommandStack History;

        public NetworkEntitiesViewModel()
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            ResetFilter = new MyICommand(OnResetFilter);
            SetFilter = new MyICommand(OnSetFilter);
            Undo = new MyICommand(OnUndo, () =>  History.CanUndo);
            Redo = new MyICommand(OnRedo, () =>  History.CanRedo);
            History = new CommandStack();
            Entities = MainWindowViewModel.Entities;
            Types = MainWindowViewModel.Types;
            FilteredEntities = Entities;
            //this.PropertyChanged += FilterChanged;
            //_filter.PropertyChanged += FilterChanged;
        }
        public NetworkEntitiesViewModel(ObservableCollection<Entity> entities)
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            ResetFilter = new MyICommand(OnResetFilter);
            SetFilter = new MyICommand(OnSetFilter);
            Undo = new MyICommand(OnUndo, () => History.CanUndo);
            Redo = new MyICommand(OnRedo, () => History.CanRedo);
            History = new CommandStack();
            Types = MainWindowViewModel.Types;
            Entities = entities;
            FilteredEntities = entities;
            //this.PropertyChanged += FilterChanged;
            //_filter.PropertyChanged += FilterChanged;
        }

        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                if (currentEntity != value)
                {
                    currentEntity = value;
                    OnPropertyChanged(nameof(CurrentEntity));
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
                    OnPropertyChanged(nameof(SelectedEntity));
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
                    //if(_filter != null)
                    //{
                    //    _filter.PropertyChanged -= FilterChanged;
                    //}
                    _filter = value;
                    //_filter.PropertyChanged += FilterChanged;
                    OnPropertyChanged(nameof(Filter));
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
                Entities.Add(entity);
                //Messenger.Default.Send<Entity>(entity, MainWindowViewModel.AddToken);
                CurrentEntity.ID = 0;
                CurrentEntity.Name = string.Empty;
                CurrentEntity.Type = null;
                var undoCmd = new MyICommand(
                    () => { Entities.Add(entity); },
                    () => { Entities.Remove(entity); }
                    );
                History.AddCommand(undoCmd);
                Refresh();
            }
        }
        private void OnDelete()
        {
            Entity toDelete = selectedEntity;
            Entities.Remove(selectedEntity);
            //Messenger.Default.Send<Entity>(selectedEntity, MainWindowViewModel.RemoveToken);
            var undoCmd = new MyICommand(
                () => { Entities.Remove(toDelete); },
                () => { Entities.Add(toDelete); }
                );
            History.AddCommand(undoCmd);
            Refresh();
        }
        private void OnResetFilter()
        {
            TableFilter oldFilter = new TableFilter(Filter);
            Filter.Clear();
            FilteredEntities = Entities;
            var undoCmd = new MyICommand(
                () => { Filter.Clear(); FilteredEntities = Entities; },
                () => { Filter = oldFilter; ApplyFilter(oldFilter); }    
            );
            History.AddCommand(undoCmd);
            Refresh();
        }
        private void OnSetFilter()
        {
            FilteredEntities = Entities;
            TableFilter oldFilter = new TableFilter(Filter);
            ApplyFilter(Filter);
            var undoCmd = new MyICommand(
                () => { Filter = oldFilter; ApplyFilter(oldFilter); },
                () => { Filter.Clear(); FilteredEntities = Entities; }
                );
            History.AddCommand(undoCmd);
            Refresh();
        }
        private void OnUndo()
        {
            History.Undo();
            Refresh();
        }
        private void OnRedo()
        {
            History.Redo();
            Refresh();
        }
        private void Refresh()
        {
            Undo.RaiseCanExecuteChanged();
            Redo.RaiseCanExecuteChanged();
        }
        private void ApplyFilter(TableFilter filter)
        {
            if (filter.Type != null)
            {
                FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.Type.Name.Equals(filter.Type.Name)));
            }
            if (filter.ID != null && filter.ID > 0 && filter.IDFilter != null)
            {
                switch (filter.IDFilter)
                {
                    case IDFilter.Lower:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID < filter.ID));
                        break;
                    case IDFilter.Higher:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID > filter.ID));
                        break;
                    case IDFilter.Equal:
                        FilteredEntities = new ObservableCollection<Entity>(FilteredEntities.ToList<Entity>().FindAll(e => e.ID == filter.ID));
                        break;

                }
            }
            if (filter.ValueFilter != null)
            {
                switch (filter.ValueFilter)
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
