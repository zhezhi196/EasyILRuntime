using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

namespace GameProject.UI.SelectSkillUI
{
    public class SlotItem: MonoBehaviour
    {
        public int index;
        
        public SkillSlot slot
        {
            get { return SkillSlot.slots[index]; }
        }

        public void Refresh()
        {
            if (slot.isGet)
            {
                BlackItem[] all = transform.GetComponentsInChildren<BlackItem>();
                
                for (int i = 0; i < all.Length; i++)
                {
                    all[i].UnSelect();
                    ObjectPool.ReturnToPool(all[i]);
                }

                Voter voter = new Voter(slot.skills.Count, () =>
                {
                    transform.Sort((a, b) => a.index.CompareTo(b.index), all);
                });
                for (int i = 0; i < slot.skills.Count; i++)
                {
                    if (slot.skills[i] != null)
                    {
                        var tempSkill = slot.skills[i];
                        AssetLoad.LoadGameObject("UI/SelectSkillUI/Black.prefab", transform, (go, arg) =>
                        {
                            BlackItem item = go.GetComponent<BlackItem>();
                            item.Select(tempSkill);
                            voter.Add();
                        });
                    }
                }
            }
            else
            {
                gameObject.OnActive(false);
            }

            slot.onSelect = Refresh;
        }
    }
}