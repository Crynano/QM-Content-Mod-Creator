using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Records
{
    internal static class GrenadeRecordExtensions
    {
        internal static ImportOperationResult SetDescriptorProperties(this GrenadeRecord grenade, CustomGrenadeDescriptor customGrenadeDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var grenadeDescriptor = ScriptableObject.CreateInstance<GrenadeItemDescriptor>();

            if (customGrenadeDescriptor == null)
            {
                operationResult.AddError("CustomGrenadeDescriptor is null.");
                return operationResult;
            }

            // Load UI sprites using the standard image properties extension
            grenadeDescriptor.LoadSprites(customGrenadeDescriptor, assetFolderPath);

            // Load sound banks
            if (customGrenadeDescriptor.SoundProperties != null)
            {
                var throwSoundResult = LoadSoundBank(customGrenadeDescriptor.SoundProperties.ThrowSoundIdOrPath, assetFolderPath);
                if (throwSoundResult.IsSuccess)
                {
                    grenadeDescriptor.throwSound = throwSoundResult.Result;
                }
                operationResult.Absorb(throwSoundResult);

                var fallSoundResult = LoadSoundBank(customGrenadeDescriptor.SoundProperties.FallSoundIdOrPath, assetFolderPath);
                if (fallSoundResult.IsSuccess)
                {
                    grenadeDescriptor.fallSound = fallSoundResult.Result;
                }
                operationResult.Absorb(fallSoundResult);

                var ricochetSoundResult = LoadSoundBank(customGrenadeDescriptor.SoundProperties.RicochetSoundIdOrPath, assetFolderPath);
                if (ricochetSoundResult.IsSuccess)
                {
                    grenadeDescriptor.ricochetSound = ricochetSoundResult.Result;
                }
                operationResult.Absorb(ricochetSoundResult);
            }

            // Load sprite arrays
            if (customGrenadeDescriptor.SpriteArrayProperties != null)
            {
                var flySpritesResult = LoadSpriteArray(customGrenadeDescriptor.SpriteArrayProperties.EntityFlySpriteIdOrPath, assetFolderPath, nameof(GrenadeItemDescriptor.entityFlySprites));
                if (flySpritesResult.IsSuccess)
                {
                    grenadeDescriptor.entityFlySprites = flySpritesResult.Result;
                }
                operationResult.Absorb(flySpritesResult);

                var shadowSpritesResult = LoadSpriteArray(customGrenadeDescriptor.SpriteArrayProperties.EntityShadowSpriteIdOrPath, assetFolderPath, nameof(GrenadeItemDescriptor.entityShadowSprites));
                if (shadowSpritesResult.IsSuccess)
                {
                    grenadeDescriptor.entityShadowSprites = shadowSpritesResult.Result;
                }
                operationResult.Absorb(shadowSpritesResult);

                var activationSpritesResult = LoadSpriteArray(customGrenadeDescriptor.SpriteArrayProperties.EntityActivationSpriteIdOrPath, assetFolderPath, nameof(GrenadeItemDescriptor.entityActivationSprites));
                if (activationSpritesResult.IsSuccess)
                {
                    grenadeDescriptor.entityActivationSprites = activationSpritesResult.Result;
                }
                operationResult.Absorb(activationSpritesResult);
            }

            grenade.ContentDescriptor = grenadeDescriptor;
            return operationResult;
        }

        private static ImportOperationResult<SoundBank> LoadSoundBank(string soundPathOrId, string assetFolderPath)
        {
            var result = new ImportOperationResult<SoundBank>();
            var soundBank = ScriptableObject.CreateInstance<SoundBank>();
            soundBank._clips = new AudioClip[1];

            if (string.IsNullOrEmpty(soundPathOrId))
            {
                result.SetResult(soundBank);
                return result;
            }

            if (QuasimorphHelper.IsGameId(soundPathOrId))
            {
                result.AddWarning($"Sound ID '{soundPathOrId}' is a game ID. Loading sounds from existing game grenades is not supported. Using empty sound bank.");
                result.SetResult(soundBank);
                return result;
            }

            var audioResult = QuasimorphHelper.LoadAudioClipFromExternalFile(soundPathOrId, assetFolderPath);
            if (audioResult.IsSuccess)
            {
                soundBank._clips[0] = audioResult.Result;
                result.SetResult(soundBank);
            }
            else
            {
                result.Absorb(audioResult);
                result.SetResult(soundBank);
            }

            return result;
        }

        private static ImportOperationResult<Sprite[]> LoadSpriteArray(string spritePathOrId, string assetFolderPath, string propertyName)
        {
            var result = new ImportOperationResult<Sprite[]>();

            if (string.IsNullOrEmpty(spritePathOrId))
            {
                result.SetResult(new Sprite[0]);
                return result;
            }

            if (QuasimorphHelper.IsGameId(spritePathOrId))
            {
                Logger.LogDebug($"Attempting to load sprite array '{propertyName}' from existing grenade with ID: '{spritePathOrId}'");

                // Try to get the sprite array from existing grenade descriptor
                var spriteArrayProperty = QuasimorphHelper.GetPropertyFromItem<GrenadeItemDescriptor>(spritePathOrId, propertyName);
                if (spriteArrayProperty is Sprite[] spriteArray && spriteArray.Length > 0)
                {
                    result.SetResult(spriteArray);
                    return result;
                }

                result.AddWarning($"Unable to load sprite array '{propertyName}' from existing grenade item with ID: {spritePathOrId}. Property may not exist or is empty. Using empty array.");
                result.SetResult(new Sprite[0]);
                return result;
            }

            var fullPath = Helper.ResolvePath(assetFolderPath, spritePathOrId);

            if (!File.Exists(fullPath))
            {
                result.AddError($"Sprite file not found for '{propertyName}' at {fullPath}");
                result.SetResult(new Sprite[0]);
                return result;
            }

            // Load sprite from file
            try
            {
                var sprite = AssetImporter.LoadOffsetSprite(fullPath);
                if (sprite != null)
                {
                    result.SetResult(new[] { sprite });
                }
                else
                {
                    result.AddError($"Failed to load sprite from {fullPath}");
                    result.SetResult(new Sprite[0]);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Exception loading sprite from {fullPath}: {ex.Message}");
                result.SetResult(new Sprite[0]);
            }

            return result;
        }
    }
}
