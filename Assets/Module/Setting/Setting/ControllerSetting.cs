using Module.Set;

namespace Module
{
    public class ControllerSetting : SettingConfig
    {
        public Sensitivity sensitivity = new Sensitivity();
        // public AimSensitivity aimSensitivity = new AimSensitivity();
        public Vibrate vibrate = new Vibrate();
        public Gyro gyro = new Gyro();
        

        public override void Init()
        {
            sensitivity.Init();
            // aimSensitivity.Init();
            vibrate.Init();
            gyro.Init();
        }

        public override void Update()
        {
        }

        public void SetSensitivity(float value)
        {
            sensitivity.WriteData(value);
        }
        
        public void SetGyroSensitivity(float value)
        {
            gyro.WriteData(value);
        }

        public void SetGyroOpen(bool open)
        {
            gyro.isOpen = open;
            gyro.WriteDataForce();
        }
    }
}