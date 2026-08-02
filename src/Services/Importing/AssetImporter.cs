using QM_ImporterAPI.Services.Audio;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Images;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Importing
{
    public static class AssetImporter
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

        public static Sprite LoadSpriteCustom(string fullPath, Vector2 center, float pixelsPerUnit = 100f)
        {
            return SpriteImporter.ImportFromFile(fullPath, center, pixelsPerUnit);
        }

        public static Sprite LoadSpriteWithDefaultScaling(string fullPath)
        {
            return SpriteImporter.ImportFromFile(fullPath, Vector2.zero, 100f);
        }

        public static Sprite LoadNewSprite(string path)
        {
            return SpriteImporter.ImportFromFile(path, Vector2.zero, 200f);
        }

        public static Sprite LoadCenteredSprite(string path)
        {
            return SpriteImporter.ImportFromFile(path, new Vector2(0.5f, 0f), 100f);
        }

        public static ImportOperationResult<T> LoadFileFromBundle<T>(string bundlePath, string fileName) where T : class
        {
            var operationResult = new ImportOperationResult<T>();

            if (string.IsNullOrEmpty(fileName))
            {
                operationResult.AddError($"fileName was empty or null");
            }

            if (string.IsNullOrEmpty(bundlePath))
            {
                operationResult.AddError($"bundlePath was empty or null");
            }
            else if (!File.Exists(bundlePath))
            {
                operationResult.AddError($"Could not find bundle at {bundlePath}");
            }
            else
            {
                var loadedBundle = AssetBundle.LoadFromFile(bundlePath);
                var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
                loadedBundle.Unload(false);
                if (loadedAsset != null)
                {
                    operationResult.SetResult(loadedAsset);
                }
                else
                {
                    operationResult.AddError($"Could not find asset {fileName} in bundle at {bundlePath}");
                }
            }

            return operationResult;
        }

        public static AudioClip[] ImportAudio(List<string> audioPaths)
        {
            List<AudioClip> result = new List<AudioClip>();
            foreach (string audioPath in audioPaths)
            {
                var audioRes = ImportAudio(audioPath);
                if (audioRes != null && audioRes.IsSuccess)
                {
                    result.Add(audioRes.Result);
                }
            }
            return result.ToArray();
        }

        public static ImportOperationResult<AudioClip> ImportAudio(string fullPath)
        {
            var operationResult = new ImportOperationResult<AudioClip>();

            if (string.IsNullOrEmpty(fullPath))
            {
                operationResult.AddError($"fullPath was empty or null");
            }
            else if (!File.Exists(fullPath))
            {
                operationResult.AddError($"Sound at {fullPath} does not exist");
            }
            else
            {
                var audioImportResult = UnityAudioFileImporter.Import(fullPath);
                if (audioImportResult.IsSuccess)
                {
                    operationResult.SetResult(audioImportResult.Result);
                }
                else
                {
                    operationResult.AddError($"Failed to import sound at {fullPath}");
                }
            }

            return operationResult;
        }
    }
}