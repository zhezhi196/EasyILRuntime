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
    unsafe class Module_BridgeBase_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.BridgeBase);

            field = type.GetField("allTypes", flag);
            app.RegisterCLRFieldGetter(field, get_allTypes_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_allTypes_0, null);


        }



        static object get_allTypes_0(ref object o)
        {
            return ((Module.BridgeBase)o).allTypes;
        }

        static StackObject* CopyToStack_allTypes_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.BridgeBase)o).allTypes;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
