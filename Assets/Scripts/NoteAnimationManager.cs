using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteAnimationManager : MonoBehaviour
{
    public List<MBMs> MBMsList = new List<MBMs>();
    [SerializeField]
    private GameObject pickedMusicBox;
    public struct MBMs
    {

        public MBMs(MusicBoxManager _MBM,int _num)
        {
            MBM = _MBM;
            num = _num;
        }
        public MusicBoxManager MBM;
        public int num;
    }
    public void PickedMusicChanged(int _num,string _name)
    {
        FindObjectOfType<TCPSender>().hasSelectedMusic = true;
        pickedMusicBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _num.ToString();
        pickedMusicBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _name;
        for (int i = 0;i < MBMsList.Count; i++)
        {
            if (MBMsList[i].num == _num)
            {
                MBMsList[i].MBM.SetPressedButton();
            }
            else
            {
                MBMsList[i].MBM.SetUnPressedButton();
            }

        }
    }
}
