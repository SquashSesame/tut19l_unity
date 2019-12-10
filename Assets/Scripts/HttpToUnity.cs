using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class HttpToUnity : MonoBehaviour
{
    public string ServerURL = "http://*:5000/";
    public bool isStop = false;

    [SerializeField] UnityEngine.UI.Text Message;
    [SerializeField] UnityEngine.UI.Button btnStopServer;

    public delegate void RequestHandler (
        string rawUrl,
        HttpListenerResponse response);

    private Dictionary<Regex, RequestHandler>
        _commandList = new Dictionary<Regex, RequestHandler> ();

    HttpListener httpListener;
    
    void Start ()
    {
        Message.text = string.Empty;
        btnStopServer.onClick.AddListener(()=>{
            isStop = true;
        });

        // 処理をするコマンドを追加
        _commandList [new Regex (@"^/log")] = Cmd_log;
        _commandList [new Regex (@"^/hoge")] = Cmd_hoge;

        // サーバー起動
        StartCoroutine (ServerMain ());
    }

    /*
        サーバー起動
     */
    IEnumerator ServerMain ()
    {
        httpListener = new HttpListener ();
        httpListener.Prefixes.Add (ServerURL);
        MessageLog("サーバー開始");
        httpListener.Start ();

        while (isStop == false)
        {
            httpListener.BeginGetContext (
                new AsyncCallback (ListenerCallback), httpListener);
            yield return new WaitForSeconds (0.1f);
        }

        httpListener.Stop ();
        MessageLog("サーバー停止");
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
            foreach (Regex r in _commandList.Keys)
            {
                // 登録されているコマンドを正規表現を使って検索
                Match m = r.Match (request.Url.AbsolutePath);
                if (m.Success)
                {
                    // 該当のコマンドを実行
                    (_commandList [r]) (request.RawUrl, response);
                    return;
                }
            }

            // ERROR（登録されているコマンドがないとき）
            response.StatusCode = 404;
            WriteResponse(response.OutputStream, "ERROR : No Command " + request.RawUrl);
        }
    }

    void WriteResponse(Stream output, string responseMsg)
    {
        MessageLog(responseMsg);
        using (var writer = new StreamWriter (output))
        {
            writer.Write (responseMsg);
        }
        output.Close ();
    }

    void MessageLog(string msg)
    {
        Debug.Log(msg);
        Message.text = msg;
    }


    void Cmd_log(string url, HttpListenerResponse response)
    {
        // とりあえず、入力されてURLをレスポンスで返すのみ
        WriteResponse(response.OutputStream, "LOG : " + url);
    }

    void Cmd_hoge(string url, HttpListenerResponse response)
    {
        // とりあえず、入力されてURLをレスポンスで返すのみ
        WriteResponse(response.OutputStream, "HOGE : " + url);
    }

}
