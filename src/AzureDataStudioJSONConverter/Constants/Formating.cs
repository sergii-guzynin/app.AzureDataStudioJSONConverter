using System.Linq;

namespace AzureDataStudioJSONConverter.Constants
{
    public class Formating
    {
        public const string n = "\n";

        public const string t = "\t";

        public static string Tabulation(int count = 1)
        {
            var result = string.Concat(Enumerable.Range(0, count).Select(_ => t));

            return result;
        }

        public static string NewLine(int count = 1)
        {
            var result = string.Concat(Enumerable.Range(0, count).Select(_ => n));

            return result;
        }
    }
}