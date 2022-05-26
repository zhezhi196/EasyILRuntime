using Module;
using UnityEngine;
using UnityEngine.UI;

public class LimitRewardIcon : MonoBehaviour
{
    public Image icon;
    public Text count;
    public float lifePos;
    public float otherPos;

    public void SetReward(RewardContent reward)
    {
        if (!(reward.reward is RecoverLife))
        {
            icon.enabled = true;
            count.alignment = TextAnchor.MiddleLeft;
            count.transform.localPosition = new Vector3(otherPos, 0, 0);
            reward.GetIcon(TypeList.High, sp => icon.sprite = sp);
            count.text = ConstKey.Cheng+reward.finalCount.value.ToString();
        }
        else
        {
            icon.enabled = false;
            count.transform.localPosition = new Vector3(lifePos, 0, 0);
            count.alignment = TextAnchor.MiddleCenter;
            count.text =Language.GetContent("1014");
        }

    }
}