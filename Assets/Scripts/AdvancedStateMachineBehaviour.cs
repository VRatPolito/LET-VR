using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedStateMachineBehaviour : StateMachineBehaviour {

 
    protected AnimatorStateInfo stateInfo;
    public string StateName;
    public string Layer;
    [Range(0.1f, 99.9f)]
    public float StatePercentagePlayedValue = 0.9f;
    bool played;
    bool percplayed;
    private void OnEnable()
    {
        played = false;
        percplayed = false;
    }
    public AnimatorStateInfo StateInfo
    {
        get { return stateInfo; }
    }

    public delegate void MyDelegate(AdvancedStateMachineBehaviour a);
    public MyDelegate StateEnter, StateExit, StateUpdate, StatePlayed, StatePercentagePlayed;

    // Use this for initialization

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.stateInfo = stateInfo;
        if (StateEnter != null)
            StateEnter.Invoke(this);
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.stateInfo = stateInfo;
        if (StateExit != null)
            StateExit.Invoke(this);
        if (played)
            played = false;
        else if (StatePlayed != null)
            StatePlayed.Invoke(this);
        if (percplayed)
            percplayed = false;
        else if (StatePercentagePlayed != null)
            StatePercentagePlayed.Invoke(this);
        base.OnStateExit(animator, stateInfo, layerIndex);

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.stateInfo = stateInfo;
        if (!played)
        {
        if (!percplayed && StateInfo.normalizedTime > StatePercentagePlayedValue / 100 && StatePercentagePlayed != null)
            {
                StatePercentagePlayed.Invoke(this);
                percplayed = true;
            }
        else if (StateInfo.normalizedTime > 1 && !animator.IsInTransition(layerIndex) && StatePlayed != null)
            {
                StatePlayed.Invoke(this);
                played = true;
            }
        else if (StateUpdate != null)
                StateUpdate.Invoke(this);
        }
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }
}

public static class Utilities
{
    public static T GetBehaviour<T>(this Animator animator, AnimatorStateInfo stateInfo) where T : AdvancedStateMachineBehaviour
    {
        return animator.GetBehaviours<T>().ToList().First(behaviour => behaviour.StateInfo.fullPathHash == stateInfo.fullPathHash);
    }
}
