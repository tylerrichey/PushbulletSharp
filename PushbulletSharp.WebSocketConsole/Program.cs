﻿using PushbulletSharp.Filters;
using PushbulletSharp.Models.Responses.WebSocket;
using System;
using Newtonsoft.Json;
using WebSocketSharp;

namespace PushbulletSharp.WebSocketConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = "--YOURKEYGOESHERE--";
            var Client = new PushbulletClient(key, TimeZoneInfo.Local);
            var lastChecked = DateTime.Now;

            using (var ws = new WebSocket(string.Concat("wss://stream.pushbullet.com/websocket/", key)))
            {
                ws.OnMessage += (sender, e) =>
                {
                    var response = JsonConvert.DeserializeObject<WebSocketResponse>(e.Data);
                    switch (response.Type)
                    {
                        case "nop":
                            Console.WriteLine(string.Format("Updated {0}", DateTime.Now));
                            break;
                        case "tickle":
                            Console.WriteLine(string.Format("Tickle recieved on {0}. Go check it out.", DateTime.Now));
                            var filter = new PushResponseFilter()
                            {
                                Active = true,
                                ModifiedDate = lastChecked
                            };

                            var pushes = Client.GetPushes(filter);
                            foreach (var push in pushes.Pushes)
                            {
                                Console.WriteLine(push.Title);
                            }

                            lastChecked = DateTime.Now;
                            break;
                        case "push":
                            Console.WriteLine(string.Format("New push recieved on {0}.", DateTime.Now));
                            //Console.WriteLine("Push Type: {0}", response.Push.Type);
                            Console.WriteLine("Response SubType: {0}", response.Subtype);
                            break;
                        default:
                            Console.WriteLine("new type that is not supported");
                            break;
                    }
                };
                ws.Connect();
                Console.ReadKey(true);
            }
        }
    }
}