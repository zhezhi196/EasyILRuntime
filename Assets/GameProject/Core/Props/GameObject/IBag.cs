using Module;

public interface IBag : ITextObject,IModelObject
{
    PutToBagStyle buttonStyle { get; }
    void OnButtonPutToBag();
}