using SQLite.Attribute;

namespace Module
{
    public abstract class SqlData
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
    }
}