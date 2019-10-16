using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Delegate_Test
    {
        // delegate の宣言
        delegate string HogeHogeFunc ();

        // delegate を利用した変数定義
        HogeHogeFunc onFuncHogeHoge;


        string func00 ()
        {
            Debug.Log ("func00");
            return "func00";
        }

        string func01 ()
        {
            Debug.Log ("func01");
            return "func01";
        }

        // A Test behaves as an ordinary method
        [Test]
        public void Test_Delegate()
        {
            string result;

            onFuncHogeHoge = func00;
            result = onFuncHogeHoge ();             //<- デリゲート実行

            Assert.AreEqual ("func00", result);

            onFuncHogeHoge = func01;
            result = onFuncHogeHoge ();             //<- デリゲート実行

            Assert.AreEqual ("func01", result);

            onFuncHogeHoge = () => {
                Debug.Log ("func02");
                return "func02";
            };
            result = onFuncHogeHoge ();             //<- デリゲート実行

            Assert.AreEqual ("func02", result);
        }

    }
}
