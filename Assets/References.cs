﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
	public GameObject[] references;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] GetReferencesAsGameObject(){
    	return references;
    }
}
