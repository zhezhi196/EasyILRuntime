using Module;
using UnityEngine;

public class PropsCreator : MonoBehaviour, Identify<int>
{
    [SerializeField]
    private int id;

    public int ID
    {
        get { return id; }
        set { id = value; }
    }
}