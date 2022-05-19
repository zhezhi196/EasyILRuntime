using System;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Module
{
    public class AnimationCallback : MonoBehaviour
    {
        private IAnimaotr agent;
        public event Action<AnimationEvent,int> onAnimationCallback;

        public void Awake()
        {
            agent = GetComponentInParent<IAnimaotr>();
        }

        public void OnAnimationCallback(AnimationEvent animationEvent)
        {
            if (agent.canReceiveEvent)
            {
                onAnimationCallback?.Invoke(animationEvent, animationEvent.animatorClipInfo.clip.events.Index(animationEvent, (a, b) => Math.Abs(a.time - b.time) < 0.01f));
            }
        }

#if UNITY_EDITOR
        [Button]
        public void SetParmator()
        {
            Animator animator = GetComponent<Animator>();
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            InitAnimator(controller);
        }
        [Button]
        public void SetEvent()
        {
            Animator animator = GetComponent<Animator>();
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            InitAnimator(controller);
        }
        
        private void InitAnimator(AnimatorController ct)
        {
            if (ct == null) return;
            
            for (int i = 0; i < ct.layers.Length; i++)
            {
                for (int j = 0; j < ct.layers[i].stateMachine.stateMachines.Length; j++)
                {
                    AddAnimatorPara(ct,ct.layers[i].stateMachine.stateMachines[j]);
                }

                for (int j = 0; j < ct.layers[i].stateMachine.states.Length; j++)
                {
                    SetState(ct,ct.layers[i].stateMachine.states[j].state);
                }
            }

            for (int i = 0; i < ct.animationClips.Length; i++)
            {
                SetAnimationEvent(ct.animationClips[i]);
            }

            AssetDatabase.Refresh();
        }
        
        private void AddAnimatorPara(AnimatorController ct, ChildAnimatorStateMachine stateMachine)
        {
            for (int i = 0; i < stateMachine.stateMachine.states.Length; i++)
            {
                SetState(ct, stateMachine.stateMachine.states[i].state);
            }

            for (int i = 0; i < stateMachine.stateMachine.stateMachines.Length; i++)
            {
                AddAnimatorPara(ct, stateMachine.stateMachine.stateMachines[i]);
            }
        }

        private void SetState(AnimatorController ct, AnimatorState state)
        {
            if (!ct.parameters.Contains(st => st.name == state.name))
            {
                ct.AddParameter(state.name, AnimatorControllerParameterType.Float);
                ct.parameters.Find(pa => pa.name == state.name).defaultFloat = 1;
            }
            
            state.speedParameter = state.name;
            state.speedParameterActive = true;
        }

        private void SetAnimationEvent(AnimationClip previewClip)
        {
            string assetPath = AssetDatabase.GetAssetPath(previewClip);
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            //读取文件序列化数据 实际就是meta里的数据
            SerializedObject serializedObject = new SerializedObject(modelImporter);
            //找到所有animation
            SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
            if (clipAnimations == null || clipAnimations.arraySize == 0)
            {
                modelImporter.clipAnimations = modelImporter.defaultClipAnimations;
                serializedObject = new SerializedObject(modelImporter);
                clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
            }

            for (int i = 0; i < clipAnimations.arraySize; i++)
            {
                SerializedProperty clipAnimationProperty = clipAnimations.GetArrayElementAtIndex(i);
                if (clipAnimationProperty.displayName == previewClip.name)
                {
                    //找到其events
                    SerializedProperty eventsProperty = clipAnimationProperty.FindPropertyRelative("events");
                    //重新写入
                    for (int j = 0; j < previewClip.events.Length; j++)
                    {
                        //eventsProperty.InsertArrayElementAtIndex(eventsProperty.arraySize);
                        SerializedProperty eventProperty = eventsProperty.GetArrayElementAtIndex(j);
                        
                        //数据中的时间是相对于总时间的0到1的小数 而不是以秒为单位的时间 所以要转换一下
                        eventProperty.FindPropertyRelative("functionName").stringValue = "OnAnimationCallback";
                        eventProperty.FindPropertyRelative("floatParameter").floatValue = previewClip.events[j].time;
                        eventProperty.FindPropertyRelative("intParameter").intValue = j;
                        eventProperty.FindPropertyRelative("data").stringValue = previewClip.name;
                    }

                    //应用
                    serializedObject.ApplyModifiedProperties();
                    //重新读取
                    AssetDatabase.ImportAsset(assetPath);
                }
            }
        }


#endif

    }
}