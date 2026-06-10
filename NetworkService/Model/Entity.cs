using NetworkService.Helpers;
using System;
using System.Linq;

namespace NetworkService.Model
{
    public class Entity : ValidationBase
    {
        private int id;
        private string name;
        private EntityType type;
        private int value;
        private int[] lastValues = new int[5];

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
                    AddValue(this.value);
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        public int[] LastValues { get { return lastValues; } }
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
        private void AddValue(int value)
        {
            if (lastValues.All(v => v > 0))
            {
                for (int i = 0; i < lastValues.Length - 1; i++)
                {
                    lastValues[i] = lastValues[i + 1];
                }
                lastValues[lastValues.Length-1] = value;
            }
            for (int i = 0; i < lastValues.Length; i++)
            {
                if (lastValues[i] == 0)
                {
                    lastValues[i] = value;
                    break;
                }
                 
            }   
        }
        public string ToLog()
        {
            return $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} [{Type.Name}] {name} Got value: {value}\n";
        }
    }
}
