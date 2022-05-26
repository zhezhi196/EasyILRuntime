using System;
using Module;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ProjectUI
{
    public class BagInteractiveUI : UIViewBase
    {
        public UIScrollBase modelScroll;
        public Text propDescribe;
        public UIBtnBase putToBag;
        public UIBtnBase backBtn;
        public IBag props;
        public RawImage modelImage;


        private const int ModelImageSize = 685;
        protected override void OnChildStart()
        {
            modelScroll.AddDrag((v1, v2, time) => UI3DShow.Instance.OnRotateModel(winName,v2));
            putToBag.AddListener(OnPutToBag);
            backBtn.AddListener(OnPutToBag);
            
            modelImage.texture = RenderTextureTools.commonTexture;
            modelImage.GetComponent<RectTransform>().sizeDelta = ModelImageSize * Tools.GetScreenScale();
        }

        private void OnPutToBag()
        {
            props.OnButtonPutToBag();
        }

        public override void OnExit(params object[] args)
        {
            props.OnButtonPutToBag();
        }

        public override void Refresh(params object[] args)
        {
            props = (IBag) args[0];

            putToBag.gameObject.OnActive(props.buttonStyle == PutToBagStyle.PutToBag);
            backBtn.gameObject.OnActive(props.buttonStyle == PutToBagStyle.BackToHud);
            
            UI3DShow.Instance.OnShow(winName,props);
            propDescribe.text = props.GetText(TypeList.BagDes);
            
        }
        
        public override void OnCloseComplete()
        {
            UI3DShow.Instance.OnClose(winName);
        }
    }
}