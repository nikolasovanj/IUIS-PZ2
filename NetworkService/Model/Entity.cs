using NetworkService.Helpers;

namespace NetworkService.Model
{
    public class Entity : ValidationBase
    {
        private int id;
        private string name;
        private EntityType type;
        private int value;

        public Entity()
        {
        }

        public int ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public EntityType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(this.name))
            {
                this.ValidationErrors["Name"] = "Name is required";
            }
            if (this.id <= 0)
            {
                this.ValidationErrors["ID"] = "Id cannot be negative";
            }
            if (this.type == null)
            {
                this.ValidationErrors["Type"] = "Type is required";
            }
        }
    }
}
