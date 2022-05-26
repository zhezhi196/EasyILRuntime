using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

public class AssExcuteBtn : MonoBehaviour, IPoolObject
{
    public bool isAssBtn = true;
    public UIBtnBase btnBase;
    public GameObject point;
    public AttackMonster monster;

    public ObjectPool pool { get; set; }

    private void Start()
    {
        if (btnBase == null)
        {
            btnBase.GetComponent<UIBtnBase>();
        }
        if (btnBase != null)
        {
            btnBase.AddListener(OnClick);
        }
    }

    public void UpdateNode()
    {
        //更新暗杀按钮位置
        if (monster == null || !monster.canAss)
        {
            if (gameObject.activeSelf)
            {
                gameObject.OnActive(false);
            }
            return;
        }
        SetBtnPos();
    }

    public void SetBtnPos()
    {
        if (monster != null)
        {
            //是否在暗杀,处决距离外
            bool outDis = Vector3.Distance(Player.player.transform.position, monster.assPoint.position) >
                             Player.player.argConfig.assDistance;
            if (point.activeSelf != outDis)
            {
                point.OnActive(outDis);
            }

            if (btnBase.gameObject.activeSelf == outDis)
            {
                btnBase.gameObject.OnActive(!outDis);
                if (!outDis)
                {
                    EventCenter.Dispatch<Monster, bool, AssExcuteBtn>(EventKey.AddAssExMonster, monster, isAssBtn, this);
                }
            }
            transform.localPosition =
                UIController.Instance.Convert3DToUI(Player.player.evCamera, monster.assPoint.position);
        }
    }

    public void OnClick()
    {
        if (monster != null)
        {
            Player.player.AssMonster(monster);
        }
    }

    public void ReturnToPool()
    {
        monster = null;
    }

    public void ResetBtn()
    {
        ObjectPool.ReturnToPool(this);
    }

    public void OnGetObjectFromPool()
    {
        //this.gameObject.OnActive(true);
    }
}
