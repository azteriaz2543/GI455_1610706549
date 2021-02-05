const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 25500 });

console.log("Azterland is running");

wss.on('connection', function connection(ws) {

console.log("client connected.");

  ws.on('message', function incoming(data) {

console.log("send from client : " + data);

    wss.clients.forEach(function each(client) {

      if (client !== ws && client.readyState === WebSocket.OPEN) {
        client.send(data);
      }
    }); //clients
  }); //message

ws.on("close",()=>
	{
		console.log("client disconnected.");
	});

}); //connect

function ArrayRemove(arr ,value)
{
	return arr.filter((element)=>{
		return element != value;
	})
}
