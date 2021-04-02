using Module.Set;

namespace Module
{
    public class ControllerSetting : SettingConfig
    {
        public Sensitivity sensitivity = new Sensitivity();
        public Vibrate vibrate = new Vibrate();
        

        public override void Init()
        {
            sensitivity.Init();
            vibrate.Init();
        }

        public override void Update()
        {
        }

        public void SetSensitivity(float value)
        {
            sensitivity.WriteData(value);
        }
    }
}