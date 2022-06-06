using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.IO;

public enum THEME_LIST
{
    THEME0, THEME1, THEME2, THEME3, THEME4
}


[System.Serializable]
public class Theme
{
    public THEME_LIST name = THEME_LIST.THEME3;

    public static void SetTheme(THEME_LIST themeName)
    {
        JsonManager.Write(Path.Combine(Application.persistentDataPath, "Theme.json"), new Theme { name = themeName });
    }

    public static Sprite GetImage(string imageName)
    {
        Theme t = JsonManager.Read<Theme>(Path.Combine(Application.persistentDataPath, "Theme.json"));
        string t_name = t == null ? ((int)THEME_LIST.THEME3).ToString() : ((int)t.name).ToString();
        return Resources.Load<Sprite>("theme" + t_name + "/" + imageName + "_Theme" + t_name);
    }
}