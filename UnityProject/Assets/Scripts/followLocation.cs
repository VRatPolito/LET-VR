using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followLocation : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        capsuleCollider.transform.position = new Vector3(mainCamera.transform.position.x, capsuleCollider.center.y, mainCamera.transform.position.z);
    }
}
