using System;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Images
{
    public static class TextureImporter
    {
        private const string DEFAULT_TEXTURE = "iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAIAAACR5s1WAAAAaklEQVRYCe3WsRFAQABFQac7PVAdPWhPtA04geBJnuSMWT8wzu1a5q7j3ucesKyT5z853ktgTCIJAtomkiCgbSIJAtomkiCgbYLEcPO+8//rfQ76SSRBQNtEEgS0TSRBQNtEEgS0TfxK4gGfNATn17aOOAAAAABJRU5ErkJggg==";
        public static Texture2D ImportFromFile(string FilePath)
        {
            Texture2D Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, true)
            {
                filterMode = FilterMode.Point
            };
            byte[] FileData;

            try
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D.BytesToTexture(FileData);
                return Tex2D;
            }
            catch(FileNotFoundException ex)
            {
                Logger.LogError($"File not found at {FilePath}. Error: {ex.Message}");
            }
            catch (Exception e)
            {
                Logger.LogError($"Could not load image from {FilePath}. Error: {e.Message}");
            }
            Tex2D.BytesToTexture(Convert.FromBase64String(DEFAULT_TEXTURE));

            return Tex2D;
        }

        /// <summary>
        /// https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Texture2D.LoadRawTextureData.html
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="byteData"></param>
        /// <returns></returns>
        private static Texture2D BytesToTexture(this Texture2D texture, byte[] byteData)
        {
            texture.LoadRawTextureData(byteData);
            return texture;
        }
    }
}