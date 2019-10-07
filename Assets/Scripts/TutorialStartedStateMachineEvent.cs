using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStartedStateMachineEvent : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.StartGameFromNormal();

        base.OnStateEnter(animator, stateInfo, layerIndex);
    }
}
