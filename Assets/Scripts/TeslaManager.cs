using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class TeslaManager : MonoBehaviour
{
    [SerializeField]
    private string CarEndpointId;

    [SerializeField]
    private string StateEndpoint;

    [SerializeField]
    private string CommandEndpoint;

    private const string GrantType = "password";

    private const string ClientID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";

    private const string ClientSecret = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";

    private const string PathToLoginCreds = "/Users/jacksonbarnes/src/Personal/Side Projects/Interaction Modality Experiments/Development/Creds";

    private const string loginFile = "login.json";

    private const string authFile = "auth.json";

    private const string baseEndpointUri = "https://owner-api.teslamotors.com/api/1/vehicles";

    public void GetAuthToken() {

        StartCoroutine(SendAuthRequest());
    }

    public void StateRequest() {

        StartCoroutine(SendStateRequest(StateEndpoint));
    }

    public void CommandRequest() {

        StartCoroutine(CarCommandRequest(CommandEndpoint));
    }

    private IEnumerator SendAuthRequest() {

        string path = Path.Combine(PathToLoginCreds, loginFile);

        if (!File.Exists(path)) {

            Debug.Log("Getting auth token...");

            WWWForm form = new WWWForm();

            form.AddField("grant_type", GrantType);
            form.AddField("client_id", ClientID);
            form.AddField("client_secret", ClientSecret);

            LoginCredentials loginInfo = RetrieveAuthInfo<LoginCredentials>(path);

            form.AddField("email", loginInfo.Values[0].email);
            form.AddField("password", loginInfo.Values[0].password);

            using (UnityWebRequest www = UnityWebRequest.Post("https://owner-api.teslamotors.com/oauth/token?grant_type=password", form)) {

                www.timeout = 5;

                www.SetRequestHeader("User-Agent", "Mind control for my Tesla Modal 3");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError) {

                    Debug.Log(www.error);

                } else {

                    Debug.Log("POST successful!");

                    StringBuilder sb = new StringBuilder();

                    foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders()) {

                        sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                    }

                    //TODO Create a local token.txt file with header and body information in a safe place

                    // Print Headers
                    Debug.Log(sb.ToString());

                    // Print Body
                    Debug.Log(www.downloadHandler.text);
                }
            }
        } else {

            Debug.LogWarning("Auth token already exists");
        }
    }

    private IEnumerator SendStateRequest(string stateRequestEndpoint) {

        string path = Path.Combine(PathToLoginCreds, authFile);

        AuthInfo authInfo = RetrieveAuthInfo<AuthInfo>(path);

        using (UnityWebRequest www = UnityWebRequest.Get($"{baseEndpointUri}/{CarEndpointId}/{stateRequestEndpoint}")) {

            www.timeout = 5;

            www.SetRequestHeader("User-Agent", "Mind control for my Tesla Modal 3");
            www.SetRequestHeader("Authorization", $"{authInfo.Values[0].token_type} {authInfo.Values[0].access_token}");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {

                Debug.Log(www.error);

            } else {

                Debug.Log("GET successful!");

                StringBuilder sb = new StringBuilder();

                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders()) {

                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator CarCommandRequest(string commandRequestEndpoint) {

        string path = Path.Combine(PathToLoginCreds, authFile);

        AuthInfo authInfo = RetrieveAuthInfo<AuthInfo>(path);

        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post($"{baseEndpointUri}/{CarEndpointId}/{commandRequestEndpoint}", form)) {

            www.timeout = 5;

            www.SetRequestHeader("User-Agent", "Mind control for my Tesla Modal 3");
            www.SetRequestHeader("Authorization", $"{authInfo.Values[0].token_type} {authInfo.Values[0].access_token}");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {

                Debug.Log(www.error);

            } else {

                Debug.Log("POST successful!");

                StringBuilder sb = new StringBuilder();

                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders()) {

                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    private T RetrieveAuthInfo<T>(string path) {

        T RequestedObject = JsonUtility.FromJson<T>(InitilizeJSON(File.ReadAllText(path), "Values"));

        return RequestedObject;
    }

    public string InitilizeJSON(string json, string objectArray) {

        return "{\"" + objectArray + "\":" + "[" + json + "]" + "}";
    }
}

[Serializable]
public class AuthInfo {

    public Auth[] Values;

    [Serializable]
    public class Auth {

        public string access_token;
        public string token_type;
        public int expires_in;
        public string refresh_token;
        public int created_at;
    }
}

[Serializable]
public class LoginCredentials {

    public Login[] Values;

    [Serializable]
    public class Login {

        public string email;
        public string password;
    }
}

[CustomEditor(typeof(TeslaManager))]
public class TeslaManagerEditor : Editor {

    public override void OnInspectorGUI() {

        base.DrawDefaultInspector();

        GUILayout.Space(10);

        TeslaManager teslaManagerTarget = (TeslaManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Get Car State")) {

            teslaManagerTarget.StateRequest();
        }

        if (GUILayout.Button("Send Car Command")) {

            teslaManagerTarget.CommandRequest();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Get Auth Token")) {

            teslaManagerTarget.GetAuthToken();
        }
    }
}
