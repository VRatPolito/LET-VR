using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace KATVR
{
    #region DataType

    [Serializable]
    public struct KATDevice_RawData
    {
        public int bodyYaw, isMoving, moveDirection;
        public double walkPower;
        public float meter;

        public KATDevice_RawData(int bodyYaw, int isMoving, int moveDirection, double walkPower, float meter)
        {
            this.bodyYaw = bodyYaw;
            this.isMoving = isMoving;
            this.moveDirection = moveDirection;
            this.walkPower = walkPower;
            this.meter = meter;
        }

        public object[] ToObjectArray()
        {
            return new object[] { bodyYaw, isMoving, moveDirection, walkPower, meter};
        }
        public static implicit operator object[] (KATDevice_RawData rawData)
        {
            return rawData.ToObjectArray();
        }

        public override string ToString()
        {
            return string.Format("bodyYaw={0}, isMoving={1}, moveDirection={2}, walkPower={3:0.00}, meter={4:0.00}", this);
        }
    }

    [Serializable]
    public struct KATDevice_Data
    {
        public float bodyYaw, meter, moveSpeed, displayedSpeed;
        public double walkPower;
        public int moveDirection, isMoving;

        public KATDevice_Data(float bodyYaw, float meter, float moveSpeed, float displayedSpeed, double walkPower, int moveDirection, int isMoving)
        {
            this.bodyYaw = bodyYaw;
            this.meter = meter;
            this.moveSpeed = moveSpeed;
            this.displayedSpeed = displayedSpeed;
            this.walkPower = walkPower;
            this.moveDirection = moveDirection;
            this.isMoving = isMoving;
        }
    }

    #endregion


    public static class KATVR_Basic {
        #region Basic Variable
        public enum LanguageList { Chinese, English };
        public static LanguageList Language;
        public static string LanguageFilePath = Application.dataPath + "/LanguageFile.xml";
        #endregion
    }

    public class KATVR_Global
    {
        public static KATDevice_Walk KDevice_Walk;
    }

}

