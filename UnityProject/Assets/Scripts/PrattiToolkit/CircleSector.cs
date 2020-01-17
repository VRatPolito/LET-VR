/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrattiToolkit
{

    public static class CircleSector
    {
        #region Events

        #endregion

        #region Editor Visible

        #endregion

        #region Private Members and Constants

        private const float RAY_SECTOR_BOUND_DELTA = 1f;

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static Collider[] OverlapSectorNonAlloc(Transform center, float radius, float angle,
            int collidersBufferSize,
            int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Collider[] sphereResults = new Collider[collidersBufferSize];
            List<Collider> sectorResults = new List<Collider>();

            Vector3 sectorBound1 = Vector3.zero, sectorBound2 = Vector3.zero;
            ComputeSectorBoundaries(center, radius, angle, out sectorBound1, out sectorBound2);

            int foundSphereColliders =
                Physics.OverlapSphereNonAlloc(center.position, radius, sphereResults, layerMask,
                    queryTriggerInteraction);

            for (int i = 0; i < foundSphereColliders; i++)
            {
                if (IsObjectInSector(sphereResults[i], center, radius, sectorBound1, sectorBound2, layerMask,
                    QueryTriggerInteraction.Ignore))
                    sectorResults.Add(sphereResults[i]);
            }

            return sectorResults.ToArray();
        }

        public static void DrawCircleSector(Transform center, float radius, float angle)
        {
            Vector3 sectorBound1 = Vector3.zero, sectorBound2 = Vector3.zero;
            ComputeSectorBoundaries(center, radius, angle, out sectorBound1, out sectorBound2);

            //Draw Sector Boundaries
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(center.position, sectorBound1);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(center.position, sectorBound2);

            /*
            Gizmos.color = Color.white;
            float theta = 0f;
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = center.position + new Vector3(x, 0f, z);
            Vector3 newPos = pos;
            Vector3 lastPos = pos;
            for (theta = 0.1f; theta < Mathf.Deg2Rad * angle * 2; theta += 0.1f)
            {
                x = radius * Mathf.Cos(theta);
                z = radius * Mathf.Sin(theta);
                newPos = center.position + new Vector3(x, 0f, z);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);
            */

        }

        #endregion

        #region Helper Methods

        private static void ComputeSectorBoundaries(Transform center, float radius, float angle,
            out Vector3 sectorBound1, out Vector3 sectorBound2)
        {
            float sectorOffsetAngle = Vector3.Angle(Vector3.right, center.right);
            Vector3 crossProduct = Vector3.Cross(Vector3.right, center.right);
            sectorOffsetAngle *= -Mathf.Sign(crossProduct.y);

            float sectorMaxRadius = radius;
            Vector3 sectorCenterPos = center.position;

            float boundary_1_Angle = 90f - (angle / 2f) + sectorOffsetAngle;
            float boundary_2_Angle = boundary_1_Angle + angle;

            Vector3 boundary_1_OnCirclePos = new Vector3(
                center.position.x + sectorMaxRadius * Mathf.Cos(boundary_1_Angle * Mathf.Deg2Rad),
                center.position.y,
                center.position.z + sectorMaxRadius * Mathf.Sin(boundary_1_Angle * Mathf.Deg2Rad));

            Vector3 boundary_2_OnCirclePos = new Vector3(
                center.position.x + sectorMaxRadius * Mathf.Cos(boundary_2_Angle * Mathf.Deg2Rad),
                center.position.y,
                center.position.z + sectorMaxRadius * Mathf.Sin(boundary_2_Angle * Mathf.Deg2Rad));

            sectorBound1 = boundary_1_OnCirclePos - sectorCenterPos;
            sectorBound2 = boundary_2_OnCirclePos - sectorCenterPos;

        }

        private static bool IsObjectInSector(Collider hittedObject, Transform center, float radius,
            Vector3 sectorBound1, Vector3 sectorBound2, int layerMask = ~0,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            //CONDITION 1
            //First Check if object is inside wave sector
            Vector3 objectHittedPosZeroY = hittedObject.transform.position.ZeroY();
            Vector3 sectorPosZeroY = center.position.ZeroY();

            //Code for checking if point is inside circle sector inspired from:
            //https://stackoverflow.com/questions/13652518/efficiently-find-points-inside-a-circle-sector

            Vector3 relPointPosition = objectHittedPosZeroY - sectorPosZeroY;
            //Check CounterClockwise of Point with respect to sector Start
            if (!IsPointClockWise(relPointPosition, sectorBound1) && IsPointClockWise(relPointPosition, sectorBound2))
                return true;


            //CONDITION 2
            //If Inside Wave Sector Fails, check for collision on the edge of the sector
            Ray[] sectorBoundsRay =
            {
                new Ray(center.position, sectorBound1.normalized),
                new Ray(center.position, sectorBound2.normalized)
            };
            //DEBUG SHOW RAYS
            //Debug.DrawRay(center.position, sectorBound1.normalized * (radius + RAY_SECTOR_BOUND_DELTA), Color.magenta, 2f);
            //Debug.DrawRay(center.position, sectorBound2.normalized * (radius + RAY_SECTOR_BOUND_DELTA), Color.yellow, 2f);

            RaycastHit hit;
            for (int i = 0; i < sectorBoundsRay.Length; i++)
            {

                if (Physics.Raycast(sectorBoundsRay[i], out hit, radius + RAY_SECTOR_BOUND_DELTA, layerMask,
                    queryTriggerInteraction))
                {
                    Collider hitted = hit.collider;
                    if (hitted == hittedObject)
                        return true;
                }
            }

            //If non of the conditions are met, it is not an element inside of the wave sector
            return false;
        }

        private static bool IsPointClockWise(Vector3 v1, Vector3 v2)
        {
            Vector3 counterClockWiseNor = new Vector3(-v1.z, v1.y, v1.x);
            float dot = Vector3.Dot(v2, counterClockWiseNor);
            return dot > 0f;
        }

        #endregion

        #region Events Callbacks

        #endregion

        #region Coroutines

        #endregion

    }
}
