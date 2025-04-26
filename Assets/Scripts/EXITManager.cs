using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EXITManager : MonoBehaviour
{
    Image image;
    // private TextMeshProUGUI EXITText;
    [SerializeField]
    private float timeSpan;
    [SerializeField]
    private GameObject EXITButton;
    [SerializeField]
    private GameObject PickedMusicPanel;
    private float alpha = 0;
    public bool fade;
    private bool PMPFade;
    private TextMeshProUGUI PMPNumText;
    private TextMeshProUGUI PMPNameText;
    private Image PMPImage;
    private void Start()
    {
        EXITButton = this.gameObject;
        image = this.GetComponent<Image>();
        image.color = new Color(0.72f, 1, 0.95f, 0);
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            PMPNumText = PickedMusicPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            PMPNameText = PickedMusicPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            PMPImage = PickedMusicPanel.transform.GetChild(2).GetComponent<Image>();
        }
    }
    private void Update()
    {
        
        if (!fade) return;
        if (alpha <= 1)
        {
            alpha += Time.deltaTime / timeSpan;
        }
        else
        {
            EXITButton.GetComponent<Button>().interactable = true;
            fade = false;
        }

        image.color = new Color(0.72f, 1, 0.95f, alpha);
        if (SceneManager.GetActiveScene().name == "MainScene" && PMPFade)
        {
            PMPNumText.color = new Color(0.72f, 1, 0.95f, alpha);
            PMPNameText.color = new Color(0.72f, 1, 0.95f, alpha);
            PMPImage.color = new Color(0.72f, 1, 0.95f, alpha);
        }
    }
    public void StartFade()
    {
        if (SceneManager.GetActiveScene().name == "MainScene" && PickedMusicPanel.activeSelf == false)
        {
            PMPNumText.color = new Color(0.72f, 1, 0.95f, alpha);
            PMPNameText.color = new Color(0.72f, 1, 0.95f, alpha);
            PMPImage.color = new Color(0.72f, 1, 0.95f, alpha);
            PickedMusicPanel.SetActive(true);
            PMPFade = true;
        }
        fade = true;
        
    }
    public void EXITGame()
    {
        SceneManager.LoadScene("StartSelectScene");
    }
}
