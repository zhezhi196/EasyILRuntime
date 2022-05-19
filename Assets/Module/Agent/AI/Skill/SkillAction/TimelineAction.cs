using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Module.SkillAction
{
    public class TimelineAction : ISkillAction
    {
        private PlayableAsset _asset;
        private List<(float, Action<float>)> frameCallback;
        private Dictionary<string, PlayableBinding> bindings = new Dictionary<string, PlayableBinding>();

        public ITimelineSkillObject owner { get; }


        public TimelineAction(PlayableAsset asset,ITimelineSkillObject owner)
        {
            this._asset = asset;
            this.owner = owner;
            foreach (PlayableBinding binding in asset.outputs)
            {
                if (!bindings.ContainsKey(binding.streamName))
                {
                    bindings.Add(binding.streamName, binding);
                }
            }
        }

        public bool isEnd
        {
            get
            {
                return owner.timeLinePlayer.state != PlayState.Playing;
            }
        }

        public float percent { get; }

        public void OnStart()
        {
            owner.timeLinePlayer.timeUpdateMode = DirectorUpdateMode.Manual;
            owner.timeLinePlayer.Play(_asset);
        }

        public void OnEnd(bool complete)
        {
            if (!complete)
            {
                owner.timeLinePlayer.Stop();
                frameCallback = null;
                owner.timeLinePlayer.playableAsset = null;
                owner.LogFormat("动画结束: {0}", _asset.name);
            }
        }

        public void Break()
        {
            owner.timeLinePlayer.Stop();
            owner.timeLinePlayer.playableAsset = null;
            frameCallback = null;
        }

        public void OnPause()
        {
            owner.timeLinePlayer.Pause();
        }

        public void OnContinue()
        {
            owner.timeLinePlayer.Resume();
        }

        public void Dispose()
        {
            owner.timeLinePlayer.Stop();
            frameCallback = null;
        }

        public void OnUpdate()
        {
            if (owner.timeLinePlayer.state == PlayState.Playing)
            {
                owner.timeLinePlayer.playableGraph.Evaluate(owner.GetDelatime(false));
                float percent = (float) owner.timeLinePlayer.time / (float)_asset.duration;
                if (frameCallback != null)
                {
                    for (int i = 0; i < frameCallback.Count; i++)
                    {
                        var temp = frameCallback[i];
                        if (percent >= temp.Item1)
                        {
                            temp.Item2?.Invoke(temp.Item1);
                            frameCallback.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (percent >= 1)
                {
                    //OnEnd();
                }
            }
        }

        public void SetTimeCallback(float percent, Action<float> callback)
        {
            if (frameCallback == null) frameCallback = new List<(float, Action<float>)>();
            frameCallback.Add((percent, callback));
        }

        public void SetValue(string key, Object value)
        {
            PlayableBinding resu;
            if (bindings.TryGetValue(key, out resu))
            {
                if (owner is ITimelineSkillObject timectrl)
                {
                    timectrl.timeLinePlayer.SetGenericBinding(bindings[key].sourceObject, value);
                }
            }
        }

        public K GetPlayableAsset<T, K>() where T : TrackAsset where K : PlayableAsset
        {
            if (owner.timeLinePlayer.playableAsset != null)
            {
                foreach (PlayableBinding bind in owner.timeLinePlayer.playableAsset.outputs)
                {
                    if (bind.sourceObject != null && bind.sourceObject is T playableTrack)
                    {
                        foreach (var clip in playableTrack.GetClips())
                        {
                            Object clipAsset = clip.asset;
                            if (clipAsset is K resutle)
                            {
                                return resutle;
                            }
                        }
                    }
                }
            }

            return default;
        }
    }
}