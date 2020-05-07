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
    unsafe class xasset_Assets_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(xasset.Assets);
            args = new Type[]{typeof(System.String), typeof(System.Type)};
            method = type.GetMethod("LoadAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadAsync_0);

            field = type.GetField("source", flag);
            app.RegisterCLRFieldGetter(field, get_source_0);
            app.RegisterCLRFieldSetter(field, set_source_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_source_0, AssignFromStack_source_0);


        }


        static StackObject* LoadAsync_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Type @type = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @path = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = xasset.Assets.LoadAsync(@path, @type);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_source_0(ref object o)
        {
            return xasset.Assets.source;
        }

        static StackObject* CopyToStack_source_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = xasset.Assets.source;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_source_0(ref object o, object v)
        {
            xasset.Assets.source = (xasset.ResourceSource)v;
        }

        static StackObject* AssignFromStack_source_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            xasset.ResourceSource @source = (xasset.ResourceSource)typeof(xasset.ResourceSource).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            xasset.Assets.source = @source;
            return ptr_of_this_method;
        }



    }
}
