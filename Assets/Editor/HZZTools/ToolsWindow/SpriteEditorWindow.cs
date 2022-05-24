using System;
using System.Collections.Generic;
using System.IO;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EditorModule
{
    public class SpriteEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/策划工具/图集动态图工具")]
        private static void OpenWindiow()
        {
            GetWindow<SpriteEditorWindow>();
        }

        public Vector2 maxTexSize = new Vector2(512, 512);
        public Vector2 altasSize = new Vector2(1024, 1024);
        public float areaPlus = 0.9f;
        [ListDrawerSettings(Expanded = true)] public List<IconAsset> asset;
        private SpriteLoader _assetSpriteLoader;


        protected override void OnEnable()
        {
            base.OnEnable();
            _assetSpriteLoader = AssetDatabase.LoadAssetAtPath<SpriteLoader>("Assets/Config/Icon.asset");
            asset = _assetSpriteLoader.iconInfo;
        }
        
        [Button("添加选中")]
        public void AddSelect()
        {
            var select = Selection.objects;
            for (int i = 0; i < select.Length; i++)
            {
                if (select[i] is Texture2D sp)
                {
                    string path = AssetDatabase.GetAssetPath(sp);
                    var sprit = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (!asset.Contains(ass => ass.icon == sprit))
                    {
                        asset.Add(new IconAsset() {icon = sprit});
                    }
                }
            }

            EditorUtility.SetDirty(_assetSpriteLoader);
        }

        [Button("清除相同")]
        public void ClearSame()
        {
            for (int i = asset.Count - 1; i >= 0; i--)
            {
                if (asset[i].icon == null)
                {
                    asset.RemoveAt(i);
                }
            }

            for (int i = 0; i < asset.Count - 1; i++)
            {
                for (int j = i + 1; j < asset.Count; j++)
                {
                    if (asset[i].icon == asset[j].icon)
                    {
                        asset.RemoveAt(j);
                        ClearSame();
                        return;
                    }
                }
            }

            EditorUtility.SetDirty(_assetSpriteLoader);
        }

        public void ClearAtlas()
        {
            //删除老图集
            string altasPath = string.Join("/", Application.dataPath,
                ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle),
                ConstKey.GetFolder(AssetLoad.AssetFolderType.Altas));

            DirectoryInfo altasDir = new DirectoryInfo(altasPath);
            FileInfo[] oldAltas = altasDir.GetFiles("*.spriteatlas");
            for (int i = 0; i < oldAltas.Length; i++)
            {
                string path = Pathelper.GetReleativePath(oldAltas[i].FullName);
                AssetDatabase.DeleteAsset(path);
            }
        }

        [Button("打图集")]
        private void MakeAltas()
        {
            ClearSame();
            ClearAtlas();

            //拿到不同UI的不同图集
            string dirPath = string.Join("/", Application.dataPath,
                ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle), ConstKey.GetFolder(AssetLoad.AssetFolderType.UI));
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            DirectoryInfo[] dirChild = dir.GetDirectories();
            Dictionary<string, List<Sprite>> canMakeAtlasTex = new Dictionary<string, List<Sprite>>();//可以打图集的图片

            HashSet<Sprite> unableMakeAtlas = new HashSet<Sprite>();
            for (int i = 0; i < dirChild.Length; i++)
            {
                var list = SearchDir(dirChild[i] , unableMakeAtlas);
                canMakeAtlasTex.Add(dirChild[i].Name, list);
            }

            //不可以加入图集的打印
            foreach (var sprite in unableMakeAtlas)
            {
                Debug.Log($"{sprite.texture.name},{sprite.rect.size},{sprite.texture.isReadable} 尺寸不符合，未加入图集");
            }

            //生成共同图集
            List<Sprite> commonAltas = new List<Sprite>();
            Dictionary<string, List<Sprite>> mirrorOne = new Dictionary<string, List<Sprite>>(canMakeAtlasTex);
            foreach (KeyValuePair<string, List<Sprite>> keyValue in canMakeAtlasTex)
            {
                for (int i = 0; i < keyValue.Value.Count; i++)
                {
                    foreach (KeyValuePair<string, List<Sprite>> mirrKey in mirrorOne)
                    {
                        if (mirrKey.Key != keyValue.Key)
                        {
                            if (mirrKey.Value.Contains(keyValue.Value[i]) && !commonAltas.Contains(keyValue.Value[i]))
                            {
                                commonAltas.Add(keyValue.Value[i]);
                            }
                        }
                    }
                }
            }

            List<SpriteAtlas> altasAsset = new List<SpriteAtlas>();
            foreach (KeyValuePair<string, List<Sprite>> keyValuePair in canMakeAtlasTex)
            {
                //生成不同UI的不同图集
                altasAsset.AddRange(CreatPrefabAltas(keyValuePair.Value, commonAltas, keyValuePair.Key));
            }

            //生成公共图集
            altasAsset.AddRange(CreatPrefabAltas(commonAltas, null, SpriteLoader.commonAtlas));
            //生成动态图集
            altasAsset.AddRange(CreatDynamicAltas(canMakeAtlasTex, commonAltas));

            SpriteAtlasUtility.PackAllAtlases(GetBuildTarget());

            for (int i = 0; i < asset.Count; i++)
            {
                for (int j = 0; j < altasAsset.Count; j++)
                {
                    if (altasAsset[j].CanBindTo(asset[i].icon))
                    {
                        asset[i].altasName = altasAsset[j].name;
                    }
                }
            }

            for (int i = 0; i < altasAsset.Count; i++)
            {
                EditorUtility.SetDirty(altasAsset[i]);
            }

            EditorUtility.SetDirty(_assetSpriteLoader);
            AssetDatabase.SaveAssets();
            var names = Enum.GetNames(typeof(ChannelType));
            for (int i = 0; i < names.Length; i++)
            {
                _assetSpriteLoader.ReleaseText(names[i]);
            }
            AssetDatabase.Refresh();
        }

        private List<Sprite> SearchDir(DirectoryInfo dir, HashSet<Sprite> unableMakeAtlas)
        {
            FileInfo[] files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
            List<Sprite> tex = new List<Sprite>();
            for (int i = 0; i < files.Length; i++)
            {
                string assetPath = Pathelper.GetReleativePath(files[i].FullName);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                Image[] images = go.transform.GetComponentsInChildren<Image>(true);

                for (int j = 0; j < images.Length; j++)
                {
                    Sprite sprite = images[j].sprite;
                    if (sprite != null && !tex.Contains(sprite))
                    {
                        if (CanSpriteMakeAtlas(sprite))
                        {
                            tex.Add(sprite);
                        }
                        else
                        {
                            unableMakeAtlas.Add(sprite);
                        }
                    }
                }
            }
            
            return tex;
        }

        /// <summary>
        /// 图片是否可以打图集
        /// </summary>
        private bool CanSpriteMakeAtlas(Sprite sprite)
        {
            return sprite.texture.width <= maxTexSize.x && sprite.texture.height <= maxTexSize.y && !sprite.texture.isReadable;
        }

        private BuildTarget GetBuildTarget()
        {
            if (Channel.channel == ChannelType.AppStore || Channel.channel == ChannelType.AppStoreCN)
            {
                return BuildTarget.iOS;
            }
            else
            {
                return BuildTarget.Android;
            }
        }

        private List<SpriteAtlas> CreatDynamicAltas(Dictionary<string, List<Sprite>> texutres, List<Sprite> commonAltas)
        {
            List<Sprite> resultAtts = new List<Sprite>();

            for (int i = 0; i < asset.Count; i++)
            {
                //如果在common图集里
                if (!commonAltas.Contains(asset[i].icon) && !ContainTexture(asset[i].icon, texutres))
                {
                    if (asset[i].icon != null)
                    {
                        resultAtts.Add(asset[i].icon);
                    }
                }
            }

            return SpiteAltas(resultAtts, SpriteLoader.dynamicAltas);
        }

        private bool ContainTexture(Sprite sp, Dictionary<string, List<Sprite>> texutres)
        {
            foreach (KeyValuePair<string, List<Sprite>> keyValuePair in texutres)
            {
                if (keyValuePair.Value.Contains(sp))
                {
                    return true;
                }
            }

            return false;
        }

        private List<SpriteAtlas> CreatPrefabAltas(List<Sprite> spriteList, List<Sprite> commonAltas, string path)
        {
            List<Sprite> addResult = new List<Sprite>(spriteList);
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (commonAltas != null && commonAltas.Contains(spriteList[i]))
                {
                    addResult.Remove(spriteList[i]);
                }
            }

            return SpiteAltas(addResult, path);
        }

        private List<SpriteAtlas> SpiteAltas(List<Sprite> addResult, string path)
        {
            addResult.Sort(
                (a, b) => -(b.texture.width * b.texture.height).CompareTo(a.texture.width * a.texture.height));

            List<SpriteAtlas> result = new List<SpriteAtlas>();

            //三件套
            List<Sprite> tempAdd = new List<Sprite>();
            SpriteAtlas atlas = CreatAltas();
            int area = 0;
            
            float areaThreshold = altasSize.x * altasSize.y * areaPlus;
            
            for (int i = 0; i < addResult.Count; i++)
            {
                tempAdd.Add(addResult[i]);
                //算面积
                area += (addResult[i].texture.width * addResult[i].texture.height);
                //如果面积大于临界值或者最后一个图还没到临界值,则加入到结果里
                if (area >= areaThreshold || i == addResult.Count - 1)
                {
                    result.Add(atlas);
                    AddSpriteToAltas(tempAdd, atlas);
                    if (i != addResult.Count - 1)
                    {
                        //新建三件套
                        tempAdd = new List<Sprite>();
                        atlas = CreatAltas();
                        area = 0;
                    }
                }
            }

            //生成所有altas
            for (int i = 0; i < result.Count; i++)
            {
                string extn = result.Count == 1 ? "" : " " + (i + 1);
                string name = path + extn;

                AssetDatabase.CreateAsset(result[i], GetAltasPath(name));
            }

            return result;
        }

        private void AddSpriteToAltas(List<Sprite> sp, SpriteAtlas altas)
        {
            List<Object> tar = new List<Object>(sp);
            altas.Add(tar.ToArray());
        }

        private SpriteAtlas CreatAltas()
        {
            SpriteAtlas atlas = new SpriteAtlas();

            // 设置参数 可根据项目具体情况进行设置
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                //Check this box to allow the Sprites to rotate when Unity packs them
                //into the Sprite Atlas. This maximizes the density of Sprites in the
                //combined Texture, and is enabled by default. Disable this option if
                //the Sprite Atlas contains Canvas UI element Textures, as when Unity
                //rotates the Textures in the Sprite Atlas during packing, it rotates
                //their orientation in the Scene as well.
                enableRotation = false,
                //Check this box to pack Sprites based on their Sprite outlines
                //instead of the default rectangle outline. This maximizes the
                //density of Sprites in the combined Texture, and is enabled by default.
                enableTightPacking = false,
                //Defines how many pixels are between the individual Sprite Textures
                //in the Sprite Atlas. This is a buffer to prevent pixel overlap
                //between Sprites that are next to each other in the Sprite Atlas.
                //The default value is 4 pixels.
                padding = 4,
            };
            atlas.SetPackingSettings(packSetting);

            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                //Enable this property to specify that the Texture is stored in gamma space.
                //This should always be checked for non-HDR color Textures (such as Albedo
                //and Specular Color). If the Texture stores information that has a specific
                //meaning, and you need the exact values in the Shader (for example, the
                //smoothness or the metalness), disable this property. This property is
                //enabled by default.
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);

            TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 1024,
                format = TextureImporterFormat.Automatic,
            };
            atlas.SetPlatformSettings(platformSetting);
            return atlas;
        }

        private string GetAltasPath(string name)
        {
            return string.Join("/", "Assets", ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle),
                ConstKey.GetFolder(AssetLoad.AssetFolderType.Altas), name + ".spriteatlas");
        }
    }
}