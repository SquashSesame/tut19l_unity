using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPPlayerSender : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button btnConnect;
    [SerializeField] GameObject RefObject;

    public string HostIP = "127.0.0.1";
    public int HostPort = 6000;

    UdpClient myClient;

    byte[] sendBytes = new byte[32];

    // Start is called before the first frame update
    void Start()
    {
        btnConnect.onClick.AddListener(()=>{
            // UDP -> Host へ接続
            myClient = new UdpClient();
            myClient.Connect(HostIP, HostPort);            
        });
    }

    void OnApplicationQuit()
    {
        if (myClient != null){
            myClient.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RefObject == null){
            // 操作するオブジェクトが決まっていないときは何もしない
            return;
        }

        const float SPEED = 1.0f;
        var pos = RefObject.transform.position;
        bool isSend = false;

        // プレイヤーの場合：移動操作＆送信
        if (Input.GetKey(KeyCode.LeftArrow)){
            pos.x -= SPEED * Time.deltaTime;
            isSend = true;
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            pos.x += SPEED * Time.deltaTime;
            isSend = true;
        }
        if (Input.GetKey(KeyCode.UpArrow)){
            pos.z += SPEED * Time.deltaTime;
            isSend = true;
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            pos.z -= SPEED * Time.deltaTime;
            isSend = true;
        }

        RefObject.transform.position = pos;

        if (isSend && myClient != null){
            // BOXの名前と、ポジションを送信
            Vec2Byte(RefObject.name, ref pos, sendBytes);
            myClient.Send(sendBytes, sendBytes.Length);
            Debug.Log("SEND : " + RefObject.name );
        }
    }

    public void Vec2Byte(string name, ref Vector3 pos, byte[] bytes)
    {
        /*
            バイト配列のフォーッマット
            byte [16]   Name
            byte [4]    pox.x
            byte [4]    pox.y
            byte [4]    pox.z
        */
        byte[] data = System.Text.Encoding.ASCII.GetBytes(name);
        for (int i=0 ; i<15 && i<data.Length ; ++i){
            bytes[i] = data[i];
        }
        for (int i=data.Length ; i<15 ; ++i){
            bytes[i] = 0;
        }
        bytes[15] = 0;

        byte[] x = System.BitConverter.GetBytes(pos.x);
        byte[] y = System.BitConverter.GetBytes(pos.y);
        byte[] z = System.BitConverter.GetBytes(pos.z);
        for (int i=0 ; i<4 ; ++i){
            bytes[16+i] = x[i];
            bytes[20+i] = y[i];
            bytes[24+i] = z[i];
        }
    }
}
