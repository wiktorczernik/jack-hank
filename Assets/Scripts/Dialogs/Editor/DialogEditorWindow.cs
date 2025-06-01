using UnityEditor;
using UnityEngine;
using JackHank.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DialogEditorWindow : EditorWindow
{
    const string groupsRoot = "Assets\\Dialogs\\Instances";

    private Dialog currentDialog;
    private Vector2 scrollPos;

    // Example data
    private Dictionary<string, List<string>> entries = new();
    private Dictionary<string, bool> groupFoldouts = new();
    private int selectedGroupIndex;
    private string dialogName;

    [MenuItem("JackHank/Dialogs/Dialog Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<DialogEditorWindow>("Dialog Editor");
        window.minSize = new Vector2(600, 400);
        window.maxSize = new Vector2(600, 400);
    }

    private void OnGUI()
    {
        if (!currentDialog)
        {
            OnDialogSelectGUI();
        }
    }

    private void OnDialogSelectGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        UpdateDialogsList();

        EditorGUILayout.BeginHorizontal();
        selectedGroupIndex = EditorGUILayout.Popup(selectedGroupIndex, entries.Keys.ToArray(), GUILayout.Width(100));
        dialogName = EditorGUILayout.TextField("", dialogName);
        if (GUILayout.Button("Create", GUILayout.Width(50)))
        {
            Directory.CreateDirectory(Path.Combine(groupsRoot, entries.Keys.ElementAt(selectedGroupIndex), dialogName));
            string assetPath = Path.Combine(groupsRoot, entries.Keys.ElementAt(selectedGroupIndex), dialogName, $"{dialogName}.asset");
            var newDialog = ScriptableObject.CreateInstance<Dialog>();
            AssetDatabase.CreateAsset(newDialog, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            currentDialog = newDialog;
        }
        EditorGUILayout.EndHorizontal();

        foreach (var group in entries)
        {
            if (!groupFoldouts.ContainsKey(group.Key))
            {
                groupFoldouts[group.Key] = true;
            }
            groupFoldouts[group.Key] = EditorGUILayout.Foldout(groupFoldouts[group.Key], group.Key, true, EditorStyles.foldoutHeader);

            if (groupFoldouts[group.Key])
            {
                EditorGUI.indentLevel++;
                foreach (var buttonLabel in group.Value)
                {
                    bool shouldBreak = false;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(buttonLabel))
                    {
                        currentDialog = AssetDatabase.LoadAssetAtPath<Dialog>(Path.Combine(groupsRoot, group.Key, buttonLabel, $"{buttonLabel}.asset"));
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(40)))
                    {
                        string fullpath = Path.Combine(groupsRoot, group.Key, buttonLabel);
                        if (Directory.Exists(fullpath))
                        {
                            AssetDatabase.DeleteAsset(fullpath);
                            AssetDatabase.Refresh();
                            shouldBreak = true;
                            Debug.Log(fullpath);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                    if (shouldBreak) break;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
    }

    private void UpdateDialogsList()
    {
        entries.Clear();

        foreach(string groupDirectory in Directory.GetDirectories(groupsRoot))
        {
            string groupName = Path.GetFileName(groupDirectory);
            List<string> dialogNames = new List<string>();
            foreach(var directory in Directory.GetDirectories(groupDirectory))
            {
                dialogNames.Add(Path.GetFileName(directory));
            }
            entries.Add(groupName, dialogNames);
        }

    }
}
