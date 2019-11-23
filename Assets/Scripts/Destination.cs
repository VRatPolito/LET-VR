using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    public event Action<Destination> OnDisabled, OnEnabled;
    public GameObject Next;
    // Use this for initialization
    public virtual void Start () {
		
	}

    // Update is called once per frame
    public virtual void Update () {
		
	}

    public virtual void OnDisable()
    {
        if(Next != null)
            Next.SetActive(true);
        if(OnDisabled != null)
            OnDisabled.Invoke(this);
    }
    public virtual void OnEnable()
    {
        if (OnEnabled != null)
            OnEnabled.Invoke(this);
    }
}
