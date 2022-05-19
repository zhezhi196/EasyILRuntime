namespace Module
{
    public interface ILocalSave
    {
        string localGroup { get; }
        string localUid { get; }
        string GetWriteDate();
    }
}