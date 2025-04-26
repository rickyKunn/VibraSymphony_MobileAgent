

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading; // CancellationTokenSource 用

using UnityEngine;
using System.IO;
using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// TCP 通信を行うサーバ側のコンポーネント
/// </summary>
public class TCPGetIP : MonoBehaviour
{
    //================================================================================
    // 変数
    //================================================================================
    public string m_ipAddress = "127.0.0.1";
    public int m_port = 8000;
    private TcpListener m_tcpListener;
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    private OSCListener osclistner;
    private bool hasRecieved, adjustMode, adjustStart, startDelay, adjustEnd, musicStart;
    private bool NormalCetecStart, VibeCetecStart, musicStartCetec, musicEndCetec;
    private string VRIP;
    private int VRPort, adjustSec;
    private float delaySec, endAdjustSec, startMusicDelaySec;
    private DateTime expectVibingTime, expectStartTime;
    private CancellationTokenSource cancellationTokenSource;

    //================================================================================
    // 関数
    //================================================================================

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();

        // OnProcessを非同期実行
        _ = OnProcess(cancellationTokenSource.Token);
    }

    private void OnDestroy()
    {
        try
        {
            cancellationTokenSource?.Cancel(); // 非同期タスクをキャンセル
            cancellationTokenSource?.Dispose();

            m_networkStream?.Close();
            m_networkStream?.Dispose();

            m_tcpClient?.Close();
            m_tcpClient = null;

            m_tcpListener?.Stop();
            m_tcpListener = null;

            UnityEngine.Debug.Log("OnDestroyでリソースを解放しました");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"OnDestroyエラー: {e.Message}");
        }
    }

    private void Update()
    {
        if (hasRecieved)
        {
            hasRecieved = false;
            SendMusic(VRIP, VRPort);
        }

        if (adjustStart)
        {
            adjustStart = false;
            FindObjectOfType<MusicManager>().StartMusicForAdjust(adjustSec);
        }

        if (startDelay)
        {
            startDelay = false;
            FindObjectOfType<MusicManager>().StartMusicDelay(delaySec);
        }

        if (adjustEnd)
        {
            adjustEnd = false;
            FindObjectOfType<MusicManager>().EndAdjust(endAdjustSec);
        }

        if (musicStart)
        {
            musicStart = false;
            FindObjectOfType<MusicManager>().StartMusic(endAdjustSec);
        }

        // cetec
        if (NormalCetecStart)
        {
            GameObject.Find("StartManager").GetComponent<StartManager>().StartButtonPressed();
            FindObjectOfType<MusicManager>().SetAudioCetec();
            NormalCetecStart = false;
        }

        if (VibeCetecStart)
        {
            VibeCetecStart = false;
            WaitUntilDateTime(expectVibingTime, "vibe").Forget();
        }

        if (musicStartCetec)
        {
            musicStartCetec = false;
            WaitUntilDateTime(expectStartTime, "start").Forget();
        }

        if (musicEndCetec)
        {
            musicEndCetec = false;
            OnDestroy();
            FindObjectOfType<EXITManager>().EXITGame();
        }
    }

    private async UniTask OnProcess(CancellationToken cancellationToken)
    {
        try
        {
            // TcpListener を初期化
            m_tcpListener = new TcpListener(IPAddress.Any, m_port);
            m_tcpListener.Start();
            UnityEngine.Debug.Log("待機中");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // キャンセル可能な形でクライアント接続を非同期で待機
                    var acceptTask = m_tcpListener.AcceptTcpClientAsync();
                    await Task.WhenAny(acceptTask, Task.Delay(-1, cancellationToken)); // キャンセル可能なタスク待機

                    // キャンセルがリクエストされた場合、ループを終了
                    if (cancellationToken.IsCancellationRequested)
                    {
                        UnityEngine.Debug.Log("キャンセルリクエストが検出されました");
                        break;
                    }

                    if (acceptTask.IsCompletedSuccessfully)
                    {
                        m_tcpClient = acceptTask.Result;
                        UnityEngine.Debug.Log("接続完了");

                        // クライアントのストリームを取得
                        m_networkStream = m_tcpClient.GetStream();

                        // クライアントごとの処理
                        await HandleClient(m_networkStream, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // キャンセル処理時の特定メッセージ
                    UnityEngine.Debug.Log("非同期タスクがキャンセルされました");
                    break;
                }
                catch (ObjectDisposedException ex)
                {
                    // リスナーやクライアントが解放済みの場合の例外
                    UnityEngine.Debug.Log($"リソースが解放済みです: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    // その他のエラー処理
                    UnityEngine.Debug.LogError($"接続エラー: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"リスナーエラー: {ex.Message}");
        }
        finally
        {
            // 最後にリソースを確実に解放
            if (m_tcpListener != null)
            {
                m_tcpListener.Stop();
                UnityEngine.Debug.Log("リスナーを停止しました");
            }

            if (m_tcpClient != null)
            {
                m_tcpClient.Close();
                m_tcpClient.Dispose();
                UnityEngine.Debug.Log("クライアント接続を閉じました");
            }

            m_networkStream?.Close();
            m_networkStream?.Dispose();
            UnityEngine.Debug.Log("ネットワークストリームを閉じました");
        }
    }

    private async UniTask HandleClient(NetworkStream stream, CancellationToken cancellationToken)
    {
        print("待機中");
        var buffer = new byte[256];
        var messageBuffer = new StringBuilder();

        while (!cancellationToken.IsCancellationRequested && m_tcpClient.Connected)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead == 0)
                {
                    UnityEngine.Debug.Log("クライアント切断");
                    break;
                }

                var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                messageBuffer.Append(receivedData);
                ProcessMessage(receivedData);
                while (messageBuffer.ToString().Contains("\n"))
                {
                    int newlineIndex = messageBuffer.ToString().IndexOf("\n");
                    var fullMessage = messageBuffer.ToString(0, newlineIndex);
                    messageBuffer.Remove(0, newlineIndex + 1);
                    print(fullMessage);

                }
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Debug.Log("クライアント通信がキャンセルされました");
                break;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"通信エラー: {ex.Message}");
                break;
            }
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            UnityEngine.Debug.Log(message);
            var splitSlash = message.Split("/");
            switch (splitSlash[0])
            {
                case "Normal":
                    hasRecieved = true;
                    var newSplit = splitSlash[1].Split("B");
                    VRIP = newSplit[0];
                    VRPort = int.Parse(newSplit[1]);
                    break;

                case "Adjust":
                    hasRecieved = true;
                    adjustMode = true;
                    UnityEngine.Debug.Log("Have Sent the Adjustment info");
                    var AdjustSplit = splitSlash[1].Split("B");
                    VRIP = AdjustSplit[0];
                    VRPort = int.Parse(AdjustSplit[1]);
                    break;

                case "Time":
                    adjustStart = true;
                    adjustSec = int.Parse(splitSlash[1]);
                    break;

                case "DelayTime":
                    startDelay = true;
                    delaySec = float.Parse(splitSlash[1]);
                    break;

                case "EndAdjust":
                    adjustEnd = true;
                    endAdjustSec = float.Parse(splitSlash[1]);
                    break;

                case "StartMusic":
                    musicStart = true;
                    startMusicDelaySec = float.Parse(splitSlash[1]);
                    break;

                case "NormalCetec":
                    NormalCetecStart = true;
                    break;

                case "VibeCetec":
                    VibeCetecStart = true;
                    expectVibingTime = DateTime.Parse(splitSlash[1]);
                    break;

                case "StartMusicCetec":
                    musicStartCetec = true;
                    expectStartTime = DateTime.Parse(splitSlash[1]);
                    break;

                case "MusicEndCetec":
                    musicEndCetec = true;
                    break;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"メッセージ処理中にエラー: {ex.Message}");
        }
    }

    private void SendMusic(string VRIP, int VRPort)
    {
        this.GetComponent<TCPSender>().ConnectToVR(VRIP, VRPort, adjustMode);
    }

    private async UniTask WaitUntilDateTime(DateTime targetTime, string kind)
    {
        if (DateTime.Now >= targetTime)
            return;

        var waitTime = targetTime - DateTime.Now;
        if (kind == "vibe")
            waitTime -= TimeSpan.FromSeconds(0.08);
        else if (kind == "start")
            waitTime -= TimeSpan.FromSeconds(0.42);

        await UniTask.Delay(waitTime);
        if (kind == "vibe")
            osclistner.VibeCetec();
        else if (kind == "start")
            FindObjectOfType<MusicManager>().StartMusic(600);
    }

    public void Start()
    {
        osclistner = FindObjectOfType<OSCListener>();
    }
}
