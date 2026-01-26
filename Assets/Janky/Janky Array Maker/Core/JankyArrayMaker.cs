using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace JankyArrayMaker
{
    [System.Serializable]
    public class TextureEntry
    {
        public Texture2D texture;
        public bool invert;
        public bool normalize = false;
        public bool useColorize = false;
        public Color colorize = Color.white;
        public float colorizeWeight = 1f;
        public bool useRemapGradient = false;
        public Gradient remapGradient = null;
        public float remapGradientWeight = 1f;
        public Vector2 tiling = Vector2.one;
        public Vector2 offset = Vector2.zero;
        public bool optionsFoldout = false;

        public TextureEntry()
        {
            if (remapGradient == null)
            {
                remapGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0].color = Color.black;
                colorKeys[0].time = 0f;
                colorKeys[1].color = Color.white;
                colorKeys[1].time = 1f;
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0].alpha = 1f;
                alphaKeys[0].time = 0f;
                alphaKeys[1].alpha = 1f;
                alphaKeys[1].time = 1f;
                remapGradient.colorKeys = colorKeys;
                remapGradient.alphaKeys = alphaKeys;
            }
        }
    }

    [System.Serializable]
    public class MaterialEntry
    {
        public bool foldout = true;
        public List<TextureEntry> textures = new List<TextureEntry>();
    }

    [CreateAssetMenu(fileName = "JankyArrayMaker", menuName = "Janky/Janky Array Maker/Create New", order = 1)]
    public class JankyArrayMaker : ScriptableObject
    {
        public List<MaterialEntry> materials = new List<MaterialEntry>();
        public List<string> textureTypes = new List<string>();
        public int outputResolution = 1024;
        public string arrayName = "TextureArray";
        public string exportFolder = "Assets";

        public void CreateTextureArrays()
        {
            try
            {
                if (textureTypes.Count == 0)
                {
                    Debug.LogError("No texture types defined.");
                    return;
                }

                if (materials.Count == 0)
                {
                    Debug.LogError("No materials added.");
                    return;
                }

                if (!AssetDatabase.IsValidFolder(exportFolder))
                {
                    Debug.LogError($"Export folder '{exportFolder}' does not exist.");
                    return;
                }

                EditorUtility.DisplayProgressBar("Creating Texture Arrays", "Initializing...", 0f);
                int totalSteps = textureTypes.Count * materials.Count;
                int currentStep = 0;

                for (int textureTypeIndex = 0; textureTypeIndex < textureTypes.Count; textureTypeIndex++)
                {
                    string textureType = textureTypes[textureTypeIndex];

                    List<Texture2D> texturesForArray = new List<Texture2D>();
                    List<bool> invertFlags = new List<bool>();
                    List<Vector2> tilings = new List<Vector2>();
                    List<Vector2> offsets = new List<Vector2>();
                    List<bool> useColorizeList = new List<bool>();
                    List<Color> colorizes = new List<Color>();
                    List<float> colorizeWeights = new List<float>();
                    List<bool> useRemapGradientList = new List<bool>();
                    List<Gradient> remapGradients = new List<Gradient>();
                    List<float> remapGradientWeights = new List<float>();
                    List<bool> normalizeFlags = new List<bool>();

                    foreach (var material in materials)
                    {
                        if (textureTypeIndex >= material.textures.Count)
                        {
                            Debug.LogWarning($"Material has fewer texture entries than texture types. Skipping.");
                            continue;
                        }

                        TextureEntry textureEntry = material.textures[textureTypeIndex];
                        Texture2D tex = textureEntry.texture;
                        bool invert = textureEntry.invert;

                        if (tex != null)
                        {
                            texturesForArray.Add(tex);
                            invertFlags.Add(invert);
                        }
                        else
                        {
                            Debug.LogWarning($"A material is missing texture '{textureType}'. Using default black texture.");
                            texturesForArray.Add(Texture2D.blackTexture);
                            invertFlags.Add(false);
                        }

                        tilings.Add(textureEntry.tiling);
                        offsets.Add(textureEntry.offset);
                        useColorizeList.Add(textureEntry.useColorize);
                        colorizes.Add(textureEntry.colorize);
                        colorizeWeights.Add(textureEntry.colorizeWeight);
                        useRemapGradientList.Add(textureEntry.useRemapGradient);
                        remapGradients.Add(textureEntry.remapGradient);
                        remapGradientWeights.Add(textureEntry.remapGradientWeight);
                        normalizeFlags.Add(textureEntry.normalize);

                        currentStep++;
                        EditorUtility.DisplayProgressBar("Creating Texture Arrays", $"Processing {textureType} textures...", (float)currentStep / totalSteps);
                    }

                    int arraySize = texturesForArray.Count;
                    Texture2DArray textureArray = new Texture2DArray(outputResolution, outputResolution, arraySize, TextureFormat.RGBA32, true);

                    for (int i = 0; i < texturesForArray.Count; i++)
                    {
                        Texture2D tex = texturesForArray[i];
                        Vector2 tiling = tilings[i];
                        Vector2 offset = offsets[i];
                        bool useColorize = useColorizeList[i];
                        Color colorize = colorizes[i];
                        float colorizeWeight = colorizeWeights[i];
                        bool useRemapGradient = useRemapGradientList[i];
                        Gradient remapGradient = remapGradients[i];
                        float remapGradientWeight = remapGradientWeights[i];
                        bool normalize = normalizeFlags[i];

                        Texture2D resizedTex = ResizeTexture(tex, outputResolution, outputResolution);
                        resizedTex = ApplyTilingOffset(resizedTex, tiling, offset);

                        if (invertFlags[i])
                        {
                            resizedTex = InvertTexture(resizedTex);
                        }

                        if (normalize)
                        {
                            resizedTex = NormalizeTexture(resizedTex);
                        }

                        if (useColorize || useRemapGradient)
                        {
                            resizedTex = ApplyColorModifications(resizedTex, useColorize, colorize, colorizeWeight, useRemapGradient, remapGradient, remapGradientWeight);
                        }

                        for (int mip = 0; mip < resizedTex.mipmapCount; mip++)
                        {
                            Graphics.CopyTexture(resizedTex, 0, mip, textureArray, i, mip);
                        }
                    }

                    string path = $"{exportFolder}/{arrayName}_{textureType}.asset";
                    AssetDatabase.CreateAsset(textureArray, path);
                    Debug.Log($"Texture Array '{path}' created.");
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Texture Arrays creation completed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while creating texture arrays: {ex.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(source, rt);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, source.mipmapCount > 1);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            return result;
        }

        private Texture2D InvertTexture(Texture2D source)
        {
            Texture2D invertedTex = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);
            Color[] pixels = source.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color c = pixels[i];
                c.r = 1.0f - c.r;
                c.g = 1.0f - c.g;
                c.b = 1.0f - c.b;
                pixels[i] = c;
            }

            invertedTex.SetPixels(pixels);
            invertedTex.Apply();

            return invertedTex;
        }

        private Texture2D ApplyTilingOffset(Texture2D source, Vector2 tiling, Vector2 offset)
        {
            RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height);
            Material tilingOffsetMaterial = new Material(Shader.Find("Hidden/TileOffsetShader"));
            tilingOffsetMaterial.SetVector("_TileOffset", new Vector4(tiling.x, tiling.y, offset.x, offset.y));
            Graphics.Blit(source, rt, tilingOffsetMaterial);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D result = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);
            result.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            result.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            DestroyImmediate(tilingOffsetMaterial);
            return result;
        }

        private Texture2D ApplyColorModifications(Texture2D source, bool useColorize, Color colorize, float colorizeWeight, bool useRemapGradient, Gradient remapGradient, float remapGradientWeight)
        {
            Texture2D modifiedTex = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);
            Color[] pixels = source.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color c = pixels[i];

                if (useColorize)
                {
                    c = Color.Lerp(c, c * colorize, colorizeWeight);
                }

                if (useRemapGradient && remapGradient != null)
                {
                    float luminance = c.grayscale;
                    Color remappedColor = remapGradient.Evaluate(luminance);
                    c = Color.Lerp(c, remappedColor, remapGradientWeight);
                }

                pixels[i] = c;
            }

            modifiedTex.SetPixels(pixels);
            modifiedTex.Apply();

            return modifiedTex;
        }

        private Texture2D NormalizeTexture(Texture2D source)
        {
            Texture2D normalizedTex = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);
            Color[] pixels = source.GetPixels();

            float minLuminance = float.MaxValue;
            float maxLuminance = float.MinValue;

            for (int i = 0; i < pixels.Length; i++)
            {
                float luminance = pixels[i].grayscale;
                if (luminance < minLuminance) minLuminance = luminance;
                if (luminance > maxLuminance) maxLuminance = luminance;
            }

            float luminanceRange = maxLuminance - minLuminance;
            if (luminanceRange < Mathf.Epsilon)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    float luminance = pixels[i].grayscale;
                    pixels[i] = new Color(luminance, luminance, luminance, pixels[i].a);
                }
            }
            else
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    float luminance = pixels[i].grayscale;
                    float normalizedLuminance = (luminance - minLuminance) / luminanceRange;
                    pixels[i] = new Color(normalizedLuminance, normalizedLuminance, normalizedLuminance, pixels[i].a);
                }
            }

            normalizedTex.SetPixels(pixels);
            normalizedTex.Apply();

            return normalizedTex;
        }
    }
}
