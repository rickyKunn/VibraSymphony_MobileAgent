using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class TCPSender: MonoBehaviour
{

    private byte[] newbits;
    private bool converted;
    [SerializeField]
    private string Name;
    public string m_ipAddress = "127.0.0.1";
    public int m_port = 2001;
    private bool start = false;
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    private bool m_isConnection;
    public string path;
    public bool hasSelectedMusic;
    private void Start()
    {
    }
    public async void ConnectToVR(string ip, int port, bool isAdjustMode)
    {
        try
        {
            // 指定された IP アドレスとポートでサーバに接続します
            m_isConnection = true;
            Debug.LogFormat("接続成功");
            if (!hasSelectedMusic)
            {
                if (Application.platform == RuntimePlatform.Android) await UniTask.WaitUntil(() => hasSelectedMusic);
            }

            TrySend(path, ip,port,isAdjustMode);

        }
        catch (SocketException)
        {
            // サーバが起動しておらず接続に失敗した場合はここに来ます
            Debug.LogError("接続失敗");
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            ConnectToVR(ip, port,isAdjustMode);

        }
    }

    private void OnDestroy()
    {
        m_tcpClient?.Dispose();
        m_networkStream?.Dispose();

        Debug.Log("切断");
    }
    private async void TrySend(string filePath,string ip,int port, bool _adjust)
    {
        var testPath = Application.dataPath + "/Musics/" + Name + ".mp3";
        FindObjectOfType<MusicManager>().SetAudio(filePath, _adjust);
        try
        {
            using (TcpClient client = new TcpClient(ip, port))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {

                            print("sending...");
                            await fileStream.CopyToAsync(stream); // FileStreamからNetworkStreamへ非同期でコピー
                            GameObject.Find("StartManager").GetComponent<StartManager>().StartButtonPressed();
                            print("sent");
                        }
                    }
                    catch(FileNotFoundException e)
                    {
                        Debug.Log(e);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException: " + socketException.ToString());
        }
    }
    public void StartSending()
    {
        start = true;
    }
    public void Send()
    {

        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 2222))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    var buffer = Encoding.UTF8.GetBytes("I am sending you");
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException: " + socketException.ToString());
        }

    }

    public static long GetFileSize(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
        {
            return fileInfo.Length; // ファイルサイズをバイト単位で返す
        }
        else
        {
            return -1; // ファイルが存在しない場合は-1を返す
        }
    }
}
