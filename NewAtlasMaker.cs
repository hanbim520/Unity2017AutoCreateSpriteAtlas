using UnityEngine;

using UnityEngine.Events;

using UnityEngine.UI;

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using System.IO;
using System.Linq;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor.PackageManager;

namespace Tools
{
    public class NewAtlasMaker : EditorWindow
    {
        private static string sptDesDir = Application.dataPath + "/Resources";
        private static string sptSrcDir = Application.dataPath + "/Art";

        [MenuItem("Tools/NewAtlasMaker By Folders")]

        public static void CreateAtlasByFolders()
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(sptSrcDir);
            //add folders
            List<Object> folders = new List<Object>();
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                folders.Clear();
                if (dirInfo != null)
                {
                    string assetPath = dirInfo.FullName.Substring(dirInfo.FullName.IndexOf("Assets"));
                    var o = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPath);
                    if (IsPackable(o))
                        folders.Add(o);
                }
                string atlasName = dirInfo.Name + ".spriteatlas";
                CreateAtlas(atlasName);
                SpriteAtlas sptAtlas = Resources.Load<SpriteAtlas>(dirInfo.Name);
                Debug.Log(sptAtlas.tag);
                AddPackAtlas(sptAtlas, folders.ToArray());
            }

            //add texture by your self
        }

        [MenuItem("Tools/NewAtlasMaker By Sprite")]
        public static void CreateAtlasBySprite()
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(sptSrcDir);

            //add sprite

            List<Sprite> spts = new List<Sprite>();
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                spts.Clear();
                foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (IsPackable(sprite))
                        spts.Add(sprite);
                }
                string atlasName = dirInfo.Name + ".spriteatlas";
                CreateAtlas(atlasName);
                SpriteAtlas sptAtlas = Resources.Load<SpriteAtlas>(dirInfo.Name);
                Debug.Log(sptAtlas.tag);
                AddPackAtlas(sptAtlas, spts.ToArray());
            }
            

            //add texture by your self
        }
        static bool IsPackable(Object o)
        {
            return o != null && (o.GetType() == typeof(Sprite) || o.GetType() == typeof(Texture2D) || (o.GetType() == typeof(DefaultAsset) && ProjectWindowUtil.IsFolder(o.GetInstanceID())));
        }

        static void AddPackAtlas(SpriteAtlas atlas, Object[] spt)
        {
            MethodInfo methodInfo = System.Type
                 .GetType("UnityEditor.U2D.SpriteAtlasExtensions, UnityEditor")
                 .GetMethod("Add", BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null)
                methodInfo.Invoke(null, new object[] { atlas, spt });
            else
                Debug.Log("methodInfo is null");
            PackAtlas(atlas);
        }

        static void PackAtlas(SpriteAtlas atlas)
        {
            System.Type
                .GetType("UnityEditor.U2D.SpriteAtlasUtility, UnityEditor")
                .GetMethod("PackAtlases", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { new[] { atlas }, EditorUserBuildSettings.activeBuildTarget });
        }

        public static void CreateAtlas(string atlasName)
        {
            string yaml = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!687078895 &4343727234628468602
SpriteAtlas:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_Name: New Sprite Atlas
  m_EditorData:
    textureSettings:
      serializedVersion: 2
      anisoLevel: 1
      compressionQuality: 50
      maxTextureSize: 2048
      textureCompression: 0
      filterMode: 1
      generateMipMaps: 0
      readable: 0
      crunchedCompression: 0
      sRGB: 1
    platformSettings: []
    packingParameters:
      serializedVersion: 2
      padding: 4
      blockOffset: 1
      allowAlphaSplitting: 0
      enableRotation: 1
      enableTightPacking: 1
    variantMultiplier: 1
    packables: []
    bindAsDefault: 1
  m_MasterAtlas: {fileID: 0}
  m_PackedSprites: []
  m_PackedSpriteNamesToIndex: []
  m_Tag: New Sprite Atlas
  m_IsVariant: 0
";
            AssetDatabase.Refresh();

            if (!Directory.Exists(sptDesDir ))
            {
                Directory.CreateDirectory(sptDesDir );
                AssetDatabase.Refresh();
            }
            string filePath = sptDesDir + "/" + atlasName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                AssetDatabase.Refresh();
            }
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            byte[] bytes = new UTF8Encoding().GetBytes(yaml);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            AssetDatabase.Refresh();
        }

    }

}
