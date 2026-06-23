using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Templates;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for localization files.
    /// </summary>
    public class LocalizationLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(LocalizationLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var localizationFiles = FilterByType<LocalizationTemplate>(deserializedObjects);

            LogLoadStart(localizationFiles.Count());

            foreach (var locFile in localizationFiles)
            {
                QuasimorphHelper.AddLocalization(locFile);
            }

            return operationResult;
        }
    }
}
