using UnityEngine;

namespace QM_ImporterAPI.Services.Images
{
    public static class SpriteImporter
    {
        public static Sprite ImportFromFile(string fullPath, Vector2 size, float imagePixelScaling)
        {
            Texture2D importedTexture = TextureImporter.ImportFromFile(fullPath);
            Rect oldRect = new Rect(0, 0, importedTexture.width, importedTexture.height);
            Sprite NewSprite = Sprite.Create(importedTexture, oldRect, size, imagePixelScaling);
            return NewSprite;
        }
    }
}