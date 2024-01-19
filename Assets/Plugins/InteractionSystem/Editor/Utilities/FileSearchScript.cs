using UnityEditor;

public static class FileSearchScript
{
    public static string SearchFile(string filename)
    {
        string[] allFiles = AssetDatabase.GetAllAssetPaths();

        foreach (string filePath in allFiles)
            if (IsFileMatch(filePath, filename))
                return filePath;

        return null;
    }

    private static bool IsFileMatch(string filePath, string filename)
    {
        if (filePath.EndsWith(filename)) return true;
        return false;
    }
}
