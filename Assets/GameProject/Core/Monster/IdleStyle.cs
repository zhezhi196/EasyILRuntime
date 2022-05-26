using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class IdleStyle : MonoBehaviour
{
    public Monster monster;
    [LabelText("休闲间隔")]
    public float interval = 5;
    public Vector2Int range;
    [LabelText("休闲样式")]
    public string styleKey = "IdleStyle";
    [LabelText("触发休闲的trigger")]
    public string triggerKey = "IdleTrigger";
    [LabelText("离开休闲的trigger")]
    public string outFree = "OutFree";
    public bool autoRandomIdle = true;
    public float currTime;

    private void Update()
    {
        if (autoRandomIdle && monster.fightState == FightState.Normal && monster.moveStyle == MoveStyle.None)
        {
            currTime += TimeHelper.deltaTime;
            if (currTime >= interval)
            {
                Style();
            }
        }
        else
        {
            currTime = 0;
        }
    }

    public void Style()
    {
        monster.animator.ResetTrigger(outFree);
        if (!styleKey.IsNullOrEmpty())
        {
             monster.animator.SetInteger(styleKey, Random.Range(range.x, range.y + 1));
        }
        monster.animator.SetTrigger(triggerKey);
        currTime = 0;
    }

    public void BreakFree()
    {
        if (!outFree.IsNullOrEmpty())
        {
            monster.animator.SetTrigger(outFree);
        }
        currTime = 0;
    }


#if UNITY_EDITOR
    public AnimationClip[] clip;
    [Button,LabelText("添加动画休闲key")]
    public void AddParamater()
    {
        Animator animator = transform.GetComponentInParent<Animator>();
        UnityEditor.Animations.AnimatorController controller = (UnityEditor.Animations.AnimatorController) animator.runtimeAnimatorController;
        var allPar = controller.parameters;
        var idleStyle = allPar.Find(fd => fd.name == styleKey);
        if (idleStyle == null)
        {
            controller.AddParameter(styleKey, AnimatorControllerParameterType.Int);
        }
        
        var triggerIn = allPar.Find(fd => fd.name == triggerKey);
        if (triggerIn == null)
        {
            controller.AddParameter(triggerKey, AnimatorControllerParameterType.Trigger);
        }
        
        var triggerOut = allPar.Find(fd => fd.name == outFree);
        if (triggerOut == null)
        {
            controller.AddParameter(outFree, AnimatorControllerParameterType.Trigger);
        }
    }

    [Button,LabelText("添加休闲动画")]
    public void AddIdleStyle()
    {
        Animator animator = transform.GetComponentInParent<Animator>();
        UnityEditor.Animations.AnimatorController controller = (UnityEditor.Animations.AnimatorController) animator.runtimeAnimatorController;
        var defaultState = controller.layers[0].stateMachine.defaultState;
        
        for (int i = 0; i < clip.Length; i++)
        {
            var state = controller.AddMotion(clip[i]);
            var translation = defaultState.AddTransition(state, false);
            translation.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, i, styleKey);
            translation.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, i, triggerKey);
            var translation2 = state.AddTransition(defaultState, false);
            var translation3 = state.AddTransition(defaultState, true);
            translation2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, i, outFree);
        }
    }
#endif
}