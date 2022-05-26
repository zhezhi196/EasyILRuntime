using Module;
using UnityEngine;
using UnityEngine.UI;

public class FpsShow : MonoBehaviour
{
#if LOG_ENABLE
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }
    
    private void Update()
    {
        text.text = Setting.graphicSetting.fps.value.ToString();
    }
#endif
}