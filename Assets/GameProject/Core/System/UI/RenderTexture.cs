﻿using UnityEngine;

public class RenderTextureTools
{
    private static UnityEngine.RenderTexture _commonTexture;
    /// <summary>
    /// 这个是背包和物品详细展示的通用renderTexture，是由UIRoot上的3DParent/Camera输出的
    /// </summary>
    public static UnityEngine.RenderTexture commonTexture
    {
        get
        {
            if (_commonTexture == null)
            {
                _commonTexture = Resources.Load<UnityEngine.RenderTexture>("RenderTexture/Bag");
            }

            return _commonTexture;

        }
    }

    private static UnityEngine.RenderTexture _videoTexture;
    public static UnityEngine.RenderTexture videoTexture
    {
        get
        {
            if (_videoTexture == null)
            {
                _videoTexture = Resources.Load<UnityEngine.RenderTexture>("RenderTexture/Video");
            }

            return _videoTexture;

        }
    }
}
