using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for FlamethrowerProjectileView. Clones the view of an in-game projectile (Data.Projectiles),
    /// overrides its serialized fields from the descriptor and registers a new projectile under ItemId.
    /// </summary>
    internal class FlamethrowerProjectileViewLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(FlamethrowerProjectileViewLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var result = new ImportOperationResult();
            var descriptors = FilterByType<CustomFlamethrowerProjectileViewDescriptor>(deserializedObjects);

            LogLoadStart(descriptors.Count());

            foreach (var descriptor in descriptors)
            {
                result.Absorb(Create(descriptor));
            }

            return result;
        }

        private static ImportOperationResult Create(CustomFlamethrowerProjectileViewDescriptor descriptor)
        {
            var result = new ImportOperationResult();

            if (string.IsNullOrEmpty(descriptor.ItemId))
            {
                result.AddError($"{nameof(CustomFlamethrowerProjectileViewDescriptor)} has an empty ItemId.");
                return result;
            }

            if (string.IsNullOrEmpty(descriptor.BaseProjectileId) || !Data.Projectiles.Ids.Contains(descriptor.BaseProjectileId))
            {
                result.AddError($"Projectile view '{descriptor.ItemId}': in-game projectile '{descriptor.BaseProjectileId}' does not exist.");
                return result;
            }

            var baseDescriptor = Data.Projectiles.GetRecord(descriptor.BaseProjectileId).ContentDescriptor as ProjectileDescriptor;
            var baseView = baseDescriptor != null ? baseDescriptor.Bullet : null;
            if (baseView == null || baseView.GetComponent<FlamethrowerProjectileView>() == null)
            {
                result.AddError($"In-game projectile '{descriptor.BaseProjectileId}' is not a {nameof(FlamethrowerProjectileView)}.");
                return result;
            }

            var clone = PrefabFactory.ClonePrefab(baseView.gameObject);
            clone.name = descriptor.ItemId;
            var view = clone.GetComponent<FlamethrowerProjectileView>();

            ApplyDescriptor(view, descriptor, result);

            var projectileDescriptor = ScriptableObject.CreateInstance<ProjectileDescriptor>();
            projectileDescriptor._bullet = view;

            var record = new ProjectileRecord
            {
                Id = descriptor.ItemId,
                Speed = descriptor.BulletSpeed,
                MakeBloodDecals = descriptor.MakeBloodDecals,
                ContentDescriptor = projectileDescriptor
            };

            if (Data.Projectiles.Ids.Contains(record.Id))
            {
                Data.Projectiles.RemoveRecord(record.Id);
                result.AddWarning($"Projectile with ID '{record.Id}' was overriden.");
            }

            Logger.LogDebug($"Adding projectile view '{record.Id}' to the game.");
            Data.Descriptors["projectiles"].AddDescriptor(record.Id, projectileDescriptor);
            Data.Projectiles.AddRecord(record.Id, record);
            result.AddItem(record.Id);

            return result;
        }

        private static void ApplyDescriptor(FlamethrowerProjectileView view, CustomFlamethrowerProjectileViewDescriptor d, ImportOperationResult result)
        {
            view._bulletSpeed = d.BulletSpeed;
            view._makeBloodDecals = d.MakeBloodDecals;
            view._putShotDecalsOnWalls = d.PutShotDecalsOnWalls;
            view._putBulletShellsOnFloor = d.PutBulletShellsOnFloor;
            view._rotateBulletInShotDir = d.RotateBulletInShotDir;
            view._shakeDuration = d.ShakeDuration;
            view._shakeStrength = d.ShakeStrength;

            view._bulletLifetime = d.BulletLifetime;
            view._emitUntlTime = d.EmitUntilTime;
            view._emitFireInterval = d.EmitFireInterval;
            view._flameDropSpeed = d.FlameDropSpeed;
        }
    }
}
