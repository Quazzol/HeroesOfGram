using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionHandler : MonoBehaviour
{
    private void Awake()
    {
        Application.logMessageReceived += HandleException;
    }

    private void HandleException(string condition, string stackTrace, LogType type)
    {
        JsonDataSaver.Save<string>("stackTrace", "error_log");
    }
}
