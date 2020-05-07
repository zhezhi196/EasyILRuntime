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
    unsafe class Module_UIViewReference_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.UIViewReference);

            field = type.GetField("tweenInterval", flag);
            app.RegisterCLRFieldGetter(field, get_tweenInterval_0);
            app.RegisterCLRFieldSetter(field, set_tweenInterval_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_tweenInterval_0, AssignFromStack_tweenInterval_0);
            field = type.GetField("tweenCurve", flag);
            app.RegisterCLRFieldGetter(field, get_tweenCurve_1);
            app.RegisterCLRFieldSetter(field, set_tweenCurve_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_tweenCurve_1, AssignFromStack_tweenCurve_1);
            field = type.GetField("delay", flag);
            app.RegisterCLRFieldGetter(field, get_delay_2);
            app.RegisterCLRFieldSetter(field, set_delay_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_delay_2, AssignFromStack_delay_2);


        }



        static object get_tweenInterval_0(ref object o)
        {
            return ((Module.UIViewReference)o).tweenInterval;
        }

        static StackObject* CopyToStack_tweenInterval_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.UIViewReference)o).tweenInterval;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_tweenInterval_0(ref object o, object v)
        {
            ((Module.UIViewReference)o).tweenInterval = (System.Single)v;
        }

        static StackObject* AssignFromStack_tweenInterval_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @tweenInterval = *(float*)&ptr_of_this_method->Value;
            ((Module.UIViewReference)o).tweenInterval = @tweenInterval;
            return ptr_of_this_method;
        }

        static object get_tweenCurve_1(ref object o)
        {
            return ((Module.UIViewReference)o).tweenCurve;
        }

        static StackObject* CopyToStack_tweenCurve_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.UIViewReference)o).tweenCurve;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_tweenCurve_1(ref object o, object v)
        {
            ((Module.UIViewReference)o).tweenCurve = (UnityEngine.AnimationCurve)v;
        }

        static StackObject* AssignFromStack_tweenCurve_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.AnimationCurve @tweenCurve = (UnityEngine.AnimationCurve)typeof(UnityEngine.AnimationCurve).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.UIViewReference)o).tweenCurve = @tweenCurve;
            return ptr_of_this_method;
        }

        static object get_delay_2(ref object o)
        {
            return ((Module.UIViewReference)o).delay;
        }

        static StackObject* CopyToStack_delay_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.UIViewReference)o).delay;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_delay_2(ref object o, object v)
        {
            ((Module.UIViewReference)o).delay = (System.Single)v;
        }

        static StackObject* AssignFromStack_delay_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @delay = *(float*)&ptr_of_this_method->Value;
            ((Module.UIViewReference)o).delay = @delay;
            return ptr_of_this_method;
        }



    }
}
