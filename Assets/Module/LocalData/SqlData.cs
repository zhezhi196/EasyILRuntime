using SQLite.Attribute;

namespace Module
{
    public abstract class SqlData
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
    }
    public abstract class IDataBase
    {
        public int Id;
        public IDataBase(){}
    }
}