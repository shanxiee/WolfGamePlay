using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecorateNode : Node
{
    [HideInInspector]public Node child;

    public override Node Clone()
    {
        DecorateNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }

}
