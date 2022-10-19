using System;
using System.Collections.Generic;
using System.IO;
using AzureDataStudioJSONConverter.Converters;

namespace AzureDataStudioJSONConverter
{
    class Program
    {
        private static string appDirectory;

        private static string currentSource;

        static void InitParams(string[] args) {

            appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (args.Length != 0)
            {
                currentSource = args[0].Trim();
            }
            else {
                currentSource = appDirectory;
            }
        }

        static void screenDelay() {

            Console.WriteLine("File(s) processing finished. Press any key to close ...");
            Console.ReadLine();

        } 

        static void Main(string[] args)
        {

            InitParams(args);

            if (!Directory.Exists(currentSource) && !File.Exists(currentSource))
            {
                Console.WriteLine("Please enter correct file(s) source. Processing interrupted ...");
                screenDelay();
                return;
            }
            else {
                Console.WriteLine("Processing Source: {0}", currentSource);
            }

            var Converter = new JSONFileConverter(currentSource);
            List<String> ProcessedFiles = Converter.AutoGenerateClasses();

            foreach (string fileName in ProcessedFiles)
            {
                Console.WriteLine("File processed: {0}", fileName);
            }

            screenDelay();
        }
    }
}