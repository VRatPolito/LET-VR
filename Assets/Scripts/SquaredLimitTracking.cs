using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class SquaredLimitTracking : LimitTracking
{
    [Range(0.0f, 5.0f)]
    public float Xmax;
    [Range(-5.0f, 0.0f)]
    public float Xmin;
    [Range(0.0f, 5.0f)]
    public float Ymax;
    [Range(-5.0f, 0.0f)]
    public float Ymin;
    [Range(0.0f, 5.0f)]
    public float Zmax;
    [Range(-5.0f, 0.0f)]
    public float Zmin;

    public override void CheckBoundaries(Vector3 NewCenterEyeLocalPos, out bool outofbounds, out bool vibrate, out float x, out float y, out float z)
    {
        x = y = z = 0;
        vibrate = false;
        outofbounds = false;

        //Calculate the offsets between the actual and the initial VRNode.CenterEye localPositions
        var offsx = NewCenterEyeLocalPos.x - StartCenterEyeLocalPos.x;
        var offsz = NewCenterEyeLocalPos.z - StartCenterEyeLocalPos.z;
        var offsy = NewCenterEyeLocalPos.y - StartCenterEyeLocalPos.y;

        if (offsx > Xmax)
            { 
            /*if the offset on is above the maximum, CameraRig has to be displaced by:
            1) -StartCenterEyeLocalPos.x to keep the initial centering
            2) -offsx to put again the user at center
            3) +Xmax to maintain the user at the max distance until he will return inside the available space*/
            x = -StartCenterEyeLocalPos.x - offsx + Xmax;
            //signal the outofbounds status
            outofbounds = true;
            }
        else if (offsx < Xmin)
            { 
            /*else if it is below the minimum, CameraRig has to be displaced by:
            1) -StartCenterEyeLocalPos.x to keep the initial centering
            2) -offsx to put again the user at center
            3) +Xmin to maintain the user at the min distance until he will return inside the available space*/
            x = -StartCenterEyeLocalPos.x - offsx + Xmin;
            //signal the outofbounds status
            outofbounds = true;
            }
        else
            //else only the initial centering has to be applied
            x = -StartCenterEyeLocalPos.x;

        //only Xmax and Xmin are not both 0, otherwise it would vibrate 24/7
        if (Xmax > 0 || Xmin > 0)
        {
            //if I'm below or above the warning vibration thresholds, vibrate
            if ((NewCenterEyeLocalPos.x > StartCenterEyeLocalPos.x + VibrationThreshold / 100 * Xmax) || (NewCenterEyeLocalPos.x < StartCenterEyeLocalPos.x + VibrationThreshold / 100 * Xmin))
                vibrate = true;
        }

        //Skip the Y if IgnoreY is true
        if (!IgnoreY)
            {
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
            y = CameraRig.localPosition.y;

        if (offsz > Zmax)
            {
            z = -StartCenterEyeLocalPos.z - offsz + Zmax;
            outofbounds = true;
            }
        else if (offsz < Zmin)
            {
            z = -StartCenterEyeLocalPos.z - offsz + Zmin;
            outofbounds = true;
            }
        else
            z = -StartCenterEyeLocalPos.z;

        if (Zmax > 0 || Zmin > 0)
        {
            if ((NewCenterEyeLocalPos.z > StartCenterEyeLocalPos.z + VibrationThreshold / 100 * Zmax) || (NewCenterEyeLocalPos.z < StartCenterEyeLocalPos.z + VibrationThreshold / 100 * Zmin))
                vibrate = true;
        }
    }
    
    internal override void CopyConstraints(LimitTracking lt)
    {
        var l = (SquaredLimitTracking)lt;
        if (l != null)
        {
            Xmax = l.Xmax;
            Xmin = l.Xmin;
            Ymax = l.Ymax;
            Ymin = l.Ymin;
            Zmax = l.Zmax;
            Zmin = l.Zmin;
        }
    }
    internal void FlipXLimit()
    {
        var x = Xmin;
        Xmin = -Xmax;
        Xmax = -Xmin;
    }
}
