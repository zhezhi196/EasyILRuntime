using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     var proxy = AudioManager.PlayMusic("a");
        // }
        //
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     AudioManager.Play("b", "1", transform).SetSpeed(0.5f).OnComplete(() =>
        //     {
        //         Debug.Log("b OnComplete");
        //     });
        // }
        //
        // if (Input.GetKeyDown(KeyCode.V))
        // {
        //     AudioManager.StopGroup("1");
        // }
        //
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     AudioManager.PauseAll();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     AudioManager.ResumeAll();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     AudioManager.MuteMusic(true);
        // }
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     AudioManager.MuteMusic(false);
        // }

        if (Input.GetKeyDown(KeyCode.G))
        {
            AudioManager.Play("b", GameObject.Find("Point1").transform);
        }
        
    }
}
