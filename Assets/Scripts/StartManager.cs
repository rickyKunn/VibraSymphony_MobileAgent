using System.Collections;
using System.Collections.Generic;
using System.Net;
using RDG;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleTextObject;
    [SerializeField]
    private CubeMoveManager CMM;
    [SerializeField]
    private GameObject PickMusicButton;
    [SerializeField]
    private GameObject PickMusicPanel;
    private bool MoveCanvas;
    private bool buttonPressed;
    private void Start()
    {
        Vibration.Init();

    }
    private void Update()
    {
        if (MoveCanvas)
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                PickMusicButton.transform.position = Vector3.MoveTowards(PickMusicButton.transform.position, new Vector3(PickMusicButton.transform.position.x, PickMusicButton.transform.position.y, -70), 50 * Time.deltaTime);
                PickMusicPanel.transform.position = Vector3.MoveTowards(PickMusicPanel.transform.position, new Vector3(PickMusicPanel.transform.position.x, PickMusicPanel.transform.position.y, -150), 50 * Time.deltaTime);
            }
            titleTextObject.transform.position = Vector3.MoveTowards(titleTextObject.transform.position, new Vector3(titleTextObject.transform.position.x, titleTextObject.transform.position.y, -70), 50 * Time.deltaTime);
            //this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(this.transform.position.x, this.transform.position.y, -70), 50 * Time.deltaTime);
        }
    }
    public void StartButtonPressed()
    {
        //vibration.Vibrate(1, 1, true);

        MoveCanvas = true;
        CMM.CubeGoFront();
        Invoke("DestroyCanvas", 2);

    }

    public async void MainButtonPressed()
    {
        if (buttonPressed) return;
        buttonPressed = true;
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        SceneManager.LoadScene("MainScene");
    }

    public async void SubButtonPresseed()
    {
        if (buttonPressed) return;
        buttonPressed = true;
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        SceneManager.LoadScene("SubScene");
    }

    private void DestroyCanvas()
    {
        Destroy(titleTextObject);
        Destroy(this.gameObject);
    }

}
