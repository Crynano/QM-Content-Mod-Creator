using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services.Importing
{
    internal static class PrefabFactory
    {
        private static IEnumerable<string> ValidFileExtensions => new[] { ".obj" };
        private static Transform ROOT_FOR_PREFABS;

        public static bool HasValidModelExtension(string fullPath)
        {
            return ValidFileExtensions.Any(ext => Path.GetExtension(fullPath).Equals(ext, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetModelExtension(string fullPath)
        {
            return Path.GetExtension(fullPath).ToLowerInvariant();
        }

        public static ImportOperationResult<GameObject> LoadPrefab(string assetPath, string root)
        {
            var result = new ImportOperationResult<GameObject>();

            var resolvedPath = Helper.ResolveAndValidatePath(root, assetPath);
            result.Absorb(resolvedPath);
            if (!resolvedPath.IsSuccess)
            {
                return result.SetResult(null);
            }

            var finalPath = resolvedPath.Result;
            if (!HasValidModelExtension(finalPath))
            {
                result.AddWarning($"Unsupported model extension for id: {assetPath}");
                return result.SetResult(null);
            }

            Logger.LogDebug($"Loading model from path: {finalPath}");
            var modelExtension = GetModelExtension(finalPath);

            Mesh meshResult;
            switch (modelExtension)
            {
                case ".obj":
                    var import = MeshImporter.ImportMeshFromObj(finalPath);
                    result.Absorb(import);
                    meshResult = import.Result;
                    break;
                default:
                    result.AddError($"Unsupported model extension for id: {assetPath}");
                    return result.SetResult(null);
            }

            var prefabFromModel = PrepareModelForGame(meshResult);
            Logger.LogDebug($"Prefab prepared for model with id: {prefabFromModel.name}");

            if (prefabFromModel != null)
            {
                return result.SetResult(prefabFromModel);
            }
            else
            {
                result.AddWarning($"Failed to prepare model for id: {assetPath}");
                return result.SetResult(null);
            }
        }

        public static GameObject ClonePrefab(GameObject source)
        {
            if (source == null)
            {
                return null;
            }

            EnsurePrefabsRoot();
            var clone = UnityEngine.Object.Instantiate(source, ROOT_FOR_PREFABS, false);
            clone.name = source.name;
            return clone;
        }

        private static void EnsurePrefabsRoot()
        {
            if (ROOT_FOR_PREFABS == null)
            {
                var prefabsRoot = new GameObject("MgsPackMod_Prefabs");
                prefabsRoot.transform.position = Vector3.zero;
                prefabsRoot.SetActive(false);
                GameObject.DontDestroyOnLoad(prefabsRoot);
                ROOT_FOR_PREFABS = prefabsRoot.transform;
            }
        }

        private static GameObject PrepareModelForGame(Mesh meshResult)
        {
            if (meshResult == null)
            {
                return null;
            }

            meshResult.name = "ImportedMesh";

            EnsurePrefabsRoot();

            var prefabInstance = new GameObject("ImportedPrefab");
            prefabInstance.transform.SetParent(ROOT_FOR_PREFABS, false);
            prefabInstance.transform.position = Vector3.zero;

            var meshFilter = prefabInstance.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = meshResult;

            var meshRenderer = prefabInstance.AddComponent<MeshRenderer>();
            // Maybe its a good idea to clone any of the default objects.

            prefabInstance.AddComponent<ItemBone>();

            return prefabInstance;
        }
    }
}
