using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestServer
{
    class ServerHandle
    {
        //a - b,  c - d
        public static void WelcomeReceived(int _fromclient, Packet _packet)
        {
            var _clientIdCheck = _packet.ReadInt();
            var _userAccount = JsonSerializer.Deserialize<UserAccount>(_packet.ReadString()).GetValueOrDefault();

            Console.WriteLine($"{Server.clients[_fromclient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromclient}");
            Console.WriteLine($"UserAccount Info {{ ID: {_userAccount.ID}, PW: {_userAccount.PW} }}");

            if (_fromclient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_userAccount.ID}\" (ID: {_fromclient}) has assumed the wrong client ID ({_clientIdCheck}) ");
            }

            // TODO: Checking DB and get userName

          //  var _userName = "TempName_" + _userAccount.ID;
          //  Server.clients[_fromclient].SendIntoGame(_userName);
        }

        /*public static void UDPTestReceived(int _fromclient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Received packet via UDP. Contains message {_msg}");
        }*/
    }

    
}
