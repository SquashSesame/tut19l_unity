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

    // スレッド用
    Thread receiveThread;
    bool isHostStop = false;

    // IPアドレス一覧
    IPAddress[] adrList;

    // 受け取るパケットのデータ
    struct sPacket {
        public string  objName;
        public float   px;
        public float   py;
        public float   pz;

        public sPacket(string name, float x, float y, float z){
            this.objName = name;
            this.px = x;
            this.py = y;
            this.pz = z;
        }
    };
    // 受け取るパケットのキュー
    Queue<sPacket> packetQueue = new Queue<sPacket>();

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
        receiveThread.Start();
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null){
            isHostStop = true;
            Thread.Sleep(1);
            try {
                receiveThread.Interrupt();
                receiveThread.Abort();
            }
            catch {}
            receiveThread = null;
        }

        if (hostClient != null){
            hostClient.Close();
            hostClient = null;
        }
    }

    // 通信スレッド
    void ReceiveMain()
    {
        IPEndPoint remoteEP = null;
        while (isHostStop == false)
        {
            Thread.Sleep(1);

            byte[] receiveBytes;
            try {
                receiveBytes = hostClient.Receive(ref remoteEP);
            }
            catch {
                continue;
            }

            // 受信データを取得
            try {
                // パケットデータを取得して、キューに積む（細かい処理はメインスレッドでやる）
                var packet = Bytes2Packet(receiveBytes);
                packetQueue.Enqueue(packet);
                Debug.Log("RECEIVE : " + packet.objName );
            }
            catch(System.Exception ex) {
                Debug.LogWarning("ERROR: " + ex.Message);
                continue;
            }
        }
    }

    sPacket Bytes2Packet(byte[] bytes)
    {
        /*
            バイト配列のフォーッマット
            byte [16]   Name
            byte [4]    pox.x
            byte [4]    pox.y
            byte [4]    pox.z
        */
        return new sPacket(
            System.Text.Encoding.ASCII.GetString(bytes),
            System.BitConverter.ToSingle(bytes, 16),
            System.BitConverter.ToSingle(bytes, 20),
            System.BitConverter.ToSingle(bytes, 24)
        );
    }

    void Update()
    {
        // 受け取ったパケットをメインスレッド側で処理
        if (packetQueue.Count > 0)
        {
            do {
                // キューから取り出す
                var packet = packetQueue.Dequeue();
                
                // 参照するオブジェクトを検索
                var findObj = GameObject.Find(packet.objName);
                if (findObj != null){
                    // 見つかったオブジェクトへ位置を設定
                    findObj.transform.position = new Vector3(packet.px, packet.py, packet.pz);
                } else {
                    Debug.LogWarning("WARN: オブジェクトが見つかりません " + packet.objName);
                }
            }
            while (packetQueue.Count > 0);
        }

    }

    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.textArea);
        myStyle.fontSize = 16;

        int posY = 10;
        if (adrList != null && adrList.Length > 0){
            for(int i=0; i<adrList.Length; ++i)
            {
                GUI.Label(new Rect(10,posY, 300, 100), "HOST IP : " + adrList[i].ToString(), myStyle);
                posY += 40;
            }
        }
        GUI.Label(new Rect(10, posY, 300, 100), "HOST Port : " + HostPort.ToString(), myStyle);
        posY += 40;
    }
}
