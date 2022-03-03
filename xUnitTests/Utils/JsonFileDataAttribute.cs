using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

//idea from https://andrewlock.net/creating-a-custom-xunit-theory-test-dataattribute-to-load-data-from-json-files/

namespace xUnitTests.Utils
{
    public class JsonFileDataAttribute : DataAttribute
    {
        private readonly string _filePath;
        private readonly string? _propertyName;

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        public JsonFileDataAttribute(string filePath)
            : this(filePath, null) { }

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        /// <param name="propertyName">The name of the property on the JSON file that contains the data for the test</param>
        public JsonFileDataAttribute(string filePath, string? propertyName)
        {
            _filePath = filePath;
            _propertyName = propertyName;
        }

        /// <inheritDoc />
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            string fileData, path;
            
            try
            {
                // Get the absolute path to the JSON file
                path = Path.IsPathRooted(_filePath)
                    ? _filePath
                    : Path.Combine(Directory.GetCurrentDirectory(), _filePath);
                path = Path.GetFullPath(path);

                if (!File.Exists(path))
                {
                    throw new ArgumentException($"Could not find file at path: {path}");
                }

                // Load the file
                fileData = File.ReadAllText(_filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting JSON test parameters from {_filePath}\n{ex.Message}");
            }

            try
            {
                if (string.IsNullOrEmpty(_propertyName))
                {
                    //whole file is the data
                    var retValue = JsonConvert.DeserializeObject<List<object[]>>(fileData)!;
                    return retValue;
                }

                // Only use the specified property as the data
                var allData = JObject.Parse(fileData);
                var data = allData.SelectToken(_propertyName)!;
                return data.ToObject<List<object[]>>()!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deserializing JSON test parameters from {_filePath}\n{ex.Message}");
            }
        }
    }
}
