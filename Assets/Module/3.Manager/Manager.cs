using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Module
{
    public abstract class Manager
    {
        private static Dictionary<string, Manager> _manager = new Dictionary<string, Manager>();

        public static RunTimeSequence Initialize(RunTimeSequence runtime)
        {
            Type[] types = Assembly.Load("Module").GetTypes();
            List<Manager> managerList = new List<Manager>();

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(Manager))
                {
                    Manager manager = Activator.CreateInstance(types[i]) as Manager;
                    if (types[i].FullName != null)
                    {
                        manager.BeforeInit();
                        managerList.Add(manager);
                    }
                    else
                    {
                        GameDebug.LogError("type =" + types[i].FullName + ".name==null");
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
                UIComponent.Loading.AddLoading(keyValuePair.Key, result);
                runtime.Add(new RunTimeAction<Manager>(keyValuePair.Value, () => { keyValuePair.Value.Init(runtime); }));
            }

            runtime.OnNextAction(run =>
            {
                RunTimeAction<Manager> target = (RunTimeAction<Manager>) run;
                float loading = UIComponent.SetLoading(target.arg.GetType().FullName, target.arg.processDiscription, 1);
                GameDebug.Log($"管理器: {target.arg}初始化结束: 进度到{100 * loading}%");
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