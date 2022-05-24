using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace EditorModule
{
    public class ChechAnimtorEmpty : EditorWindow
    {
        [MenuItem("Tools/裴亚龙专用/检查动画状态机空状态")]
        public static void OpenWindow()
        {
            EditorWindow window = GetWindow(typeof(ChechAnimtorEmpty), false, "更新动画状态机");
            window.maxSize = new Vector2(200f, 100f);
            window.minSize = window.maxSize;
            window.Show();
        }

        UnityEngine.Animator animator;

        public void OnGUI()
        {
            EditorGUILayout.LabelField("指定动画状态机ヽ(ー_ー)ノ");
            animator = EditorGUILayout.ObjectField(animator, typeof(Animator), true) as Animator;

            EditorGUI.BeginDisabledGroup(animator == null);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查状态机"))
            {
                CheckAnimtor();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        AnimatorStateMachine am;
        ChildAnimatorState[] ams;

        private void CheckAnimtor()
        {
            AnimatorController animatorController =
                UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(
                    UnityEditor.AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                AnimatorControllerLayer layer = animatorController.layers[i];
                am = layer.stateMachine;
                ams = am.states;
                for (int j = 0; j < ams.Length; j++)
                {
                    Debug.Log(ams[j].state.name + " Check");
                    if (ams[j].state.motion == null)
                    {
                        Debug.LogError(string.Format("Layer({0}),({1})没有动画文件", i, ams[j].state.name));
                    }
                }

                if (am.stateMachines.Length > 0)
                {
                    CheckSubMachine(am, i);
                }
            }

            Debug.Log("动画状态机检查完成");
        }

        private void CheckSubMachine(AnimatorStateMachine animatorStateMachine, int layer)
        {
            for (int j = 0; j < animatorStateMachine.stateMachines.Length; j++)
            {
                ChildAnimatorState[] amsTemp = animatorStateMachine.stateMachines[j].stateMachine.states;
                for (int k = 0; k < amsTemp.Length; k++)
                {
                    Debug.Log(amsTemp[k].state.name + " Check");
                    if (amsTemp[k].state.motion == null)
                    {
                        Debug.LogError(string.Format("Layer({0}),({1})没有动画文件", layer, amsTemp[j].state.name));
                    }
                }

                if (animatorStateMachine.stateMachines[j].stateMachine.stateMachines.Length > 0)
                {
                    CheckSubMachine(animatorStateMachine.stateMachines[j].stateMachine, layer);
                }
            }
        }
    }
}