using System;
using System.Text;
using TMPro;
using UnityEngine;

public class RuntimeConsole_GUI : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] TMP_Text text;

    private StringBuilder buffer = new StringBuilder();

    bool isVisible = false;

    private void Awake()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);

        Application.logMessageReceived += HandleLog;
        Application.logMessageReceivedThreaded += HandleLog;
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceivedThreaded -= HandleLog;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            isVisible = !isVisible;
        parent.SetActive(isVisible);
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Warning) return;

        buffer.AppendLine($"[{DateTime.Now.ToShortTimeString()}] [{type}] {condition}");
        buffer.AppendLine(stackTrace);
        text.text = buffer.ToString();
    }
}
