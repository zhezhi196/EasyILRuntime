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
    unsafe class Module_UIComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.UIComponent);
            args = new Type[]{};
            method = type.GetMethod("get_Loading", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Loading_0);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.Single)};
            method = type.GetMethod("SetLoading", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetLoading_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Freeze", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Freeze_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("UnFreeze", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnFreeze_3);
            args = new Type[]{};
            method = type.GetMethod("get_NormalTransform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_NormalTransform_4);
            args = new Type[]{};
            method = type.GetMethod("get_PopupTransform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PopupTransform_5);

            field = type.GetField("isFreezed", flag);
            app.RegisterCLRFieldGetter(field, get_isFreezed_0);
            app.RegisterCLRFieldSetter(field, set_isFreezed_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isFreezed_0, AssignFromStack_isFreezed_0);


        }


        static StackObject* get_Loading_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Module.UIComponent.Loading;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetLoading_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @progress = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @discription = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Module.UIComponent.SetLoading(@key, @discription, @progress);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Freeze_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Module.UIComponent.Freeze(@key);

            return __ret;
        }

        static StackObject* UnFreeze_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Module.UIComponent.UnFreeze(@key);

            return __ret;
        }

        static StackObject* get_NormalTransform_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Module.UIComponent.NormalTransform;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_PopupTransform_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Module.UIComponent.PopupTransform;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_isFreezed_0(ref object o)
        {
            return Module.UIComponent.isFreezed;
        }

        static StackObject* CopyToStack_isFreezed_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Module.UIComponent.isFreezed;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isFreezed_0(ref object o, object v)
        {
            Module.UIComponent.isFreezed = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isFreezed_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isFreezed = ptr_of_this_method->Value == 1;
            Module.UIComponent.isFreezed = @isFreezed;
            return ptr_of_this_method;
        }



    }
}
