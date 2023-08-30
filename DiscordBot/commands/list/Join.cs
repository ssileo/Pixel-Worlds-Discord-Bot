using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer;
using TcpServer.API;
using DiscordBot;

namespace TcpServer.commands.list
{
    class Join : SimpleCommand
    {
        public override void execute(string cmd, string[] args)
        {
            
            if (args.Length > 3 || args.Length < 2)
            {
                return;
            }
            if (args.Length == 2)
            {
                Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.JoinWorld, Data = $"{args[1]}|" });
            }
            else
            {
                Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.JoinWorld, Data = $"{args[1]}|{args[2]}" });
            }
            

        }
    }
}
