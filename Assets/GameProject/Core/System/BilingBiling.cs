using Sirenix.OdinInspector;
using UnityEngine;

public class BilingBiling : MonoBehaviour
{
    public string field = "Vector1_F4E3D5DD";
    public Renderer renderTar;
    public float minValue;
    public float maxValue = 1;
    public float biliTime = 5;

    public bool inCamera;

    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0, 1.5f, 1.5f), new Keyframe(0.5f, 1, 3, -3),new Keyframe(1, 0, -1.5f, -1.5f));
    
    private IBilingObject bilingObject;
    public bool isStop;

    private void Awake()
    {
        bilingObject = transform.GetComponentInParent<IBilingObject>();
    }

    private void OnBecameInvisible()
    {
        inCamera = false;
        renderTar = GetComponent<Renderer>();
    }

    [Button]
    public void EditorInit()
    {
    }


    private void OnBecameVisible()
    {
        inCamera = true;
    }

    private void Update()
    {
        if (bilingObject != null)
        {
            if (inCamera && !isStop)
            {
                if (bilingObject.showBiling)
                {
                    float time = Time.time % biliTime;
                    renderTar.material.SetFloat(field,curve.Evaluate(time / biliTime) * (maxValue - minValue) + minValue);
                }
                else
                {
                    OnReset();
                    isStop = true;
                }

            }
        }
    }

    public void OnReset()
    {
        renderTar.material.SetFloat(field, 0);
    }
}