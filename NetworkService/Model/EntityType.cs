using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntityType
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public EntityType(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
