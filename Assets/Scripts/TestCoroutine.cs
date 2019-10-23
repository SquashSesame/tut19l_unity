using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour
{
    // デリゲート宣言
    delegate void EndAction ();

    // コルーチンで処理
    IEnumerator CoroutineFunc00 (EndAction endFunc)
    {
        Debug.Log ("---Start---");
        for (var i = 0; i < 10; ++i) {
            yield return new WaitForSeconds (0.01f);
            Debug.Log ($"Count:{i}");
        }
        Debug.Log ("---End---");

        // 終了を通知
        if (endFunc != null) {
            endFunc.Invoke ();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine (
            CoroutineFunc00 (() => { Debug.Log ("終了通知"); })
        );
        Debug.Log ("次の処理");
    }

}
