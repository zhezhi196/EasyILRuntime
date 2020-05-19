using UnityEngine;
using System.Collections;
using Module;
using xasset;

public class ReporterMessageReceiver : MonoBehaviour 
{
	Reporter reporter ;
	void Awake()
	{
		if (!Configuration.isGM)
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			reporter = gameObject.GetComponent<Reporter>();
			reporter.enabled = true;
		}
	}

    void OnPreStart(){
        //To Do : this method is called before initializing reporter,
        //we can for example check the resultion of our device ,then change the size of reporter
        if ( reporter == null )
			reporter = gameObject.GetComponent<Reporter>();

		if( Screen.width < 1000 )
			reporter.size = new Vector2( 32 , 32 );
		else 
			reporter.size = new Vector2( 48 , 48);

		reporter.UserData = "Put user date here like his account to know which user is playing on this device";
	}

	void OnHideReporter()
	{
		UIComponent.UnFreeze("GMPannle");
	}

	void OnShowReporter()
	{
		UIComponent.Freeze("GMPannle");
	}

	void OnLog( Reporter.Log log )
	{
		//TO DO : put you custom code 
	}

}
