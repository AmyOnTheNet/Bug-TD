using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel_Anims : MonoBehaviour
{
	private bool showing = false;
	private Animator c_anim;

	void Start(){
		c_anim = GetComponent<Animator>();
	}

	public void ToggleMenu(){
		showing = !showing;

		if (showing) 
		{
			c_anim.SetTrigger("Show");
		}else{
			c_anim.SetTrigger("Hide");
		}
	}
}
