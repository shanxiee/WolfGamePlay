using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComposeNode : Node
{
    List<Node> children = new List<Node>();
}
