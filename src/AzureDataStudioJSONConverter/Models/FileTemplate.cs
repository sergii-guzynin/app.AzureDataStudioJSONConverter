using AzureDataStudioJSONConverter.Constants;
using System.IO;
using System.Text;

namespace AzureDataStudioJSONConverter.Models
{
    public class FileTemplate
    {
        private string _directory;

        private string _fileName;

        private string _fileContent;

        public FileTemplate(string fileName, string directory, StringBuilder fileContent)
        {
            _directory = directory;
            _fileName = fileName;
            _fileContent = fileContent.ToString();
        }

        public string CreateTemplate()
        {
            
            var filePath = $@"{_directory}{_fileName}.{FileExtansions.CS}";

            var array = Encoding.Default.GetBytes(_fileContent);

            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                using var fStreamTruncate = new FileStream(filePath, FileMode.Truncate);
                fStreamTruncate.Write(array, 0, array.Length);
            }
            else
            {
                using var fStream = new FileStream(filePath, FileMode.OpenOrCreate);
                fStream.Write(array, 0, array.Length);
            }

            return filePath;
        }
    }
}