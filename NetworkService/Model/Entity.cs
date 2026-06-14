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
        private DateTime timeStamp;
        private int[] lastValues = new int[5];
        private DateTime[] lastTimeStamps = new DateTime[5];

        public Entity()
        {
        }
        public Entity(Entity entity)
        {
            id = entity.ID;
            name = entity.Name;
            type = entity.Type;
            value = entity.Value;
            timeStamp = entity.TimeStamp;
            lastValues = entity.LastValues;
            lastTimeStamps = entity.LastTimeStamps;
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
                this.value = value;
                AddValue(this.value);
                OnPropertyChanged(nameof(Value));
                
            }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set
            {
                if(timeStamp != value)
                {
                    timeStamp = value;
                    AddTimeStamp(value);
                    OnPropertyChanged(nameof(TimeStamp));
                }
            }
        }
        public int[] LastValues { get { return lastValues; } }
        public DateTime[] LastTimeStamps { get { return lastTimeStamps; } }
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
                lastValues[lastValues.Length - 1] = value;
            }
            else
            {

                for (int i = 0; i < lastValues.Length; i++)
                {
                    if (lastValues[i] == 0)
                    {
                        lastValues[i] = value;
                        break;
                    }

                }   
            }
        }
        private void AddTimeStamp(DateTime timeStamp)
        {
            if (lastTimeStamps.All(v => v != null))
            {
                for (int i = 0; i < lastTimeStamps.Length - 1; i++)
                {
                    lastTimeStamps[i] = lastTimeStamps[i + 1];
                }
                lastTimeStamps[lastTimeStamps.Length - 1] = timeStamp;
            }
            else
            {

                for (int i = 0; i < lastTimeStamps.Length; i++)
                {
                    if (lastTimeStamps[i] == null)
                    {
                        lastTimeStamps[i] = timeStamp;
                        break;
                    }

                }
            }
        }
        public string ToLog()
        {
            return $"{TimeStamp.ToString("dd/MM/yyyy HH:mm:ss")} [{Type.Name}] {name} Got value: {value}\n";
        }
    }
}
