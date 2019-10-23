using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Test_Delegate
    {
        // delegate の宣言
        delegate string HogeHogeFunc ();

        // delegate を利用した変数定義
        HogeHogeFunc onFuncHogeHoge;

        // event を定義
        event HogeHogeFunc onHogeListner;

        string func00 (){
            return "func00";
        }

        string func01 (){
            return "func01";
        }



        [Test]
        public void Test_Delegate_Func00 ()
        {
            onFuncHogeHoge = func00;            //← デリゲート変数に関数を登録
            string result = onFuncHogeHoge ();  //← デリゲートを実行

            Assert.AreEqual ("func00", result);
        }

        [Test]
        public void Test_Delegate_Func01 ()
        {
            string result;

            onFuncHogeHoge = func01;
            result = onFuncHogeHoge ();             //<- デリゲート実行
            
            Assert.AreEqual ("func01", result);
        }

        [Test]
        public void Test_Delegate_Func02 ()
        {
            string result;

            onFuncHogeHoge = () => {
                Debug.Log ("func02");
                return "func02";
            };
            result = onFuncHogeHoge ();             //<- デリゲート実行

            Assert.AreEqual ("func02", result);
        }
    }
}
