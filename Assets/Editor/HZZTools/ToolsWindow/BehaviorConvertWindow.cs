using BehaviorDesigner.Editor;
using BehaviorDesigner.Runtime;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace EditorModule
{
    public class BehaviorConvertWindow : OdinEditorWindow
    {
        public AgentBehaviorTree behavior;
        [UnityEditor.MenuItem("Tools/程序工具/行为树转换器")]
        public static void OpenWindiw()
        {
            GetWindow<BehaviorConvertWindow>();
        }
        [Button]
        public void Convert()
        {

        }
    }
}