using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    public static IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
    {
        using(UnityWebRequest r = UnityWebRequest.Get(url))
        {
            r.SetRequestHeader("Accept", "application/json");
            r.SetRequestHeader("X-Unity-Req", "true");
            yield return r.SendWebRequest();
            callback(r);
        }
    }

    public static IEnumerator GetTexture(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest r = UnityWebRequestTexture.GetTexture(url))
        {
            r.SetRequestHeader("Accept", "image/*");
            r.SetRequestHeader("X-Unity-Req", "true");
            yield return r.SendWebRequest();
            callback(r);
        }
    }

    public static IEnumerator GetFile(string url, string localPath, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest r = UnityWebRequest.Get(url))
        {
            r.SetRequestHeader("X-Unity-Req", "true");
            r.downloadHandler = new DownloadHandlerFile(localPath + @"/temp.pdf");
            yield return r.SendWebRequest();
            callback(r);
        }
    }
}
