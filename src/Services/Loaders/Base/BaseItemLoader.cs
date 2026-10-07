using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers.Import;
using QM_ImporterAPI.Services.Validation.Base;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    internal abstract class BaseItemLoader<TRecord, TDescriptor> : BaseItemLoader where TRecord : ConfigTableRecord where TDescriptor : CustomBaseDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the BaseItemLoader class with the specified validator.
        /// </summary>
        /// <param name="validator">The validator to use for validating records and descriptors.</param>
        protected BaseItemLoader(ConfigTableRecordValidator<TRecord, TDescriptor> validator)
        {
            Validator = validator;
        }

        /// <summary>
        /// Gets or sets the validator for the specific record and descriptor types.
        /// </summary>
        protected ConfigTableRecordValidator<TRecord, TDescriptor> Validator { get; set; }

        /// <summary>
        /// Creates a new item in the game based on the provided record and descriptor.
        /// </summary>
        /// <param name="record">The record containing the item's data</param>
        /// <param name="descriptor">The descriptor providing additional item details</param>
        /// <param name="assetFolderPath">Path to the mod's asset folder</param>
        /// <returns>Result of the import operation</returns>
        protected virtual ImportOperationResult Create(TRecord record, TDescriptor descriptor, string assetFolderPath) 
        {
            Logger.LogDebug($"Creating {typeof(TRecord).Name} \"{record.Id}\" with descriptor \"{descriptor.ItemId}\"");
            var result = new ImportOperationResult();

            var validationResult = ImportHelper.PerformImportValidation(record, descriptor);
            result.Absorb(validationResult);

            var augmentationValidation = Validator.Validate(record, descriptor);
            result.Absorb(augmentationValidation);

            return result;
        }

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var result = new ImportOperationResult();

            var records = FilterByType<TRecord>(deserializedObjects);
            var descriptors = FilterByType<TDescriptor>(deserializedObjects);

            LogLoadStart(records.Count(), descriptors.Count());

            foreach (var descriptor in descriptors)
            {
                var record = records.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (record != null)
                {
                    Logger.LogDebug($"Trying to add {typeof(TRecord).Name} \"{record.Id}\" (with descriptor) to the game!");
                    var opResult = Create(record, descriptor, assetFolderPath);
                    result.Absorb(opResult);
                }
                else
                {
                    result.AddWarning($"Could not find a {typeof(TRecord).Name} record with id '{descriptor.ItemId}' for the {typeof(TDescriptor).Name} descriptor. Skipping.");
                }
            }
            return result;
        }
    }   

    /// <summary>
    /// Abstract base class for loading items into the game.
    /// Provides a generic framework for filtering and processing records and descriptors.
    /// </summary>
    internal abstract class BaseItemLoader
    {

        /// <summary>
        /// Gets the name of this loader for logging purposes.
        /// </summary>
        protected abstract string LoaderName { get; }

        /// <summary>
        /// Loads items from the provided deserialized objects.
        /// </summary>
        /// <param name="deserializedObjects">All deserialized JSON objects from mod files</param>
        /// <param name="assetFolderPath">Path to the mod's asset folder</param>
        /// <returns>Result of the import operation</returns>
        public abstract ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath);

        /// <summary>
        /// Filters objects to only those of the specified type.
        /// </summary>
        protected static IEnumerable<T> FilterByType<T>(IEnumerable<object> objects) where T : class
        {
            return objects.OfType<T>();
        }

        /// <summary>
        /// Logs the start of loading with record and descriptor counts.
        /// </summary>
        protected void LogLoadStart(int recordCount, int descriptorCount)
        {
            Logger.LogDebug($"{LoaderName}: Found {recordCount} records and {descriptorCount} descriptors.");
        }

        /// <summary>
        /// Logs the start of loading with only record count (for loaders without descriptors).
        /// </summary>
        protected void LogLoadStart(int recordCount)
        {
            Logger.LogDebug($"{LoaderName}: Found {recordCount} records.");
        }
    }
}
