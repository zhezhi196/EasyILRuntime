using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Module
{
    public static class ILBindingHelper
    {
        public static void Binding(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            //adaptor
            appdomain.RegisterCrossBindingAdaptor(new ILBridgeAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new BehaviourAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new IDisposeAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new IDataBaseAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());

            //method
            appdomain.DelegateManager.RegisterMethodDelegate<xasset.Asset>();
            appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appdomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<int>();
            appdomain.DelegateManager.RegisterMethodDelegate<bool>();
            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<string>();
            appdomain.DelegateManager.RegisterMethodDelegate<bool, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<int, string>();
            appdomain.DelegateManager.RegisterMethodDelegate<string, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<string, float>();
            appdomain.DelegateManager.RegisterMethodDelegate<string, string>();
            appdomain.DelegateManager.RegisterMethodDelegate<string, string, float>();
            appdomain.DelegateManager.RegisterMethodDelegate<int, int, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<WWW>();
            appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<GameObject, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<object>();
            appdomain.DelegateManager.RegisterMethodDelegate<int, bool, GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<int, ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance, object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<Vector2>();
            appdomain.DelegateManager.RegisterMethodDelegate<Vector3>();
            appdomain.DelegateManager.RegisterMethodDelegate<Color>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<Module.IRunTime>();
            appdomain.DelegateManager.RegisterMethodDelegate<Module.ViewReference[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();




            //
            // Function
            //
            appdomain.DelegateManager.RegisterFunctionDelegate<bool>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string>();
            appdomain.DelegateManager.RegisterFunctionDelegate<int>();
            appdomain.DelegateManager.RegisterFunctionDelegate<float>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string[], object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Func<bool>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, bool>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, ILTypeInstance, int>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Tuple<int, int, int, RectTransform>, bool>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Tuple<int, int, int, RectTransform>, int>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Vector2>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Vector3>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Color>();

            appdomain.DelegateManager.RegisterFunctionDelegate<string, AndroidJavaObject>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string, AndroidJavaObject, string>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string, AndroidJavaObject, string, string>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Module.PlayerData, System.Boolean>();
            
            //Dotween
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<Vector2>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<Vector2>(() =>
                {
                    return ((Func<Vector2>)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<Vector2>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<Vector2>((pnewValue) => { ((Action<Vector2>)act)(pnewValue); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<int>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<int>(() =>
                {
                    return ((Func<int>)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<int>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<int>((pNewValue) =>
                {
                    ((Action<int>)act)(pNewValue);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<Vector2>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<Vector2>(() =>
                {
                    return ((Func<Vector2>)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<Color>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<Color>(() => { return ((Func<Color>)act)(); });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<Color>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<Color>((value) => { ((Action<Color>)act)(value); });
            });
            //dotween-float
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<float>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<float>((value) => { ((Action<float>)act)(value); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<float>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<float>(() => { return ((Func<float>)act)(); });
            });

            //Dotween.TweenCallback
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });

            //UnityAction
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            //Predicate<ILTypeInstance>
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<ILTypeInstance>>((act) =>
            {
                return new Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILTypeInstance, bool>)act)(obj);
                });
            });
            
            //Sort函数
            appdomain.DelegateManager.RegisterDelegateConvertor<Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILTypeInstance, ILTypeInstance, int>)act)(x, y);
                });
            });

            //Action<Vector2>
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Vector2>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Vector2>((arg0) =>
                {
                    ((Action<Vector2>)act)(arg0);
                });
            });
            //Action<Vector3>
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Vector3>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Vector3>((arg0) =>
                {
                    ((Action<Vector3>)act)(arg0);
                });
            });

            //IBaseEventData
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
                {
                    ((Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Module.PlayerData>>((act) =>
            {
                return new System.Predicate<Module.PlayerData>((obj) =>
                {
                    return ((Func<Module.PlayerData, System.Boolean>)act)(obj);
                });
            });



        }
    }


}
