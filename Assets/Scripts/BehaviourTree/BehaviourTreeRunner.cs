using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    BehaviourTree tree;

    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "Hello --1-- !!";

        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "Hello --2-- !!";

        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "Hello --3-- !!";

        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence.children.Add(log1);
        sequence.children.Add(log2);
        sequence.children.Add(log3); 
 
        var loopNode = ScriptableObject.CreateInstance<RepeatNode>();
        loopNode.child = sequence;

        tree.rootNode = loopNode;

    }

    void Update()
    {
        tree.Update();
    }
}
