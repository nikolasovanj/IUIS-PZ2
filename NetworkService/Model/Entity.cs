using MVVM3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Entity : ValidationBase
    {
        private int id;
        private string name;
        private EntityType type;
        private int value;

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
            if (string.IsNullOrWhiteSpace(name)){
                ValidationErrors[nameof(Name)] = "Name is required";
            }
            if(id <= 0)
            {
                ValidationErrors[nameof(ID)] = "Id cannot be negative";
            }
        }
    }
}
