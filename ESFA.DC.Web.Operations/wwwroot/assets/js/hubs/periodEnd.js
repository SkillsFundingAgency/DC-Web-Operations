"use strict";

let connection = new signalR
    .HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
    .build();



connection.on("ReceiveMessage", renderPaths);

connection.start();