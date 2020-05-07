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
    unsafe class Module_InspectorBoolData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.InspectorBoolData);

            field = type.GetField("value", flag);
            app.RegisterCLRFieldGetter(field, get_value_0);
            app.RegisterCLRFieldSetter(field, set_value_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_value_0, AssignFromStack_value_0);


        }



        static object get_value_0(ref object o)
        {
            return ((Module.InspectorBoolData)o).value;
        }

        static StackObject* CopyToStack_value_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.InspectorBoolData)o).value;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_value_0(ref object o, object v)
        {
            ((Module.InspectorBoolData)o).value = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_value_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @value = ptr_of_this_method->Value == 1;
            ((Module.InspectorBoolData)o).value = @value;
            return ptr_of_this_method;
        }



    }
}
