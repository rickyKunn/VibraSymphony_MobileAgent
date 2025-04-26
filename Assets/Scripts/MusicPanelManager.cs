using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MusicsListPanel;
    private GameObject canvas;
    [SerializeField]
    private GameObject PickMusicBox;
    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    public void PickButtonPressed()
    {
        MusicsListPanel.SetActive(true);
        this.gameObject.SetActive(false);
        PickMusicBox.SetActive(false);

    }
}
