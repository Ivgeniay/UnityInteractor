using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace InteractionSystem
{
    public class FileSearchScript : EditorWindow
    {
        private const string FileNameToSearch = "NodeEngineStyles.uss";

        [MenuItem("Tools/Search File")]
        private static void SearchFile()
        {
            string[] allFiles = AssetDatabase.GetAllAssetPaths();

            foreach (string filePath in allFiles)
            {
                if (IsFileMatch(filePath))
                {
                    Debug.Log($"File found: {filePath}");
                    return;
                }
            }

            Debug.LogError($"File '{FileNameToSearch}' not found.");
        }

        private static bool IsFileMatch(string filePath)
        {
            if (filePath.EndsWith(FileNameToSearch))
            {
                // Здесь вы можете добавить дополнительные проверки, если это необходимо
                return true;
            }

            return false;
        }
    }
}
