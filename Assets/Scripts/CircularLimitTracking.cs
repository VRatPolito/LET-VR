using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.AI;

public class CircularLimitTracking : LimitTracking
{
    [Range(0.0f, 5.0f)]
    public float Ymax;
    [Range(-5.0f, 0.0f)]
    public float Ymin;
    [Range(0.01f, 5.0f)]
    public float MaxDist;

    public override void CheckBoundaries(Vector3 NewCenterEyeLocalPos, out bool outofbounds, out bool vibrate, out float x, out float y, out float z)
    {
        x = y = z = 0;
        vibrate = false;
        outofbounds = false;
        //Generate the Vector2(x, z) versions of StartCenterEyeLocalPos and NewCenterEyeLocalPos for the circular limit check
        Vector2 StartCenterEyeLocalPosXZ = new Vector2(StartCenterEyeLocalPos.x, StartCenterEyeLocalPos.z);
        var NewCenterEyeLocalPosXZ = new Vector2(NewCenterEyeLocalPos.x, NewCenterEyeLocalPos.z);


        //Calculate the offsets between the actual and the initial VRNode.CenterEye localPositions
        var offsxz = Vector2.Distance(StartCenterEyeLocalPosXZ, NewCenterEyeLocalPosXZ);
        //Calculate the vector useful to generate the max radial position in case of outofbounds
        var newPosXZ = (NewCenterEyeLocalPosXZ - StartCenterEyeLocalPosXZ).normalized;
        var offsx = NewCenterEyeLocalPos.x - StartCenterEyeLocalPos.x;
        var offsz = NewCenterEyeLocalPos.z - StartCenterEyeLocalPos.z;
        var offsy = NewCenterEyeLocalPos.y - StartCenterEyeLocalPos.y;

        if (MaxDist > 0 && offsxz > MaxDist * VibrationThreshold / 100.0f)
            vibrate = true;

        if (offsxz > MaxDist)
            {
                outofbounds = true;
                newPosXZ *= MaxDist;

                if (IgnoreY)
                {
                    x = -StartCenterEyeLocalPos.x - offsx + newPosXZ.x;
                    y = CameraRig.localPosition.y;
                    z = -StartCenterEyeLocalPos.z - offsz + newPosXZ.y;

                }
                else
                {
                    x = -StartCenterEyeLocalPos.x - offsx + newPosXZ.x;
                    z = -StartCenterEyeLocalPos.z - offsz + newPosXZ.y;
                

                    if (offsy > Ymax)
                        y = -StartCenterEyeLocalPos.y - offsy + Ymax;
                    else if (offsy < Ymin)
                        y = -StartCenterEyeLocalPos.y - offsy + Ymin;
                    else
                        y = -StartCenterEyeLocalPos.y;
                }
            }
            else if (!IgnoreY)
            {
                x = -StartCenterEyeLocalPos.x;
                z = -StartCenterEyeLocalPos.z;

                if (offsy > Ymax)
                    {
                        y = -StartCenterEyeLocalPos.y - offsy + Ymax;
                        outofbounds = true;
                    }
                    else if (offsy < Ymin)
                    {
                        y = -StartCenterEyeLocalPos.y - offsy + Ymin;
                        outofbounds = true;
                    }
                    else
                        y = -StartCenterEyeLocalPos.y;

                if (Ymax > 0 || Ymin > 0)
                {
                    if ((NewCenterEyeLocalPos.y > StartCenterEyeLocalPos.y + VibrationThreshold / 100 * Ymax) || (NewCenterEyeLocalPos.y < StartCenterEyeLocalPos.y + VibrationThreshold / 100 * Ymin))
                        vibrate = true;
                }
            }
            else
            {
                x = -StartCenterEyeLocalPos.x;
                y = CameraRig.localPosition.y;
                z = -StartCenterEyeLocalPos.z;
            }
    }

    internal override void CopyConstraints(LimitTracking lt)
    {
        var l = (CircularLimitTracking)lt;
        if (l != null)
        {
            MaxDist = l.MaxDist;
            Ymax = l.Ymax;
            Ymin = l.Ymin;
        }
    }
}
