namespace Module
{
    public interface ILocalSave
    {
        string localFileName { get; }
        string localGroup { get; }
        string localUid { get; }
        string GetWriteDate();
    }
}