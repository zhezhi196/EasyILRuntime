using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("日志是一个简单的任务，它将输出指定的文本并返回成功。它可以用于调试。")]
    [TaskIcon("{SkinColor}LogIcon.png")]
    public class Log : Action
    {
        [Tooltip("Text to output to the log")]
        public SharedString text;
        [Tooltip("Is this text an error?")]
        public SharedBool logError;
        
        public override TaskStatus OnUpdate()
        {
            // Log the text and return success
            if (logError.Value) {
                Debug.LogError(text);
            } else {
                Debug.Log(text);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            text = "";
            logError = false;
        }
    }
}