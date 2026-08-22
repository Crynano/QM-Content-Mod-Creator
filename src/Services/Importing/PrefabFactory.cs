using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services.Importing
{
    internal static class PrefabFactory
    {
        private static Dictionary<string , GameObject> CachedPrefabsById = new Dictionary<string, GameObject>();
        private static IEnumerable<string> ValidFileExtensions => new[] { ".obj" };

        public static bool HasValidModelExtension(string fullPath)
        {
            return ValidFileExtensions.Any(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
        
        private static string GetModelExtension(string fullPath)
        {
            return ValidFileExtensions.FirstOrDefault(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        public static ImportOperationResult<GameObject> LoadPrefab(string assetPath, string root)
        {
            var result = new ImportOperationResult<GameObject>();

            var resolvedPath = Helper.ResolveAndValidatePath(root, assetPath);
            if (resolvedPath.HasWarnings)
            {
                result.AddWarning(resolvedPath.GetWarningsAsString());
                return result.SetResult(null);
            }

            var modelExtension = GetModelExtension(resolvedPath.Result);

            Mesh meshResult;
            switch (modelExtension)
            {
                case ".obj":
                    meshResult = ObjImporter.ImportModelFromPath(resolvedPath.Result);
                    break;
                default:
                    result.AddWarning($"Unsupported model extension for id: {assetPath}");
                    return result.SetResult(null);
            }

            var prefabFromModel = PrepareModelForGame(meshResult);

            if (prefabFromModel != null)
            {
                CachedPrefabsById[assetPath] = prefabFromModel;
                return result.SetResult(prefabFromModel);
            }
            else
            {
                result.AddWarning($"Failed to prepare model for id: {assetPath}");
                return result.SetResult(null);
            }
        }

        private static GameObject PrepareModelForGame(Mesh meshResult)
        {
            throw new NotImplementedException();
        }

        public static ImportOperationResult<GameObject> GetPrefabById(string id)
        {
            var result = new ImportOperationResult<GameObject>();
            if (CachedPrefabsById.TryGetValue(id, out GameObject prefab))
            {
                return result.SetResult(prefab);
            }
            return result;
        }
    }
}
