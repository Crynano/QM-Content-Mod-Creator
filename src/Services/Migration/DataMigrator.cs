using QM_ImporterAPI.Templates;
using System;

namespace QM_ImporterAPI.Services.Migration
{
    public class DataMigrator
    {
        public ImportableJson MigrateJson(string jsonData, Type objectType)
        {
            var importableJson = new ImportableJson
            {
                RecordType = objectType.FullName.ToString(),
                Data = jsonData
            };
            return importableJson;
        }
    }
}