using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class HttpToUnity : MonoBehaviour
{

    public IPAddress IPAddress;
    public string IPAddressString = "localhost";
    public int Port = 5000;
    public bool isStop = false;
    const string pathSeparater = "/";
    const float TIMEOUT_TIME = 5.0f;

    public delegate void RequestHandler (
        string rawUrl,
        System.Text.RegularExpressions.Match match,
        HttpListenerResponse response);

    private Dictionary<System.Text.RegularExpressions.Regex, RequestHandler>
        _requestHandlers = new Dictionary<System.Text.RegularExpressions.Regex, RequestHandler> ();

    HttpListener httpListener;
    void Start ()
    {
        _requestHandlers [new System.Text.RegularExpressions.Regex (
            @"^/log")] = Cmd_log;

        // サーバー起動
        StartCoroutine (ServerMain ());
    }

    /*
        サーバー起動
     */
    IEnumerator ServerMain ()
    {
        httpListener = new HttpListener ();
        httpListener.Prefixes.Add (
            "http://" + IPAddressString + ":" + Port + pathSeparater);
        httpListener.Start ();

        while (isStop == false)
        {
            httpListener.BeginGetContext (
                new AsyncCallback (ListenerCallback), httpListener);
            yield return new WaitForSeconds (0.1f);
        }

        httpListener.Stop ();
        httpListener = null;
        yield return null;
    }

    /*
        コマンドを呼び出す
     */
    void ListenerCallback (IAsyncResult result)
    {
        var listener = (HttpListener)result.AsyncState;
        var context = listener.EndGetContext (result);
        var request = context.Request;
        using (var response = context.Response)
        {
            foreach (System.Text.RegularExpressions.Regex r in _requestHandlers.Keys)
            {
                System.Text.RegularExpressions.Match m = r.Match (request.Url.AbsolutePath);
                if (m.Success)
                {
                    // 該当のコマンドを実行
                    (_requestHandlers [r]) (request.RawUrl, m, response);
                    return;
                }
            }

            // ERROR
            Debug.LogWarning ("ERROR : " + request.Url.ToString ());
            response.StatusCode = 404;
            using (var output = response.OutputStream)
            {
                using (var writer = new System.IO.StreamWriter (output))
                {
                    writer.Write ("ERROR\n");
                }
                output.Close ();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
