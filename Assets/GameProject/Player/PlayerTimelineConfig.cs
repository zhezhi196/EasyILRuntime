using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Module;

/// <summary>
/// 玩家暗杀,处决怪物,被怪物处决timeline配置
/// </summary>
[CreateAssetMenu(menuName = "Player/Timeline配置")]
public class PlayerTimelineConfig : ScriptableObject
{
	//暗杀怪物配置
	[System.Serializable]
	public class AssMonsterConfig
	{
		public MonsterModelName monster;
		[FilePath(ParentFolder = "Assets/Bundles"),LabelText("暗杀死")] public string killTimeline;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("暗杀不死")] public string noKillTimeline;
		//[FilePath(ParentFolder = "Assets/Bundles"), LabelText("暗杀吸能量")] public string killAndAbsorb;
	}
	//处决怪物配置
	[System.Serializable]
	public class ExcuteMonsterConfig
	{
		public MonsterModelName monster;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("处决")] public string timeline;
	}
	//被怪物杀死的timeline
	[System.Serializable]
	public class DeathConfig
	{
		public MonsterModelName monster;
		public bool someSkill = false;
		[ShowIf("someSkill")]
		public SkillName skillName;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("玩家死")] public string timeline;
	}
	//挣脱
	[System.Serializable]
	public class GetOutConfig
	{
		public MonsterModelName monster;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("挣脱")] public string timeline;
	}

	//战斗开始动画
	[System.Serializable]
	public class MonserShowConfig
	{
		public MonsterModelName monster;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("战斗开始动画")] public string timeline;
	}

	//结局动画
	[System.Serializable]
	public class EndAnimConfig
	{
		public GameDifficulte difficulte;
		[FilePath(ParentFolder = "Assets/Bundles"), LabelText("结局动画")] public string timeline;
	}

	public AssMonsterConfig[] assMonsterConfigs;
	public ExcuteMonsterConfig[] excuteMonsterConfigs;
	public GetOutConfig[] getoutConfigs;
	public DeathConfig[] deathConfigs;
	public MonserShowConfig[] monsterShowConfigs;
	public EndAnimConfig[] endAnimConfig;
	[FilePath(ParentFolder = "Assets/Bundles"), LabelText("死亡剧情动画")]
	public string[] deathStory;
#if UNITY_EDITOR //编辑器,加载timeline资源
	[Button("暗杀死")]
	private void LoadAssAsset(int index)
	{
		if (!string.IsNullOrEmpty(assMonsterConfigs[index].killTimeline))
		{
			LoadAss(assMonsterConfigs[index].monster, assMonsterConfigs[index].killTimeline);
		}
	}
	[Button("暗杀不死")]
	private void LoadNoAssAsset(int index)
	{
		if (!string.IsNullOrEmpty(assMonsterConfigs[index].noKillTimeline))
		{
			LoadAss(assMonsterConfigs[index].monster, assMonsterConfigs[index].noKillTimeline);
		}
	}
	[Button("挣脱")]
	private void LoadAbsorbAsset(int index)
	{
		if (!string.IsNullOrEmpty(getoutConfigs[index].timeline))
		{
			LoadAss(getoutConfigs[index].monster, getoutConfigs[index].timeline);
		}
	}
    [Button("处决")]
	private void LoadExcuteAsset(int index)
	{
		if (!string.IsNullOrEmpty(excuteMonsterConfigs[index].timeline))
		{
			LoadAss(excuteMonsterConfigs[index].monster, excuteMonsterConfigs[index].timeline);
		}
	}
	[Button("玩家死")]
	private void LoadDeathAsset(int index)
	{
		if (!string.IsNullOrEmpty(deathConfigs[index].timeline))
		{
			LoadAss(deathConfigs[index].monster, deathConfigs[index].timeline);
		}
	}

	[System.Serializable]
	public class MosterModel
	{
		public MonsterModelName monster;
		[FilePath(ParentFolder = "Assets")] public string modelPath;
	}
	public bool editor = false;
	[FilePath(ParentFolder = "Assets"), ShowIf("editor")]
	public string PlayerModelPath = "";
	[ShowIf("editor")] public MosterModel[] mosterModels;

	public void LoadAss(MonsterModelName modelName, string path)
	{
		//load timeline
		GameObject timeline = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/Bundles/{0}", path));
		GameObject t = UnityEditor.PrefabUtility.InstantiatePrefab(timeline) as GameObject;
		t.transform.position = Vector3.zero;
		TimelineController tlCtrl = t.GetComponent<TimelineController>();
		Dictionary<string,UnityEngine.Playables.PlayableBinding> bindingDict = new Dictionary<string, UnityEngine.Playables.PlayableBinding>();
        foreach (var at in tlCtrl.playable.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(at.streamName))
            {
                bindingDict.Add(at.streamName, at);
            }
        }
        //load monster
        MosterModel modelInfo = mosterModels.Find(f => f.monster == modelName);
		Animator monsterAnim = null;
		if (modelInfo != null)
		{
			GameObject m = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", modelInfo.modelPath));
			GameObject mPrefab = UnityEditor.PrefabUtility.InstantiatePrefab(m) as GameObject;
			mPrefab.transform.position = t.transform.position;
			monsterAnim = mPrefab.GetComponent<Animator>();
		}
        if (bindingDict.ContainsKey("MonsterAnim") && monsterAnim != null)
            tlCtrl.playable.SetGenericBinding(bindingDict["MonsterAnim"].sourceObject, monsterAnim);
        //load player
        Animator playerAnim = null;
        if (!string.IsNullOrEmpty(PlayerModelPath))
        {
            GameObject p = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", PlayerModelPath));
            GameObject pPrefab = UnityEditor.PrefabUtility.InstantiatePrefab(p) as GameObject;
            pPrefab.transform.position = tlCtrl.playerPoint.transform.position;
            pPrefab.transform.rotation = tlCtrl.playerPoint.transform.rotation;
            playerAnim = pPrefab.GetComponent<Animator>();
        }
        if (bindingDict.ContainsKey("PlayerAnim") && playerAnim != null)
            tlCtrl.playable.SetGenericBinding(bindingDict["PlayerAnim"].sourceObject, playerAnim);
    }
	
#endif
}
