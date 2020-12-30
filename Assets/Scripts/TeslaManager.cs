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
    private const string GrantType = "password";
    private const string ClientID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";
    private const string ClientSecret = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";
    private const string PathToLoginCreds = "/Users/jacksonbarnes/src/Personal/Side Projects/Interaction Modality Experiments/Development/Creds";
    private const string loginFile = "login.json";

    public void GetAuthToken() {

        StartCoroutine(SendAuth2Request());
    }

    private IEnumerator SendAuth2Request() {

        string path = Path.Combine(PathToLoginCreds, loginFile);

        if (!File.Exists(path)) {

            Debug.Log("Getting auth token...");

            WWWForm form = new WWWForm();

            form.AddField("grant_type", GrantType);
            form.AddField("client_id", ClientID);
            form.AddField("client_secret", ClientSecret);

            LoginInfo loginInfo = RetrieveLocalLoginInfo(path);

            form.AddField("email", loginInfo.Values[0].email);
            form.AddField("password", loginInfo.Values[0].password);

            using (UnityWebRequest www = UnityWebRequest.Post("https://owner-api.teslamotors.com/oauth/token?grant_type=password", form)) {

                www.SetRequestHeader("User-Agent", "Mine control for my Tesla Modal 3");

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

    private LoginInfo RetrieveLocalLoginInfo(string path) {

        LoginInfo loginInfo = JsonUtility.FromJson<LoginInfo>(InitilizeJSON(File.ReadAllText(path), "Values"));

        return loginInfo;
    }

    public string InitilizeJSON(string json, string objectArray) {

        return "{\"" +objectArray + "\":" + json + "}";
    }
}

[Serializable]
public class LoginInfo {

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

        TeslaManager teslaManagerTarget = (TeslaManager)target;

        if (GUILayout.Button("Get Auth Token")) {

            teslaManagerTarget.GetAuthToken();
        }
    }
}
