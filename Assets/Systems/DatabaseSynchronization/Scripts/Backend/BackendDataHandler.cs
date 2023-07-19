using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace Atomix.Backend
{
    /// <summary>
    /// Une classe templatée basée sur un pattern repository que j'utilise dans mes projets personnels
    /// Des adaptations ont été nécéssaires car la build WEB GL ne permet pas d'utiliser les requêtes HTTP C# 
    /// On doit donc passer par les UnityWebRequest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BackendDataHandler<T> : MonoBehaviour where T : IBackendData
    {
        public List<T> Datas = new List<T>();

        public virtual void ClearDatas() => Datas.Clear();

        public virtual void LoadAll(Action<List<T>> onDoneCallback, Action<string> onFailedCallback) 
        {
            //StartCoroutine(GetRequest("LoadAll", null, onDoneCallback, onFailedCallback));
        }

        public virtual void CommitCreate(T data, Action<bool> resultCallback)
        {
            StartCoroutine(PostRequest("Create", JsonConvert.SerializeObject(data), resultCallback));
        }

        public virtual void CommitUpdate(T data, Action<bool> resultCallback)
        {
            StartCoroutine(PostRequest("Update", JsonConvert.SerializeObject(data), resultCallback));
        }

        public virtual void CommitDelete(T data, Action<bool> resultCallback)         
        {
            StartCoroutine(PostRequest("Delete", JsonConvert.SerializeObject(data), resultCallback));
        }

        public IEnumerator GetRequest(string endpoint, Dictionary<string, string> queryString, Action<string> onDoneCallback, Action<string> onFailedCallback)
        {
            string _url = BackendCore.CreateURL(endpoint, queryString);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(_url))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = _url.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        onFailedCallback.Invoke(webRequest.result.ToString());
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                        onDoneCallback.Invoke(webRequest.downloadHandler.text);
                        break;
                }

            }
        }

        public IEnumerator PostRequest(string endpoint, string data, Action<bool> resultCallback)
        {
            string _url = BackendCore.CreateURL(endpoint);

            using (UnityWebRequest www = UnityWebRequest.Post(_url, data))
            {

                byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Form upload complete!" + www.downloadHandler.text);
                }

                resultCallback.Invoke(www.result == UnityWebRequest.Result.Success);
            }
        }
    }    
}
