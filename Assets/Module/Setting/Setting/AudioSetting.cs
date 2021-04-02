using System;

namespace Module.Set
{
    public class AudioSetting : SettingConfig
    {
        public MusicVolume musicVolume = new MusicVolume();
        public AudioVolume audioVolume = new AudioVolume();
        public Mute mute = new Mute();

        public override void Init()
        {
            musicVolume.Init();
            audioVolume.Init();
            mute.Init();
        }

        public override void Update()
        {
        }

        public void SetMusicVolume(float value)
        {
            musicVolume.WriteData(value);
        }

        public void SetAudioVolume(float value)
        {
            audioVolume.WriteData(value);
        }

        public void SetMute(bool mute)
        {
            this.mute.WriteData(mute);
        }
    }
}