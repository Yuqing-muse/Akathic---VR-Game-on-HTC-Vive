using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using simpleExampleForAI;
public class AIRT_FY : MonoBehaviour
{
    [HideInInspector]
    public AIControl controlMachine;
    // Use this for initialization
    private DragonAnim drag_anim;
    public Material dyingmat;
    private bool isDied;
    public database db;
    public int bossid;
    private float timer;
    public float allTime;
    public GameObject Player;
    public GameObject mr;
    void Start()
    {
        drag_anim = new DragonAnim();
        drag_anim.animator = GetComponent<Animator>();
        controlMachine = new simpleControl();
        controlMachine.ai_this = this.gameObject;
        controlMachine.player_this = Player;
        controlMachine.animation_control = drag_anim;
        controlMachine.Init();
        if (db.fileExist)
        {
            if (db.md.bossDied[bossid])
            {
                Destroy(this.gameObject);
            }
        }

    }

    void OnTriggerEnter(Collider c)
    {
        //controlMachine.CollideChecker.OnTriggerEnter (c);
    }
    // Update is called once per frame
    void Update()
    {
        if (controlMachine.aiState.tempDebuff == AIDebuff.FIRE)
            Debug.Log("FIRE");
        controlMachine.Update();
        if (!isDied)
        {
            if (controlMachine.aiState.isDied)
            {
                dying();
            }
        }
        else
        {
            timer += Time.deltaTime;
            mr.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_MeltScale", timer / allTime);
            if (timer > allTime)
                Destroy(this.gameObject);
        }
    }

    public void dying()
    {
        isDied = true;
        db.changeBoss(bossid);
        mr.GetComponent<SkinnedMeshRenderer>().material = dyingmat;
    }
}
