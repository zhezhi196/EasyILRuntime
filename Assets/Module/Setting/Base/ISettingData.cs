using System;

namespace Module
{
    public interface ISettingData<T>
    {
        string key { get; }
        T value { get; set; }
        void Init();
        T ReadData();
        void WriteData(T value1);
        bool HasSetting();
    }
}