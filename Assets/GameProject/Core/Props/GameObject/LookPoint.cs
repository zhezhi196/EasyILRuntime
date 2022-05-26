using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class LookPoint : MonoBehaviour
{
    public static float interDistance = 2f;
    public static float interactiveAngle = 30;
    public static float circleDistance = 3f;
    
    [ReadOnly]
    public bool inCamera;
    [ReadOnly,LabelText("在角色前方")]
    public bool isForward;
    [LabelText("是否朝向角色才能显示")]
    public bool forwardShow;
    [LabelText("是否朝着角色"),ReadOnly]
    public bool isLookAtPlayer; //
    [ReadOnly]
    public float currInterDistance;
    [ReadOnly]
    public bool isCircle = true;
    [ReadOnly]
    public bool needCircle = true;
    
    // public string lookTag;
    public InteractiveObject target;
    
    [ReadOnly]
    public UIInteractionNode uiNode;

    private Player _player;
    private bool isLoadNode;
    [LabelText("打开调试")]
    public bool isDebug;
    

    public InteractiveSource source
    {
        get
        {
            if (_player == null)
            {
                _player = Player.player;
            }

            return _player;
        }
    }

    public bool isActive
    {
        get { return target != null && target.isActive; }
    }

    public PropsStation canInteractiveStation;
    
    public virtual bool canInteractive => CanInteractive();

    public virtual InterActiveStyle interactiveStyle => target == null ? InterActiveStyle.None : target.interactiveStyle;
    public virtual bool isButtonActive => target != null && target.isButtonActive;
    public virtual string tips => target == null ? string.Empty : target.tips;
    public virtual string interactiveTips => target == null ? string.Empty : target.interactiveTips;
    public Vector3 position => transform.position;

    public bool onCircle => needCircle && isCircle;


    
    //必须有mesh才能生效
    public void OnBecameVisible() 
    {
        inCamera = true;
        this.enabled = true;
    }

    public void OnBecameInvisible()
    {
        inCamera = false;
        this.enabled = false;
    }

    private bool CanInteractive()
    {
        bool propStation = true;
        if (target is PropsBase prop)
        {
            propStation = prop.ContainStation(canInteractiveStation);
        }

        if (!propStation)
        {
            LogPoint("LookPoint状态设置不对");
            return false;
        }

        if (!inCamera)
        {
#if UNITY_EDITOR
            if (GetComponent<MeshRenderer>() == null)
            {
                LogPoint("没有Mesh");
            }
            else
            {
                LogPoint("不在摄像机范围内");
            }
#endif
            return false;
        }

        if (!isForward)
        {
            LogPoint("在玩家后方");
            return false;
        }

        if (!target.canInteractive)
        {
            if (!target.isActive)
            {
                LogPoint("物品没有初始化");
            }
            else
            {
                LogPoint("物品初始化了,但还是无法交互");
            }
                
            return false;
        }

        isLookAtPlayer = Vector3.Dot(source.eyePoint.position - transform.position, transform.forward) > 0;

        if (forwardShow && !isLookAtPlayer)
        {
            LogPoint("应正对玩家才能显示");
            return false;
        }

        return true;
    }
    
    public bool IsInVisiable(float k = 1)
    {
        if (source == null)
        {
            currInterDistance = 0;
            return false;
        }

        if (currInterDistance < interDistance * k) //是否在可视距离
        {
            float angle = (transform.position - source.eyePoint.position).Angle(source.eyePoint.forward);
            if (angle < interactiveAngle * k) //是否在视角内
            {
                return true;
            }
        }

        return false;
    }

    public void Init(InteractiveObject tar)
    {
        target = tar;
    }

    protected virtual void Update()
    {
        if (target != null && source != null)
        {
            isCircle = true;
            Vector3 eyePos = source.eyePoint.position;

            isForward = Vector3.Dot(eyePos - position, source.eyePoint.forward) <= 0;
            //和玩家的距离
            currInterDistance = eyePos.Distance(position);
            if (currInterDistance >= circleDistance)
            {
                LogPoint("距离太远");
            }
            else
            {
                if (canInteractive)
                {
                    if (RayPlayer(eyePos))
                    {
                        OpenInteractionNode();
                        if (uiNode != null)
                        {
                            isCircle = !IsInVisiable();
                        }
                        return;
                    }
                }
            }
        }
        else
        {
            if(target==null) LogPoint("找不到物品");
            if(source==null) LogPoint("没有为玩家");
        }

        CloseInteractionNode();
    }
    
    private bool RayPlayer(Vector3 eyePos)
    {
        Ray ray = new Ray(position, (eyePos - position));

        RaycastHit[] hitTemp = Physics.RaycastAll(ray, currInterDistance, MaskLayer.LookPointBlock);

        for (int i = 0; i < hitTemp.Length; i++)
        {
            RaycastHit hit = hitTemp[i];
            if (hit.collider.gameObject == gameObject || hit.collider.gameObject == target.gameObject) continue;
            if (hit.collider.gameObject.layer != MaskLayer.Playerlayer)
            {
                GameDebug.DrawLine(position, hit.point, Color.red);
#if UNITY_EDITOR
                LogPoint($"遮挡物挡着了{hit.collider.gameObject}");
#endif
                return false;
            }
        }
        GameDebug.DrawLine(position, eyePos, Color.yellow);
        return true;
    }

    private void OpenInteractionNode()
    {
        if (uiNode == null && !isLoadNode && PropsCtrl.onShowLookPoint != null)
        {
            isLoadNode = true;
            PropsCtrl.OnShowLookPoint(true, this, () => isLoadNode = false);
        }
    }

    public void CloseInteractionNode()
    {
        if (uiNode != null)
        {
            PropsCtrl.OnShowLookPoint(false, this, null);
            ObjectPool.ReturnToPool(uiNode);
            uiNode = null;
        }
    }

    private void OnDisable()
    {
        CloseInteractionNode();
    }

    public virtual bool Interactive()
    {
        if (target != null)
        {
            return target.Interactive();
        }

        return false;
    }

    private void LogPoint(string content)
    {
        if (isDebug)
        {
            GameDebug.Log(content);
        }
    }
    
    #if UNITY_EDITOR
    [Button("添加MeshRender", ButtonSizes.Medium)]
    public void AddMeshRender()
    {
        MeshRenderer render = gameObject.AddComponent<MeshRenderer>();
        render.materials = new Material[0];
        render.shadowCastingMode = ShadowCastingMode.Off;
        render.lightProbeUsage = LightProbeUsage.Off;
        render.reflectionProbeUsage = ReflectionProbeUsage.Off;
        gameObject.name = "LookPoint";
    }
    #endif
    
}