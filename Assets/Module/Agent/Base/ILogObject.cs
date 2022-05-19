using System.Diagnostics;

namespace Module
{
    public interface ILogObject
    {
        string logName { get; }
        bool isLog { get; set; }
        void LogFormat(string obj, params object[] args);
    }
}