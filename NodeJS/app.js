const websocket = require("ws");
const app = require('express')();
const server = require();


const wss = new websocket.Server({server});

var wsList = [];
var roomList = [];

wss.on("connection" ,(ws)=>{
{

    console.log("client connected.");

    ws.on("message" ,(data)=>{

        var toJsonObj = JSON.parse(data);

        if(toJsonObj.eventName == "CreateRoom")
        {

            var isFoundRoom = false;

            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.roomName)
                {
                    isFoundRoom = true;
                    break;
                }
            }

            if(isFoundRoom == true)
            {

            }
            else
            {
                var newRoom = {
                    roomName: toJsonObj.roomName,
                    wsList : []
                }

                newRoom.wsList.push(ws);

                roomList.push(newRoom);
            }

            
        }
        else if (spli)
        {
n
        }

    });

}});