using UnityEngine;
using UnityEditor;

namespace JankyStartupWindow
{
    public class JankyStartupWindow : EditorWindow
    {
        private bool showOnStartup = true;
        private static Vector2 windowSize = new Vector2(600, 600);

        private JankyStartupWindowInfo[] windowInfos;

        private const float LogoWidth = 80f; // Width of the logo
        private const float LogoHeight = 80f; // Height of the logo
        private const float Padding = 10f; // Padding around the logo
        private const float ButtonWidth = 150f; // Fixed width of buttons
        private const float ButtonSpacing = 10f; // Spacing between buttons
        private Vector2 scrollPosition; // For the scroll view
        private const string ImagePath = "Assets/Janky/Janky Window/UI/UI_StartupWindowHeader.png"; // Path to your image

        [MenuItem("Tools/Janky/Janky Startup Window")]
        public static void ShowWindow()
        {
            JankyStartupWindow window = GetWindow<JankyStartupWindow>("Janky Gamesmiths");
            window.minSize = windowSize;
            window.maxSize = windowSize;
            window.LoadWindowInfos();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            EditorApplication.update += ShowOnStartup;
        }

        private static void ShowOnStartup()
        {
            if (EditorPrefs.HasKey("JankyWindow.ShowOnStartup"))
            {
                bool showOnStartup = EditorPrefs.GetBool("JankyWindow.ShowOnStartup");
                if (showOnStartup)
                {
                    ShowWindow();
                }
            }
            else
            {
                EditorPrefs.SetBool("JankyWindow.ShowOnStartup", true);
                ShowWindow();
            }

            EditorApplication.update -= ShowOnStartup;
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("JankyWindow.ShowOnStartup"))
            {
                showOnStartup = EditorPrefs.GetBool("JankyWindow.ShowOnStartup");
            }
            else
            {
                EditorPrefs.SetBool("JankyWindow.ShowOnStartup", showOnStartup);
            }
        }

        private void OnGUI()
        {
            // Display the image at the top
            GUILayout.BeginVertical();
            Texture2D image = AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath);
            if (image != null)
            {
                const float imageWidth = 600f; // Adjust the width as needed
                const float imageHeight = 100f; // Fixed height for the image

                GUILayout.Label(image, GUILayout.Width(imageWidth), GUILayout.Height(imageHeight));
            }
            else
            {
                GUILayout.Label("Image not found at path: " + ImagePath, EditorStyles.boldLabel, GUILayout.Height(100));
            }
            GUILayout.Space(10f); // Space between the image and the scroll view

            // Main layout
            GUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            if (windowInfos == null || windowInfos.Length == 0)
            {
                EditorGUILayout.HelpBox("No JankyStartupWindowInfo assets found.", MessageType.Warning);
                if (GUILayout.Button("Refresh"))
                {
                    LoadWindowInfos();
                }
            }
            else
            {
                foreach (var info in windowInfos)
                {
                    DisplayWindowInfo(info);
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Buttons area
            GUILayout.BeginHorizontal();
            float buttonWidth = (position.width - 1.5f * ButtonSpacing) / 4; // 3 * ButtonSpacing is for spacing between buttons

            if (GUILayout.Button("Asset Store", GUILayout.Width(buttonWidth)))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/102581"); // Replace with your Asset Store URL
            }

            if (GUILayout.Button("Discord", GUILayout.Width(buttonWidth)))
            {
                Application.OpenURL("https://discord.gg/xd7c3vRaEp"); // Replace with your Discord URL
            }

            if (GUILayout.Button("Website", GUILayout.Width(buttonWidth)))
            {
                Application.OpenURL("https://jankygamesmiths.wixsite.com/janky-gamesmiths"); // Replace with your Website URL
            }

            if (GUILayout.Button("Contact Us", GUILayout.Width(buttonWidth)))
            {
                Application.OpenURL("https://jankygamesmiths.wixsite.com/janky-gamesmiths/janky-contact"); // Replace with your Contact Us URL
            }

            GUILayout.EndHorizontal();

            // Settings area
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            showOnStartup = EditorGUILayout.Toggle("Show on Startup", showOnStartup);

            if (GUI.changed)
            {
                EditorPrefs.SetBool("JankyWindow.ShowOnStartup", showOnStartup);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void LoadWindowInfos()
        {
            string[] guids = AssetDatabase.FindAssets("t=JankyStartupWindowInfo");
            windowInfos = new JankyStartupWindowInfo[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                windowInfos[i] = AssetDatabase.LoadAssetAtPath<JankyStartupWindowInfo>(path);
            }
        }

        private void DisplayWindowInfo(JankyStartupWindowInfo info)
        {
            GUILayout.BeginHorizontal();

            // Logo Column
            GUILayout.BeginVertical(GUILayout.Width(LogoWidth + 2 * Padding));
            GUILayout.Space(Padding);
            if (info.logo != null)
            {
                GUILayout.Label(info.logo.texture, GUILayout.Width(LogoWidth), GUILayout.Height(LogoHeight));
            }
            GUILayout.Space(Padding);
            GUILayout.EndVertical();

            // Title and Description Column
            GUILayout.BeginVertical();
            GUILayout.Label(info.title, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Label(info.description, EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // Offset for buttons
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(LogoWidth + 2 * Padding); // Offset by the width of the logo column

            if (info.viewInAssetStoreVisible)
            {
                if (GUILayout.Button("View in Asset Store", GUILayout.Width(ButtonWidth)))
                {
                    Application.OpenURL(info.viewInAssetStoreLink);
                }
            }

            if (info.documentationVisible)
            {
                if (GUILayout.Button("Documentation", GUILayout.Width(ButtonWidth)))
                {
                    // Open the documentation file in the asset folder
                    OpenAssetFile(info.documentationLink);
                }
            }

            if (info.exampleVisible)
            {
                if (GUILayout.Button("Examples", GUILayout.Width(ButtonWidth)))
                {
                    // Focus on the folder in the asset folder
                    FocusOnAssetFolder(info.exampleLink);
                }
            }

            GUILayout.EndHorizontal();

            // Draw divider line
            GUILayout.Space(10f); // Space before the line
            DrawDivider();
            GUILayout.Space(10f); // Space after the line

            EditorGUILayout.Space();
        }

        private void OpenAssetFile(string filePath)
        {
            string fullPath = "Assets/" + filePath; // Assuming filePath is relative to the "Assets" folder
            if (System.IO.File.Exists(fullPath))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(fullPath));
            }
            else
            {
                Debug.LogWarning("File not found: " + fullPath);
            }
        }

        private void FocusOnAssetFolder(string folderPath)
        {
            string fullPath = "Assets/" + folderPath; // Assuming folderPath is relative to the "Assets" folder
            if (System.IO.Directory.Exists(fullPath))
            {
                EditorUtility.FocusProjectWindow();
                Object folderAsset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
                Selection.activeObject = folderAsset;
            }
            else
            {
                Debug.LogWarning("Folder not found: " + fullPath);
            }
        }

        private void DrawDivider()
        {
            // Draw a horizontal line (divider)
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y += rect.height - 1; // Position it just below the last item
            rect.height = 1; // Height of the line
            EditorGUI.DrawRect(rect, Color.gray); // Color of the line
        }
    }
}
