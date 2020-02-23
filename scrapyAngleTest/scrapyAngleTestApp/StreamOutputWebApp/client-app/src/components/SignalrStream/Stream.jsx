import React, { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from "@aspnet/signalr";

 //https://codeburst.io/4-four-ways-to-style-react-components-ac6f323da822
 //https://medium.com/@pioul/modular-css-with-react-61638ae9ea3e#.re1pdcz87
 //See for localized css !!

const SignalRStream =() => {
  //   const [hubConnection, setHubConnection] = useState<HubConnection>({} as HubConnection); note needed atm

   //TODO : uncomment and figure out how to stream  msgs from server to here
  useEffect(() => {
    // const latestSensorData = { x: 0, y: 0, z: 0 };  ---->example from signalR30SensorsDemo (replace with my hubs stream data)

    //Set initial hub connection
    const createHubConnection = async () => {
      //Config
      const connection = new HubConnectionBuilder()
        .withUrl("/outputstream") //server- hub endpoint
        .configureLogging(LogLevel.Debug)
        .build();
      //   .withHubProtocol(new MessagePackHubProtocol()) adds new binary protocol
      try {
        //start conn
        await connection.start();
        console.log("Connected to hub!");
        connection.stream("serverEndpoint_Placeholder");
        // .subscribe({
        //     next: (item) => {
        //         latestSensorData[sensorName] = item;  ---->example from signalR30SensorsDemo (replace with my hubs stream data)
        //     },
        //     complete: () => {
        //         console.log(`${sensorName} Completed`);
        //     },
        //     error: (err) => {
        //         console.log(`${sensorName} error: "${err}"`);
        //     },

        // HubConnectionState.Connected;
      } catch (error) {
        alert(error);
      }
      //   setHubConnection(connection);
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
