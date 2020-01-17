using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DestinationEvent  : UnityEvent<Destination> {}

public class Destination : MonoBehaviour {

    public DestinationEvent OnDisabled, OnEnabled;
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
        OnDisabled?.Invoke(this);
    }
    public virtual void OnEnable()
    {
        OnEnabled?.Invoke(this);
    }
}
