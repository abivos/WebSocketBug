# WebSocketBug
Sample project to reproduce a bug in websocket-sharp

Hello, community and sta! After updating our Unity project from version 2019.3.5f1 to 2020.3.3f1 we started experiencing problems with this WebSocket Library on Android (and maybe on iOS).
We found a way to reproduce it on a simple project with a close-to real-world scenario.
We are sending a bit of data (around 30 messages 100 characters each) to the echo server wss://echo.websocket.org and pinging each second at the same time (if we don't ping a bug won't be reproduced).
In this scenario, it looks like a WebSocket is stuck in some hanging state. All messages were sent, but we got none of the responses.

This happens 99% of the time on Android with Unity 2020.3.3f1, but never on Windows, in Editor or Standalone.
websocket-sharp.dll in the sample project is built from the latest git master.

To test it in a sample project press the Start button, the square underneath the button will change its colour from red to yellow.
After a while in case of receiving all the messages square will change its colour from yellow to green.

Script: https://github.com/abivos/WebSocketBug/blob/main/Assets/DebugController.cs

Full repo: https://github.com/abivos/WebSocketBug
