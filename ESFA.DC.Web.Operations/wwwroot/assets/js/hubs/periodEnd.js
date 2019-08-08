"use strict";

let connection = new signalR
.HubConnectionBuilder()
.configureLogging(signalR.LogLevel.Information)
.withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
.build();

connection.on("ReceiveMessage", renderPaths);

connection.start().then(() => {
    setTimeout(function() {
        connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
    }, 30*1000);
});