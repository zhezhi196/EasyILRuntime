using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Module
{
    public abstract class BridgeBase
    {
        public const string BridgeFullName = "HotFix.GamePlay";
        public readonly Dictionary<string, Type> allTypes = new Dictionary<string, Type>();
        public abstract object CreatInstance(string type,params object[] args);
        public BridgeBase(byte[] dllByte, byte[] pdbByte)
        {
        }
        public abstract ILBridge iLBridge { get; }
    }

    public class AssemblyBridge : BridgeBase
    {
        private Assembly _assembly;
        public override object CreatInstance(string type,params object[] args)
        {
            return  _assembly.CreateInstance(type,false,BindingFlags.Default,null,args,null,null);
        }

        public override ILBridge iLBridge { get; }
        public AssemblyBridge(byte[] dllByte, byte[] pdbByte) : base(dllByte, pdbByte)
        {
            _assembly = Assembly.Load(dllByte, pdbByte);
            Type[] allType = _assembly.GetTypes();
            for (int i = 0; i < allType.Length; i++)
            {
                allTypes.Add(allType[i].FullName, allType[i]);
            }
            
            
            iLBridge = (ILBridge) _assembly.CreateInstance(BridgeFullName);
        }
    }

    public class AppDomainBridge : BridgeBase
    {
        public override object CreatInstance(string type, params object[] args)
        {
            return appdomain.Instantiate(type, args);
        }

        public override ILBridge iLBridge { get; }
        private AppDomain appdomain;

        public AppDomainBridge(byte[] dllByte, byte[] pdbByte) : base(dllByte, pdbByte)
        {
            appdomain = new AppDomain();
            MemoryStream fs = new MemoryStream(dllByte);
            MemoryStream p = new MemoryStream(pdbByte);
            appdomain.LoadAssembly(fs, p, new PdbReaderProvider());
            ILBindingHelper.Binding(appdomain);
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
            var values = appdomain.LoadedTypes.Values.ToList();
            foreach (IType type in values)
            {
                if (type.FullName.Contains("HotFix"))
                {
                    allTypes.Add(type.FullName, type.ReflectionType);
                }
            }


            iLBridge = appdomain.Instantiate<ILBridge>(BridgeFullName);
        }
    }
}
