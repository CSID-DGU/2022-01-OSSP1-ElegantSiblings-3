using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserAccount
{
    public string ID = "None";
    public string PW = "None";

    /* public UserAccount() { }
     public UserAccount(string _id, string _pw) { ID = _id; PW = _pw; }

     public string DataJsonSerializer() => JsonUtility.ToJson(this);
     public UserAccount DataJsonSerializer(string _SerializedData) => JsonUtility.FromJson<UserAccount>(_SerializedData);*/
}

public static class UserAccountExtensions
{
    public static string DataJsonSerializer(this UserAccount _userAccount) => JsonUtility.ToJson(_userAccount);
}