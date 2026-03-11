using QM_ImporterAPI.Services.ErrorManagement;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace QM_ImporterAPI.Services.Audio
{
    internal class UnityFileAudioImporter
    {
        public ImportOperationResult<AudioClip> Import(string path)
        {
            var operationResult = new ImportOperationResult<AudioClip>();
            AudioType audioType = AnalyzeAudioType(path, out string fileName);
            if (audioType == AudioType.UNKNOWN)
            {
                operationResult.AddError($"Audio: {path}\nERROR: AudioType was not identified correctly.");
                return operationResult;
            }

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
            request.SendWebRequest();
            do
            {
                //Console.WriteLine($"Iterating in console waiting for the request!");
            }
            while (!request.isDone);

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                operationResult.AddError("Error retrieving audio at \"" + path + "\". ERROR: " + request.error);
            }
            else
            {
                AudioClip returnClip = DownloadHandlerAudioClip.GetContent(request);
                returnClip.name = fileName;
                operationResult.SetResult(returnClip);
                return operationResult;
            }
            return operationResult;
        }

        public static AudioType AnalyzeAudioType(string path, out string fileName)
        {
            string[] pathSplit = path.Split('/');
            string completeFile = pathSplit[pathSplit.Length - 1];
            fileName = completeFile.Split('.')[0];
            string fileExtension = completeFile.Split('.')[1];
            Enum.TryParse(fileExtension.ToUpper(), out AudioType audioType);
            return audioType;
        }
    }
}