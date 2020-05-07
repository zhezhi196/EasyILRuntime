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
    unsafe class Module_ViewReference_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Module.ViewReference);

            field = type.GetField("stringList", flag);
            app.RegisterCLRFieldGetter(field, get_stringList_0);
            app.RegisterCLRFieldSetter(field, set_stringList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_stringList_0, AssignFromStack_stringList_0);
            field = type.GetField("intList", flag);
            app.RegisterCLRFieldGetter(field, get_intList_1);
            app.RegisterCLRFieldSetter(field, set_intList_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_intList_1, AssignFromStack_intList_1);
            field = type.GetField("floatList", flag);
            app.RegisterCLRFieldGetter(field, get_floatList_2);
            app.RegisterCLRFieldSetter(field, set_floatList_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_floatList_2, AssignFromStack_floatList_2);
            field = type.GetField("boolList", flag);
            app.RegisterCLRFieldGetter(field, get_boolList_3);
            app.RegisterCLRFieldSetter(field, set_boolList_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_boolList_3, AssignFromStack_boolList_3);
            field = type.GetField("animationList", flag);
            app.RegisterCLRFieldGetter(field, get_animationList_4);
            app.RegisterCLRFieldSetter(field, set_animationList_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_animationList_4, AssignFromStack_animationList_4);
            field = type.GetField("vector3List", flag);
            app.RegisterCLRFieldGetter(field, get_vector3List_5);
            app.RegisterCLRFieldSetter(field, set_vector3List_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_vector3List_5, AssignFromStack_vector3List_5);
            field = type.GetField("colorList", flag);
            app.RegisterCLRFieldGetter(field, get_colorList_6);
            app.RegisterCLRFieldSetter(field, set_colorList_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_colorList_6, AssignFromStack_colorList_6);
            field = type.GetField("target", flag);
            app.RegisterCLRFieldGetter(field, get_target_7);
            app.RegisterCLRFieldSetter(field, set_target_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_target_7, AssignFromStack_target_7);
            field = type.GetField("targetType", flag);
            app.RegisterCLRFieldGetter(field, get_targetType_8);
            app.RegisterCLRFieldSetter(field, set_targetType_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_targetType_8, AssignFromStack_targetType_8);


        }



        static object get_stringList_0(ref object o)
        {
            return ((Module.ViewReference)o).stringList;
        }

        static StackObject* CopyToStack_stringList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).stringList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_stringList_0(ref object o, object v)
        {
            ((Module.ViewReference)o).stringList = (System.Collections.Generic.List<Module.InspectorStringData>)v;
        }

        static StackObject* AssignFromStack_stringList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorStringData> @stringList = (System.Collections.Generic.List<Module.InspectorStringData>)typeof(System.Collections.Generic.List<Module.InspectorStringData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).stringList = @stringList;
            return ptr_of_this_method;
        }

        static object get_intList_1(ref object o)
        {
            return ((Module.ViewReference)o).intList;
        }

        static StackObject* CopyToStack_intList_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).intList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_intList_1(ref object o, object v)
        {
            ((Module.ViewReference)o).intList = (System.Collections.Generic.List<Module.InspectorLongData>)v;
        }

        static StackObject* AssignFromStack_intList_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorLongData> @intList = (System.Collections.Generic.List<Module.InspectorLongData>)typeof(System.Collections.Generic.List<Module.InspectorLongData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).intList = @intList;
            return ptr_of_this_method;
        }

        static object get_floatList_2(ref object o)
        {
            return ((Module.ViewReference)o).floatList;
        }

        static StackObject* CopyToStack_floatList_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).floatList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_floatList_2(ref object o, object v)
        {
            ((Module.ViewReference)o).floatList = (System.Collections.Generic.List<Module.InspectorDoubleData>)v;
        }

        static StackObject* AssignFromStack_floatList_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorDoubleData> @floatList = (System.Collections.Generic.List<Module.InspectorDoubleData>)typeof(System.Collections.Generic.List<Module.InspectorDoubleData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).floatList = @floatList;
            return ptr_of_this_method;
        }

        static object get_boolList_3(ref object o)
        {
            return ((Module.ViewReference)o).boolList;
        }

        static StackObject* CopyToStack_boolList_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).boolList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_boolList_3(ref object o, object v)
        {
            ((Module.ViewReference)o).boolList = (System.Collections.Generic.List<Module.InspectorBoolData>)v;
        }

        static StackObject* AssignFromStack_boolList_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorBoolData> @boolList = (System.Collections.Generic.List<Module.InspectorBoolData>)typeof(System.Collections.Generic.List<Module.InspectorBoolData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).boolList = @boolList;
            return ptr_of_this_method;
        }

        static object get_animationList_4(ref object o)
        {
            return ((Module.ViewReference)o).animationList;
        }

        static StackObject* CopyToStack_animationList_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).animationList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_animationList_4(ref object o, object v)
        {
            ((Module.ViewReference)o).animationList = (System.Collections.Generic.List<Module.InspectorAnimationCurveData>)v;
        }

        static StackObject* AssignFromStack_animationList_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorAnimationCurveData> @animationList = (System.Collections.Generic.List<Module.InspectorAnimationCurveData>)typeof(System.Collections.Generic.List<Module.InspectorAnimationCurveData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).animationList = @animationList;
            return ptr_of_this_method;
        }

        static object get_vector3List_5(ref object o)
        {
            return ((Module.ViewReference)o).vector3List;
        }

        static StackObject* CopyToStack_vector3List_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).vector3List;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_vector3List_5(ref object o, object v)
        {
            ((Module.ViewReference)o).vector3List = (System.Collections.Generic.List<Module.InspectorVector3Data>)v;
        }

        static StackObject* AssignFromStack_vector3List_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorVector3Data> @vector3List = (System.Collections.Generic.List<Module.InspectorVector3Data>)typeof(System.Collections.Generic.List<Module.InspectorVector3Data>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).vector3List = @vector3List;
            return ptr_of_this_method;
        }

        static object get_colorList_6(ref object o)
        {
            return ((Module.ViewReference)o).colorList;
        }

        static StackObject* CopyToStack_colorList_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).colorList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_colorList_6(ref object o, object v)
        {
            ((Module.ViewReference)o).colorList = (System.Collections.Generic.List<Module.InspectorColorData>)v;
        }

        static StackObject* AssignFromStack_colorList_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Module.InspectorColorData> @colorList = (System.Collections.Generic.List<Module.InspectorColorData>)typeof(System.Collections.Generic.List<Module.InspectorColorData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).colorList = @colorList;
            return ptr_of_this_method;
        }

        static object get_target_7(ref object o)
        {
            return ((Module.ViewReference)o).target;
        }

        static StackObject* CopyToStack_target_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).target;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_target_7(ref object o, object v)
        {
            ((Module.ViewReference)o).target = (Module.ViewBehaviour)v;
        }

        static StackObject* AssignFromStack_target_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Module.ViewBehaviour @target = (Module.ViewBehaviour)typeof(Module.ViewBehaviour).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).target = @target;
            return ptr_of_this_method;
        }

        static object get_targetType_8(ref object o)
        {
            return ((Module.ViewReference)o).targetType;
        }

        static StackObject* CopyToStack_targetType_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Module.ViewReference)o).targetType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_targetType_8(ref object o, object v)
        {
            ((Module.ViewReference)o).targetType = (System.String)v;
        }

        static StackObject* AssignFromStack_targetType_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @targetType = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Module.ViewReference)o).targetType = @targetType;
            return ptr_of_this_method;
        }



    }
}
