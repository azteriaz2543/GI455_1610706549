using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class WebsockConnection : MonoBehaviour
{
    WebSocket websocket;

    // Start is called before the first frame update
    void Start()
    {
        websocket = new WebSocket("ws://127.0.0.1:25500/"); //127.0.0.1 คือ IP ที่ใช้ในเครื่องตัวเอง

        websocket.OnMessage += OnMessage;

        websocket.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            websocket.Send("Number : " + Random.Range(0, 99999));
        }
    }

    public void OnDestroy()
    {
        if (websocket != null)
        {
            websocket.Close();
        }
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        Debug.Log("Message from server : " + messageEventArgs.Data);
    }
}

