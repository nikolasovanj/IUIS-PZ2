using NetworkService.Helpers;
using NetworkService.Helpers.Commands;
using NetworkService.Helpers.Display;
using NetworkService.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public MyICommand<int> MoveEntityCommand { get; set; }
        public MyICommand RemoveEntityCommand { get; set; }
        public MyICommand UndoCommand { get; set; }
        public MyICommand UndoAllCommand { get; set; }
        public MyICommand RedoCommand { get; set; }
        public MyICommand<int> ConnectCommand { get; set; }
        public ObservableCollection<EntityByType> EntitiesByType { get; set; }
        public ObservableCollection<Entity> Entities { get; }
        public CommandStack History { get; set; }
        public ObservableCollection<DisplayItem> Slots { get; set; }
        private ObservableCollection<DisplayItemConnection> connections { get; set; }
        private Entity _draggedEntity;
        private int _draggedIndex = -1;
        private int _sourceItemIndex = -1;
        private int _firstSelectedIndex = -1;
        public Entity DraggedEntity
        {
            get { return _draggedEntity; }
            set
            {
                if (_draggedEntity != value)
                {
                    _draggedEntity = value;
                    OnPropertyChanged(nameof(DraggedEntity));
                }
            }
        }

        public NetworkDisplayViewModel(ObservableCollection<Entity> entities)
        {
            Entities = entities;
            History = MainWindowViewModel.DisplayHistory;
            EntitiesByType = MainWindowViewModel.EntitiesByType;
            Slots = MainWindowViewModel.Slots;
            connections = MainWindowViewModel.Connections;
            foreach (var slot in Slots)
            {
                if (slot.Entity != null)
                {
                    int groupIndex = slot.Entity.Type.Name == "RTD" ? 0 : 1;
                    EntitiesByType[groupIndex].Entities.Remove(slot.Entity);
                }
            }
            RemoveEntityCommand = new MyICommand(OnReturnSlotToTree);
            MoveEntityCommand = new MyICommand<int>(OnDropOnSlot);
            UndoCommand = new MyICommand(OnUndo, () => History.CanUndo);
            UndoAllCommand = new MyICommand(OnUndoAll, () => History.CanUndo);
            RedoCommand = new MyICommand(OnRedo, () => History.CanRedo);
            ConnectCommand = new MyICommand<int>(OnConnectEntities);
        }
        public NetworkDisplayViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            EntitiesByType = new ObservableCollection<EntityByType>();
            connections = new ObservableCollection<DisplayItemConnection>();
            History = MainWindowViewModel.DisplayHistory;
            RemoveEntityCommand = new MyICommand(OnReturnSlotToTree);
            MoveEntityCommand = new MyICommand<int>(OnDropOnSlot);
            UndoCommand = new MyICommand(OnUndo, () => History.CanUndo);
            UndoAllCommand = new MyICommand(OnUndoAll, () => History.CanUndo);
            RedoCommand = new MyICommand(OnRedo, () => History.CanRedo);
            ConnectCommand = new MyICommand<int>(OnConnectEntities);
        }

        public ObservableCollection<DisplayItemConnection> Connections 
        { 
            get { return connections; }
            set
            {
                if (connections != value)
                {
                    connections = value;
                    OnPropertyChanged(nameof(Connections));
                }
            }
        }
        public bool IsDraggingFromSlot { get { return _sourceItemIndex >= 0; } }
        public void BeginDragFromTree(Entity entity)
        {
            DraggedEntity = entity;
            //_draggedIndex = FindIndexOf(entity);
            _sourceItemIndex = -1;
        }
        public void BeginDragFromSlot(int idx)
        {
            DraggedEntity = null;
            //_draggedIndex = -1;
            _sourceItemIndex = idx;
        }
        public void ResetDrag()
        {
            DraggedEntity = null;
            //_draggedIndex = -1;
            _sourceItemIndex = -1;
        }
        private void OnDropOnSlot(int idx)
        {
            if (idx == _sourceItemIndex) { ResetDrag(); return; }
            int savedSourceIdx = _sourceItemIndex;
            Entity savedDraggedEntity = _draggedEntity;

            Entity savedSourceEntity = null;
            if (savedSourceIdx >= 0)
            {
                savedSourceEntity = Slots[savedSourceIdx].Entity;
                History.History.Insert(0, $"Slot[{savedSourceIdx}] => Slot[{idx}] ");
            }
            else
            {
                History.History.Insert(0, $"List => Slot[{idx}]");
            }

            ApplyDrop(idx, savedSourceIdx, savedDraggedEntity);

            var undoCmd = new MyICommand(
                () => ApplyDrop(idx, savedSourceIdx, savedDraggedEntity),
                () =>
                {
                    if (savedSourceIdx >= 0)
                    {
                        DisplayItem src = Slots[savedSourceIdx];
                        DisplayItem tgt = Slots[idx];
                        List<int> connIdxs = CheckForConnection(tgt);
                        if (connIdxs.Count > 0)
                        {
                            RewireConnections(connIdxs, tgt, src);
                        }
                        tgt.Clear();
                        src.Entity = savedSourceEntity;
                        src.IsTaken = true;
                    }
                    else
                    {
                        DisplayItem tgt = Slots[idx];
                        ReturnToCollection(tgt);
                        tgt.Clear();
                    }
                });
            History.AddCommand(undoCmd);
            ResetDrag();
            Refresh();
        }
        public void ClearSlot(int idx)
        {
            DisplayItem item = Slots[idx];
            if (!item.IsTaken) return;
            ReturnToCollection(item);
            item.Clear();
        }
        private int FindIndexOf(Entity entity)
        {
            int groupIndex = entity.Type.Name == "RTD" ? 0 : 1;
            return EntitiesByType[groupIndex].Entities.IndexOf(entity);
        }
        private void RemoveFromCollection(Entity entity)
        {
            int groupIndex = entity.Type.Name == "RTD" ? 0 : 1;
            EntitiesByType[groupIndex].Entities.Remove(entity);

        }
        private void OnReturnSlotToTree()
        {
            if (_sourceItemIndex < 0) return;
            DisplayItem item = Slots[_sourceItemIndex];
            if (!item.IsTaken) { ResetDrag(); return; }

            int savedIdx = _sourceItemIndex;
            Entity savedEntity = item.Entity;
            List<int> idxs = CheckForConnection(item);
            if (idxs.Count > 0)
            {
                Disconnect(item);
            }
            ReturnToCollection(item);
            item.Clear();

            var undoCmd = new MyICommand(
                () =>
                {
                    ReturnToCollection(Slots[savedIdx]);
                    Disconnect(Slots[savedIdx]);
                    Slots[savedIdx].Clear();
                },
                () =>
                {
                    RemoveFromCollection(savedEntity);
                    Slots[savedIdx].Entity = savedEntity;
                    Slots[savedIdx].IsTaken = true;
                });
            History.AddCommand(undoCmd);
            History.History.Insert(0, $"Returned {savedEntity.ID} to list");
            Refresh();
            ResetDrag();
        }
        private void ReturnToCollection(DisplayItem item)
        {
            int groupIndex = item.Entity.Type.Name == "RTD" ? 0 : 1;
            EntitiesByType[groupIndex].Entities.Add(item.Entity);
        }
        private void ApplyDrop(int targetIdx, int sourceIdx, Entity draggedEntity)
        {
            DisplayItem target = Slots[targetIdx];
            if (sourceIdx >= 0)
            {
                DisplayItem source = Slots[sourceIdx];
                List<int> connIdxs = CheckForConnection(source);
                if (connIdxs.Count > 0)
                {
                    RewireConnections(connIdxs, source, target);
                }
                target.Entity = source.Entity;
                target.IsTaken = true;
                source.Clear();
            }
            else if (draggedEntity != null)
            {
                target.Entity = draggedEntity;
                target.IsTaken = true;
                RemoveFromCollection(draggedEntity);
            }
        }
        private void OnConnectEntities(int index)
        {
            if(_firstSelectedIndex < 0)
            {
                _firstSelectedIndex = index;
                Slots[index].IsSelected = true;
            }
            else if(_firstSelectedIndex == index)
            {
                _firstSelectedIndex = -1;
                Slots[index].IsSelected = false;
            }
            else
            {
                var conn = new DisplayItemConnection(Slots[_firstSelectedIndex], Slots[index]);
                if (Connections.Any(
                    c => (c.Item1.Entity.ID == conn.Item1.Entity.ID &&
                         c.Item2.Entity.ID == conn.Item2.Entity.ID) ||
                        (c.Item1.Entity.ID == conn.Item2.Entity.ID &&
                         c.Item2.Entity.ID == conn.Item1.Entity.ID))) return;
                Connections.Add(conn);
                Slots[_firstSelectedIndex].IsSelected = false;
                Slots[index].IsSelected = false;


                var undoCmd = new MyICommand(
                    () => Connections.Add(conn),
                    () => { Connections.Remove(conn); OnPropertyChanged(nameof(Connections)); }
                    );

                History.AddCommand(undoCmd);
                History.History.Insert(0, $"Connected {_firstSelectedIndex} to {index} ");
                Refresh();
                _firstSelectedIndex = -1;
            }
        }
        private List<int> CheckForConnection(DisplayItem item)
        {
            List<int> idxs = new List<int>();
            foreach (var conn in Connections)
            {
                if(conn.Item1.Entity.ID == item.Entity.ID || conn.Item2.Entity.ID == item.Entity.ID)
                {
                    idxs.Add(Connections.IndexOf(conn));
                }
            }
            return idxs;
        }
        private void RewireConnections(List<int> idxs, DisplayItem source, DisplayItem target)
        {
            foreach(int idx in idxs)
            {
                var conn = Connections[idx];
                if (conn.Item1.Entity.ID == source.Entity.ID)
                {
                    conn.Item1 = target;
                }
                else
                {
                    conn.Item2 = target;
                }
            }
        }
        private void Disconnect(DisplayItem item)
        {
            for(int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].Item1.Entity.ID == item.Entity.ID || Connections[i].Item2.Entity.ID == item.Entity.ID)
                {
                    Connections.RemoveAt(i);
                }
            }
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
    }
}
