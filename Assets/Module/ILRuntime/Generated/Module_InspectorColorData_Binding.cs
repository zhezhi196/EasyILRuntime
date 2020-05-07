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
    unsafe class Module_InspectorColorData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.InspectorColorData);

            field = type.GetField("value", flag);
            app.RegisterCLRFieldGetter(field, get_value_0);
            app.RegisterCLRFieldSetter(field, set_value_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_value_0, AssignFromStack_value_0);


        }



        static object get_value_0(ref object o)
        {
            return ((Module.InspectorColorData)o).value;
        }

        static StackObject* CopyToStack_value_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.InspectorColorData)o).value;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_value_0(ref object o, object v)
        {
            ((Module.InspectorColorData)o).value = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_value_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @value = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.InspectorColorData)o).value = @value;
            return ptr_of_this_method;
        }



    }
}
