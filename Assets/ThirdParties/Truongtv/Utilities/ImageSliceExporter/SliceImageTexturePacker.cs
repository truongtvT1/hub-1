#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Truongtv.Utilities.ImageSliceExporter
{
    [Serializable]
    public class SliceImageTexturePacker:OdinEditorWindow
    {
        [MenuItem("Truongtv/Utilities/Slice Atlas Image")]
        private static void OpenWindow()
        {
            GetWindow<SliceImageTexturePacker>().Show();
        }
        [ListDrawerSettings(Expanded = true),SerializeField] private Texture2D[] textureSlices;
        [SerializeField,FolderPath]private string outputDirectory;
        private struct SheetFrame
        {
            public string Name;
            // public Vector2 Pivot;
            public Rect Rect;
        }
        [Button]
        public void ImageSliceExporter()
        {
            if (textureSlices==null||textureSlices.Length==0)
            {
                EditorUtility.DisplayDialog("ImageSliceExporter", "Please select texture", "OK");
                return;
            }
            if (string.IsNullOrEmpty(outputDirectory))
            {
                EditorUtility.DisplayDialog("ImageSliceExporter", "Please select Output of Directory", "OK");
                return;
            }
            foreach (var activeObject in textureSlices){
                var selectedTexture = activeObject;
                if (selectedTexture == null) continue;
                var assetPath = AssetDatabase.GetAssetPath(selectedTexture);
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (!importer.isReadable)
                {
                    EditorUtility.DisplayDialog("ImageSliceExporter", "Please tick to advance/read/write enable of this texture2D.\nTexture2D name = "+activeObject.name, "OK");
                    return; 
                }
            }
            foreach (var activeObject in textureSlices)
            {
                var selectedTexture = activeObject;
                if (selectedTexture == null) continue;
                var assetPath = AssetDatabase.GetAssetPath(selectedTexture);
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (!importer) continue;
                var frames = new List<SheetFrame>();
                
                if (importer != null)
                    foreach (var spriteMetaData in importer.spritesheet)
                    {
                        var frame = new SheetFrame {Name = spriteMetaData.name};


                        // Center = 0, TopLeft = 1, TopCenter = 2, 
                        // TopRight = 3, LeftCenter = 4, RightCenter = 5, 
                        // BottomLeft = 6, BottomCenter = 7, BottomRight = 8, Custom = 9.
                        // switch (spriteMetaData.alignment)
                        // {
                        //     case 0:
                        //         frame.Pivot.x = 0.5f;
                        //         frame.Pivot.y = 0.5f;
                        //         break;
                        //     case 1:
                        //         frame.Pivot.x = 0.0f;
                        //         frame.Pivot.y = 1.0f;
                        //         break;
                        //     case 2:
                        //         frame.Pivot.x = 0.5f;
                        //         frame.Pivot.y = 1.0f;
                        //         break;
                        //     case 3:
                        //         frame.Pivot.x = 1.0f;
                        //         frame.Pivot.y = 1.0f;
                        //         break;
                        //     case 4:
                        //         frame.Pivot.x = 0.0f;
                        //         frame.Pivot.y = 0.5f;
                        //         break;
                        //     case 5:
                        //         frame.Pivot.x = 1.0f;
                        //         frame.Pivot.y = 0.5f;
                        //         break;
                        //     case 6:
                        //         frame.Pivot.x = 0.0f;
                        //         frame.Pivot.y = 0.0f;
                        //         break;
                        //     case 7:
                        //         frame.Pivot.x = 0.5f;
                        //         frame.Pivot.y = 0.0f;
                        //         break;
                        //     case 8:
                        //         frame.Pivot.x = 1.0f;
                        //         frame.Pivot.y = 0.0f;
                        //         break;
                        //     case 9:
                        //         frame.Pivot = spriteMetaData.pivot;
                        //         break;
                        // }

                        frame.Rect = spriteMetaData.rect;

                        frames.Add(frame);
                    }

                // export sliced PNG file
                foreach (var frame in frames)
                {
                    // copy frame image data
                    var x = Mathf.FloorToInt(frame.Rect.x);
                    var y = Mathf.FloorToInt(frame.Rect.y);
                    var width = Mathf.FloorToInt(frame.Rect.width);
                    var height = Mathf.FloorToInt(frame.Rect.height);
                    var pix = selectedTexture.GetPixels(x, y, width, height);
                    var frameTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
                    frameTex.SetPixels(pix);
                    frameTex.Apply();

                    // save PNG file
                    var bytes = frameTex.EncodeToPNG();
                    Object.DestroyImmediate(frameTex);
                    var outputPath = Path.Combine(outputDirectory, frame.Name) + ".png";
                    File.WriteAllBytes(outputPath, bytes);
                }
            } // foreach (Object activeObject in selectedTextures)
            EditorUtility.DisplayDialog("ImageSliceExporter", "Export finished!", "OK");

        }

    }
}
#endif
