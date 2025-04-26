using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NoteAnimationManager;
public class MusicBoxManager : MonoBehaviour
{
    private string thisMusicName;
    private int thisMusicNum;
    private TCPSender TSender;
    private string thisMusicPath;
    private NoteAnimationManager NAM;
    private bool willAnimate;
    private GameObject Note;
    private Image NoteImage;
    private TextMeshProUGUI MusicNameText;
    private TextMeshProUGUI MusicNumText;
    private void Start()
    {
        TSender = FindObjectOfType<TCPSender>();
        NAM = FindObjectOfType<NoteAnimationManager>();
        NAM.MBMsList.Add(new MBMs(this, thisMusicNum));
        MusicNumText = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        MusicNameText = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        NoteImage = this.transform.GetChild(2).GetComponent<Image>();

    }

    private void Update()
    {
    }

    public void SetThisBox(string _name, int _num, string _path)
    {
        thisMusicName = _name;
        thisMusicNum = _num;
        thisMusicPath = _path;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _num.ToString();
        this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _name;
    }

    public void ThisButtonPressed()
    {
        TSender.path = thisMusicPath;
        NAM.PickedMusicChanged(thisMusicNum, thisMusicName);
    }
    public void SetPressedButton()
    {
        MusicNumText.color = new Color(0.5f, 0.75f, 1);
        MusicNameText.color = new Color(0.5f, 0.75f, 1);
        NoteImage.color = new Color(0.5f, 0.75f, 1);
    }
    public void SetUnPressedButton()
    {
        MusicNumText.color = new Color(0.72f, 1, 0.95f);
        MusicNameText.color = new Color(0.72f, 1, 0.95f);
        NoteImage.color = new Color(0.72f, 1, 0.95f);
    }

}
