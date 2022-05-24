using Module;

public class Money: PropEntity
{
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        process.SetComplete();
        return process;
    }
}