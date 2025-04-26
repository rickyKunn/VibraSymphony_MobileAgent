using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class CubeMoveManager : MonoBehaviour
{
    private enum Phase
    {
        Waiting,
        MovingToFront,
        FinishedProcess,
    }


    Phase cubePhase = Phase.Waiting;
    private Animator m_Animator = null;
    private Vector3 rot;
    private float time;
    private bool GoFront;
    [SerializeField]
    private int MovingSpeedToFront;
    private float camSpeed;
    private GameObject Cam;
    private VisualEffect noteVfx;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        Cam = Camera.main.gameObject;
        noteVfx = transform.GetChild(0).GetComponent<VisualEffect>();
    }
    void Update()
    {

        time += Time.deltaTime * 0.5f;
        CubeRotate();
        if (cubePhase == Phase.MovingToFront)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, -30), MovingSpeedToFront * Time.deltaTime);
        }
    }

    void CubeRotate()
    {
        rot.x = Mathf.Cos(time) * 5;
        rot.y = 0.6f;
        rot.z = Mathf.Sin(time * 0.7f) * 3;

        transform.Rotate(rot * 6 * Time.deltaTime);
        if (cubePhase == Phase.FinishedProcess)
        {
            if (camSpeed <= 3)
            {
                camSpeed += Time.deltaTime * 1.5f;
            }
            Cam.transform.RotateAround(this.transform.position, Vector3.up, camSpeed * Time.deltaTime);
            //this.transform.position += this.transform.forward * Time.deltaTime;
        }
    }

    /// <summary>
    /// 最大値255(4に変換するよう計算)
    /// </summary>
    /// <param name="power"></param>
    public void SetCubeAnimation(float power)
    {
        //power = power * 4 / 255;
        //m_Animator.SetFloat("Power", power);
        //m_Animator.SetTrigger("Anime");
        noteVfx.SendEvent("OnPlay");
    }

    public void CubeGoFront()
    {
        cubePhase = Phase.MovingToFront;
        Invoke("PhaseChange", 100 / MovingSpeedToFront);
    }
    private void PhaseChange()
    {
        cubePhase = Phase.FinishedProcess;
        Invoke("ChangeBool", 0.5f);
    }
    private void ChangeBool()
    {
        FindObjectOfType<EXITManager>().StartFade();
    }
}
