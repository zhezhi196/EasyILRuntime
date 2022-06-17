using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class GeneralWeapon : MonoBehaviour
{
    public List<Chain> chains = new List<Chain>();
    public Transform orgPoint;
    public bool isFire;
    public Closer skill;
    public Vector3 startPoint;
    public Vector3 endPoint;
    
    [FoldoutGroup("伸长")]
    public float moveTime;
    [FoldoutGroup("伸长")]
    public AnimationCurve moveCurve;
    [FoldoutGroup("伸长")]
    public Transform chainParent;
    
    [FoldoutGroup("返回")]
    public float backTime;
    [FoldoutGroup("返回")]
    public AnimationCurve backCurve;

    public Line line;
    public GameObject mesh;


    [Button]
    public void ClearChain()
    {
        for (int i = 0; i < chains.Count; i++)
        {
            Object.DestroyImmediate(chains[i].gameObject);
        }
        chains.Clear();
    }
    [Button]
    public void SetChain()
    {
        for (int i = 0; i < chains.Count; i++)
        {
            Object.DestroyImmediate(chains[i].gameObject);
        }
        chains.Clear();
        float totalChainDis = 0;
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<Chain>("Assets/Bundles/Monster/General/Chain.prefab");
        int index = 0;
        while (totalChainDis < (skill.maxDistance+1))
        {
            index++;
            totalChainDis = chains.Count > 1 ? chains[0].transform.position.Distance(chains.Last().transform.position) : 0;
            Chain chain = Object.Instantiate(prefab, transform.position, transform.rotation);
            chain.transform.SetParent(chainParent);
            var lastChain = chains.Last();
            if (lastChain == null)
            {
                chain.Join(transform);
            }
            else
            {
                chain.Join(lastChain.nextChainPos);
            }

            chain.distance = totalChainDis;
            chains.Add(chain);
            chain.gameObject.name = "Chain" + index;
            chain.gameObject.OnActive(false);
        }
    }


    private void Update()
    {
        if (isFire)
        {
            line = new Line(startPoint, endPoint);
            float totalDistance = line.distance;
            var dir = line.direction;
            for (int i = 0; i < chains.Count; i++)
            {
                var item = chains[i];

                if (item.distance <= totalDistance)
                {
                    item.gameObject.OnActive(true);
                    item.transform.position = line.GetPoint(item.distance);
                    item.transform.forward = dir;
                }
                else
                {
                    item.gameObject.OnActive(false);
                }
            }
        }
    }

    public void UpdateLine(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    public Tweener moveTweener;

    public void Move(Transform org, Vector3 point)
    {
        orgPoint = org;

        mesh.OnActive(true);
        if (moveTweener != null && moveTweener.IsPlaying()) moveTweener.Kill();
        moveTweener = transform.DOMove(point, moveTime).SetEase(moveCurve).OnComplete(() => { Back(false); });
    }

    public void Back(bool hurtPlayer)
    {
        if(moveTweener.IsPlaying()) moveTweener.Kill();
        moveTweener = transform.DOMove(orgPoint.position, backTime).SetEase(backCurve).OnComplete(() =>
        {
            isFire = false;
            mesh.OnActive(false);
            for (int i = 0; i < chains.Count; i++)
            {
                chains[i].gameObject.OnActive(false);
            }

            if (hurtPlayer)
            {
                Player.player.EndDragMove();
            }
        });
    }
}