using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnityToHttp : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.InputField input;
    [SerializeField] UnityEngine.UI.Button  btnPost;
    [SerializeField] UnityEngine.UI.Button  btnGet;

    [SerializeField] public string URL = "http://localhost:880";

    // Start is called before the first frame update
    void Start()
    {
        btnPost.onClick.AddListener(()=>{
            StartCoroutine(SendPOST(URL + "/log", "area", input.text));
        });
        
        btnGet.onClick.AddListener(()=>{
            StartCoroutine(SendGET(URL + "/log?area=" + input.text));
        });
    }

    IEnumerator SendPOST(string send_url, string field, string postData)
    {
        WWWForm form = new WWWForm();
        form.AddField(field, postData);
        using(var req = UnityWebRequest.Post(send_url, form)){
            req.SendWebRequest();
            while(!req.isDone){
                yield return null;
            }
            if (req.isHttpError || req.isNetworkError){
                // 何かエラー
                Debug.Log("ERROR" + req.responseCode.ToString() + " : " + req.error);
            } else {
                // 成功
                Debug.Log("Success!!");
                Debug.Log(req.downloadHandler.text);
            }
        }
    }

    IEnumerator SendGET(string send_url)
    {
        using(var req = UnityWebRequest.Get(send_url)){
            req.SendWebRequest();
            while(!req.isDone){
                yield return null;
            }
            if (req.isHttpError || req.isNetworkError){
                // 何かエラー
                Debug.Log("ERROR" + req.responseCode.ToString() + " : " + req.error);
            } else {
                // 成功
                Debug.Log("Success!!");
                Debug.Log(req.downloadHandler.text);
            }
        }
    }    
}
