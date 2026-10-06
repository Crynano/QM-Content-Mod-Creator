using QM_ImporterAPI.Services.ErrorManagement;
using System;
using System.Collections.Generic;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class WoundLoader : BaseItemLoader
    {
        protected override string LoaderName => throw new NotImplementedException();

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var result = new ImportOperationResult();



            return result;
        }
    }
}
