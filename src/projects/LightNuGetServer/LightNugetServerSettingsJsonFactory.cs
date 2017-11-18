using System.IO;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LightNuGetServer
{
    public class LightNugetServerSettingsJsonFactory
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            }
        };

        public static LightNuGetServerSettings Create(string jsonFilePath)
        {
            Ensure.String.IsNotNullOrWhiteSpace(jsonFilePath, nameof(jsonFilePath));

            var json = File.ReadAllText(jsonFilePath);

            return JsonConvert.DeserializeObject<LightNuGetServerSettings>(json, JsonSettings);
        }
    }
}