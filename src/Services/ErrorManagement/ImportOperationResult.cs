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
        public string ResultMessage { get; private set; } = string.Empty;
        public double ExecutionTime { get; private set; } = 0f;
        public List<string> ErrorMessages { get; private set; } = new List<string>();
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

        public ImportOperationResult AddErrors(string error, IEnumerable<string> messages)
        {
            StringBuilder combinedMessage = new StringBuilder(error);
            combinedMessage.AppendLine();
            foreach (var subError in messages)
            {
                combinedMessage.Append($"\t- {subError}");
                combinedMessage.AppendLine();
            }
            ErrorMessages.Add(combinedMessage.ToString());
            return this;
        }


        public ImportOperationResult SetResult(string resultMessage)
        {
            ResultMessage = resultMessage;
            return this;
        }

        /// <summary>
        /// Returns a stringified version of the ResultInfo, ready to log.
        /// </summary>
        /// <returns>A formatted string</returns>
        public string Print()
        {
            string msg = "";

            msg += $"\tExecution Time: {this.ExecutionTime}\n";
            msg += $"\tResult: {this.IsSuccess}\n";
            msg += $"\tResult Message: {this.ResultMessage}\n";
            msg += $"\tError Messages:\n";
            foreach (var errorMessage in this.ErrorMessages)
            {
                msg += $"\t\t- {errorMessage}\n";
            }
            
            msg += $"\tContent List:\n";
            foreach (var content in this.ContentList)
            {
                msg += $"\t\t- {content}\n";
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