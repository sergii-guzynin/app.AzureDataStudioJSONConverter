using AzureDataStudioJSONConverter.Constants;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AzureDataStudioJSONConverter.Models
{
    public class JSONProperty
    {
        public string Name { get; set; }

        public string ClearName { get; set; }

        public string Type { get; set; }

        public string ConvertedType { get; set; }

        public string Value { get; set; }

        public bool Nullable { get; set; }

        public string GetClearName(string name) {

            string pattern =  $@"^C\d\d\d{GeneralTerm.FieldNameSeparator}";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(name, string.Empty);          
        }

        public JSONProperty(dynamic field) 
        {
            Name = field.Name.ToString();
            Type = field.Value.Type.ToString();
            Value = field.Value.ToString();
            ClearName = GetClearName(Name);
            Nullable = Type.Contains(GeneralTerm.NullableValue, StringComparison.OrdinalIgnoreCase);
            ConvertedType = TypeConverter.ConvertJSONType(Type);
        }
    }
}