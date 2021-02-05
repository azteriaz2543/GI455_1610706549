using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class ProgramChatManager : MonoBehaviour
{
    WebSocket websocket;

    bool isConnect = false;
    string myName;

    [SerializeField] GameObject ConnectPanel, ChatPanel;
    [SerializeField] InputField Name_Field,IP_Field ,Text_Field;
    [SerializeField] Text ChatText ,AnoterText,UserName;

    [SerializeField] string[] connectText ,disconnectText;


    void Start()
    {
        connectText[0] = " is connected. ---";
        connectText[1] = " is coming. ---";
        connectText[2] = " is around you. ---";
        connectText[3] = " is behind you. ---";
        connectText[4] = " is join the Party ~ ---";

        disconnectText[0] = " is disconnected. ---";
        disconnectText[1] = " is escape the war. ---";
        disconnectText[2] = " escaped !!! ---";
        disconnectText[3] = " leapt out of the room. ---";
        disconnectText[4] = " fell out of the world... ---";

    }

    void Update()
    {
        if (isConnect == false)
        {
            ConnectPanel.SetActive(true);
            ChatPanel.SetActive(false);
        }
        else
        {
            ConnectPanel.SetActive(false);
            ChatPanel.SetActive(true);
        }

        if (isConnect == true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SentBotton();
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SentBotton();
            }

        }

    }
    public void OnDestroy()
    {
        if (websocket != null)
        {
            websocket.Send(myName + " disconnected.");
            websocket.Close();
        }
    }
    public void ConnectBotton()
    {

        if (Name_Field.text == "")
        {
            int randomUserNum = Random.Range(0, 10000);
            myName = "User_" + randomUserNum;
        }
        else
        {
            myName = Name_Field.text;
        }

        if (IP_Field.text == "")
        {
            websocket = new WebSocket("ws://127.0.0.1:25500/");
        }
        else
        {
            websocket = new WebSocket("ws://" + IP_Field.text + ":25500/");
        }

        websocket.OnMessage += OnMessage;

        websocket.Connect();
        websocket.Send("--- " + myName + connectText[Random.Range(0,5)]);

        isConnect = !isConnect;

        UserName.text = myName;
    }

    public void SentBotton()
    {
        websocket.Send(myName + " : " + Text_Field.text);

        ChatText.text += "\n" + Text_Field.text;
        AnoterText.text += "\n ";

        Text_Field.text = "";
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
            AnoterText.text += "\n" + messageEventArgs.Data;
            ChatText.text += "\n ";
    }

    //void MoreTextZone()
    //{
        
    //}

    public void DisconnectBotton()
    {
        if (websocket != null)
        {
            websocket.Send("--- " + myName + disconnectText[Random.Range(0, 5)]);
            websocket.Close();
        }

        SceneManager.LoadScene("Week2");

    }

}
