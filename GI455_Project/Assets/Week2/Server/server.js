const WebSocket = require('ws');
const sqlite = require('sqlite3').verbose();

const wss = new WebSocket.Server({ port: 25500 });

var roomList = [];
var wsList = [];
var loginList = ["ServerUsername_III1III5III2III4III"];

var tempUsername;

console.log("Azterland is running");

wss.on('connection', function connection(ws) {

console.log("client connected.");

  ws.on('message', function incoming(data) {

    var toJsonObj = JSON.parse(data);
    
    if(toJsonObj.eventName == "Login")
    {
      var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE |sqlite.OPEN_READWRITE, (err) =>{

        if(err) throw err;
    
        var userID = toJsonObj.eventData;
        var password = toJsonObj.password;
        tempUsername = toJsonObj.eventData;

        var userAlreadyLogin;
    
        database.all("SELECT UserID,Password,Answer FROM UserData",(err, rows)=>{
    
            if(err)
            {
              console.log(err);
            }
            else
            {
              

                for(var i = 0; i < loginList.length ; i++)
                {
                  if(userID == loginList[i] )
                  {
                    userAlreadyLogin = true;
                    break;
                  }
                  else
                  {
                    userAlreadyLogin = false;
                  }
                }

                for(var j = 0 ; j < rows.length ; j++)
                {
                  if(userID == rows[j].UserID)
                  {
                    if(password == rows[j].Password)
                    {
                      if(userAlreadyLogin == false)
                      {
                        loginList.push(tempUsername);
                        console.log("Login Success");
                        var callbackMsg = 
                        {
                          eventName:"Login",
                          eventData:"success"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                      }
                      else
                      {
                        console.log("This Username Already Login");
                        var callbackMsg = 
                        {
                          eventName:"Login",
                          eventData:"thisUserAlreadyLogin"
                        }
                      var toJsonStr = JSON.stringify(callbackMsg);
                      ws.send(toJsonStr);
                      }
                    }  
                    else
                    {
                    var callbackMsg = 
                      {
                        eventName:"Login",
                        eventData:"wrongPassword"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    }
                    break;
                  }
                  else
                  {
                    var callbackMsg = 
                      {
                        eventName:"Login",
                        eventData:"userNotFound"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                  }
                }
            }
          });
    });
    }
    else if(toJsonObj.eventName == "LogOut")
    {
      var userID = toJsonObj.eventData;

      for(var i ;i <loginList.length;i++)
      {
        if(userID == loginList[i])
        {
          console.log(tempUsername + "LogOut!");
          loginList.splice(i, 1);
          console.log(loginList);
        }
      }
    }
    else if(toJsonObj.eventName == "Register")
    {
      var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE |sqlite.OPEN_READWRITE, (err) =>{

        if(err) throw err;
    
        var userID = toJsonObj.eventData;
        var password = toJsonObj.password;
        var answer = toJsonObj.answer;
    
        database.all("SELECT UserID FROM UserData",(err, rows)=>{
    
            if(err)
            {
              console.log(err);
            }
            else{
                for(var i = 0;i<rows.length;i++)
                {
                  if(userID == rows[i].UserID)
                  {
                    var callbackMsg = 
                      {
                        eventName:"Register",
                        eventData:"usernameAlreadyUse"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                  }
                  else
                  {
                    database.all(`INSERT INTO UserData (UserID ,Password ,Answer) VALUES ('${userID}','${password}','${answer}')`,(err, rows)=>{

                      if(err)
                      {
                        console.log(err);
                      }
                    });

                    var callbackMsg = 
                        {
                          eventName:"Register",
                          eventData:"success"
                        }
                      var toJsonStr = JSON.stringify(callbackMsg);
                      ws.send(toJsonStr);
                  }
                    break;
                  }
                  
                
            }
    
          });
    
    });
    }
    else if(toJsonObj.eventName == "RePassword")
    {
      var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE |sqlite.OPEN_READWRITE, (err) =>{

        if(err) throw err;

        var userID = toJsonObj.eventData;
        var password = toJsonObj.password;
        var answer = toJsonObj.answer;
    
        database.all("SELECT UserID,Password,Answer FROM UserData",(err, rows)=>{
    
            if(err)
            {
              console.log(err);
            }
            else{
                for(var i = 0;i<rows.length;i++)
                {
                  if(userID == rows[i].UserID)
                  {
                    if(answer == rows[i].Answer)
                    {
                      database.all("UPDATE UserData SET Password = '"+password+"' WHERE UserID = '"+userID+"'"),(err, rows)=>
                      {
                      }

                    console.log("RePassword Success");
                    var callbackMsg = 
                      {
                        eventName:"RePassword",
                        eventData:"success"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    }
                    else
                    {
                    var callbackMsg = 
                      {
                        eventName:"RePassword",
                        eventData:"wrongAnswer"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    }
                    break;
                  }
                  else
                  {
                    var callbackMsg = 
                      {
                        eventName:"RePassword",
                        eventData:"userNotFound"
                      }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                  }
                   
                }
                }
                
            }
    
        );
    
    });
    }
    else if(toJsonObj.eventName == "CreateRoom")
    {

      var isFoundRoom = false;

      for(var i = 0;i <roomList.length;i++)
      {
        if(roomList[i].roomName == toJsonObj.eventData)
        {
          isFoundRoom = true
          break;
        }
      }

      if(isFoundRoom == true)
      {
        var callbackMsg = 
        {
          eventName:"CreateRoom",
          eventData:"fail"
        }
        var toJsonStr = JSON.stringify(callbackMsg);
        ws.send(toJsonStr);
        console.log("client create room : fail");
      }
      else
      {
        var newRoom = 
        {
          roomName: toJsonObj.eventData,
          wsList: []
        }
  
        var callbackMsg = 
        {
          eventName:"CreateRoom",
          eventData:"success"
        }
        var toJsonStr = JSON.stringify(callbackMsg);
        ws.send(toJsonStr);

        newRoom.wsList.push(ws);//เข้าห้อง

        roomList.push(newRoom);
  
        console.log("client create room : success");
      }

      

    }
    else if(toJsonObj.eventName == "JoinRoom")
    {

      var isFoundRoom = false;
      var isInRoom = false;

      for(var i = 0; i < roomList.length; i++)
      {
          for(var j = 0; j < roomList[i].wsList.length; j++)
          {
              if(ws == roomList[i].wsList[j])
              {
                isInRoom = true;
              }
              else
              {
                isInRoom = false;
              }
          }
      }

      for(var i = 0;i <roomList.length;i++)
      {
        if(roomList[i].roomName == toJsonObj.eventData)
        {
          isFoundRoom = true
          if(isFoundRoom == true && isInRoom == false)
      {
        var callbackMsg = 
        {
          eventName:"JoinRoom",
          eventData:"success"
        }
        var toJsonStr = JSON.stringify(callbackMsg);
        ws.send(toJsonStr);

        roomList[i].wsList.push(ws);
        isInRoom == true;
        console.log("client join room : success");
      }
      else
      {
        var callbackMsg = 
        {
          eventName:"JoinRoom",
          eventData:"fail"
        }
        var toJsonStr = JSON.stringify(callbackMsg);
        ws.send(toJsonStr);
        console.log("client join room : fail");
      }
          break;
        }
      }

    }
    else if(toJsonObj.eventName == "Chat")
    {


          for(var i = 0; i < roomList.length; i++)
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)
                {
                    if(ws == roomList[i].wsList[j])
                    {
                      for(var k = 0; k < roomList[i].wsList.length; k++)
                      {
                        if(ws !== roomList[i].wsList[k])
                        {
                          roomList[i].wsList[k].send(toJsonObj.eventData);
                        }
                       
                      }
                    }
                }
            }
               
    }
    else if(toJsonObj.eventName == "LeaveRoom")
        {
            
            var isLeaveSuccess = false;
            for(var i = 0; i < roomList.length; i++)
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)
                {
                    if(ws == roomList[i].wsList[j])
                    {
                        roomList[i].wsList.splice(j, 1);

                        if(roomList[i].wsList.length <= 0)
                        {
                            roomList.splice(i, 1);
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            

            if(isLeaveSuccess)
            {
               
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    eventData:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);

                console.log("leave room success");
            }
            else
            {
               
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    eventData:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);

                console.log("leave room fail");
            }

    }

    
  });

ws.on("close",()=>
	{
    for(var i = 0; i < roomList.length; i++)
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)
            {
                if(ws == roomList[i].wsList[j])
                {
                    roomList[i].wsList.splice(j, 1);

                    if(roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i, 1);
                    }
                    break;
                }
            }
        }
    
        for(var i ;i <loginList.length;i++)
        {
          if(tempUsername == loginList[i])
          {
            console.log(tempUsername + "LogOut!");
            loginList.splice(i, 1);
            console.log(loginList);
          }
        }

        console.log("client disconnected.");

	});

});
