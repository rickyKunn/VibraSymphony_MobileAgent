using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicListExitButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MusicListPanel;
    [SerializeField]
    private GameObject PickMusicButton;
    [SerializeField]
    private GameObject PickMusicBox;
    public void ExitButtonPressed()
    {
        MusicListPanel.SetActive(false);
        PickMusicButton.SetActive(true);
        PickMusicBox.SetActive(true);
    }
}
