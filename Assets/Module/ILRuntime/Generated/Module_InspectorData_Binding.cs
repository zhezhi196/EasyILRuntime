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
    unsafe class Module_InspectorData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.InspectorData);

            field = type.GetField("key", flag);
            app.RegisterCLRFieldGetter(field, get_key_0);
            app.RegisterCLRFieldSetter(field, set_key_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_key_0, AssignFromStack_key_0);


        }



        static object get_key_0(ref object o)
        {
            return ((Module.InspectorData)o).key;
        }

        static StackObject* CopyToStack_key_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.InspectorData)o).key;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_key_0(ref object o, object v)
        {
            ((Module.InspectorData)o).key = (System.String)v;
        }

        static StackObject* AssignFromStack_key_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.InspectorData)o).key = @key;
            return ptr_of_this_method;
        }



    }
}
