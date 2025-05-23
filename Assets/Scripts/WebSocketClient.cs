using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using System; // or your preferred JSON library

public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    public string serverUrl = "ws://192.168.8.139:8082/ALL"; 

    public string Status = "unknown";

    public Action<int> OnHeartRateUpdate;
    public Action<int> OnLevelTrigger;

    [System.Serializable]
    public class Message
    {
        // Define properties matching your server's message format
        public string from;
        public DataMessage data;
    }

    [System.Serializable]
    public class DataMessage
    {
        // Define properties matching your server's message format
        public string type;
        public string timestamp;
        public DataMessageValues values;
    }


    [System.Serializable]
    public class DataMessageValues {
        public int HEART_RATE;
        public string STATUS;
        public int Level;
    }

    public DataMessage dataMessage;

    async void Start()
    {
        _log("Opening connection to " + serverUrl);
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () => {
            _log("Connection open!");
            Status = "connected";
        };
        websocket.OnError += (e) => {
            _logError("Error! " + e);
            Status = "error";
        };
        
        websocket.OnClose += (e) => {
            _log("Connection closed!");
            Status = "closed";
        };
        
        websocket.OnMessage += (bytes) =>
        {      
            // _log("OnMessage!");
            // _log(bytes);

            // getting the message as a string
            var messageString = System.Text.Encoding.UTF8.GetString(bytes);
            _log("OnMessage! " + messageString);
            
            // Parse JSON
            try
            {
                var message = JsonConvert.DeserializeObject<Message>(messageString);

                if (message.data.type == "HEART_RATE")
                {
                    this.dataMessage = message.data;

                    OnHeartRateUpdate?.Invoke(message.data.values.HEART_RATE);
                }
                else if (message.data.type == "trigger:action")
                {
                    var level = message.data.values.Level;
                    _log("Trigger action received for level: " + level);
                    OnLevelTrigger?.Invoke(message.data.values.Level);
                }
            }
            catch (JsonException e)
            {
                _logError("JSON parsing error: " + e.Message);
            }
        };

        await websocket.Connect();
    }
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private async void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private void _log(string message) 
    {
        Debug.Log($"WebSocketClient| {message}");
    }

    private void _logError(string message) 
    {
        Debug.LogError($"WebSocketClient| {message}");
    }

}