using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public enum FollowComponents { XYZ, XY, XZ, YZ, X, Y, Z};
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);
        public bool local = false;
        public FollowComponents components = FollowComponents.XYZ;


        private void LateUpdate()
        {
            if(local)
            {
                switch (components)
                {
                    case FollowComponents.XYZ:
                        transform.localPosition = target.localPosition + offset;
                        break;
                    case FollowComponents.XY:
                        transform.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, transform.localPosition.z) + offset;
                        break;
                    case FollowComponents.XZ:
                        transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z) + offset;
                        break;
                    case FollowComponents.YZ:
                        transform.localPosition = new Vector3(transform.localPosition.x, target.localPosition.y, target.localPosition.z) + offset;
                        break;
                    case FollowComponents.X:
                        transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, transform.localPosition.z) + offset;
                        break;
                    case FollowComponents.Y:
                        transform.localPosition = new Vector3(transform.localPosition.x, target.localPosition.y, transform.localPosition.z) + offset;
                        break;
                    case FollowComponents.Z:
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, target.localPosition.z) + offset;
                        break;
                }
            }
            else
            {
                switch(components)
                {
                    case FollowComponents.XYZ:
                        transform.position = target.position + offset;
                        break;
                    case FollowComponents.XY:
                        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
                        break;
                    case FollowComponents.XZ:
                        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z) + offset;
                        break;
                    case FollowComponents.YZ:
                        transform.position = new Vector3(transform.position.x, target.position.y, target.position.z) + offset;
                        break;
                    case FollowComponents.X:
                        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z) + offset;
                        break;
                    case FollowComponents.Y:
                        transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z) + offset;
                        break;
                    case FollowComponents.Z:
                        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z) + offset;
                        break;
                }
            }
        }
    }
}
