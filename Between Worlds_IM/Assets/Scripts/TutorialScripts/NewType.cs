using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewType : MobileUnit
{
    private bool _firstTarget = true;

    public override void HasReachedTarget()
    {
        //test if agent has reached target
        if (!Agent.pathPending) //if agent is looking for target it hasn't reached target
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance) //agent is as close as it can get
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f) //if agent isn't moving
                {
                    Debug.Log("Target Reached!!!");

                    if (_firstTarget) //for the first target we update the target
                    {
                        GameObject target = Manager.Instance.GetTarget();
                        UpdateTarget(target);
                        _firstTarget = false;
                        return;
                    }

                    _reachedTarget = true;
                    Agent.enabled = false;
                }
            }
        }
    }
}