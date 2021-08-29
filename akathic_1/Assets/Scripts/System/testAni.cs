using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]  
//此动画1.7秒  
public class testAni : MonoBehaviour
{

    Animator animator;
    const float kDuration = 1.7f;
    bool init = false;
    // Use this for initialization  
    IEnumerator Start()
    {
        init = true;
        Debug.Log("start");
        animator = GetComponent<Animator>();
        const float frameRate = 30f;
        const int frameCount = (int)((kDuration * frameRate) + 2);
        animator.Rebind();
        animator.StopPlayback();
        animator.recorderStartTime = 0;

        // 开始记录指定的帧数  
        animator.StartRecording(frameCount);

        for (var i = 0; i < frameCount - 1; i++)
        {
            // 记录每一帧  
            animator.Update(1.0f / frameRate);
        }
        animator.speed = 0;
        yield return new WaitForEndOfFrame();
        animator.speed = 1;
        // 完成记录  
        animator.StopRecording();
        Debug.LogFormat("{0},{1}", animator.recorderStartTime, animator.recorderStopTime);

        // 开启回放模式  
        animator.StartPlayback();
    }
    float m_CurTime = 0f;
    void Update()
    {
        if (!init)
        {
            Start();
        }
        int i = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            i = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            i = 1;
        }
        //Debug.Log(m_CurTime);  
        animator.playbackTime = m_CurTime;
        animator.Update(0);
        m_CurTime += (1 / 70f) * i;
        if (m_CurTime > 1.7f)
        {
            m_CurTime = 0;
        }
        if (m_CurTime < 0)
        {
            m_CurTime = 1.7f;
        }
    }
}
