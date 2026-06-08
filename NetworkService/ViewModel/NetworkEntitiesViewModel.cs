using MVVM3.Helpers;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public MyICommand AddNewEntity {  get; set; }
        public MyICommand DeleteEntity {  get; set; }
        private Entity currentEntity = new Entity();
        private Entity selectedEntity = new Entity();
        public ObservableCollection<EntityType> Types { get; }
        public ObservableCollection<Entity> Entities { get; }
        public EntityType SelectedType { get; set; }

        public NetworkEntitiesViewModel()
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            Types = new ObservableCollection<EntityType>()
            {
                new EntityType("RTD", "/"),
                new EntityType("TermoSprega", "/")
            };
            Entities = new ObservableCollection<Entity>();
        }
        public NetworkEntitiesViewModel(ObservableCollection<Entity> entities)
        {
            AddNewEntity = new MyICommand(OnAdd);
            DeleteEntity = new MyICommand(OnDelete);
            Types = new ObservableCollection<EntityType>()
            {
                new EntityType("RTD", "/"),
                new EntityType("TermoSprega", "/")
            };
            Entities = entities;
        }

        public Entity CurrentEntity { 
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
                Messenger.Default.Send<Entity>(entity);
                CurrentEntity.ID = 0;
                CurrentEntity.Name = string.Empty;
                CurrentEntity.Type = null;
            }
        }
        private void OnDelete() 
        {
            Entities.Remove(selectedEntity);
        }
    }
}
