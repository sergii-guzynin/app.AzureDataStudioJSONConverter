namespace AzureDataStudioJSONConverter.Constants
{
    public class TypeConverter
    {
        public static string ConvertJSONType(string type)
        {
            return type switch
            {
                "String" => "string",
                "Integer" => "int",
                "Date" => "DateTime",
                "Float" => "decimal",
                "Null" => "string",
                _ => GeneralTerm.DefaultTypeSignature
            };
        }
    }
}