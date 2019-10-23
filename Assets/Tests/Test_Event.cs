using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;

namespace Tests {
    public class Test_Event {
        // delegate の宣言
        delegate void HogeFunc (ref int count);

        // event を定義
        event HogeFunc onHogeListner;
        event HogeFunc onHoge2Listner;

        List<HogeFunc> _list = new List<HogeFunc> ();
        event HogeFunc onHoge3Listner {
            add { _list.Add (value); }
            remove { _list.Remove (value); }
        }

        void func00 (ref int count)
        {
            count++;
            Debug.Log ("func00");
        }

        void func01 (ref int count)
        {
            count++;
            Debug.Log ("func01");
        }

        [Test]
        public void Test_Event_Func00 ()
        {
            int count = 0;
            // event へ関数を登録
            onHogeListner += func00;
            onHogeListner += func01;

            // event を実行
            //onHogeListner (ref count);              //← イベント実行
            onHogeListner.Invoke (ref count);     //← イベント実行

            Assert.AreEqual (count, 2);
        }

        /*
         *  ラムダ式
         */
        [Test]
        public void Test_Event_Func01 ()
        {
            int count = 0;

            // event へ関数を登録
            onHoge2Listner += func00;
            onHoge2Listner += func01;
            onHoge2Listner += (ref int rcount) => {
                rcount++;
                Debug.Log ("func02");
            };

            // event を実行
            onHoge2Listner (ref count);             //← イベント実行
            //onHoge2Listner.Invoke (ref count);    //← イベント実行

            Assert.AreEqual (count, 3);
        }

        /*
         *  UnityEvent
         */
        UnityEngine.Events.UnityEvent onUnityListener = new UnityEngine.Events.UnityEvent ();

        UnityEngine.Events.UnityAction onUnityAction;

        [Test]
        public void Test_Event_Func02 ()
        {
            int count = 0;

            // event へ関数を登録
            onUnityListener.AddListener (()=> {
                count++;
                Debug.Log ("ufunc00");
            });
            onUnityListener.AddListener (()=> {
                count++;
                Debug.Log ("ufunc01");
            });

            // event を実行
            onUnityListener.Invoke ();              //← イベント実行

            Assert.AreEqual (count, 2);
        }


    }
}
