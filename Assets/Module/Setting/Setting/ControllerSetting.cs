using Module.Set;

namespace Module
{
    public class ControllerSetting : SettingConfig
    {
        public Sensitivity sensitivity = new Sensitivity();
        public AimSensitivity aimSensitivity = new AimSensitivity();
        public Vibrate vibrate = new Vibrate();
        

        public override void Init()
        {
            sensitivity.Init();
            aimSensitivity.Init();
            vibrate.Init();
        }

        public override void Update()
        {
        }

        public void SetSensitivity(float value)
        {
            sensitivity.WriteData(value);
        }

        public void SetAimSensitivity(float value)
        {
            aimSensitivity.WriteData(value);
        }
    }
}