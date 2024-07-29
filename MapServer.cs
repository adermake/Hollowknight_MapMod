using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Reflection;
using UnityEngine.Networking.PlayerConnection;
using WebSocketSharp.Server;
using UnityEngine;
using WebSocketSharp;


namespace Randomizer_Map
{
    public class MapServer
    {
        private HttpServer server;
        public int port = 7900;
        private WebSocketSessionManager sessions;

        internal class WSHandler : WebSocketBehavior
        {


            protected override void OnOpen()
            {
                
                    Send("test");
            }
        }

        public MapServer()
        {

        }

        public MapServer(ILogger logger)
        {

        }

        public void Start()
        {
            server = new HttpServer(port);
            server.OnGet += OnGet;

            server.AddWebSocketService<WSHandler>("/ws");


            server.Start();
            sessions = server.WebSocketServices["/ws"].Sessions;
            Randomizer_Map.MOD.Log("server start");
        }

        public void Send(string msg)
        {
            sessions.Broadcast(msg);
        }

        public void Send(JObject msg)
        {
            sessions.Broadcast(msg.ToString());
        }

        public void Send(string type, params object[] kvDataPairs)
        {
            var msg = new JObject();
            msg["type"] = type;

            for (int i = 0; i < kvDataPairs.Length; i += 2)
            {
                msg[(string)kvDataPairs[i]] = JToken.FromObject(kvDataPairs[i + 1]);
            }

            sessions.Broadcast(msg.ToString());
        }

        private void OnGet(object sender, HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;

            var path = req.Url.AbsolutePath;
            if (path == "/") path = "/index.html";

            var resourceName = "Web\\" + path.Substring(1);
            var resourceInfo = Assembly.GetExecutingAssembly().GetManifestResourceInfo(resourceName);

            if (resourceInfo == null)
            {
                res.StatusCode = 404;
                var text = Encoding.UTF8.GetBytes("Not found.");
                res.ContentType = "text/plain";
                res.ContentLength64 = text.Length;
                res.Close(text, true);
                return;
            }

            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                var len = resourceStream.Length;

                if (path.EndsWith(".js")) res.ContentType = "application/javascript";
                else if (path.EndsWith(".html")) res.ContentType = "text/html";
                else if (path.EndsWith(".css")) res.ContentType = "text/css";
                else res.ContentType = "text/plain";

                res.ContentEncoding = Encoding.UTF8;
                res.ContentLength64 = len;

                var buffer = new byte[len];
                int readCount, pos = 0;
                while ((readCount = resourceStream.Read(buffer, pos, buffer.Length - pos)) > 0)
                {
                    pos += readCount;
                }

                res.Close(buffer, true);
            }
        }

        public void Stop()
        {
            if (server == null) return;
            Randomizer_Map.MOD.Log("Stopping web server");
            foreach (var sessionId in sessions.IDs.ToArray())
            {
                sessions.CloseSession(sessionId, CloseStatusCode.Away, "GameShutdown");
            }
            server.Stop();
            server = null;
        }
    }
}
