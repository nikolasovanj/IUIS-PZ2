using System.Collections.ObjectModel;

namespace NetworkService.Model
{
    public class EntityByType
    {
        public EntityType Type { get; set; }
        public ObservableCollection<Entity> Entities { get; set; }
        public EntityByType()
        {
            Entities = new ObservableCollection<Entity>();
        }
    }
}
