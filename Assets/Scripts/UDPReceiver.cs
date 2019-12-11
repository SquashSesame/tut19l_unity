using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    public int HostPort = 6000;

    UdpClient hostClient;

    byte[] receiveBytes = new byte[32];
    Vector3 receiveVec3 = new Vector3();

    Thread receiveThread;
    bool isHostStop = false;

    IPAddress[] adrList;

    // Start is called before the first frame update
    void Start()
    {
        // 自分のIPアドレスを取得
        string hostname = Dns.GetHostName();
        adrList = Dns.GetHostAddresses(hostname);
        
        // Host を起動
        hostClient = new UdpClient(HostPort);
        hostClient.Client.ReceiveTimeout = 1000;
        receiveThread = new Thread(new ThreadStart(ReceiveMain));
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null){
            isHostStop = true;
            Thread.Sleep(1);
        }

        if (hostClient != null){
            hostClient.Close();
        }
    }

    void ReceiveMain()
    {
        while (isHostStop == false)
        {
            IPEndPoint remoteEP = null;
            receiveBytes = hostClient.Receive(ref remoteEP);

            // 受信データを取得
            string refObjName = Byte2Vec(receiveBytes, ref receiveVec3);

            // 参照するオブジェクトを検索
            var findObj = GameObject.Find(refObjName);
            if (findObj != null){
                // 見つかったオブジェクトへ位置を設定
                findObj.transform.position = receiveVec3;
            }

            Thread.Sleep(1);
        }
        receiveThread = null;
    }

    string Byte2Vec(byte[] bytes, ref Vector3 pos)
    {
        /*
            バイト配列のフォーッマット
            byte [16]   Name
            byte [4]    pox.x
            byte [4]    pox.y
            byte [4]    pox.z
        */
        name = System.Text.Encoding.ASCII.GetString(bytes);

        pos.x = System.BitConverter.ToSingle(bytes, 16);
        pos.y = System.BitConverter.ToSingle(bytes, 20);
        pos.z = System.BitConverter.ToSingle(bytes, 24);

        return name;
    }

    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.textArea);
        myStyle.fontSize = 20;

        int posY = 10;
        if (adrList != null && adrList.Length > 0){
            //for(int i=0; i<adrList.Length; ++i)
            {
                GUI.Label(new Rect(10,posY, 300, 100), "HOST IP : " + adrList[0].ToString(), myStyle);
                posY += 40;
            }
        }
        GUI.Label(new Rect(10, posY, 300, 100), "HOST Port : " + HostPort.ToString(), myStyle);
        posY += 40;
    }
}
