using AzureDataStudioJSONConverter.Constants;
using AzureDataStudioJSONConverter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AzureDataStudioJSONConverter.Converters
{
    public class JSONFileConverter
    {

        private readonly string _directory;        

        private List<string> FoundFiles { get; set; }

        private List<string> ProcessedFiles { get; set; }

        private List<JSONData> DataStructureList { get; set; }

        public JSONFileConverter(string fileSource)
        {
            FoundFiles = new List<string>();
            ProcessedFiles = new List<string>();
            DataStructureList = new List<JSONData>();

            FileAttributes attr = File.GetAttributes(fileSource);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                _directory = fileSource;
                FindFiles();
            }
            else
            {
                _directory = new FileInfo(fileSource).Directory.FullName;
                FoundFiles.Add(Path.GetFullPath(fileSource));                
            }

            if (!_directory.EndsWith(GeneralTerm.DirectoryEnding))
            {
                _directory += GeneralTerm.DirectoryEnding;
            }

            AnalyzeFiles();
        }

        private string GetModelName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private void FindFiles() {

            var Files = Directory.EnumerateFiles(_directory, $"*.{FileExtansions.JSON}", SearchOption.AllDirectories);

            foreach (string FileName in Files)
            {
                FoundFiles.Add(FileName);
            }
        }

        private void LoadFileStructure(string fileName) {

            using (StreamReader reader = new StreamReader(fileName))
            {
                string ModelName = GetModelName(fileName);
                JSONData CurrentStructure = new JSONData(ModelName);
                string json = reader.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                if (array.Count == 0)
                {
                    return;
                }

                foreach (var field in array[0]) 
                {
                    CurrentStructure.Properties.Add(new JSONProperty(field));
                }

                if (CurrentStructure.Properties.Count() != 0)
                {
                    DataStructureList.Add(CurrentStructure);
                } 
            }  
        }

        private void AnalyzeFiles() {

            foreach (string FoundFileName in FoundFiles)
            {
                try
                {
                    LoadFileStructure(FoundFileName);
                }
                catch (Exception)
                {
                    Console.WriteLine("File will be ignored: {0}", FoundFileName);
                }
            }
        }

        private void GenerateModel(JSONData jsonStructure) 
        {

            var fieldsText = new StringBuilder();
            const string getSet = "{ get; set; }" + Formating.n;
            var nameSpace = $"{GeneralTerm.DefaultApplicationName}.{GeneralTerm.Models}";

            var namespaces = new List<string>()
            {
                Namespace.System,
            };

            foreach (var field in jsonStructure.Properties)
            {          
                fieldsText.Append($"{Formating.n}{Formating.Tabulation(2)}{Incapsulation.Public} {field.ConvertedType} {field.ClearName} {getSet}");
            }

            var text = new StringBuilder();
                        
            text.Append(string.Join(Formating.n, namespaces) + Formating.n + Formating.n);
            text.Append($"{GeneralTerm.Namespace} {nameSpace}{Formating.n}");
            text.Append("{" + Formating.n);
            text.Append($"{Formating.t}{Incapsulation.Public} {GeneralTerm.Class} {jsonStructure.Name}{Formating.n}");
            text.Append(Formating.t + "{");
            text.Append(fieldsText);
            text.Append(Formating.t + "}" + Formating.n + "}");

            var modelTemplate = new FileTemplate(jsonStructure.Name, _directory, text);
            ProcessedFiles.Add(modelTemplate.CreateTemplate());     
            
        }

        private void GenerateMap(JSONData jsonStructure) 
        {

            var fieldsText = new StringBuilder();

            var nameSpaceModel = $"{GeneralTerm.DefaultApplicationName}.{GeneralTerm.Models}";
            var nameSpaceMaping = $"{GeneralTerm.DefaultApplicationName}.{GeneralTerm.DAL}.{GeneralTerm.Mapping}";
            var nameMap = $"{jsonStructure.Name}{GeneralTerm.Map}";

            var namespaces = new List<string>()
            {
                Namespace.DapperMapper,
                $"using {nameSpaceModel};"
            };

            foreach (var field in jsonStructure.Properties)
            {
                fieldsText.Append($"{Formating.n}{Formating.Tabulation(3)}Map(item => item.{field.ClearName}).ToColumn(\"{field.Name}\", false);");
            }

            var text = new StringBuilder();

            text.Append(string.Join(Formating.n, namespaces) + Formating.n + Formating.n);
            text.Append($"{GeneralTerm.Namespace} {nameSpaceMaping}{Formating.n}");
            text.Append("{" + Formating.n);
            text.Append($"{Formating.t}{Incapsulation.Public} {GeneralTerm.Class} {nameMap} : EntityMap<{jsonStructure.Name}>{Formating.n}");
            text.Append(Formating.t + "{" + Formating.n);
            text.Append($"{Formating.Tabulation(2)}{Incapsulation.Public} {nameMap}(){Formating.n}");
            text.Append(Formating.Tabulation(2) + "{");
            text.Append(fieldsText);
            text.Append(Formating.n + Formating.Tabulation(2) + "}" + Formating.n + Formating.Tabulation(1) + "}");
            text.Append(Formating.n + "}");

            var modelTemplate = new FileTemplate(nameMap, _directory, text);
            ProcessedFiles.Add(modelTemplate.CreateTemplate());

        }

        private void GenarateClasses() 
        {

            foreach (JSONData CurrentStructure in DataStructureList)
            {
                GenerateModel(CurrentStructure);
                if (CurrentStructure.NeedForMap())
                {
                    GenerateMap(CurrentStructure);
                }                                
            }            
        }

        public List<String> AutoGenerateClasses() {

            GenarateClasses();
            return ProcessedFiles;
        }
    }
}