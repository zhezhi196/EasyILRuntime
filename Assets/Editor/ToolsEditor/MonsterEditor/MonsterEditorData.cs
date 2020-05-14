using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ToolsEditor
{
    public enum MonsterType
    {
        Enemy,
        Friend,
        Neutral,
        Player
    }

    public enum MonsterBase
    {
        Remote,
        Melee,
    }
    
    [Serializable]
    public class MonsterEditorData 
    {
        #region 数据信息
        [FoldoutGroup("@UID")]
        [FoldoutGroup("@UID/数据信息")]
        [PropertyTooltip("AI唯一标识")]
        [LabelText("UID")]
        public int UID;

        [FoldoutGroup("@UID/数据信息")]
        [LabelText("数据ID")]
        public int dataID;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"isKey\"]==true")] 
        [LabelText("关键怪")]
        public bool isKey;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"isBoss\"]==true")]
        [LabelText("是否Boss")]
        public bool isBoss;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"camp\"]==true")] 
        [LabelText("阵营")] [EnumPaging]
        public MonsterType camp;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"baseType\"]==true")] 
        [LabelText("类型")] [EnumPaging]
        public MonsterBase baseType;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"defaultBT\"]==true")] 
        [LabelText("默认BT")]
        public string defaultBT;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"defaultBTParams\"]==true")] 
        [LabelText("默认BT参数")]
        public string defaultBTParams;

        [FoldoutGroup("@UID/数据信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"pauseBT\"]==true")] 
        [LabelText("暂停行为树")]
        public bool pauseBT;

        [FoldoutGroup("@UID/数据信息")]
        [PropertyTooltip("击杀目标的优先级")]
        [LabelText("击杀队列")]
        [ShowIf("@MonsterEditor.fieldBool[\"killQueue\"]==true")]
        public string killQueue;

        [FoldoutGroup("@UID/数据信息")]
        [LabelText("击杀时间")]
        [ShowIf("@MonsterEditor.fieldBool[\"killTime\"]==true")]
        public float killTime = 10;

        [FoldoutGroup("@UID/数据信息")]
        [LabelText("血量")]
        [ProgressBar(0, 10, 1, 0, 0, Segmented = true, DrawValueLabel = true)]
        [ShowIf("@MonsterEditor.fieldBool[\"HP\"]==true")]
        public int HP = 10;

        [FoldoutGroup("@UID/数据信息")]
        [PropertyTooltip("勾选则会生成一个AI生成器， 每隔N秒生成一只上面参数的AI，最大生成N只")]
        [LabelText("是否延迟生成")]
        [ShowIf("@MonsterEditor.fieldBool[\"isCreatorMachine\"]==true")]
        public bool isCreatorMachine;

        [ShowIf("@isCreatorMachine&&MonsterEditor.fieldBool[\"delayCreate\"]==true")]
        [FoldoutGroup("@UID/数据信息")]
        [LabelText("延迟N秒")]
        public float delayCreate;

        [ShowIf("@isCreatorMachine")]
        [FoldoutGroup("@UID/数据信息")]
        [LabelText("最大生成数量")]
        [ShowIf("@MonsterEditor.fieldBool[\"delayCreateCount\"]==true")]
        public int delayCreateCount;

        [TextArea]
        [FoldoutGroup("@UID/数据信息")]
        [PropertyTooltip("写上备注避免遗忘，可根据需要随意填写")]
        [LabelText("备注：")]
        [ShowIf("@MonsterEditor.fieldBool[\"mark\"]==true")]
        public string mark;

        #endregion

        #region 位置信息

        [FoldoutGroup("@UID/位置信息")]
        [LabelText("出生点")]
        [InlineButton("LoadNPC", "加载")]
        [ShowIf("@MonsterEditor.fieldBool[\"createPointTransform\"]==true")] 
        public Transform createPointTransform;

        private void LoadNPC()
        {
            
        }

        [FoldoutGroup("@UID/位置信息")]
        [LabelText("巡逻点")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        [ShowIf("@MonsterEditor.fieldBool[\"patrolPoint\"]==true")] 
        public List<Transform> patrolPoint;
        
        [FoldoutGroup("@UID/位置信息")]
        [LabelText("逃跑点")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        [ShowIf("@MonsterEditor.fieldBool[\"fleePoint\"]==true")] 
        public List<Transform> fleePoint;
        
        [FoldoutGroup("@UID/位置信息")]
        [LabelText("攻击点")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        [ShowIf("@MonsterEditor.fieldBool[\"attackPoint\"]==true")] 
        public List<Transform> attackPoint;
        
        [FoldoutGroup("@UID/位置信息")]
        [LabelText("躲藏点")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        [ShowIf("@MonsterEditor.fieldBool[\"hidePoint\"]==true")] 
        public List<Transform> hidePoint;
        
        [FoldoutGroup("@UID/位置信息")]
        [LabelText("是否载人")]
        [ShowIf("@MonsterEditor.fieldBool[\"isVehicle\"]==true")] 
        public bool isVehicle;
        
        [FoldoutGroup("@UID/位置信息")]
        [ShowIf("@isVehicle&&MonsterEditor.fieldBool[\"isDriverGod\"]==true")]
        [LabelText("是否无敌")]
        public bool isDriverGod;

        #endregion

        #region 物品信息

        [FoldoutGroup("@UID/物品信息")]
        [HorizontalGroup("@UID/物品信息/head",LabelWidth = 40)]
        [ShowIf("@MonsterEditor.fieldBool[\"head\"]==true")] 
        [LabelText("头部")]
        public string head;
        
        [FoldoutGroup("@UID/物品信息")]
        [HorizontalGroup("@UID/物品信息/head")]
        [HideLabel]
        public Color headColor = Color.white;
        
        
        
        [FoldoutGroup("@UID/物品信息")]
        [LabelText("左手")]
        [HorizontalGroup("@UID/物品信息/head")]
        public string leftHandProp;
        [FoldoutGroup("@UID/物品信息")]
        [HideLabel]
        [HorizontalGroup("@UID/物品信息/head")]
        public Color leftHandColor = Color.white;
        
        
        [FoldoutGroup("@UID/物品信息")]
        [LabelText("右手")]
        [HorizontalGroup("@UID/物品信息/rightHandProp",LabelWidth = 40)]
        public string rightHandProp;
        [FoldoutGroup("@UID/物品信息")]
        [HorizontalGroup("@UID/物品信息/rightHandProp")]
        [HideLabel]
        public Color rightHandColor = Color.white;
        
        
        [FoldoutGroup("@UID/物品信息")]
        [LabelText("背部")]
        [HorizontalGroup("@UID/物品信息/rightHandProp")]
        public string backProp;
        [FoldoutGroup("@UID/物品信息")]
        [HorizontalGroup("@UID/物品信息/rightHandProp")]
        [HideLabel]
        public Color backColor = Color.white;
        
        
        [FoldoutGroup("@UID/物品信息")]
        [PreviewField(25, ObjectFieldAlignment.Center)]
        [FoldoutGroup("@UID/物品信息")]
        [LabelText("AI贴图")]
        public Texture2D[] texture2Ds;

        #endregion

        #region 技能信息

        [FoldoutGroup("@UID/技能信息")]
        [ShowIf("@MonsterEditor.fieldBool[\"skillInfo\"]==true")] 
        [LabelText("技能信息")]
        [InlineButton("OpenSkill","打开技能编辑器")]
        public string skillInfo;
        
        private void OpenSkill()
        {
            
        }
        #endregion
    }
}