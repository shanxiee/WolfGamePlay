using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{

    public float duraction = 1;
    float startTime;

    protected override void OnStart()
    {
        state = State.Running;
        startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (Time.time - startTime > duraction)
        {
            return State.Success;
        }
        return State.Running;
    }
}
