using NetworkService.Helpers;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public ObservableCollection<EntityByType> EntitiesByType { get; set; }
        public ObservableCollection<Entity> Entities { get; }
        public ObservableCollection<DisplayItem> Slots { get; set; }
        private Entity _draggedEntity;
        private int _draggedIndex = -1;
        private int _sourceItemIndex = -1;

        public Entity DraggedEntity
        {
            get {  return _draggedEntity; }
            set
            {
                if( _draggedEntity != value)
                {
                    _draggedEntity = value;
                    OnPropertyChanged(nameof(DraggedEntity));
                }
            }
        }

        public NetworkDisplayViewModel(ObservableCollection<Entity> entities)
        {
            Entities = entities;
            EntitiesByType = new ObservableCollection<EntityByType>(
                Entities.GroupBy(et => et.Type).Select(g => new EntityByType
                {
                    Type = g.Key,
                    Entities = new ObservableCollection<Entity>(g)
                })
            );
            Slots = MainWindowViewModel.Slots;
            foreach(var slot in Slots)
            {
                if(slot.Entity != null)
                {
                    int groupIndex = slot.Entity.Type.Name == "RTD" ? 0 : 1;
                    EntitiesByType[groupIndex].Entities.Remove(slot.Entity);
                }
            }
        }
        public NetworkDisplayViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            EntitiesByType = new ObservableCollection<EntityByType>();
        }
        public bool IsDraggingFromSlot { get { return _sourceItemIndex >= 0; } }
        public void BeginDragFromTree(Entity entity)
        {
            DraggedEntity = entity;
            _draggedIndex = FindIndexOf(entity);
            _sourceItemIndex = -1;
        }
        public void BeginDragFromSlot(int idx)
        {
            DraggedEntity = null;
            _draggedIndex = -1;
            _sourceItemIndex = idx;
        }
        public void ResetDrag()
        {
            DraggedEntity = null;
            _draggedIndex = -1;
            _sourceItemIndex = -1;
        }
        public void DropOnSlot(int idx)
        {
            if (idx == _sourceItemIndex) { ResetDrag(); return; }
            DisplayItem target = Slots[idx];
            if (_sourceItemIndex >= 0)
            {
                DisplayItem source = Slots[_sourceItemIndex];
                target.Entity = source.Entity;
                target.IsTaken = true;
                source.Clear();
            }
            else if (_draggedEntity != null)
            { 
                target.Entity = DraggedEntity;
                target.IsTaken = true;
                RemoveFromCollection(DraggedEntity);
            }
            ResetDrag();
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
           if(_draggedIndex >= 0)
           {
                EntitiesByType[groupIndex].Entities.RemoveAt(_draggedIndex);
           }
        }
        public void ReturnSlotToTree()
        {
            if(_sourceItemIndex < 0) return;
            DisplayItem item = Slots[_sourceItemIndex];
            if(!item.IsTaken) { ResetDrag(); return; }

            ReturnToCollection(item);
            item.Clear();
            ResetDrag();
        }
        private void ReturnToCollection(DisplayItem item)
        {
            int groupIndex = item.Entity.Type.Name == "RTD" ? 0 : 1;
            EntitiesByType[groupIndex].Entities.Add(item.Entity);
        }
    }
}
