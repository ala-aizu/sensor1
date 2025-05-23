using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WsStatusUpdate : MonoBehaviour
{
    private const string LOG_PREFIX = "[WsStatusUpdate]";

    public WebSocketClient webSocketClient;

    TextMeshPro text;

    void Start()
    {
        Log("Starting ");

        text = GetComponent<TextMeshPro>();
        text.text = "starting..";

        if (webSocketClient == null) 
        {
            Log("WebSocketClient not assigned, searching for 'ws' tag");
            
            var ws = GameObject.FindWithTag("ws");
            if (ws == null) 
            {
                Log("No 'ws' GameObject found", LogLevel.Warning);
                
                text.text = "No 'ws' found";
                return;
            }

            webSocketClient = ws.GetComponent<WebSocketClient>();
            Log("WebSocketClient found and assigned");

            text.text = webSocketClient.serverUrl;
        }
        Log("Initialization complete");
    }

    void Update()
    {
        if (webSocketClient!=null) {
            text.text = webSocketClient.serverUrl + " " + webSocketClient.Status;
        }
        
    }

 // Log level enum for different types of logging
    private enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    // Generic logging method
    private void Log(string message, LogLevel level = LogLevel.Info, Object context = null)
    {
        string formattedMessage = $"{LOG_PREFIX} {message}";
        
        switch (level)
        {
            case LogLevel.Info:
                Debug.Log(formattedMessage, context ?? this);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(formattedMessage, context ?? this);
                break;
            case LogLevel.Error:
                Debug.LogError(formattedMessage, context ?? this);
                break;
        }
    }

}
