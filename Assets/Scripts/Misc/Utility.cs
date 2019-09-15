using System;
using UnityEngine;

public class Utility {
    public static string LoadTextFile(string filePath) {
        return (Resources.Load<TextAsset>(filePath)).text;
    }
    
    public static T CreateObjectFromJSON<T>(string filePath) {
        return JsonUtility.FromJson<T>(LoadTextFile(filePath));
    }

    public static Sprite LoadSpriteFile(string filePath) {
        return Resources.Load<Sprite>(filePath);
    }
}
