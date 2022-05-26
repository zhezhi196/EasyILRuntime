using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DIYGameUI : UIViewBase, ILocationSettingUI
{
    public RectTransform recTransform;
    public GameUISettingButton[] write;
    public UIBtnBase up;
    public UIBtnBase down;
    public UIBtnBase left;
    public UIBtnBase right;

    public UIBtnBase back;
    public UIBtnBase reset;
    public UIBtnBase save;
    public Slider scaleSlide;
    public Slider alphaSlide;
    public Toggle drop;
    public float moveSpeed = 100;

    public RectTransform dropPannle;
    public GameUISettingButton currentWrite;
    public Text scalePercent;
    public Text alphaPercent;
    public UIBtnBase unSelect;
    public Image bg;

    public bool isChanged
    {
        get
        {
            for (int i = 0; i < write.Length; i++)
            {
                if (write[i].isChanged) return true;
            }

            return false;
        }
    }

    [Button]
    public void SearchButton()
    {
        write = transform.GetComponentsInChildren<GameUISettingButton>(true);
    }

    protected override void OnChildStart()
    {
        up.AddPointDown(OnUp);
        down.AddPointDown(OnDown);
        left.AddPointDown(OnLeft);
        right.AddPointDown(OnRight);

        up.AddPointing(OnPointUp);
        down.AddPointing(OnPointDown);
        left.AddPointing(OnPointLeft);
        right.AddPointing(OnPointRight);

        back.AddListener(OnBack);
        reset.AddListener(OnResetValue);
        save.AddListener(Save);
        scaleSlide.onValueChanged.AddListener(OnScaleSlide);
        alphaSlide.onValueChanged.AddListener(OnAlphaSlide);
        drop.onValueChanged.AddListener(OnDrop);
        unSelect.AddListener(OnUnSelect);
        for (int i = 0; i < write.Length; i++)
        {
            write[i].onBeginDrag += OnSelect;
        }
    }

    private void OnUnSelect()
    {
        if (currentWrite != null)
        {
            currentWrite.bgLight.gameObject.OnActive(false);
            currentWrite = null;
            alphaSlide.value = 1;
            scaleSlide.value = 1;
        }
    }

    public override void OnOpenStart()
    {
        base.OnOpenStart();
        BattleController.Instance.Pause(winName);
    }

    public override void OnCloseComplete()
    {
        drop.isOn = true;
        BattleController.Instance.Continue(winName);
    }

    private void OnResetValue()
    {
        CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("2509"),null,
            new PupupOption(null, Language.GetContent("703")),
            new PupupOption(() =>
            {
                for (int i = 0; i < write.Length; i++)
                {
                    write[i].ResetDefault();
                }

                alphaSlide.value = 1;
                scaleSlide.value = 1;
                ReloadUI(null);
            }, Language.GetContent("702")));
    }

    public override void Refresh(params object[] args)
    {
        bg.SetAlpha((float)args[0]);
        // if (args[0].ToString() == "半透")
        // {
        //     bg.SetAlpha(0.9f);
        // }
        // else
        // {
        //     bg.SetAlpha(1);
        // }
        for (int i = 0; i < write.Length; i++)
        {
            write[i].ResetValue();
        }
    }

    private void OnPointRight(float obj)
    {
        if (currentWrite == null) return;

        if (obj > 1)
        {
            Vector3 curr = currentWrite.transform.position;
            Move(new Vector3(curr.x + TimeHelper.deltaTime * moveSpeed, curr.y, curr.z));
        }
    }

    private void OnPointLeft(float obj)
    {
        if (currentWrite == null) return;

        if (obj > 1)
        {
            Vector3 curr = currentWrite.transform.position;
            Move(new Vector3(curr.x - TimeHelper.deltaTime * moveSpeed, curr.y, curr.z));
        }
    }

    private void OnPointDown(float obj)
    {
        if (currentWrite == null) return;

        if (obj > 1)
        {
            Vector3 curr = currentWrite.transform.position;
            Move(new Vector3(curr.x, curr.y - TimeHelper.deltaTime * moveSpeed, curr.z));
        }
    }

    private void OnPointUp(float obj)
    {
        if (currentWrite == null) return;

        if (obj > 1)
        {
            Vector3 curr = currentWrite.transform.position;
            Move(new Vector3(curr.x, curr.y + TimeHelper.deltaTime * moveSpeed, curr.z));
        }
    }

    private void OnSelect(UILocationWrite obj)
    {
        if (currentWrite != null)
        {
            currentWrite.bgLight.gameObject.OnActive(false);
        }

        currentWrite = (GameUISettingButton) obj;
        scaleSlide.value = obj.transform.localScale.x;
        alphaSlide.value = obj.canvasGroup.alpha;
    }

    private void OnDrop(bool arg0)
    {
        if (arg0)
        {
            dropPannle.DOBlendableLocalMoveBy(new Vector3(0, -dropPannle.rect.size.y, 0), 0.3f).OnComplete(() =>
            {
                drop.transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
            }).SetUpdate(true);
            if (currentWrite != null)
            {
                alphaSlide.value = currentWrite.canvasGroup.alpha;
                scaleSlide.value = currentWrite.transform.localScale.x;
            }
            else
            {
                alphaSlide.value = 1;
                scaleSlide.value = 1;
            }
        }
        else
        {
            dropPannle.DOBlendableLocalMoveBy(new Vector3(0, dropPannle.rect.size.y, 0), 0.3f).OnComplete(() =>
            {
                drop.transform.GetChild(0).eulerAngles = new Vector3(0, 0, 180);
            }).SetUpdate(true);
        }
    }

    private void OnAlphaSlide(float arg0)
    {
        if (currentWrite != null)
        {
            currentWrite.canvasGroup.alpha = arg0;
        }

        alphaPercent.text = arg0.ToString("P0");
    }

    private void OnScaleSlide(float arg0)
    {
        if (currentWrite != null)
        {
            currentWrite.transform.localScale = Vector3.one * arg0;
        }

        scalePercent.text = arg0.ToString("P0");
    }

    private void OnBack()
    {
        if (isChanged)
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("2507"),null,
                new PupupOption(null, Language.GetContent("703")),
                new PupupOption(() =>
                {

                    OnExit();
                    ResetValue();
                }, Language.GetContent("702")));
        }
        else
        {
            OnExit();
            ResetValue();
        }
    }

    private void Move(Vector3 pos)
    {
        if (currentWrite == null) return;

        currentWrite.OnBeginDragItem(currentWrite.transform.position);
        currentWrite.OnDraggItem(pos);
        currentWrite.OnEndDraggItem(pos);
    }

    private void OnRight(ButtonTriggerType type)
    {
        if (currentWrite != null)
        {
            Move(new Vector3(currentWrite.transform.position.x + 5, currentWrite.transform.position.y));
        }
    }

    private void OnLeft(ButtonTriggerType type)
    {
        if (currentWrite != null)
        {
            Move(new Vector3(currentWrite.transform.position.x - 5, currentWrite.transform.position.y));
        }
    }

    private void OnDown(ButtonTriggerType type)
    {
        if (currentWrite != null)
        {
            Move(new Vector3(currentWrite.transform.position.x, currentWrite.transform.position.y - 5));
        }
    }

    private void OnUp(ButtonTriggerType type)
    {
        if (currentWrite != null)
        {
            Move(new Vector3(currentWrite.transform.position.x, currentWrite.transform.position.y + 5));
        }
    }

    public string uiName
    {
        get { return "GameUI"; }
    }

    public Rect uiRect
    {
        get { return recTransform.rect; }
    }

    public void ResetValue()
    {
        for (int i = 0; i < write.Length; i++)
        {
            write[i].ResetValue();
        }
    }

    public void Save()
    {
        CommonPopup.Popup(Language.GetContent("2510"), Language.GetContent("2511"),null,
            new PupupOption(() =>
            {
                for (int i = 0; i < write.Length; i++)
                {
                    write[i].Save();
                }

                OnExit();
            }, Language.GetContent("702")));
    }

    [Button]
    public void SerarchOldScript()
    {
        MonoBehaviour[] mono = transform.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < mono.Length; i++)
        {
            var temp = mono[i];
            if (temp is Component || temp is DIYGameUI || temp is GameUISettingButton)
            {
                continue;
            }
            Debug.Log(mono[i],mono[i].gameObject);
        }
    }
}