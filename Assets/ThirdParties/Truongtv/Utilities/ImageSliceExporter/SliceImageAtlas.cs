#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SliceImageAtlas : OdinEditorWindow
{
    [MenuItem("Truongtv/Utilities/Slice Image")]
    private static void OpenWindow()
    {
        GetWindow<SliceImageAtlas>().Show();
    }
    [SerializeField] private Texture2D textureSlices;
    [SerializeField] private TextAsset atlasFile;
    [SerializeField,FolderPath]private string outputDirectory;

    [SerializeField] private Alignment align;
    [SerializeField] private List<Frame> frames;
    [Serializable]
    private class Frame
    {
        public string name;
        public bool isRotate;
        public Vector2 position;
        public Vector2 size;
        public Vector2 origin;
    }

    private enum Alignment{
        Center = 0, TopLeft = 1, TopCenter = 2, 
         TopRight = 3, LeftCenter = 4, RightCenter = 5, 
         BottomLeft = 6, BottomCenter = 7, BottomRight = 8
    }
    readonly string[] splitLine = { "\r\n", "\r", "\n" };
    [Button]
    private void Splice()
    {
        if (textureSlices==null)
        {
            EditorUtility.DisplayDialog("ImageSliceExporter", "Please select texture", "OK");
            return;
        }
        if (atlasFile==null)
        {
            EditorUtility.DisplayDialog("ImageSliceExporter", "Please select Atlas File", "OK");
            return;
        }
        if (string.IsNullOrEmpty(outputDirectory))
        {
            EditorUtility.DisplayDialog("ImageSliceExporter", "Please select Output of Directory", "OK");
            return;
        }
        var assetPath = AssetDatabase.GetAssetPath(textureSlices);
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (!importer.isReadable)
        {
            EditorUtility.DisplayDialog("ImageSliceExporter", "Please tick to advance/read/write enable of this texture2D.\nTexture2D name = "+textureSlices.name, "OK");
            return; 
        }
        // pair Data text
        var totalLine = atlasFile.text.Split(splitLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        var whLine = totalLine[1].Split(':')[1];
        var width = int.Parse(whLine.Split(',')[0].Trim());
        var height = int.Parse(whLine.Split(',')[1].Trim());
        totalLine.RemoveRange(0,5);
        var numObj = totalLine.Count / 7;
        frames = new List<Frame>();
        for (var i = 0; i < numObj; i++)
        {
            var index = i * 7;
            var nameLine = totalLine[index].Trim();
            var rotate = totalLine[index+1].Contains("true");
            
            var sizeLine = totalLine[index + 3].Split(':')[1];
            var xy = sizeLine.Split(',');
            var size = new Vector2(int.Parse(xy[0].Trim()),int.Parse(xy[1].Trim()));
            
            var originLine = totalLine[index + 4].Split(':')[1];
            xy = originLine.Split(',');
            var origin = new Vector2(int.Parse(xy[0].Trim()),int.Parse(xy[1].Trim()));
            
            var xyLine = totalLine[index + 2].Split(':')[1];
            xy = xyLine.Split(',');
            var y = height - int.Parse(xy[1].Trim()) - (rotate ? size.x : size.y);
            var position = new Vector2(int.Parse(xy[0].Trim()),y);
            //TODO: offset and index
            var frame = new Frame
            {
                name = nameLine,
                isRotate = rotate,
                position = position,
                size = size,
                origin = origin
            };
            frames.Add(frame);
        }
        // split image
        for (var i = 0; i < frames.Count; i++)
        {
            var frame = frames[i];
            var x = Mathf.FloorToInt(frame.position.x);
            var y = Mathf.FloorToInt(frame.position.y);
            var widthImg =frame.isRotate?Mathf.FloorToInt(frame.size.y): Mathf.FloorToInt(frame.size.x);
            var heightImg = frame.isRotate?Mathf.FloorToInt(frame.size.x):Mathf.FloorToInt(frame.size.y);
            var pix = textureSlices.GetPixels(x, y, widthImg, heightImg);

            var data = pix;
            
            var frameTex = new Texture2D(widthImg, heightImg, TextureFormat.ARGB32, false);
            frameTex.SetPixels(data);
            frameTex.Apply();
            if (frame.isRotate)
            {
                frameTex = rotateTexture(frameTex, true);
                //frameTex.Rotate(Texture2DExtensions.Rotation.Right);
            }
            // save PNG file
            var bytes = frameTex.EncodeToPNG();
            Object.DestroyImmediate(frameTex);
            var outputPath = Path.Combine(outputDirectory, frame.name) + ".png";
            var path = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(outputPath, bytes);
        }
        EditorUtility.DisplayDialog("Slice Atlas Image", "Export finished!", "OK");
    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;
 
        int iRotated, iOriginal;
 
        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }
 
        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}

#endif