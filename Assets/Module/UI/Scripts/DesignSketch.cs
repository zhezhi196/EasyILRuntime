using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace EditorModule
{
    //[ExecuteInEditMode]
    public class DesignSketch: MonoBehaviour
    {
        public Image image;
        public float touming = 0.5f;
        public float butouming = 1;
        private void Awake()
        {
            image = GetComponent<Image>();
        }
        [Button]
        public void Show()
        {
            image.color = new Color(1, 1, 1, butouming);

        }
        [Button]
        public void Hide()
        {
            image.color = new Color(1, 1, 1, touming);
            //.canvasRenderer.GetColor();
        }
        
    }
}