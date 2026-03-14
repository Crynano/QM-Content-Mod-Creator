using System;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Images
{
    public static class TextureImporter
    {
        private const string DEFAULT_TEXTURE = "iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAIAAACR5s1WAAAAaklEQVRYCe3WsRFAQABFQac7PVAdPWhPtA04geBJnuSMWT8wzu1a5q7j3ucesKyT5z853ktgTCIJAtomkiCgbSIJAtomkiCgbYLEcPO+8//rfQ76SSRBQNtEEgS0TSRBQNtEEgS0TfxK4gGfNATn17aOOAAAAABJRU5ErkJggg==";
        private const int DEFAULT_SQUARE_SIZE = 2;
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