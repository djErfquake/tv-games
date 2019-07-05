using System.Threading;
using System;
using System.Net.WebSockets;
using NiceJson;
using UnityEngine;
using System.Net;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class ServerCommunicator
{
    private ClientWebSocket socket = null;
    public bool connected = false;

    private System.Uri uri;
    private ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[10 * 1024 * 1024]);

    private string incomingMessageType = string.Empty;
    private string incomingMessage = string.Empty;

    public PlayerJoinedEvent OnPlayerJoined = new PlayerJoinedEvent();
    public PlayerLeftEvent OnPlayerLeft = new PlayerLeftEvent();
    public MessageEvent OnMessage = new MessageEvent();


    public ServerCommunicator(string serverURL = "192.168.0.69:1818")
    {
        uri = new System.Uri("ws://" + serverURL);
        Connect();
    }

    private async void Connect()
    {
        socket = new ClientWebSocket();
        socket.Options.KeepAliveInterval = new TimeSpan(0, 0, 30);
        try
        {
            await socket.ConnectAsync(uri, CancellationToken.None);
            if (socket.State == WebSocketState.Open) { Debug.Log("Connected to server."); }
            JoinServer();
            GetMessage();
        }
        catch (Exception e)
        {
            Debug.Log("woe " + e.Message);
        }
    }


    private async void GetMessage() {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult r = await socket.ReceiveAsync(buffer, CancellationToken.None);
            string messageString = System.Text.Encoding.UTF8.GetString(buffer.Array, 0, r.Count);
            //Debug.Log("New Message: " + messageString);

            JsonNode message = JsonNode.ParseJsonString(messageString);
            string messageType = message["messageType"];
            switch (messageType)
            {
                case "PLAYER_JOINED":
                    PlayerJoined(message["player"]);
                    break;
                case "PLAYER_LEFT":
                    OnPlayerLeft.Invoke(message["player"]);
                    break;
                default:
                    OnMessage.Invoke(message);
                    break;   
            }




            GetMessage();
        }
    }

    private void JoinServer() {
        JsonObject message = new JsonObject();
        message.Add("messageType", "JOIN_ROOM");
        message.Add("name", "BIG_SCREEN");
        SendMessage(message);
    }




    public void Close()
    {
        if (socket != null)
        {
            Debug.Log("Disconnecting from server.");
            connected = false;

            socket.Abort();
            socket.Dispose();
            socket = null;
        }
    }



    private void PlayerJoined(JsonNode eventInfo)
    {
        string mainColorHTML = eventInfo["color"]["main"];
        Color mainColor = new Color(int.Parse(mainColorHTML.Substring(1, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
                                    int.Parse(mainColorHTML.Substring(3, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
                                    int.Parse(mainColorHTML.Substring(5, 2), System.Globalization.NumberStyles.HexNumber) / 255f);
        string secondaryColorHTML = eventInfo["color"]["secondary"];
        Color secondaryColor = new Color(int.Parse(secondaryColorHTML.Substring(1, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
                                         int.Parse(secondaryColorHTML.Substring(3, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
                                         int.Parse(secondaryColorHTML.Substring(5, 2), System.Globalization.NumberStyles.HexNumber) / 255f);

        PlayerColor color = new PlayerColor(mainColor, secondaryColor);

        OnPlayerJoined.Invoke(eventInfo["name"], color);
    }


    public async void SendMessage(JsonObject message)
    {
        Debug.Log($"sending " + message.ToJsonPrettyPrintString());
        ArraySegment<byte> b = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message.ToJsonString()));
        await socket.SendAsync(b, WebSocketMessageType.Text, true, CancellationToken.None);
    }


    // private void DrawingPointAdded(string playerName, JsonNode pointInfo)
    // {
    //     // List<List<DrawingPoint>> drawingPoints = new List<List<DrawingPoint>>();

    //     // for (int i = 0; i < drawingData.Count; i++)
    //     // {
    //     //     drawingPoints.Add(new List<DrawingPoint>());
    //     //     JsonArray points = drawingData[i] as JsonArray;

    //     //     for (int j = 0; j < points.Count; j++)
    //     //     {
    //     //         drawingPoints[i].Add(new DrawingPoint(points[j]["x"], points[j]["y"], points[j]["dragging"]));
    //     //     }
    //     // }

    //     // this.gameManager.PlayerSubmittedDrawing(playerName, drawingPoints);

    //     DrawingPoint point = new DrawingPoint(pointInfo["x"], pointInfo["y"], pointInfo["dragging"]);
    //     this.gameManager.PlayerAddedDrawingPoint(playerName, point);
    // }

}


public class PlayerJoinedEvent : UnityEvent<string, PlayerColor> { }
public class PlayerLeftEvent : UnityEvent<string> { }
public class MessageEvent :UnityEvent<JsonNode> { }