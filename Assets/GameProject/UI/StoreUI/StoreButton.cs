using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class StoreButton : ToggleTree
{
    public GameObject light;
    public List<IRewardBag> rewards = new List<IRewardBag>();
    
    public void Select(bool select)
    {
        interactable = !select;
        light.gameObject.OnActive(select);
    }

    public void SetReward(List<IRewardBag> resultBag)
    {
        rewards = resultBag;
    }

    // private void Update()
    // {
    //     for (int i = 0; i < rewards.Count; i++)
    //     {
    //         if (rewards[i].stationCode != 0)
    //         {
    //             gameObject.OnActive(false);
    //             return;
    //         }
    //     }
    //     gameObject.OnActive(true);
    // }
}