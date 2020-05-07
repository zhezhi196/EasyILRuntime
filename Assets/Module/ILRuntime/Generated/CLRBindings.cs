using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            xasset_Assets_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            System_String_Binding.Register(app);
            System_IO_DirectoryInfo_Binding.Register(app);
            System_IO_FileSystemInfo_Binding.Register(app);
            System_IO_StreamReader_Binding.Register(app);
            Module_HotFixManager_Binding.Register(app);
            Module_BridgeBase_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Type_Binding.Register(app);
            System_IO_TextReader_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_IDataBase_Array_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Type_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Type_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            xasset_Asset_Binding.Register(app);
            Module_RunTimeSequence_Binding.Register(app);
            System_Char_Binding.Register(app);
            Module_IDataBase_Binding.Register(app);
            LitJson_JsonMapper_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            xasset_Configuration_Binding.Register(app);
            Module_MonoSingleton_1_GMConsole_Binding.Register(app);
            Module_GMConsole_Binding.Register(app);
            UnityEngine_PlayerPrefs_Binding.Register(app);
            Module_EventCenter_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            Module_UIComponent_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            Module_Loading_Binding.Register(app);
            Module_RunTimeAction_1_ILTypeInstance_Binding.Register(app);
            Module_GameDebug_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Behaviour_Binding.Register(app);
            System_Reflection_FieldInfo_Binding.Register(app);
            Module_ViewReference_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorStringData_Binding.Register(app);
            Module_InspectorData_Binding.Register(app);
            Module_InspectorStringData_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorLongData_Binding.Register(app);
            Module_InspectorLongData_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorDoubleData_Binding.Register(app);
            Module_InspectorDoubleData_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorBoolData_Binding.Register(app);
            Module_InspectorBoolData_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorAnimationCurveData_Binding.Register(app);
            Module_InspectorAnimationCurveData_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorVector3Data_Binding.Register(app);
            Module_InspectorVector3Data_Binding.Register(app);
            System_Collections_Generic_List_1_InspectorColorData_Binding.Register(app);
            Module_InspectorColorData_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_FieldInfo_Array_Binding.Register(app);
            Module_ViewBehaviour_Binding.Register(app);
            Module_ObjectPool_Binding.Register(app);
            Module_GamePlay_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            Module_Manager_Binding.Register(app);
            Module_BundleManager_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            System_Action_1_GameObject_Binding.Register(app);
            Clock_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            Module_Voter_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            Module_ExtendUtil_Binding.Register(app);
            System_Threading_Interlocked_Binding.Register(app);
            System_Action_Binding.Register(app);
            UnityEngine_CanvasGroup_Binding.Register(app);
            Module_UIViewReference_Binding.Register(app);
            DG_Tweening_ShortcutExtensions46_Binding.Register(app);
            DG_Tweening_TweenSettingsExtensions_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
