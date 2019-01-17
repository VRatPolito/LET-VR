using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : MonoBehaviour {

    bool vibrate = false;
    Coroutine Vibration;
    float NextVibration = 0;
    public class VibrationRequest
    {
        public float Length;
        public float Strength;
        public float PulseTime;
        public object Requester;

        public VibrationRequest(float Length, float Strength, float PulseTime, object Requester)
        {
            this.Length = Length;
            this.Strength = Strength;
            this.PulseTime = PulseTime;
            this.Requester = Requester;
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            VibrationRequest v = (VibrationRequest)obj;
            if ((Length == v.Length) && (Strength == v.Strength) && (PulseTime == v.PulseTime) && (Requester == v.Requester))
                    return true;
            else
                    return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    Queue<VibrationRequest> requests;
    VibrationRequest currequest;
    SteamVR_TrackedController controller;
    // Use this for initialization
    void Start () {
        controller = GetComponent<SteamVR_TrackedController>();
    }

    private void Awake()
    {
        requests = new Queue<VibrationRequest>();
    }

    // Update is called once per frame
    void Update ()
    {
        Vibrate();
    }

    public void ShortVibration()
    {
        if (!vibrate)
        {
            if (Vibration != null)
                StopCoroutine(Vibration);
            StartCoroutine(LongVibration(0.1f, 0.2f));
        }
    }
    public void ShortVibration(float intensity)
    {
        if (!vibrate)
        {
            if (Vibration != null)
                StopCoroutine(Vibration);
            StartCoroutine(LongVibration(0.1f, intensity));
        }
    }

    IEnumerator LongVibration(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            SteamVR_Controller.Input((int)controller.controllerIndex).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
        Vibration = null;
    }

    public bool StartVibration(float Length, float Strength, float PulseTime, object Requester)
    {
        VibrationRequest v = new VibrationRequest(Length, Strength, PulseTime, Requester);
        v.Length = Length;
        v.Strength = Strength;
        v.PulseTime = PulseTime;
        if (currequest != null)
            requests.Enqueue(v);
        else
            currequest = v;
        
        if (!vibrate)
        {
            NextVibration = Time.time;
            vibrate = true;
        }
        return true;

    }

    private bool AlreadyRequested(object requester)
    {
        foreach (VibrationRequest v in requests)
            if (v.Requester == requester)
                return true;
        return false;
    }

    public bool StopVibration(float Length, float Strength, float PulseTime, object Requester)
    {
        VibrationRequest v = new VibrationRequest(Length, Strength, PulseTime, Requester);
        var stopped = false;
        if (currequest != null && v.Equals(currequest))
            stopped = StopCurrentVibration();
        else if (requests.Contains(v))
        {
            var q = new Queue<VibrationRequest>();
            foreach (VibrationRequest r in requests)
                if (!r.Equals(v))
                    q.Enqueue(r);
            requests.Clear();
            requests = q;
            stopped = true;
        }
        return stopped;
    }

    bool StopCurrentVibration()
    {
        if (requests.Count == 0)
        {
            vibrate = false;
            currequest = null;
            return false;
        }
        else
        {
            currequest = requests.Dequeue();
            NextVibration = Time.time;
            return true;
        }
    }

    void Vibrate()
    {
        if (vibrate && currequest != null)
        {
            var now = Time.time;
            if (now >= NextVibration)
            {
                Vibration = StartCoroutine(LongVibration(currequest.Length, currequest.Strength));
                NextVibration += currequest.PulseTime;
            }
        }
    }
}
