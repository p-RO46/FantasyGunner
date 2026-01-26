using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JankyArrayMaker
{
    [CustomEditor(typeof(JankyArrayMaker))]
    public class JankyArrayMakerEditor : Editor
    {
        private JankyArrayMaker maker;
        private Vector2 scrollPos;

        private void OnEnable()
        {
            maker = (JankyArrayMaker)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Janky Array Maker", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            maker.arrayName = EditorGUILayout.TextField(new GUIContent("Array Name", "Base name for the generated texture arrays."), maker.arrayName);

            int newResolution = EditorGUILayout.IntField(new GUIContent("Output Resolution", "Resolution for the output texture arrays (must be a power of two)."), maker.outputResolution);
            if (newResolution != maker.outputResolution)
            {
                if (IsPowerOfTwo(newResolution) && newResolution > 0)
                {
                    maker.outputResolution = newResolution;
                }
                else
                {
                    EditorGUILayout.HelpBox("Resolution must be a positive power of two.", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Export Folder", "Destination folder within the Assets directory for saving the texture arrays."), GUILayout.Width(100));
            maker.exportFolder = EditorGUILayout.TextField(maker.exportFolder);
            if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Select Export Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    if (selectedFolder.StartsWith(Application.dataPath))
                    {
                        maker.exportFolder = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder within the Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Texture Types", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Texture Type"))
            {
                maker.textureTypes.Add("NewTextureType");
                foreach (var material in maker.materials)
                {
                    material.textures.Add(new TextureEntry());
                }
            }

            for (int i = 0; i < maker.textureTypes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                maker.textureTypes[i] = EditorGUILayout.TextField(new GUIContent($"Texture Type {i + 1}", "Name of the texture type (e.g., Albedo, Normal)."), maker.textureTypes[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    maker.textureTypes.RemoveAt(i);
                    foreach (var material in maker.materials)
                    {
                        if (i < material.textures.Count)
                        {
                            material.textures.RemoveAt(i);
                        }
                    }
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            GUILayout.Label("Materials", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Material"))
            {
                MaterialEntry newMaterial = new MaterialEntry();
                for (int j = 0; j < maker.textureTypes.Count; j++)
                {
                    newMaterial.textures.Add(new TextureEntry());
                }
                maker.materials.Add(newMaterial);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));

            for (int i = 0; i < maker.materials.Count; i++)
            {
                MaterialEntry material = maker.materials[i];
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                material.foldout = EditorGUILayout.Foldout(material.foldout, $"Material {i + 1}", true);
                if (GUILayout.Button("Up", GUILayout.Width(40)) && i > 0)
                {
                    var temp = maker.materials[i];
                    maker.materials[i] = maker.materials[i - 1];
                    maker.materials[i - 1] = temp;
                }
                if (GUILayout.Button("Down", GUILayout.Width(50)) && i < maker.materials.Count - 1)
                {
                    var temp = maker.materials[i];
                    maker.materials[i] = maker.materials[i + 1];
                    maker.materials[i + 1] = temp;
                }
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    maker.materials.RemoveAt(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                if (material.foldout)
                {
                    EditorGUI.indentLevel++;
                    GUILayout.Label("Textures", EditorStyles.boldLabel);
                    for (int j = 0; j < maker.textureTypes.Count; j++)
                    {
                        if (j >= material.textures.Count)
                        {
                            material.textures.Add(new TextureEntry());
                        }

                        TextureEntry textureEntry = material.textures[j];
                        GUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent($"Texture: {maker.textureTypes[j]}", "Assign the texture here."), GUILayout.Width(150));
                        textureEntry.texture = (Texture2D)EditorGUILayout.ObjectField(textureEntry.texture, typeof(Texture2D), false, GUILayout.Width(150));
                        textureEntry.optionsFoldout = EditorGUILayout.Foldout(textureEntry.optionsFoldout, "Options", true);
                        EditorGUILayout.EndHorizontal();

                        if (textureEntry.optionsFoldout)
                        {
                            EditorGUILayout.BeginHorizontal();
                            textureEntry.invert = EditorGUILayout.Toggle(new GUIContent("Invert", "Invert the colors of this texture."), textureEntry.invert);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            textureEntry.normalize = EditorGUILayout.Toggle(new GUIContent("Normalize", "Normalize the texture to a 0-1 luminance range."), textureEntry.normalize);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            textureEntry.useColorize = EditorGUILayout.Toggle(new GUIContent("Use Colorize", "Enable color multiplication for this texture."), textureEntry.useColorize);
                            EditorGUILayout.EndHorizontal();
                            if (textureEntry.useColorize)
                            {
                                EditorGUILayout.BeginVertical();
                                textureEntry.colorize = EditorGUILayout.ColorField(new GUIContent("Colorize", "Multiply the texture color by this color."), textureEntry.colorize);
                                textureEntry.colorizeWeight = EditorGUILayout.Slider(new GUIContent("Weight", "Weight of the colorize effect."), textureEntry.colorizeWeight, 0f, 1f);
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.BeginHorizontal();
                            textureEntry.useRemapGradient = EditorGUILayout.Toggle(new GUIContent("Use Remap Gradient", "Enable remapping of grayscale values using a gradient."), textureEntry.useRemapGradient);
                            EditorGUILayout.EndHorizontal();
                            if (textureEntry.useRemapGradient)
                            {
                                EditorGUILayout.BeginVertical();
                                textureEntry.remapGradient = EditorGUILayout.GradientField(new GUIContent("Remap Gradient", "Remap the grayscale values of the texture using this gradient."), textureEntry.remapGradient);
                                textureEntry.remapGradientWeight = EditorGUILayout.Slider(new GUIContent("Weight", "Weight of the remap gradient effect."), textureEntry.remapGradientWeight, 0f, 1f);
                                EditorGUILayout.EndVertical();
                            }

                            GUILayout.BeginVertical("box");
                            GUILayout.Label("Tiling and Offset", EditorStyles.boldLabel);
                            textureEntry.tiling = EditorGUILayout.Vector2Field(new GUIContent("Tiling", "Scale of the texture on the X and Y axes."), textureEntry.tiling);
                            textureEntry.offset = EditorGUILayout.Vector2Field(new GUIContent("Offset", "Offset of the texture on the X and Y axes."), textureEntry.offset);
                            GUILayout.EndVertical();
                        }

                        GUILayout.EndVertical();
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Arrays"))
            {
                maker.CreateTextureArrays();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(maker);
            }
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0 && x > 0;
        }
    }
}
