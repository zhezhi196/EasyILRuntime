using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Module_HotFixManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.HotFixManager);

            field = type.GetField("bridge", flag);
            app.RegisterCLRFieldGetter(field, get_bridge_0);
            app.RegisterCLRFieldSetter(field, set_bridge_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_bridge_0, AssignFromStack_bridge_0);


        }



        static object get_bridge_0(ref object o)
        {
            return Module.HotFixManager.bridge;
        }

        static StackObject* CopyToStack_bridge_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Module.HotFixManager.bridge;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_bridge_0(ref object o, object v)
        {
            Module.HotFixManager.bridge = (Module.BridgeBase)v;
        }

        static StackObject* AssignFromStack_bridge_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Module.BridgeBase @bridge = (Module.BridgeBase)typeof(Module.BridgeBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Module.HotFixManager.bridge = @bridge;
            return ptr_of_this_method;
        }



    }
}
