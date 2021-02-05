var websocket = require("ws"); //using

var websocketServer = new websocket.Server({port:25500},()=>
{
	console.log("Azterland is running");
});

var wsList = [];

websocketServer.on("connection",(ws ,rq)=>
{

	console.log("client connected.");

	wsList.push(ws);

	ws.on("message" ,(data)=>
	{
		console.log("send from client : " + data);
		Boardcast(data);
	});

	ws.on("close",()=>
	{

		wsList = ArrayRemove(wsList ,ws);

		console.log("client disconnected.");
	});
});

function ArrayRemove(arr ,value)
{
	return arr.filter((element)=>{
		return element != value;
	})
}

function Boardcast(data)
{
	for(var i = 0;i <wsList.length; i++)
	{
		wsList[i].send(data);
	}
}