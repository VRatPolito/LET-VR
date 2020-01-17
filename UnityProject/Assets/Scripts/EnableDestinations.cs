using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDestinations : MonoBehaviour {

    [SerializeField] protected Destination[] Destinations;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        for(int i=0; i< Destinations.Length; i++)
        {
            if (i == 0)
                Destinations[i].gameObject.SetActive(true);
            else
                Destinations[i].gameObject.SetActive(false);
        }
    }
}
