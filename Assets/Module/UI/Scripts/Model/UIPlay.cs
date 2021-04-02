/*
 * 脚本名称：UITween
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-06 15:56:14
 * 脚本作用：
*/

using System;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public enum UITweenType
    {
        None,
        Fade,
        Left,
        Right,
        Up,
        Down,
        Scale
    }

    public enum OpenFlag
    {
        /// <summary>
        /// ABCD---打开B---ABCDB
        /// </summary>
        Inorder,
        /// <summary>
        /// ABCD---打开B---AB
        /// </summary>
        Insertion,
    }
}
