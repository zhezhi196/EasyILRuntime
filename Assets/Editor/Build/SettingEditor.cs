using System;
using System.IO;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using xasset;
using xasset.editor;
using Debug = UnityEngine.Debug;

namespace ModuleEditor
{
    public class SettingEditor : OdinEditorWindow
    {
        private const string localUrl = "https://127.0.0.1:7888";

        [MenuItem("Setting/Setting")]
        public static void OpenSetting()
        {
            GetWindow<SettingEditor>().Show();
        }

        [EnumPaging, OnValueChanged("OnChannelChanged"), HorizontalGroup("ChannelChanged")]
        public Channel channel;

        [Button("拷贝资源到streaming"), HorizontalGroup("ChannelChanged")]
        public void CopyStreamAsset()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, AssetPath.AssetBundles));
            AssetDatabase.Refresh();
        }

        public string hotfixUrl;
        private AssetsManifest _manifest;
        private Settings bundleSetting;

        private void OnEnable()
        {
            if (bundleSetting == null)
            {
                bundleSetting = BuildScript.GetSettings();
            }

            _manifest = BuildScript.GetManifest();
            channel = _manifest.channel;

            if (channel == Channel.LocalDebug)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    hotfixUrl = localUrl;
                }
                else
                {
                    hotfixUrl = _manifest.localServer;
                }
            }
            else
            {
                hotfixUrl = _manifest.remoteServer;
            }
        }

        private void OnChannelChanged()
        {
            _manifest.channel = channel;

            if (channel == Channel.LocalDebug)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    hotfixUrl = localUrl;
                }
                else
                {
                    hotfixUrl = _manifest.localServer;
                }
            }
            else
            {
                hotfixUrl = _manifest.remoteServer;
            }
        }

        [Button("打热更包"), HorizontalGroup("操作")]
        public void BuildBundle()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }

        [Button("关闭资源服务器"), HorizontalGroup("操作")]
        public void OpenServer()
        {
            if (LaunchLocalServer.IsRunning())
                LaunchLocalServer.KillRunningAssetBundleServer();
        }

        [Button("上传服务器"), HorizontalGroup("操作")]
        public void UploadBundles()
        {
            Debug.Log(Application.dataPath);
        }

        [Button("生成ILBinding"), HorizontalGroup("操作")]
        public void GenerateILBinding()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.FileStream dllFs = new System.IO.FileStream("Assets/Bundles/Code/Hotfix.dll.bytes",
                System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                appdomain.LoadAssembly(dllFs);
                ILBindingHelper.Binding(appdomain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appdomain,
                    "Assets/Module/ILRuntime/Generated");
                AssetDatabase.Refresh();
            }
        }
    }
}