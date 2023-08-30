using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using TcpServer.API;


namespace DiscordBot
{
    class Program
    {
        private static TcpListener _listener;
        public static TcpClient _client;

        public static void Main()
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", "{\n" +
                    "   \"token\": \"MTAxMDY3MDIwNjcyMjUwNjg0Mw.GiFLey.yRRF7TRpEwKh3_awPiTBwRA7GQcQfwj3yVGEEM\",\n" +
                    "   \"prefix\":  \"!\"\n" +
                    "}");
            }
            

            Thread thr = new Thread(StartDiscordBot);
            thr.Start();

            Console.Title = "TCPSever";
            _listener = new TcpListener(IPAddress.Any, 420);
            _listener.Start();

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                _client = client;

                NetworkStream stream = client.GetStream();

                Task.Run(() =>
                {
                    while (true)
                    {

                        if (stream.DataAvailable)
                        {
                            List<APIRequest> reqs = ReadToEnd(stream);
                            ulong channel;
                            foreach (var req in reqs)
                            {
                                switch (req.opcode)
                                {
                                    case Opcode.GetServerVersion:
                                        Write(stream, new APIRequest() { opcode = Opcode.GetServerVersion, Data = "server version is v1.0.1" });
                                        Console.WriteLine("Sent server version to client");
                                        break;
                                    case Opcode.GetGems:
                                        string dats = req.Data;
                                        Bot.SendGemAmountEmbed(dats, req.Channel);
                                        break;
                                    case Opcode.LockWorldData:
                                        string ok = req.Data;
                                        Console.WriteLine("Receive lock worlds data packet.");

                                        channel = req.Channel;
                                        Utils.LockWorldDataDiscordMessage(ok, channel);
                                        break;
                                    case Opcode.itemIDs:
                                        string itemIDs = req.Data;
                                        channel = req.Channel;
                                        Utils.Dumpids(itemIDs, channel);
                                        break;
                                    case Opcode.World:
                                        channel = req.Channel;
                                        Utils.WorldInfo(req.Data, channel);
                                        break;
                                    case Opcode.Gems:
                                        channel = req.Channel;
                                        Utils.GemInfo(req.Data, channel);
                                        break;



                                }
                            }



                        }
                        else
                            Thread.Sleep(1);




                    }
                });
            }
        }
        public static void StartDiscordBot()
        {
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }

        private static List<APIRequest> ReadToEnd(NetworkStream stream)
        {
            List<byte> receivedBytes = new List<byte>();
            while (stream.DataAvailable)
            {
                byte[] buffer = new byte[1024];

                stream.Read(buffer, 0, buffer.Length);
                receivedBytes.AddRange(buffer);
            }

            receivedBytes.RemoveAll(b => b == 0);

            string[] payloads = Encoding.UTF8.GetString(receivedBytes.ToArray()).Split('`');

            List<APIRequest> requests = new List<APIRequest>();
            foreach (var payload in payloads)
            {
                try
                {
                    requests.Add(JsonConvert.DeserializeObject<APIRequest>(payload));
                }
                catch
                { }
            }
            requests.RemoveAll(r => r == null);
            return requests;
        }

        public static void Write(NetworkStream stream, APIRequest request)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request) + "`");
            stream.Write(data, 0, data.Length);
        }

    }
}
