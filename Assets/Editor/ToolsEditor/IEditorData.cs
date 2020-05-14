using Sirenix.OdinInspector;
using UnityEngine;

namespace ToolsEditor
{

    
    public interface IEditorData
    {
        void SaveEditorJson();
        void GenerateGameClass();
        void GenerateGameJson();
    }
}