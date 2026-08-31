// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Lists {

    // Talks to the RPC server (see /server at the repo root) over HTTP.
    // Every call is a coroutine (start with StartCoroutine from a
    // MonoBehaviour) that reports its result through onComplete, since
    // UnityWebRequest's async model doesn't return a value directly.
    public static class ServerClient {
        private const string BaseUrl = "https://list-two-pi.vercel.app";

        // Vercel's zero-config Python/FastAPI detection auto-mounts the app
        // under this prefix (see the deployment summary) - there's no
        // vercel.json rewrite overriding it, so every route needs it too.
        private const string ApiPrefix = "/fastapi";

        [Serializable]
        private class GetUserRequestBody {
            public string name;
        }

        [Serializable]
        private class GetUserResponseBody {
            public string publicKey;
            public int image;
        }

        [Serializable]
        private class RegisterRequestBody {
            public string name;
            public string publicKey;
            public int image;
        }

        [Serializable]
        private class RegisterResponseBody {
            public bool success;
        }

        // onComplete(requestSucceeded, publicKey, image). If requestSucceeded is
        // true and publicKey is empty, name is not registered.
        public static IEnumerator GetUser(string name, Action<bool, string, int> onComplete)
        {
            string requestJson = JsonUtility.ToJson(new GetUserRequestBody { name = name });

            using (UnityWebRequest request = PostJson("/getUser", requestJson)) {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success) {
                    AuditLog.Log("ServerClient.GetUser failed: " + request.error);
                    onComplete?.Invoke(false, "", 0);
                    yield break;
                }

                GetUserResponseBody body = JsonUtility.FromJson<GetUserResponseBody>(request.downloadHandler.text);
                onComplete?.Invoke(true, body.publicKey ?? "", body.image);
            }
        }

        // onComplete(success). success is false both for network/HTTP failures
        // and for a rejected duplicate name (see /server/API.md) - the caller
        // is expected to have already checked availability via GetUser first,
        // so a duplicate here should only happen in a rare race.
        public static IEnumerator Register(string name, string publicKey, int image, Action<bool> onComplete)
        {
            string requestJson = JsonUtility.ToJson(new RegisterRequestBody { name = name, publicKey = publicKey, image = image });

            using (UnityWebRequest request = PostJson("/register", requestJson)) {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success) {
                    AuditLog.Log("ServerClient.Register failed: " + request.error);
                    onComplete?.Invoke(false);
                    yield break;
                }

                RegisterResponseBody body = JsonUtility.FromJson<RegisterResponseBody>(request.downloadHandler.text);
                onComplete?.Invoke(body.success);
            }
        }

        private static UnityWebRequest PostJson(string path, string json)
        {
            string url = BaseUrl.TrimEnd('/') + ApiPrefix + path;
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }
    }
}
