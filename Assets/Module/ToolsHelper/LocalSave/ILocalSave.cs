namespace Module
{
    public interface ILocalSave
    {
        string localUid { get; }
        void ReadData(string data);
        string GetWriteDate();
    }
}