using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;

namespace Tests
{
    public class Test_AsyncAwait
    {
//        [UnityTest]
//        public IEnumerator Test_Coroutine_Func00 ()
//        {
//            yield return null;
//        }


        delegate void EndAction ();

        async Task ASyncFunc00 (EndAction endFunc)
        {
            Debug.Log ("---Start---");
            for (var i = 0; i < 10; ++i) {
                await Task.Delay (2);
                Debug.Log ($"Count:{i}");
            }
            Debug.Log ("---End---");

            // 終了を通知
            if (endFunc != null) {
                endFunc.Invoke ();
            }
        }

        [UnityTest]
        public IEnumerator Test_Async_Func00 ()
        {
            bool isEnd = false;

            Task.Run (() =>
                ASyncFunc00 (() => {
                    // 終了処理
                    isEnd = true;
                    Debug.Log ("終了通知");
                })
            );
            Debug.Log ("次の処理");
            while (isEnd == false) {
                yield return null;
            }
        }

    }
}
