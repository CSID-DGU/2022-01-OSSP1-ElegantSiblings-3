using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Net;
using System.Net.Sockets;
using FreeNet;


public class CGameMain : MonoBehaviour
{
    string input_text;
    List<string> received_texts;
    CNetworkManager network_manager;

    Vector2 currentScrollPos = new Vector2();

    private void Awake()
    {
        this.input_text = "";
        this.received_texts = new List<string>();
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();
    }

    public void on_receive_chat_msg(string text)
    {
        this.received_texts.Add(text);
        this.currentScrollPos.y = float.PositiveInfinity;
    }

    private void OnGUI()
    {
        // text ¼ö½Å
        GUILayout.BeginHorizontal();
        currentScrollPos = GUILayout.BeginScrollView(currentScrollPos,
            GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width),
            GUILayout.MaxHeight(Screen.height - 100), GUILayout.MinHeight(Screen.height - 100));

        foreach (string text in this.received_texts)
        {
            GUILayout.BeginHorizontal();
            GUI.skin.label.wordWrap = true;
            GUILayout.Label(text);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();


        // input
        GUILayout.BeginHorizontal();
        this.input_text = GUILayout.TextField(this.input_text,
            GUILayout.MaxWidth(Screen.width - 100), GUILayout.MinWidth(Screen.width - 100),
            GUILayout.MaxHeight(50), GUILayout.MinHeight(50));

        if (GUILayout.Button("Send",
            GUILayout.MaxWidth(100), GUILayout.MinWidth(100),
            GUILayout.MaxHeight(50), GUILayout.MinHeight(50)))
        {
            CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
            msg.push(this.input_text);
            this.network_manager.send(msg);

            this.input_text = "";
        }

        GUILayout.EndHorizontal();
    }
}
