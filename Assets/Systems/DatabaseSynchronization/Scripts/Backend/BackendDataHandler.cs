using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public virtual void Add(T data)
        {
            if (!Datas.Exists(t => t.ID == data.ID))
            {
                data.PendingAction = PendingAction.Create;
                Datas.Add(data);
            }
            else
            {
                Debug.LogError("Your are trying to add" + data + " into handler datas but the data already exists. Auto switch to Update.");
                data.PendingAction = PendingAction.Update;
            }
        }

        public virtual void Remove(T data)
        {
            data.PendingAction = PendingAction.Delete;
        }

        public void Update(T data)
        {
            data.PendingAction = PendingAction.Update;
        }

        public virtual void ClearDatas() => Datas.Clear();

        public virtual void LoadAll(Action<List<T>> onDoneCallback, Action<string> onFailedCallback) 
        {
            //StartCoroutine(GetRequest("LoadAll", null, onDoneCallback, onFailedCallback));

        }

        protected virtual void CommitCreate(T data, Action<bool> resultCallback)
        {
            StartCoroutine(PostRequest("Create", JsonConvert.SerializeObject(data), resultCallback));
        }

        protected virtual void CommitUpdate(T data, Action<bool> resultCallback)
        {
            StartCoroutine(PostRequest("Update", JsonConvert.SerializeObject(data), resultCallback));
        }

        protected virtual void CommitDelete(T data, Action<bool> resultCallback)         
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
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                }

                resultCallback.Invoke(www.result == UnityWebRequest.Result.Success);
            }
        }
    }

    /*    public static class PostRequest<T>
        {
            private static HttpClient httpClient;

            public static async Task<bool> Execute(string apiHook, T data, List<(string, string)> headers, Dictionary<string, string> queryStrings, Action<string> onDoneCallback, Action<string> onFailedCallback)
            {
                httpClient = new HttpClient();

                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; ++i)
                    {
                        httpClient.DefaultRequestHeaders.Add(headers[i].Item1, headers[i].Item2);
                    }
                }

                string URL = BackendCore.GetURL(apiHook, qu);

                HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                try
                {
                    var postResponse = await httpClient.PostAsync(URL, content);
                    var responseString = await postResponse.Content.ReadAsStringAsync();

                    if (responseString.Contains("error"))
                    {
                        onFailedCallback?.Invoke(responseString);
                    }
                    else if (!postResponse.IsSuccessStatusCode)
                    {
                        onFailedCallback?.Invoke(postResponse.ReasonPhrase);
                    }
                    else
                    {
                        onDoneCallback?.Invoke(responseString);
                        return true;
                    }
                }
                catch (HttpRequestException e)
                {
                    onFailedCallback?.Invoke(e.Message);
                }
                catch (NotSupportedException e)
                {
                    onFailedCallback?.Invoke(e.Message);
                }
                catch (Exception e)
                {
                    onFailedCallback?.Invoke(e.Message);
                }

                return false;
            }
        }

        public static class GetRequest<T>
        {
            private static HttpClient httpClient;

            public static async Task<T> Execute(string apiHook, List<(string, string)> headers, Dictionary<string, string> queryStrings, Action<T> onDoneCallback, Action<string> onFailedCallback)
            {
                httpClient = new HttpClient();
                string contentString = "";

                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; ++i)
                    {
                        httpClient.DefaultRequestHeaders.Add(headers[i].Item1, headers[i].Item2);
                    }
                }

                string URL = BackendCore.CreateURL(apiHook, queryStrings);

                try
                {
                    Debug.Log($"GetRequest at {URL} ...");

                    var responseHttp = await httpClient.GetAsync(URL);
                    responseHttp.EnsureSuccessStatusCode();

                    contentString = await responseHttp.Content.ReadAsStringAsync();

                    if (contentString.Contains("error"))
                    {
                        Debug.LogError(contentString);
                        onFailedCallback?.Invoke(contentString);
                    }
                    else
                    {
                        T result = JsonConvert.DeserializeObject<T>(contentString);
                        onDoneCallback?.Invoke(result);
                        return result;
                    }
                }
                catch (HttpRequestException e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }
                catch (NotSupportedException e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }
                catch (Exception e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }

                return default(T);
            }

            public static async Task<string> ExecuteString(string apiHook, List<(string, string)> headers, Dictionary<string, string> queryStrings, Action<string> onDoneCallback = null, Action<string> onFailedCallback = null)
            {
                httpClient = new HttpClient();
                string contentString = "";

                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; ++i)
                    {
                        httpClient.DefaultRequestHeaders.Add(headers[i].Item1, headers[i].Item2);
                    }
                }

                string URL = BackendCore.CreateURL(apiHook, queryStrings);

                try
                {
                    Debug.Log($"GetRequest at {URL} ...");

                    var responseHttp = await httpClient.GetAsync(URL);
                    responseHttp.EnsureSuccessStatusCode();

                    contentString = await responseHttp.Content.ReadAsStringAsync();

                    if (contentString.Contains("error"))
                    {
                        Debug.LogError(contentString);
                        onFailedCallback?.Invoke(contentString);
                    }
                    else
                    {
                        onDoneCallback?.Invoke(contentString);
                        return contentString;
                    }
                }
                catch (HttpRequestException e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }
                catch (NotSupportedException e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }
                catch (Exception e)
                {
                    onFailedCallback?.Invoke(e.Message + " // " + contentString);
                }

                return null;
            }
        }
    */
}
