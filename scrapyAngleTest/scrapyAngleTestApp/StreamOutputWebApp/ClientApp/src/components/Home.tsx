import * as React from "react";
import { connect } from "react-redux";

const Home = () => (
  <div>
    <h1>Hello, world!</h1>
    <p>Welcome to your new single-page application, built with:</p>
    <ul>
      <li>
        <a href="https://get.asp.net/">ASP.NET Core</a> and{" "}
        <a href="https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx">C#</a> for cross-platform server-side code
      </li>
      <li>
        <a href="https://facebook.github.io/react/">React</a> and <a href="https://redux.js.org/">Redux</a> for
        client-side code
      </li>
      <li>
        <a href="http://getbootstrap.com/">Bootstrap</a> for layout and styling
      </li>
    </ul>
    <p>To help you get started, we've also set up:</p>
    <ul>
      <li>
        <strong>Client-side navigation</strong>. For example, click <em>Counter</em> then <em>Back</em> to return here.
      </li>
      <li>
        <strong>Development server integration</strong>. In development mode, the development server from{" "}
        <code>create-react-app</code> runs in the background automatically, so your client-side resources are
        dynamically built on demand and the page refreshes when you modify any file.
      </li>
      <li>
        <strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and
        your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.
      </li>
    </ul>
    <p>
      The <code>ClientApp</code> subdirectory is a standard React application based on the <code>create-react-app</code>{" "}
      template. If you open a command prompt in that directory, you can run <code>npm</code> commands such as{" "}
      <code>npm test</code> or <code>npm install</code>.
    </p>
  </div>
);

export default connect()(Home);

/* client side hub example: TODO: call it here in home.tsx or in index.tsx
    <script>
        (async () => {
            const latestSensorData = { x: 0, y: 0, z: 0 };

            const connection = new signalR.HubConnectionBuilder()
                .withAutomaticReconnect()// add if you want auto reconnect clients to server
                .withUrl("/sensors")
                .configureLogging(signalR.LogLevel.Information)
                .build();

            //get sensor data when we reconnect and reconnect each sensor
            connection.onreconnected(async function () {
                const sensorNames = await connection.invoke("GetSensorNames");
                sensorNames.forEach(subscribeToSensor);
            });

            function subscribeToSensor(sensorName) {
                //this returns like RxJs observable ---we can pull library to have more extension methods on top( filtering..)
                //When you need to stream from server--> client you call .stream()
                connection.stream("GetSensorData", sensorName)
                    .subscribe({
                        next: (item) => {
                            latestSensorData[sensorName] = item;
                        },
                        complete: () => {
                            console.log(`${sensorName} Completed`);
                        },
                        error: (err) => {
                            console.log(`${sensorName} error: "${err}"`);
                        },
                    });
            }

            await connection.start();

            //what are all sensors connected to our app --> string[]
            const sensorNames = await connection.invoke("GetSensorNames");

            //subscribe - foreach sensor
            sensorNames.forEach(subscribeToSensor);
            connection.on("SensorAdded", subscribeToSensor);

            startRealTimeLineChart(latestSensorData);
        })();
    </script>
    */
