using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PIDPars", menuName = "PID/Pars", order = 1)]
public class PIDPars : ScriptableObject
{
    public float Kp = 5f;
    public float Ki = 2f;
    public float Kd = 1f;
   

    public int offset = 0;
    public int maxout = 10000;
    public int minout = -10000;

    [Range(0,1)] public float smoothing = 0f;

    //public int violenzaPiattaforma = 7;
}
