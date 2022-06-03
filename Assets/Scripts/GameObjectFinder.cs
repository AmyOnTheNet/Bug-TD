using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFinder : MonoBehaviour
{
	public string tagToFind;
	public string messageToSend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMessageToGO(){
    	GameObject _target = GameObject.FindWithTag(tagToFind);

    	if (_target != null) 
    	{
    		_target.SendMessage(messageToSend);
    	}else{
    		Debug.LogError("GameObject not found.");
    	}
    }
}
