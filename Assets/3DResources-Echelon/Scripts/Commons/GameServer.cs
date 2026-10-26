using System;
using System.Collections;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Client for the Vector Unknown game server (Server/AppsScript/Code.gs in the repository).
/// Every request is a JSON POST; every response has "ok" plus either the result fields or "error",
/// a message that can be shown to the player as-is.
///
/// The server's web app URL is read from Resources/GameServerUrl.txt so it can be changed without
/// touching code. Passwords are only ever sent to the server, which checks them; the game keeps
/// just the session token it gets back.
/// </summary>
public class GameServer : MonoBehaviour
{
    public static string Token { get; private set; }
    public static bool IsLoggedIn { get { return !string.IsNullOrEmpty(Token); } }

    /// <summary>
    /// False when the player unticked "Save my data" at login: progress and play records are then
    /// not sent (SendToGoogle checks this). Classroom/expo session players always save.
    /// </summary>
    public static bool SaveData = true;

    private const float TimeoutSeconds = 30f;
    private static GameServer runner;
    private static string url;

    public static void Logout()
    {
        Token = null;
    }

    /// <summary>
    /// Sends {"action": action, ...fields} and calls exactly one of onSuccess / onError.
    /// A "token" in the response (login, createAccount, joinSession) becomes the session token;
    /// requests are sent with the current token when there is one.
    /// </summary>
    public static void Post(string action, JSONObject fields, Action<JSONNode> onSuccess, Action<string> onError)
    {
        if (runner == null)
        {
            GameObject go = new GameObject("GameServer");
            DontDestroyOnLoad(go);
            runner = go.AddComponent<GameServer>();
        }

        JSONObject request = fields ?? new JSONObject();
        request["action"] = action;
        if (IsLoggedIn && request["token"] == null)
            request["token"] = Token;
        runner.StartCoroutine(Send(request, onSuccess, onError));
    }

    private static string Url
    {
        get
        {
            if (url == null)
            {
                TextAsset config = Resources.Load<TextAsset>("GameServerUrl");
                url = config != null ? config.text.Trim() : "";
            }
            return url;
        }
    }

    private static IEnumerator Send(JSONObject request, Action<JSONNode> onSuccess, Action<string> onError)
    {
        if (Url.Length == 0)
        {
            Fail(onError, "The game server address is not set (Resources/GameServerUrl.txt).");
            yield break;
        }

        using (UnityWebRequest www = new UnityWebRequest(Url, UnityWebRequest.kHttpVerbPOST))
        {
            // text/plain keeps this a "simple" cross-origin request (no CORS preflight, which Apps Script can't answer)
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.ToString()));
            www.uploadHandler.contentType = "text/plain;charset=utf-8";
            www.downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequestAsyncOperation op = www.SendWebRequest();
            float elapsed = 0f;
            while (!op.isDone)
            {
                elapsed += Time.unscaledDeltaTime;
                if (elapsed > TimeoutSeconds)
                {
                    www.Abort();
                    Fail(onError, "The game server did not answer. Check your internet connection and try again.");
                    yield break;
                }
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[GameServer] " + request["action"].Value + " failed: " + www.error);
                Fail(onError, "Can't reach the game server. Check your internet connection and try again.");
                yield break;
            }

            JSONNode response = null;
            try
            {
                response = JSON.Parse(www.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[GameServer] unreadable response: " + e.Message);
            }

            if (response == null || !response["ok"].AsBool)
            {
                string error = response != null && response["error"] != null ? response["error"].Value : "";
                Fail(onError, error.Length > 0 ? error : "The game server sent an unexpected answer.");
                yield break;
            }

            if (response["token"] != null && response["token"].Value.Length > 0)
                Token = response["token"].Value;
            if (onSuccess != null)
                onSuccess(response);
        }
    }

    private static void Fail(Action<string> onError, string message)
    {
        Debug.LogWarning("[GameServer] " + message);
        if (onError != null)
            onError(message);
    }
}
