using UnityEngine;
using UnityEditor;
using System.IO;

namespace JankyArrayMaker
{

public class JankyArrayBreaker : EditorWindow
{
    // The Texture2DArray to process
    private Texture2DArray textureArray;

    // Add a menu item to open the window
    [MenuItem("Tools/Janky/JankyArrayMaker/Slice Arrays")]
    public static void ShowWindow()
    {
        GetWindow<JankyArrayBreaker>("JankyArrayBreaker");
    }

    private void OnGUI()
    {
        GUILayout.Label("JankyArrayBreaker", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Object field to assign the Texture2DArray
        textureArray = (Texture2DArray)EditorGUILayout.ObjectField(
            new GUIContent("Texture2DArray"),
            textureArray,
            typeof(Texture2DArray),
            false
        );

        GUILayout.Space(20);

        // Disable the button if no Texture2DArray is assigned
        EditorGUI.BeginDisabledGroup(textureArray == null);
        if (GUILayout.Button("Slice to File", GUILayout.Height(40)))
        {
            SliceToFile();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void SliceToFile()
    {
        if (textureArray == null)
        {
            EditorUtility.DisplayDialog("JankyArrayBreaker", "Please assign a Texture2DArray.", "OK");
            return;
        }

        // Open a folder panel to select the output directory
        string folderPath = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, "");

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("No folder selected. Operation cancelled.");
            return;
        }

        // Ensure the folder path ends with a directory separator
        if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            folderPath += Path.DirectorySeparatorChar;
        }

        // Get the number of slices in the Texture2DArray
        int sliceCount = textureArray.depth;

        // Iterate through each slice
        for (int i = 0; i < sliceCount; i++)
        {
            // Retrieve all pixels from the current slice at mip level 0
            Color[] pixels = textureArray.GetPixels(i, 0);

            if (pixels == null || pixels.Length == 0)
            {
                Debug.LogWarning($"Slice {i} has no pixels or failed to retrieve pixels.");
                continue;
            }

            // Create a new Texture2D with a compatible format for PNG encoding
            Texture2D tex = new Texture2D(textureArray.width, textureArray.height, TextureFormat.RGBA32, false);

            // Assign the pixel data to the new Texture2D
            tex.SetPixels(pixels);
            tex.Apply();

            // Encode texture to PNG
            byte[] bytes = tex.EncodeToPNG();

            // Define the file path
            string filePath = Path.Combine(folderPath, $"TextureSlice_{i}.png");

            try
            {
                // Write the PNG file
                File.WriteAllBytes(filePath, bytes);
                Debug.Log($"Saved slice {i} to {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to save slice {i} to {filePath}: {e.Message}");
            }
            finally
            {
                // Clean up
                DestroyImmediate(tex);
            }
        }

        // Refresh the AssetDatabase to recognize new files
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("JankyArrayBreaker", "Slicing completed successfully!", "OK");
    }
}
}
