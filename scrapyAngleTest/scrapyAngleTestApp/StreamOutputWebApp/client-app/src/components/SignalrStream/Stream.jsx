import React, { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from "@aspnet/signalr";
//import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack'; add npm if i want to switch protocols

//https://codeburst.io/4-four-ways-to-style-react-components-ac6f323da822
//https://medium.com/@pioul/modular-css-with-react-61638ae9ea3e#.re1pdcz87
//See for localized css !!

const SignalRStream = () => {
    //   const [hubConnection, setHubConnection] = useState();

    //NOTE : Stream will disconnect after 20 sec if it doesnt get msg from server
    /* Retry logic ...something like this
     * $.connection.hub.disconnected(function() {
   setTimeout(function() {
       $.connection.hub.start();
   }, 5000); // Restart connection after 5 seconds.
     *
     */
    //TODO : this verison works without errors , onyly need to output data to dictionary , somehow ..since "ListStreams" returns []
    useEffect(() => {
        //--->example from signalR30SensorsDemo (replace with my hubs stream data)
        // const latestSensorData = { x: 0, y: 0, z: 0 };  -

        //Set initial hub connection
        const createHubConnection = async () => {
            //Config
            const connection = new HubConnectionBuilder()
                .withUrl("/outputstream") //server- hub endpoint
                .configureLogging(LogLevel.Debug)
                .build();
            //   .withHubProtocol(new MessagePackHubProtocol()) adds new binary protocol

            ////reconnect  ---Last ,after stream is done
            //connection.onreconnected(async function () {
            //    const connectedClients = await connection.invoke("ListStreams");
            //    connectedClients.forEach(subscribeToStream);
            //});

            //const connectedClients = await connection.invoke("ListStreams");
            //console.log(connectedClients);
            //subscribe - foreach client
            //connectedClients.forEach(subscribeToStream);
            //connectedClients.on("SenStreamCreated", subscribeToStream);

            try {
                //start conn
                await connection.start();
                console.log("Connected to hub!");
                connection.stream("WatchStream"); //subscribe is method
                //subscribeToStream();
                const connectedClients = await connection.invoke("ListStreams");
                console.log(connectedClients);
                console.log("Connection info..")
                console.log(connection);

                // HubConnectionState.Connected;
            } catch (error) {
                alert(error);
            }
            //   setHubConnection(connection);

            function subscribeToStream() {
                console.log("Connected to hub!");
                connection.stream("WatchStream"); //subscribe is method
                // .subscribe({ Todoooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                //     next: (item) => {
                //         latestSensorData[sensorName] = item;  ---->example from signalR30SensorsDemo (replace with my hubs stream data)
                //     },
                //     complete: () => {
                //         console.log(`${sensorName} Completed`);
                //     },
                //     error: (err) => {
                //         console.log(`${sensorName} error: "${err}"`);
                //     },
            }

            //not logging anything ATM
            //connection.on("StreamCreated", stream => {
            //    console.log("Stream created");
            //    console.log(stream);
            //});

            //connection.on("StreamRemoved", stream => {
            //    console.log("Stream removed...reason (20 sec without server msg)");
            //    console.log(stream);
            //});
        };
        createHubConnection();
    }, []);
    // [] -->specifies that we will be calling this effect only when the component first mounts.
    return (
        <>
            <div className="inc-exp-container">
                <h1>SignalR Stream component-placeholder</h1>
            </div>
        </>
    );
}

export default SignalRStream;