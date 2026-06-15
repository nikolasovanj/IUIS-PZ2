using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Commands;
using NetworkService.Helpers.Filters;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand UndoCommand { get; set; }
        public MyICommand UndoAllCommand { get; set; }
        public MyICommand RedoCommand { get; set; }
        public MyICommand ResetFilterCommand { get; set; }
        public MyICommand SetFilterCommand { get; set; }

        private Entity currentEntity = new Entity();
        private Entity selectedEntity;
        public ObservableCollection<EntityType> Types { get; }
        public ObservableCollection<Entity> Entities { get; }
        private ObservableCollection<Entity> _filteredEntities;

        private readonly int HighLimit = 350;
        private readonly int LowLimit = 250;
        private TableFilter _filter = new TableFilter();

        public CommandStack History { get; }

        public NetworkEntitiesViewModel()
        {
            
            History = MainWindowViewModel.EntitiesHistory;
            Entities = MainWindowViewModel.Entities;
            Types = MainWindowViewModel.Types;
            FilteredEntities = Entities;
            //this.PropertyChanged += FilterChanged;
            //_filter.PropertyChanged += FilterChanged;
        }
        public NetworkEntitiesViewModel(ObservableCollection<Entity> entities)
        {
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            ResetFilterCommand = new MyICommand(OnResetFilter, CanResetFilter);
            SetFilterCommand = new MyICommand(OnSetFilter);
            UndoCommand = new MyICommand(OnUndo, () => History.CanUndo);
            UndoAllCommand = new MyICommand(OnUndoAll, () => History.CanUndo);
            RedoCommand = new MyICommand(OnRedo, () => History.CanRedo);
            History = MainWindowViewModel.EntitiesHistory;
            Types = MainWindowViewModel.Types;
            Entities = entities;
            FilteredEntities = entities;
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
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public TableFilter Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged(nameof(Filter));
                    SetFilterCommand.RaiseCanExecuteChanged();
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
                //Entities.Add(entity);
                Messenger.Default.Send(entity, MainWindowViewModel.AddToken);
                FilteredEntities = Entities;
                ApplyFilter(Filter);
                //Messenger.Default.Send<Entity>(entity, MainWindowViewModel.AddToken);
                CurrentEntity.ID = 0;
                CurrentEntity.Name = string.Empty;
                CurrentEntity.Type = null;
                var undoCmd = new MyICommand(
                    () => { Messenger.Default.Send(entity, MainWindowViewModel.AddToken); FilteredEntities = Entities; ApplyFilter(Filter); },
                    () => { Messenger.Default.Send(entity, MainWindowViewModel.RemoveToken); FilteredEntities = Entities; ApplyFilter(Filter); }
                    );
                History.AddCommand(undoCmd);
                History.History.Insert(0, $"Added ID:{entity.ID}");
                Refresh();
            }
        }
        private void OnDelete()
        {
            Entity toDelete = selectedEntity;
            //Entities.Remove(selectedEntity);
            Messenger.Default.Send(selectedEntity, MainWindowViewModel.RemoveToken);
            FilteredEntities = Entities;
            ApplyFilter(Filter);
            //Messenger.Default.Send<Entity>(selectedEntity, MainWindowViewModel.RemoveToken);
            var undoCmd = new MyICommand(
                () => { Messenger.Default.Send(toDelete, MainWindowViewModel.RemoveToken); FilteredEntities = Entities; ApplyFilter(Filter); },
                () => { Messenger.Default.Send(toDelete, MainWindowViewModel.AddToken); FilteredEntities = Entities; ApplyFilter(Filter); }
                );
            History.AddCommand(undoCmd);
            History.History.Insert(0, $"Deleted ID:{toDelete.ID}");
            Refresh();
        }
        private void OnResetFilter()
        {
            TableFilter oldFilter = new TableFilter(Filter);
            Filter.Clear();
            ApplyFilter(Filter);
            var undoCmd = new MyICommand(
                () => { Filter.Clear(); ApplyFilter(Filter); },
                () => { Filter = oldFilter; ApplyFilter(oldFilter); }
            );
            History.AddCommand(undoCmd);
            History.History.Insert(0, "Reset Filter");
            Refresh();
            ResetFilterCommand.RaiseCanExecuteChanged();
            SetFilterCommand.RaiseCanExecuteChanged();
        }
        private void OnSetFilter()
        {
            TableFilter oldFilter = new TableFilter(Filter);
            ApplyFilter(Filter);
            var undoCmd = new MyICommand(
                () => { Filter = oldFilter; ApplyFilter(oldFilter); },
                () => { Filter.Clear(); ApplyFilter(Filter); }
                );
            History.AddCommand(undoCmd);
            History.History.Insert(0, "Set Filter");
            Refresh();
            ResetFilterCommand.RaiseCanExecuteChanged();
            SetFilterCommand.RaiseCanExecuteChanged();
        }
        private bool CanDelete() { return SelectedEntity != null; }
        private bool CanResetFilter()
        {
            return FilteredEntities.Count != Entities.Count;
        }

        private bool CanSetFilter()
        {
            return Filter.ValueFilter == ValueFilter.All && Filter.ID == null && Filter.Type == null;
        }

        private void OnUndo()
        {
            History.Undo();
            History.History.Insert(0, "Undo");
            Refresh();
        }
        private void OnUndoAll()
        {
            History.UndoAll();
            History.History.Insert(0, "Undo All");
            Refresh();
        }
        private void OnRedo()
        {
            History.Redo();
            History.History.Insert(0, "Redo");
            Refresh();
        }
        private void Refresh()
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            UndoAllCommand.RaiseCanExecuteChanged();
        }
        private void ApplyFilter(TableFilter filter)
        {
            FilteredEntities = Entities;
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
