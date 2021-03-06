/*
 * 脚本名称：CameraEffect
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-09-07 17:38:36
 * 脚本作用：
*/

using UnityEngine;

namespace Module
{ 
    public class GameCamera : MonoBehaviour
    {
        public Camera camera { get; protected set; }

        protected virtual void Awake()
        {
            this.enabled = false;
            CameraController.Instance.cameraList.Add(gameObject.name, this);
            camera = GetComponent<Camera>();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            CameraController.Instance.OnRenderImage(source, destination);
        }

        protected virtual void OnDestroy()
        {
            CameraController.Instance.cameraList.Remove(gameObject.name);
        }


    }
}