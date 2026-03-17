using System.Collections.Generic;
using System.Text;

namespace QM_ImporterAPI.Services.ErrorManagement
{
    /// <summary>
    /// A class that will contain the process information of the import execution
    /// </summary>
    public class ImportOperationResult
    {
        public bool IsSuccess => ErrorMessages.Count == 0;
        public double ExecutionTime { get; private set; } = 0f;
        public List<string> ErrorMessages { get; private set; } = new List<string>();
        public List<string> WarningMessages { get; private set; } = new List<string>();
        public List<string> ContentList { get; private set; } = new List<string>();

        public ImportOperationResult AddError(string message)
        {
            ErrorMessages.Add(message);
            return this;
        }

        public ImportOperationResult AddErrors(IEnumerable<string> messages)
        {
            ErrorMessages.AddRange(messages);
            return this;
        }

        public ImportOperationResult AddWarning(string message)
        {
            WarningMessages.Add(message);
            return this;
        }

        public ImportOperationResult AddWarnings(IEnumerable<string> messages)
        {
            WarningMessages.AddRange(messages);
            return this;
        }

        public void SetExecutionTime(double executionTime)
        {
            ExecutionTime = executionTime;
        }

        /// <summary>
        /// Returns a stringified version of the ResultInfo, ready to log.
        /// </summary>
        /// <returns>A formatted string</returns>
        public string Print()
        {
            string msg = "";

            msg += $"Result: {this.IsSuccess}\n";
            msg += $"Execution Time: {this.ExecutionTime}\n";

            if (ErrorMessages.Count > 0)
            {
                msg += $"Error Messages:\n";
                foreach (var errorMessage in this.ErrorMessages)
                {
                    msg += $"\t- {errorMessage}\n";
                }
            }

            if (WarningMessages.Count > 0)
            {
                msg += $"Warning Messages:\n";
                foreach (var errorMessage in this.WarningMessages)
                {
                    msg += $"\t- {errorMessage}\n";
                }
            }

            if (ContentList.Count > 0)
            {
                msg += $"Content List:\n";
                foreach (var content in this.ContentList)
                {
                    msg += $"\t- {content}\n";
                }
            }
            
            return msg;
        }

        public void AddItem(string itemId)
        {
            ContentList.Add(itemId);
        }
    }

    public class ImportOperationResult<T> : ImportOperationResult
    {
        public T Result { get; private set; }

        public void SetResult(T item)
        {
            Result = item;
        }
    }
}