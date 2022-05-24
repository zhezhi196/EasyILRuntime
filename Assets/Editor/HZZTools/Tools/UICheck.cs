using Module;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EditorModule
{
    public static class UICheck
    {
        [MenuItem("Tools/程序工具/UI/添加字体设置")]
        public static void AddFontSetting()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i] is GameObject go)
                {
                    Text[] texts = go.transform.GetComponentsInChildren<Text>(true);
                    for (int j = 0; j < texts.Length; j++)
                    {
                        FontSetting setting = texts[j].gameObject.AddOrGetComponent<FontSetting>();
                        setting.text = texts[j];
                    }
                }
                EditorUtility.SetDirty(Selection.objects[i]);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/程序工具/UI/添加字体自匹配")]
        public static void AddTextBestFit()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i] is GameObject go)
                {
                    Text[] texts = go.transform.GetComponentsInChildren<Text>(true);
                    for (int j = 0; j < texts.Length; j++)
                    {
                        texts[j].resizeTextForBestFit = true;
                        texts[j].resizeTextMinSize = (int) (texts[j].fontSize * 0.5f);
                    }
                }
                EditorUtility.SetDirty(Selection.objects[i]);
            }
            AssetDatabase.Refresh();
        }
    }
}