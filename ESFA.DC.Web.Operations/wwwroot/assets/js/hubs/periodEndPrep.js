"use strict";

let connection = new signalR
    .HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/periodEndPrepHub", { transport: signalR.HttpTransportType.WebSockets }) 
    .build();

connection.on("ReceiveMessage", doNothing);

connection.start();