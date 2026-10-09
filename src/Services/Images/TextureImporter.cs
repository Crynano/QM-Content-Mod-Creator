using QM_ImporterAPI.Services.ErrorManagement;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace QM_ImporterAPI.Services.Images
{
    public static class TextureImporter
    {
        private const string DEFAULT_TEXTURE = "iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAIAAACR5s1WAAAAaklEQVRYCe3WsRFAQABFQac7PVAdPWhPtA04geBJnuSMWT8wzu1a5q7j3ucesKyT5z853ktgTCIJAtomkiCgbSIJAtomkiCgbYLEcPO+8//rfQ76SSRBQNtEEgS0TSRBQNtEEgS0TfxK4gGfNATn17aOOAAAAABJRU5ErkJggg==";
        private const int DEFAULT_SQUARE_SIZE = 2;

        private readonly static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        public static ImportOperationResult<Texture> ImportFromFile(string basePath, string relativePath)
        {
            var result = new ImportOperationResult<Texture>();

            var fullPath = Path.Combine(basePath, relativePath);
            if (_textures.TryGetValue(fullPath, out var value))
            {
                result.SetResult(value);
                return result;
            }

            if (!File.Exists(fullPath))
            {
                result.AddWarning($"File not found: {fullPath}");
                return result;
            }

            var tex = new Texture2D(2, 2, (TextureFormat)4, false);
            if (!ImageConversion.LoadImage(tex, File.ReadAllBytes(fullPath)))
            {
                result.AddWarning($"Failed to load image from file: {fullPath}");
                return result;
            }

            tex.name = Path.GetFileNameWithoutExtension(relativePath);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.anisoLevel = 1;

            _textures[fullPath] = tex;

            result.SetResult(tex);
            return result;
        }

        public static Texture2D ImportFromFile(string FilePath)
        {
            Texture2D Tex2D;

            if (File.Exists(FilePath))
            {
                Tex2D = CreateTexture(DEFAULT_SQUARE_SIZE, DEFAULT_SQUARE_SIZE);
                var byteData = File.ReadAllBytes(FilePath);
                Tex2D.BytesToTexture(byteData);
            }
            else
            {
                Tex2D = CreateTexture(DEFAULT_SQUARE_SIZE, DEFAULT_SQUARE_SIZE);
                Tex2D.BytesToTexture(Convert.FromBase64String(DEFAULT_TEXTURE));
            }

            return Tex2D;
        }

        private static Texture2D CreateTexture(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false, true)
            {
                filterMode = FilterMode.Point
            };
        }

        private static Texture2D BytesToTexture(this Texture2D texture, byte[] byteData)
        {
            ImageConversion.LoadImage(texture, byteData);
            return texture;
        }
    }
}