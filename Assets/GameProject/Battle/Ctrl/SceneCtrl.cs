using Module;

public class SceneCtrl : BattleSystem
{
    public RunTimeAction loadScene;
    public RunTimeAction unloadScene;
    
    public override void OnNodeEnter(TaskNode node)
    {
        if (!node.nodeSetting.loadScene.IsNullOrEmpty())
        {
            loadScene = new RunTimeAction(() =>
            {
                Voter voter = new Voter(node.nodeSetting.loadScene.Count, () =>
                {
                    BattleController.Instance.NextFinishAction("loadScene");
                    loadScene = null;
                });

                for (int i = 0; i < node.nodeSetting.loadScene.Count; i++)
                {
                    string loadScene = node.nodeSetting.loadScene[i].ToString();
                    GameScene.LoadAdditive(loadScene, voter.Add);
                }
            });
        }

        if (!node.nodeSetting.unloadScene.IsNullOrEmpty())
        {
            unloadScene = new RunTimeAction(() =>
            {
                Voter voter = new Voter(node.nodeSetting.unloadScene.Count, () =>
                {
                    BattleController.Instance.NextFinishAction("unloadScene");
                    unloadScene = null;
                });

                for (int i = 0; i < node.nodeSetting.unloadScene.Count; i++)
                {
                    string loadScene = node.nodeSetting.unloadScene[i].ToString();
                    GameScene.UnLoad(loadScene, voter.Add);
                }
            });
        }
    }
}