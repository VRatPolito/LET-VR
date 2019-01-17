using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace KATVR
{
    public class KATDevice_Walk : MonoBehaviour
    {
        #region Basic Variable

        public bool LogRawData = false;

        [SerializeField] private static KATDevice_RawData _rawData;
        //public static int bodyYaw;
        //public static int isMoving;
        //public static int moveDirection;
        //public static double walkPower;
        //public static float meter;
        public static bool Launched;
        private static float _moveSpeed, _maxMovePower, _bodyRotation, _newBodyYaw, _newCameraYaw;




        #region Rec

        public KATDevice_Data Data;

        //[HideInInspector]
        //public float data_bodyYaw, data_meter, data_moveSpeed, data_DisplayedSpeed;
        //[HideInInspector]
        // public double data_walkPower;
        //[HideInInspector]
        //public int data_moveDirection, data_isMoving;
        #endregion

        #endregion

        #region Helper Methods

        public void Initialize(int count)
        {
            Init(count);
        }

        public bool LaunchDevice()
        {
            if (CheckForLaunch())
            {
                Launch();
                Launched = true;
            }
            else
            {
                Launched = false;
            }
            return Launched;
        }

        public bool Stop()
        {
            Halt();
            return true;
        }

        public void UpdateData()
        {
            if (!Launched) return;

            GetWalkerData(0, ref _rawData.bodyYaw, ref _rawData.walkPower, ref _rawData.moveDirection, ref _rawData.isMoving, ref _rawData.meter);
            _rawData.bodyYaw = (int)Math.Floor((float)_rawData.bodyYaw / 1024 * 360);
            _bodyRotation = (float)_rawData.bodyYaw - _newBodyYaw + _newCameraYaw;
            _rawData.walkPower = Math.Round((double)_rawData.walkPower, 2);
            _moveSpeed = (float)_rawData.walkPower / 3000f;
            _rawData.moveDirection = -_rawData.moveDirection;

            if (_moveSpeed > 1) _moveSpeed = 1;
            else if (_moveSpeed < 0.3f) _moveSpeed = 0;

            Data.bodyYaw = _bodyRotation;
            Data.walkPower = _rawData.walkPower;
            Data.moveSpeed = Data.displayedSpeed = _moveSpeed;
            Data.moveDirection = _rawData.moveDirection;
            Data.isMoving = _rawData.isMoving;
            Data.meter = _rawData.meter;
            if (LogRawData)
            {
                Debug.Log("Raw Data = " + _rawData);
            }
        }

        public void ResetCamera(Transform handset)
        {
            if (handset != null)
            {
                _newCameraYaw = handset.transform.localEulerAngles.y;
                _newBodyYaw = (float)_rawData.bodyYaw;
            }
            else
            {
                Debug.LogError("Data does not exist");
            }
        }

        #endregion


        #region Dllinput

        [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
        static extern void Init(int count);

        [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
        static extern int Launch();

        [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
        static extern void Halt();

        [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
        static extern bool GetWalkerData(int index, ref int bodyyaw, ref double walkpower, ref int movedirection, ref int ismoving, ref float distancer);

        [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CheckForLaunch();

        #endregion
    }
}

