using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrattiToolkit
{
    [RequireComponent(typeof(Collider))]
    public class ColliderEventsListener : MonoBehaviour
    {
        public event Action<Collider> OnTriggerEnterAction;
        public event Action<Collider> OnTriggerStayAction;
        public event Action<Collider> OnTriggerExitAction;
        public event Action<Collision> OnColliderEnterAction;
        public event Action<Collision> OnColliderStayAction;
        public event Action<Collision> OnColliderExitAction;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction.RaiseEvent(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction.RaiseEvent(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayAction.RaiseEvent(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            OnColliderEnterAction.RaiseEvent(other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnColliderStayAction.RaiseEvent(other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnColliderExitAction.RaiseEvent(other);
        }
    }
}