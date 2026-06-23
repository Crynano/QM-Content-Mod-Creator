using QM_ImporterAPI.Services.ErrorManagement;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Abstract base class for loading items into the game.
    /// Provides a generic framework for filtering and processing records and descriptors.
    /// </summary>
    public abstract class BaseItemLoader
    {
        /// <summary>
        /// Loads items from the provided deserialized objects.
        /// </summary>
        /// <param name="deserializedObjects">All deserialized JSON objects from mod files</param>
        /// <param name="assetFolderPath">Path to the mod's asset folder</param>
        /// <returns>Result of the import operation</returns>
        public abstract ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath);

        /// <summary>
        /// Gets the name of this loader for logging purposes.
        /// </summary>
        protected abstract string LoaderName { get; }

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
