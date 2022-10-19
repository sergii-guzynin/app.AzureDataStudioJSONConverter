using System.Collections.Generic;

namespace AzureDataStudioJSONConverter.Models
{
    public class JSONData
    {

        public string Name { get; set; }

        public List<JSONProperty> Properties { get; set; }

        public JSONData(string name) 
        {
            Name = name;
            Properties = new List<JSONProperty>();
        }

        public bool NeedForMap() {

            foreach (JSONProperty property in Properties)
            {
                if (!property.Name.Equals(property.ClearName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;            
        }
    }
}