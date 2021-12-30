using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();
    public BlackBoard blackboard = new BlackBoard();

    public Node.State Update()
    {
        if (rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

#if UNITY_EDITOR 

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCompleteObjectUndo(node, "Behaviour Tree (CreateNode)");

        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {

        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);

        // AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecorateNode decorateNode = parent as DecorateNode;
        if (decorateNode)
        {
            Undo.RecordObject(decorateNode, "Behaviour Tree (AddChild)");
            decorateNode.child = child;
            EditorUtility.SetDirty(decorateNode);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);
        }

        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            Undo.RecordObject(compositeNode, "Behaviour Tree (AddChild)");
            compositeNode.children.Add(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecorateNode decorateNode = parent as DecorateNode;
        if (decorateNode)
        {
            Undo.RecordObject(decorateNode, "Behaviour Tree (RemoveChild)");
            decorateNode.child = null;
            EditorUtility.SetDirty(decorateNode);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            Undo.RecordObject(compositeNode, "Behaviour Tree (RemoveChild)");
            compositeNode.children.Remove(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }
#endif
    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        DecorateNode decorateNode = parent as DecorateNode;
        if (decorateNode && decorateNode.child != null)
        {
            children.Add(decorateNode.child);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }


        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            return compositeNode.children;
        }

        return children;
    }

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, (n) =>
        {
            tree.nodes.Add(n);
        });
        return tree;
    }

    public void Bind(AiAgent agent)
    {
        Traverse(rootNode, node =>
        {
            node.agent = agent;
            node.blackBoard = blackboard;
        });
    }
}
