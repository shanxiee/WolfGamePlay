using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Callbacks;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIBuilder/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);


        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        blackboardView = root.Q<IMGUIContainer>();
        blackboardView.onGUIHandler = () =>
        {
            treeObject.Update();
            EditorGUILayout.PropertyField(blackboardProperty);
            treeObject.ApplyModifiedProperties();
        };

        treeView.OnNodeSelected = OnNodeSelectionChange;

        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
    }
    private void OnPlayModeStateChange(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if (Application.isPlaying)
        {
            if (tree)
            {
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.OpenAsset(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }

        if (tree != null)
        {
            treeObject = new SerializedObject(tree);
            blackboardProperty = treeObject.FindProperty("blackboard");
        }

    }

    private void OnNodeSelectionChange(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }

}