using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Module;
using Service;

public class CDKey
{
    public static void TryGet(string inputKey, Action<CDKey> callback, Action<string> failcallback)
    {
        inputKey = inputKey.Replace(" ", "");
        Regex regex = new Regex(@"^[0-9a-zA-Z]{7,10}$");
        var match = regex.Match(inputKey);
        if (match.Success)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("code", inputKey);
            HttpArgs args = new HttpArgs();
            args.AddArgs("code", inputKey);
            args.AddArgs("playerId", GameInfo.serverUid);
            HttpCache.Instance.Get("api/v1/activity/redeem", args, str =>
            {
                JsonData data = JsonMapper.ToObject(str);
                var code = data["code"].ToString();
                if (code == "200")
                {
                    var rewardData = data["data"]["reward"];
                    CDKey cdkey = new CDKey(rewardData);
                    callback?.Invoke(cdkey);
                }
                else
                {
                    failcallback?.Invoke(code);
                }
            }, 0,30);
        }
        else
        {
            failcallback?.Invoke("match fail");
        }
    }

    private CDKey(JsonData data)
    {
        string rewardId = data.ToJson();
        string[] reward = JsonMapper.ToObject<string[]>(rewardId);
        
        rewards = new RewardContent[reward.Length];
        for (int i = 0; i < rewards.Length; i++)
        {
            string[] content = reward[i].Split(ConstKey.Spite1);
            rewards[i] = Commercialize.GetRewardContent(content[0].ToInt(), content[1].ToInt());
        }
    }

    public RewardContent[] rewards { get; }

    public List<RewardContent> GetReward()
    {
        List<RewardContent> result = new List<RewardContent>();

        if (!rewards.IsNullOrEmpty())
        {
            for (int i = 0; i < rewards.Length; i++)
            {
                if (rewards[i].stationCode == 0)
                {
                    result.Add(rewards[i]);
                    rewards[i].GetReward(1, RewardFlag.NoAudio | RewardFlag.NoRecord);
                }
            }
        }

        return result;
    }
}
