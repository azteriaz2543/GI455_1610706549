using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public struct SocketEvent
{
    public string eventName;
    public string eventData;
    public string password;
    public string answer;
    public SocketEvent(string eventName, string eventData ,string password ,string answer)
    {
        this.eventName = eventName;
        this.eventData = eventData;
        this.password = password;
        this.answer = answer;
    }
}

public class ProgramChatManager : MonoBehaviour
{

    WebSocket websocket;

    bool isConnect = false, inLobby = false, inCreateRoom = false, inJoinRoom = false, inChatRoom = false;
    bool canCreateRoom = false, canJoinRoom = false;
    string myName, roomName;

    [SerializeField] GameObject ConnectPanel, ChatPanel, LobbyPanel, CreateRoomPanel, JoinRoomPanel, CantCreatNotify, CantJoinNotify; //Notification
    [SerializeField] InputField IP_Field, Text_Field, NewRoomName_Field, RoomName_Field;
    [SerializeField] Text ChatText, AnoterText, LobbyUserName, CreateRoomUserName, JoinRoomUserName, HereIs;
    [SerializeField] string[] connectText, disconnectText;

    string sendEventName, sendEventData;

    [SerializeField] GameObject LoginPanel, RegisterPanel, RePasswordPanel, LoginError, RegisterError, RePasswordError;
    [SerializeField] InputField Login_Username, Login_Password, Register_Username, Register_Password, Register_Answer, RePassword_Username, RePassword_NewPassword, RePassword_Answer;
    [SerializeField] Text LoginErrorText, RegisterErrorText, RePasswordErrorText;

    bool inLogin = false, inRegister =false, inRePassword = false;
    string loginStatus;

    [SerializeField] Text [] NowIP;

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

        CantCreatNotify.SetActive(false);
        CantJoinNotify.SetActive(false);

        LoginError.SetActive(false);
        RegisterError.SetActive(false);
        RePasswordError.SetActive(false);

