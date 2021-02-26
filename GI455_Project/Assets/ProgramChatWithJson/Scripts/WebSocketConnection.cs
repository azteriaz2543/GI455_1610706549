using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        class MessageData
        {
            public string username;
            public string message;
        }

        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField inputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:25500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMessenger.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData messagedata = new MessageData();
            messagedata.username = inputUsername.text;
            messagedata.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(messagedata);

            ws.Send(toJsonStr);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            if ( tempMessageString != null && tempMessageString != "")
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);
                //receiveText.text += tempMessageString + "\n";

                if (receiveMessageData.username == inputUsername.text)
                {
                    sendText.text += receiveMessageData.message + "\n ";
                    receiveText.text += "\n ";
                }
                else
                {
                    receiveText.text += receiveMessageData.message + "\n ";
                    sendText.text += "\n ";
                }

                tempMessageString = "";
            }

        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
            Debug.Log(messageEventArgs.Data);
        }
    }
}


