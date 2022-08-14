namespace TemplateFiller.Core.Models
{
    public class DataField
    {
        public string Name { get; }
        public string Description { get; }
        public string Value { get; set; }

        public DataField(string name, string description)
        {
            Name = name;
            Description = description;
            Value = string.Empty;
        }
    }
}
