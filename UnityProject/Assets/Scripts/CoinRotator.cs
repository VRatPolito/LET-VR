using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    [SerializeField] private float _speed = 75.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _speed * Time.deltaTime);
    }
}
