using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using xasset;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Module
{
    /// <summary>
    /// 只跟hotfix有关系
    /// </summary>
    public enum CodeSource
    {
        Reflection,
        InnerDll,
        ServerDll
    }
    
    public class HotFixManager : Manager
    {
        public static BridgeBase bridge;
        protected override int runOrder { get; } = -998;

        protected override string processDiscription
        {
            get { return "脚本初始化完成"; }
        }

        private AppDomain appdomain;

        public static CodeSource codeSource = CodeSource.ServerDll;
        
        protected override void BeforeInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            ReadBytes((dll, pdb) =>
            {
                if (codeSource == CodeSource.Reflection)
                {
                    bridge = new AssemblyBridge(dll, pdb);
                }
                else
                {
                    bridge = new AppDomainBridge(dll, pdb);
                }

                runtime.NextAction();
            });
        }

        private void ReadBytes(Action<byte[],byte[]> callBack)
        {
            if (codeSource == CodeSource.ServerDll)
            {
                Assets.LoadAsync("Assets/Bundles/Code/Hotfix.dll.bytes", typeof(TextAsset)).onComplete += dll =>
                {
                    if (dll.error == null)
                    {
                        Assets.LoadAsync("Assets/Bundles/Code/Hotfix.pdb.bytes", typeof(TextAsset)).onComplete += pdb =>
                        {
                            if (pdb.error == null)
                            {
                                byte[] dllByte = (dll.asset as TextAsset).bytes;
                                byte[] pdbByte = (pdb.asset as TextAsset).bytes;
                                callBack(dllByte, pdbByte);
                            }
                        };
                    }
                };
            }
            else
            {
                string dllPath = GetPath("dll");
                using (WWW www = new WWW(dllPath))
                {
                    byte[] dll = www.bytes;
                    string pdbPath = GetPath("pdb");

                    using (WWW wwwPdb = new WWW(pdbPath))
                    {
                        byte[] pdb = wwwPdb.bytes;
                        callBack(dll, pdb);
                    }
                }
            }
        }
        
        private string GetPath(string extend)
        {
            return $"{Application.dataPath}/Bundles/Code/Hotfix.{extend}.bytes";
        }

        public static T GetInstance<T>(string type, params object[] args)
        {
            return (T)bridge.CreatInstance(type, args);
        }

        public static object GetInstance(string type, params object[] args)
        {
            return bridge.CreatInstance(type, args);
        }

        public static BridgeBase GetBridge()
        {
            bridge.iLBridge.Init(bridge);
            return bridge;
        }
    }


}