        myName = "";
    }

    void Update()
    {
        ConnectPanel.SetActive(!isConnect);
        LobbyPanel.SetActive(inLobby);
        CreateRoomPanel.SetActive(inCreateRoom);
        JoinRoomPanel.SetActive(inJoinRoom);
        ChatPanel.SetActive(inChatRoom);

        if (inChatRoom == true)
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

        LobbyUserName.text = "Hi ! \n{ " + myName + " }";
        CreateRoomUserName.text = myName;
        JoinRoomUserName.text = myName;

        HereIs.text = "Here is... " + roomName + " !!!";

        LoginPanel.SetActive(inLogin);
        RegisterPanel.SetActive(inRegister);
        RePasswordPanel.SetActive(inRePassword);

    }

    public void OnDestroy()
    {
        if (websocket != null)
        {
            sendEventName = "Chat";
            sendEventData = "--- " + myName + disconnectText[Random.Range(0, 5)];
            SendSocketEvent(sendEventName, sendEventData);

            sendEventName = "LeaveRoom";
            sendEventData = "";
            SendSocketEvent(sendEventName, sendEventData);

            LogOut();

            websocket.Close();
        }
    }

    public void ConnectBotton()
    {

        if (IP_Field.text == "")
        {
            websocket = new WebSocket("ws://127.0.0.1:25500/");
        }
        else
        {
            websocket = new WebSocket("ws://" + IP_Field.text + ":25500/");
            for (var i = 0; i < NowIP.Length; i++)
            {
                NowIP[i].text = IP_Field.text;
            }
        }

        websocket.OnMessage += OnMessage;

        websocket.Connect();

        isConnect = true;
        inLogin = true;
    }

    public void SentBotton()
    {
        sendEventName = "Chat";
        sendEventData = myName + " : " + Text_Field.text;
        SendSocketEvent(sendEventName, sendEventData);

        ChatText.text += "\n" + Text_Field.text;
        AnoterText.text += "\n ";

        Text_Field.text = "";
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        if (inChatRoom == true)
        {
            AnoterText.text += "\n" + messageEventArgs.Data;
            ChatText.text += "\n ";
        }
        else
        {
            SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(messageEventArgs.Data);

            Debug.Log(receiveMessageData.eventName);
            Debug.Log(receiveMessageData.eventData);

            if (receiveMessageData.eventName == "CreateRoom")
            {

                if (receiveMessageData.eventData == "success")
                {
                    canCreateRoom = true;
                }
                else if (receiveMessageData.eventData == "fail")
                {
                    canCreateRoom = false;
                }

            }
            else if (receiveMessageData.eventName == "JoinRoom")
            {
                if (receiveMessageData.eventData == "success")
                {
                    canJoinRoom = true;
                }
                else if (receiveMessageData.eventData == "fail")
                {
                    canJoinRoom = false;
                }
            }
            else if (receiveMessageData.eventName == "Login")
            {
                if (receiveMessageData.eventData == "success")
                {
                    loginStatus = "Login_success";
                }
                else if (receiveMessageData.eventData == "userNotFound")
                {
                    loginStatus = "Login_userNotFound";
                }
                else if (receiveMessageData.eventData == "wrongPassword")
                {
                    loginStatus = "Login_wrongPassword";
                }
                else if (receiveMessageData.eventData == "thisUserAlreadyLogin")
                {
                    loginStatus = "Login_thisUserAlreadyLogin";
                }
            }
            else if (receiveMessageData.eventName == "Register")
            {
                if (receiveMessageData.eventData == "success")
                {
                    loginStatus = "Register_success";
                }
                else if (receiveMessageData.eventData == "usernameAlreadyUse")
                {
                    loginStatus = "Register_usernameAlreadyUse";
                }
            }
            else if (receiveMessageData.eventName == "RePassword")
            {
                if (receiveMessageData.eventData == "success")
                {
                    loginStatus = "RePassword_success";
                }
                else if (receiveMessageData.eventData == "userNotFound")
                {
                    loginStatus = "RePassword_userNotFound";
                }
                else if (receiveMessageData.eventData == "wrongAnswer")
                {
                    loginStatus = "RePassword_wrongAnswer";
                }
            }

            
        }
    }

    public void DisconnectBotton()
    {
        if (websocket != null)
        {
            sendEventName = "Chat";
            sendEventData = "--- " + myName + disconnectText[Random.Range(0, 5)];
            SendSocketEvent(sendEventName, sendEventData);

            sendEventName = "LeaveRoom";
            sendEventData = "";
            SendSocketEvent(sendEventName, sendEventData);

            LogOut();

            websocket.Close();
        }

        SceneManager.LoadScene("Week2");

    }

    void SendSocketEvent(string sendingEventName, string sendingEventData)
    {
        SocketEvent socketEvent = new SocketEvent(sendingEventName, sendingEventData, "","");

        string toJsonStr = JsonUtility.ToJson(socketEvent);

        websocket.Send(toJsonStr);
    }

    public void LobbyCreateRoomButton()
    {
        inLobby = false;
        inCreateRoom = true;
    }

    public void LobbyJoinRoomButton()
    {
        inLobby = false;
        inJoinRoom = true;
    }

    public void CreateRoomButton()
    {
        sendEventName = "CreateRoom";
        sendEventData = NewRoomName_Field.text;
        roomName = sendEventData;
        SendSocketEvent(sendEventName, sendEventData);

        StartCoroutine(LoadingCreateRoom());

    }

    public void JoinRoomButton()
    {
        sendEventName = "JoinRoom";
        sendEventData = RoomName_Field.text;
        roomName = sendEventData;
        SendSocketEvent(sendEventName, sendEventData);

        StartCoroutine(LoadingJoinRoom());

    }

    IEnumerator NotifcationFade(GameObject NotifyText)
    {
        NotifyText.SetActive(true);

        yield return new WaitForSeconds(5);

        NotifyText.SetActive(false);
    }

    IEnumerator LoadingCreateRoom()
    {

        yield return new WaitForSeconds(0.1f);

        if (canCreateRoom == true)
        {
            inCreateRoom = false;
            inChatRoom = true;

            sendEventName = "Chat";
            sendEventData = "--- " + myName + connectText[Random.Range(0, 5)];
            SendSocketEvent(sendEventName, sendEventData);

        }
        else
        {
            StartCoroutine(NotifcationFade(CantCreatNotify));

        }
    }

    IEnumerator LoadingJoinRoom()
    {

        yield return new WaitForSeconds(0.1f);

        if (canJoinRoom == true)
        {
            sendEventName = "Chat";
            sendEventData = "--- " + myName + connectText[Random.Range(0, 5)];
            SendSocketEvent(sendEventName, sendEventData);

            inJoinRoom = false;
            inChatRoom = true;
        }
        else
        {
            StartCoroutine(NotifcationFade(CantJoinNotify));
        }
    }

    void SendSocketLogin(string sendingEventName, string sendingEventData, string sendingPassword ,string sendingAnswer)
    {
        SocketEvent socketEvent = new SocketEvent(sendingEventName, sendingEventData, sendingPassword ,sendingAnswer);

        string toJsonStr = JsonUtility.ToJson(socketEvent);

        websocket.Send(toJsonStr);
    }

    public void Login_Login()
    {
        if (Login_Username.text != "")
        {
            if (Login_Password.text != "")
            {
                SendSocketLogin("Login",Login_Username.text,Login_Password.text,"");

                //loadingLogin
                StartCoroutine(LoadingLogin());
            }
            else
            {
                LoginErrorText.text = "Please enter your password...";
                StartCoroutine(NotifcationFade(LoginError));
            }
        }
        else
        {
            LoginErrorText.text = "Please enter your username...";
            StartCoroutine(NotifcationFade(LoginError));
        }
    }

    public void Login_Register()
    {
        Login_Username.text = "";
        Login_Password.text = "";
        inLogin = false;
        inRegister = true;
    }

    public void Login_RePassword()
    {
        Login_Username.text = "";
        Login_Password.text = "";
        inLogin = false;
        inRePassword = true;
    }

    public void Register_Confirm()
    {
        if (Register_Username.text == "" && Register_Password.text == "" && Register_Answer.text =="")
        {
            RegisterErrorText.text = "Please complete all information.";
            StartCoroutine(NotifcationFade(RegisterError));
        }
        else
        {
            SendSocketLogin("Register", Register_Username.text, Register_Password.text, Register_Answer.text);

            //loadingRegister
            StartCoroutine(LoadingRegister());
        }
    }

    public void Register_Back()
    {
        Register_Username.text = "";
        Register_Password.text = "";
        Register_Answer.text = "";
        inLogin = true;
        inRegister = false;
    }

    public void RePassword_Confirm()
    {
        if (RePassword_Username.text == "" && RePassword_NewPassword.text == "" && RePassword_Answer.text == "")
        {
            RePasswordErrorText.text = "Please complete all information.";
            StartCoroutine(NotifcationFade(RePasswordError));
        }
        else
        {
            SendSocketLogin("RePassword", RePassword_Username.text, RePassword_NewPassword.text, RePassword_Answer.text);

            //loadingRePassword
            StartCoroutine(LoadingRePassword());  
        }
    }

    public void RePassword_Back()
    {
        RePassword_Username.text = "";
        RePassword_NewPassword.text = "";
        RePassword_Answer.text = "";
        inLogin = true;
        inRePassword = false;
    }

    public void Lobby_Back()
    {
        LogOut();
        inLobby = false;
        inLogin = true;
    }

    public void CreateRoom_Back()
    {
        NewRoomName_Field.text = "";
        inLobby = true;
        inCreateRoom = false;
    }

    public void JoinRoom_Back()
    {
        RoomName_Field.text = "";
        inLobby = true;
        inJoinRoom = false;
    }

    IEnumerator LoadingLogin()
    {

        yield return new WaitForSeconds(0.1f);

        if (loginStatus == "Login_success")
        {
            myName = Login_Username.text;
            inLobby = true;
            inLogin = false;
        }
        else if(loginStatus == "Login_userNotFound")
        {
            Login_Username.text = "";
            Login_Password.text = "";

            LoginErrorText.text = "Username not found, Please Try Again or Register.";
            StartCoroutine(NotifcationFade(LoginError));
        }
        else if (loginStatus == "Login_wrongPassword")
        {
            Login_Password.text = "";

            LoginErrorText.text = "Wrong password, Please Try Again.";
            StartCoroutine(NotifcationFade(LoginError));
        }
        else if (loginStatus == "Login_thisUserAlreadyLogin")
        {
            Login_Password.text = "";

            LoginErrorText.text = "This Username Already Login...";
            StartCoroutine(NotifcationFade(LoginError));
        }
    }

    IEnumerator LoadingRegister()
    {

        yield return new WaitForSeconds(0.1f);

        if (loginStatus == "Register_success")
        {
            Register_Back();
        }
        else if (loginStatus == "Register_usernameAlreadyUse")
        {
            Register_Username.text = "";

            RegisterErrorText.text = "This username is already in use.";
            StartCoroutine(NotifcationFade(RegisterError));
        }
    }

    IEnumerator LoadingRePassword()
    {
        yield return new WaitForSeconds(0.1f);

        if (loginStatus == "RePassword_success")
        {
            RePassword_Back();
        }
        else if (loginStatus == "RePassword_userNotFound")
        {
            RePassword_Username.text = "";

            RePasswordErrorText.text = "Username not found, Please Try Again or Register.";
            StartCoroutine(NotifcationFade(RePasswordError));
        }
        else if (loginStatus == "RePassword_wrongAnswer")
        {
            RePassword_Answer.text = "";

            RePasswordErrorText.text = "Wrong answer, Please Try Again.";
            StartCoroutine(NotifcationFade(RePasswordError));
        }
    }

    public void ShowPassword(InputField target)
    {
        if (target.contentType == InputField.ContentType.Password)
        {
            target.contentType = InputField.ContentType.Standard;
        }
        else if (target.contentType == InputField.ContentType.Standard)
        {
            target.contentType = InputField.ContentType.Password;
        }
        
    }

    void LogOut()
    {
        if (myName != "")
        {
            SendSocketEvent("LogOut",myName);
        }
    }
}