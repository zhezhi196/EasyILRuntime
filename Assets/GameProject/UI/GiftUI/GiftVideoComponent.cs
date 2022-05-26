using System;
using Module;
using Project.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Video;
using GameGift;

/// <summary>
/// 视频播放组件
/// </summary>
public class GiftVideoComponent:MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage giftPreview;
    public GameObject mask;
    public Image giftImage; //静态图

    private GiftData mCurrentPlayData = null;
    
    /// <summary>
    /// 初始化giftVideo
    /// </summary>
    public void Init()
    {
        Assert.IsFalse(videoPlayer == null || giftPreview == null || mask == null,"视频组件初始化出错！");
        
        //videoPlayer.targetTexture = RenderTexture.videoTexture;
        giftPreview.texture = videoPlayer.targetTexture;
        videoPlayer.prepareCompleted += OnLoadVideo;
    }



    public void Play(Gift giftData)
    {
        if (mCurrentPlayData == giftData.dbData)
        {
            return;
        }
        
        mCurrentPlayData = giftData.dbData;
        mask.OnActive(true);
        if (videoPlayer.isPrepared || videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        
        if (!string.IsNullOrEmpty(giftData.dbData.giftVideo))
        {
            //存在视频默认播放视频
            videoPlayer.url = $"{Application.streamingAssetsPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Video)}/{mCurrentPlayData.giftVideo}";
            videoPlayer.Prepare();
        }
        else if (!string.IsNullOrEmpty(giftData.dbData.giftPic))
        {
            //存在图片默认给图
            SpriteLoader.LoadIcon(giftData.dbData.giftPic,OnLoadSprite);
        }
        else
        {
            //啥都没有隐藏
            giftImage.gameObject.OnActive(false);
            giftPreview.gameObject.OnActive(false);
            mask.OnActive(false);
        }
    }

    private void OnLoadVideo(VideoPlayer vp)
    {
        if (!string.IsNullOrEmpty(mCurrentPlayData.giftPic))
        {
            return;
        }
        
        vp.Play();
        giftImage.gameObject.OnActive(false);
        giftPreview.gameObject.OnActive(true);
        mask.OnActive(false);
    }

    private void OnLoadSprite(Sprite sprite)
    {
        if (!sprite.name.Contains(mCurrentPlayData.giftPic))
        {
            return;
        }
        giftImage.sprite = sprite;
        giftImage.gameObject.OnActive(true);
        giftPreview.gameObject.OnActive(false);
        mask.OnActive(false);
    }
    

}