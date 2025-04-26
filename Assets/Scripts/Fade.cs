using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Fade : MonoBehaviour
{
    private bool StartProcess;
    Image image;
    [SerializeField]
    private float timeSpan;
    [SerializeField]
    private GameObject StartButton;
    [SerializeField]
    private bool FadeIn;

    private float alpha; 
    private void Start()
    {
        image = this.GetComponent<Image>();
        if (FadeIn)
        {
            alpha = 1;
            image.color = new Color(0, 0, 0, 1);
            StartFadeIn();
        }
        else
        {
            alpha = 0;
            image.color = new Color(0, 0, 0, 0);
        }

    }

    private void Update()
    {
        if (!StartProcess) return;
        switch (FadeIn)
        {
            case true:
                if (alpha >= 0)
                {
                    alpha -= Time.deltaTime / timeSpan;
                }
                else
                {
                    if(StartButton != null)
                    {
                        StartButton.GetComponent<Button>().interactable = true;
                    }
                    Destroy(this.gameObject);
                }
                break;
            case false:
                if (alpha <= 1)
                {
                    alpha += Time.deltaTime / timeSpan;
                }
                break;

        }
        image.color = new Color(0,0,0,alpha);
    }
    public void StartFadeOut()
    {
        StartProcess = true;
    }
    private async void StartFadeIn()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        StartProcess = true;
    }
}
