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
