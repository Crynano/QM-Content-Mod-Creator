using MGSC;
using QM_ImporterAPI.Services.Helpers;
using System;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions
{
    internal static class SpriteExtensions
    {
        internal static void LoadSprite(this Sprite sprite, string assetFolderPath, string path, string propertyName, Func<string, Sprite> loadFunc)
        {
            if (QuasimorphHelper.IsGameId(path))
            {
                var propertyFromItem = QuasimorphHelper.GetPropertyFromItem<WeaponDescriptor>(path, propertyName);
                if (propertyFromItem is Sprite spriteProperty)
                {
                    sprite = QuasimorphHelper.CloneSprite(spriteProperty);
                }
                Logger.LogWarning("Failed to load sprite for property [" + propertyName + "] from existing game item with ID: " + path + ". The property is either missing or not a Sprite.");
            }
            var fullPath = Helper.ResolvePath(assetFolderPath, path);
            sprite = loadFunc(fullPath);
        }
    }
}