using Module;

public interface ICollectionData : ISqlData
{
    int isCollection { get; set; }
    string icon { get; set; }
    int collectionType { get; set; }
    int collectionIndex { get; set; }
}