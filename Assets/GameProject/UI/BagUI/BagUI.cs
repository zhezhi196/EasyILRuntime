using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectUI
{
    public class BagUI : UIViewBase
    {
        public BagCell[] cellList;
        public UIBtnBase back;
        public UIBtnBase use;
        public UIBtnBase detail;
        public UIScrollBase modelScroll;
        [HideInInspector] public BagCell currentCell;
        public Label uiTitle;
        public BagItem targetItem;
        private Action<PropEntity> onUse;
        public CanvasGroup alaphCanvas;
        public RawImage renderTexture;
        public ToggleGroup toggleGroup;
        public Transform bagCellTrans;
        private PropsBase targetProp;
        [Space]
        public GameObject desPanle;
        public Text propDes;
        public UIBtnBase desPanleBack;
        public ScrollRect scrollRect;
        [Space] 
        public List<BagMoneyItem> moneyItems;
        
        protected override void OnChildStart()
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                cellList[i].Init();
                cellList[i].onSelectCell += OnClickCell;
            }
            back.AddListener(OnBackButton);
            use.AddListener(OnUseButton);
            detail.AddListener(OnDetail);
            desPanleBack.AddListener(OnDesPanelBack);
            modelScroll.AddDrag(((v1, v2, time) => { UI3DShow.Instance.OnRotateModel(winName,v2); }));
            renderTexture.texture = RenderTextureTools.commonTexture;
            renderTexture.GetComponent<RectTransform>().sizeDelta = 650* Tools.GetScreenScale();;
            bagCellTrans.localScale= Tools.GetScreenScale();
        }

        public override void Refresh(params object[] args)
        {
            //AudioPlay.PlayOneShot("beiBao_daKai");
            uiTitle.transform.parent.gameObject.OnActive(BattleController.GetCtrl<BagPackCtrl>().bagList.Count > 0);
            alaphCanvas.alpha = 0;
            DOTween.Kill(gameObject);
            Voter v = new Voter(cellList.Length, () =>
            {
                alaphCanvas.DOFade(1, 1).SetId(gameObject).SetUpdate(true);
            });
            BagItem matchItem = null;
            if (args.Length > 0)
            {
                targetProp = args[0] as PropsBase;
                matchItem = BattleController.GetCtrl<BagPackCtrl>().Match(targetProp);
            }

            if (BattleController.GetCtrl<BagPackCtrl>().bagList.Count <= 0)
            {
                toggleGroup.allowSwitchOff = true;

            }
            BagCell targetCell = null;
            for (int i = 0; i < cellList.Length; i++)
            {
                cellList[i].ClearInfo();
                if (i < BattleController.GetCtrl<BagPackCtrl>().bagList.Count)
                {
                    cellList[i].SetInfo(BattleController.GetCtrl<BagPackCtrl>().bagList[i], () => v.Add());
                    if (matchItem == BattleController.GetCtrl<BagPackCtrl>().bagList[i])
                    {
                        targetItem = cellList[i].cellEntity;
                        targetCell = cellList[i];
                    }
                }
                else
                {
                    v.Add();
                }
            }
           
            //通过匹配打开背包
            if (targetCell != null)    
            {
                if (targetCell.toggle.isOn)
                {
                    targetCell.Select(true);
                }
                else {
                    targetCell.toggle.isOn = true;
                }
            }
            //直接打开背包
            else
            {
                if (cellList[0].cellEntity != null)
                {
                    OnClickCell(cellList[0]);
                    cellList[0].Select(true);
                    cellList[0].toggle.isOn = true;
                }
            }
            if (BattleController.GetCtrl<BagPackCtrl>().bagList.Count > 0)
            {
                toggleGroup.allowSwitchOff = false;
                renderTexture.gameObject.OnActive(true);
            }
            else
            {
                renderTexture.gameObject.OnActive(false);
            }
        }

        public override void OnCloseComplete()
        {
            UI3DShow.Instance.OnClose("BagUI");
            if (currentCell != null)
            {
                currentCell.Select(false);
            }
            targetProp = null;
            currentCell = null;
            targetItem = null;
            onUse = null;
            uiTitle.transform.parent.gameObject.OnActive(false);
            use.gameObject.OnActive(false);
            renderTexture.gameObject.OnActive(false);
        }

        public override void OnExit(params object[] args)
        {
            if (desPanle.gameObject.activeInHierarchy)
            {
                desPanle.gameObject.OnActive(false);
            }
            else
            {
                base.OnExit(args);
            }
        }

        //private void OnDestroy()
        //{
        //    //BagPackCtrl.Instance.onUse -= OnUseObject;
        //    //BagPackCtrl.Instance.onDestroy -= OnDestroyObject;
        //    //BagPackCtrl.Instance.onPutToPag -= OnPutToBag;
        //}

        private void OnDetail()
        {
            propDes.text = string.Format("\n{0}\n", currentCell.cellEntity.entity.GetText(TypeList.BagDes));
            desPanle.gameObject.OnActive(true);
            alaphCanvas.alpha = 0;
        }
        private void OnDesPanelBack()
        {
            desPanle.gameObject.OnActive(false);
            alaphCanvas.alpha = 1;
            scrollRect.content.anchoredPosition = Vector2.zero;
        }

        private void OnUseButton()
        {
            currentCell.cellEntity.entity.OnButtonInBag(currentCell.cellEntity);
            if (currentCell.cellEntity == targetItem)
            {
                targetProp.MatchSuccess();//使用的是匹配对象
                OnBackButton();
            }
            else {
                RefreshUse(false);
            }
        }

        private void OnBackButton()
        {
            OnExit();
        }

        private void OnClickCell(BagCell obj)
        {
            if (obj.cellEntity == null)
            {
                use.gameObject.OnActive(false);
                return;
            }

            currentCell = obj;
            uiTitle.transform.parent.gameObject.OnActive(true);
            uiTitle.SetKey(currentCell.cellEntity.entity.dbData.title);
            //RefreshUse(false);
            detail.gameObject.OnActive(currentCell != null);
            renderTexture.gameObject.OnActive(true);
            use.OnActive(obj.cellEntity.entity.IsShowButton(targetProp,currentCell.cellEntity));
        }

        private void RefreshUse(bool autoSelectFirst)
        {
            if (currentCell.cellEntity.count <= 0)
            {
                currentCell = null;
                uiTitle.transform.parent.gameObject.OnActive(BattleController.GetCtrl<BagPackCtrl>().bagList.Count > 0);
                if (BattleController.GetCtrl<BagPackCtrl>().bagList.Count <= 0)
                {
                    toggleGroup.allowSwitchOff = true;
                    UI3DShow.Instance.OnClose("BagUI");
                    renderTexture.gameObject.OnActive(false);
                }
                
                for (int i = 0; i < cellList.Length; i++)
                {
                    cellList[i].ClearInfo();
                    if (i < BattleController.GetCtrl<BagPackCtrl>().bagList.Count)
                    {
                        cellList[i].SetInfo(BattleController.GetCtrl<BagPackCtrl>().bagList[i], null);
                    }
                }
                if (cellList[0].cellEntity != null)
                {
                    OnClickCell(cellList[0]);
                    cellList[0].Select(true);
                    cellList[0].toggle.isOn = true;
                }
            }
            else
            {
                currentCell.RefreshCount();
            }
            use.OnActive(currentCell==null?false:(currentCell.cellEntity.entity.IsShowButton(targetProp,currentCell.cellEntity)));
        }

        #region 事件回调

        private void OnPutToBag(BagItem obj)
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                if (cellList[i].isEmpty)
                {
                    cellList[i].SetInfo(obj,null);
                    break;
                }
            }

        }

        private void OnDestroyObject(PropEntity obj)
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                if (cellList[i].cellEntity.entity == obj)
                {
                    cellList[i].ClearInfo();
                }
            }
        }

        private void OnUseObject(PropEntity obj)
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                if (cellList[i].cellEntity.entity == obj)
                {
                    if (cellList[i].cellEntity.count > 0)
                    {
                        cellList[i].RefreshCount();
                    }
                }
            }
            
        }
        #endregion
    }
}