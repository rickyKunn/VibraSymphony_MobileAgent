using UnityEngine;
using OscJack;
using RDG;
using Unity.VisualScripting;
using TMPro;
using System;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;


public class OSCListener : MonoBehaviour
{
    [SerializeField]
    private GameObject particle;

    [SerializeField]
    private bool Dbug;

    [SerializeField]
    private TextMeshProUGUI DebugFeild;

    [SerializeField]
    private int port;

    private int firstPortNum;

    [SerializeField]
    private GetIP IPManager;

    OscServer server;

    [SerializeField]
    private TextMeshProUGUI IPFeild;
    private bool StartMusic;
    private int vibeNum;
    private float power, sharp;
    private int vibeTime;
    public bool vibe, end;
    private CubeMoveManager CMM;
    void Start()
    {
        firstPortNum = port;
        bool portDetermined = false;
        CMM = FindObjectOfType<CubeMoveManager>();

        while (portDetermined == false)
        {
            try
            {
                server = new OscServer(port); // Port number
                portDetermined = true;
            }
            catch (Exception e)
            {
                print("error");
                port++;
                portDetermined = false;
            }
        }
        Invoke("IPCheck", 0.5f);
        CMM = FindObjectOfType<CubeMoveManager>();
        Vibration.Init();
        vibeTime = 200; //0.2ms;


        server.MessageDispatcher.AddCallback(
            "/Vibe", // OSC address
            (string address, OscDataHandle data) =>
            {
                power = data.GetElementAsFloat(0);
                vibe = true;
            }
        );
        server.MessageDispatcher.AddCallback(
        "/End", // OSC address
        (string address, OscDataHandle data) =>
        {
            end = true;
        }
        );
        server.MessageDispatcher.AddCallback("/StartMusic", // OSC address
        (string address, OscDataHandle data) =>
        {
            StartMusic = true;
        }
        );
    }

    private void IPCheck()
    {
        string IDNum = (port - firstPortNum).ToString();
        if (!IPManager.isNetworked)
        {
            IPFeild.text = "No Network!\nPlease Retry App!";
            return;
        }
        if (IDNum == "0")
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                IPFeild.text = "ID: B" + IPManager.IPLastString;

            }
            else
            {
                IPFeild.text = "ID: " + IPManager.IPLastString;

            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                IPFeild.text = "ID: B" + IDNum + "A" + IPManager.IPLastString;

            }
            else
            {
                IPFeild.text = "ID: " + IDNum + "A" + IPManager.IPLastString;
            }
        }
    }
    private void OnDestroy()
    {
        server?.Dispose();
    }
    private void Update()
    {
        if (vibe)
        {
            int intPower = (int)(power * 255);
            Vibe(intPower);
            vibe = false;
        }
        if (end)
        {
            server.Dispose();
            end = false;
        }
        if (StartMusic)
        {
            StartMusic = false;
            //FindObjectOfType<MusicManager>().StartMusic();
        }

    }

    //public void Received(float power)
    //{
    //    int intPower = (int)(power * 255);
    //    print("a");
    //    if (!Dbug)
    //    {
    //        if (vibeTime == 0)
    //        {
    //            Handheld.Vibrate();
    //        }
    //        else
    //        {

    //            vibration.Vibrate(vibeTime, intPower, true);
    //            CMM.SetCubeAnimation(intPower);
    //        }
    //    }
    //    var newOBJ = Instantiate(particle, new Vector3(0,0,-20), Quaternion.identity);
    //    Destroy(newOBJ, 4);
    //    CMM.SetCubeAnimation(intPower);
    //    vibeNum++;
    //    DebugFeild.text = $"{vibeNum}!!!{vibeTime}ms  power:{intPower}  ";
    //}

    private void Vibe(int power)
    {
        DebugFeild.text = $"{vibeNum}!!!  {power}   :   {sharp}";
        vibration.Vibrate(vibeTime, power, true);
        //var newOBJ = Instantiate(particle, new Vector3(0, 0, -20), Quaternion.identity);
        //Destroy(newOBJ, 4);
        CMM.SetCubeAnimation(power);
        vibeNum++;
    }
    public void VibeCetec()
    {
        vibration.Vibrate(200, 255, true);
        //var newOBJ = Instantiate(particle, new Vector3(0, 0, -20), Quaternion.identity);
        //Destroy(newOBJ, 4);
        CMM.SetCubeAnimation(power);
        vibeNum++;
    }
}

