using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module;
using UnityEngine;

namespace HotFix
{
    public abstract class Manager
    {
        private static Dictionary<string, Manager> _manager = new Dictionary<string, Manager>();

        public static RunTimeSequence Initialize(RunTimeSequence runtime, BridgeBase modulBridge)
        {
            List<Manager> managerList = new List<Manager>();

            foreach (KeyValuePair<string,Type> keyValuePair in modulBridge.allTypes)
            {
                if (keyValuePair.Value.BaseType == typeof(Manager))
                {
                    Manager manager = (Manager) Activator.CreateInstance(keyValuePair.Value);
                    
                    if (manager != null)
                    {
                        manager.BeforeInit();
                        managerList.Add(manager);
                    }
                }
            }
            
            managerList.Sort((left, right) =>
            {
                if (left.runOrder < right.runOrder)
                {
                    return -1;
                }
                else if (left.runOrder == right.runOrder)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            for (int i = 0; i < managerList.Count; i++)
            {
                _manager.Add(managerList[i].GetType().FullName, managerList[i]);
            }

            float result = 0;
            foreach (KeyValuePair<string, Manager> keyValuePair in _manager)
            {
                result += (float) 1 / (2 * _manager.Count);
                Module.UIComponent.Loading.AddLoading(keyValuePair.Key, result + 0.5f);
                runtime.Add(new RunTimeAction<Manager>(keyValuePair.Value, () => { keyValuePair.Value.Init(runtime); }));
            }

            runtime.OnNextAction(run =>
            {
                RunTimeAction<Manager> target = (RunTimeAction<Manager>) run;
                float loading = Module.UIComponent.SetLoading(target.arg.GetType().FullName, target.arg.processDiscription, 1);
                GameDebug.Log($"管理器: {target.arg}初始化结束 : 进度到{100 * loading}%");
            });
            
            return runtime;
        }

        public static T Get<T>() where T : Manager
        {
            return _manager[typeof(T).FullName] as T;
        }

        protected abstract int runOrder { get; }
        protected abstract string processDiscription { get; }
        protected abstract void BeforeInit();
        protected abstract void Init(RunTimeSequence runtime);
    }
}