
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	#region Events
		
	#endregion
	
	#region Editor Visible
		
	#endregion
	
	#region Private Members and Constants

    [SerializeField] [Range(1,400)]private float _speed = 150;
		
	#endregion
	
    #region Properties
		
    #endregion
 
    #region MonoBehaviour

	
    #endregion
 
	#region Public Methods
		
	#endregion
 
    #region Helper Methods
		
	#endregion
	
	#region Events Callbacks
		
	#endregion
	
	#region Coroutines
		
	#endregion

    public float Speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }
}

// The rotation speed component simply stores the Speed value
[Serializable]
public struct RotationSpeed : IComponentData
{
    public float Value;
}

//public class RotationSpeedComponent : ComponentDataWrapper<RotationSpeed> { }

public class RotatorSystem : ComponentSystem
{
    struct Components
    {
        public Rotator Rotator;
        public Transform Transform;
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.deltaTime;
        foreach (var e in GetEntities<Components>())
        {
            e.Transform.Rotate(0, e.Rotator.Speed * deltaTime, 0, Space.World);
        }
    }


    //  [BurstCompile]
    //  struct RotationSpeedRotation : IJobProcessComponentData<Rotation, RotationSpeed>
    //  {
    //      public float dt;

    //      public void Execute(ref Rotation rotation, [ReadOnly]ref RotationSpeed speed)
    //      {
    //          rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.up(), speed.Value * dt));
    //      }
    //  }

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //  {
    //      Debug.Log("JOB?");
    //      var job = new RotationSpeedRotation() { dt = Time.deltaTime };
    //      return job.Schedule(this, inputDeps);
    //  }
}