using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomReveived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            UserAccount userAccount = new UserAccount { ID = LoginPage.instance.userIDField.text, PW = LoginPage.instance.userPWField.text };

            _packet.Write(Client.instance.myId);
            _packet.Write(JsonUtility.ToJson(userAccount));

            SendTCPData(_packet);
        }
    }

    public static void SendGameData2()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            UserAccount userAccount = new UserAccount { ID = LoginPage.instance.userIDField.text, PW = LoginPage.instance.userPWField.text };

            _packet.Write(Client.instance.myId);
            _packet.Write(JsonUtility.ToJson(userAccount));

            SendTCPData(_packet);
        }
    }

    /*public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet.");
            SendUDPData(_packet);
        }
    }*/

    #endregion
}
