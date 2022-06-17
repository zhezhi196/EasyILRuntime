using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameProject
{
    public class MapSingleArea : MonoBehaviour
    {
        public MapSingleChunkArea[] areas;
        public string[] node;


#if UNITY_EDITOR
        [Button]
        public void EditorInit()
        {
            areas = transform.GetComponentsInChildren<MapSingleChunkArea>();
        }
        [Button]
        public void AddFontSetting()
        {
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i].title.gameObject.AddComponent<FontSetting>();
            }
        }
#endif
    }
}