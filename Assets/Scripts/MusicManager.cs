using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MP3Loding;
using Cysharp.Threading.Tasks;
using OscJack;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    string filePath;
    private AudioClip audioclip;
    private AudioSource thisAudioSource;
    [SerializeField] AudioClip testClip;
    // Start is called before the first frame update
    void Start()
    {
        thisAudioSource = this.GetComponent<AudioSource>();
        filePath = Path.Combine(Application.persistentDataPath, "new.csv");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetAudio(string _audiopath, bool isAdjust)
    {
        audioclip = Mp3Loader.LoadMp3(_audiopath);
        thisAudioSource.clip = audioclip;
        if (isAdjust) thisAudioSource.bypassEffects = true;
    }

    public void SetAudioCetec()
    {
        thisAudioSource.clip = testClip;
    }

    public async void StartMusic(float ms)
    {
        //var delaySec = GetFromCSV() + sec;
        //Debug.Log($"Waiting Until{sec}");
        //await UniTask.WaitUntil(() => DateTime.UtcNow.Second == delaySec);
        //Debug.Log("Play");
        thisAudioSource.Play();
    }
    public async void StartMusicForAdjust(int sec)
    {
        Debug.Log("waiting until:" + sec);
        await UniTask.WaitUntil(() => DateTime.UtcNow.Second == sec);
        thisAudioSource.Play();
    }
    public async void StartMusicDelay(float sec)
    {
        Debug.Log("Delay for:" + sec);
        thisAudioSource.Pause();
        await UniTask.Delay(TimeSpan.FromSeconds(sec));
        thisAudioSource.Play();
    }
    public void EndAdjust(float sec)
    {
        SaveCSV(sec.ToString());
        SceneManager.LoadScene("StartSelectScene");
    }


    private float GetFromCSV()
    {
        if (File.Exists(filePath))
        {
            // ファイルから全ての行を読み込む
            string[] lines = File.ReadAllLines(filePath);
            return float.Parse(lines[0]);
        }
        return 0;
    }
    void SaveCSV(string info)
    {
        // ファイルパスの設定

        // ファイルストリームとストリームライターの作成
        using (StreamWriter outStream = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            outStream.WriteLine(info);
        }

        Debug.Log("CSV saved at " + filePath);
    }


}
