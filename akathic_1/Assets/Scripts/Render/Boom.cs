using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour
{
    [SerializeField]
    GameObject[] BoomCube;
    Vector3 centerpos;
    float MAXDISTANCE = 0.0f;
    [SerializeField]
    float BoomForce;
    [SerializeField]
    float BoomUp;
    [SerializeField]
    float BoomTime;

    public int hp;

    public Animator animator;
    // Use this for initialization
    void Start()
    {
        //BoomCube = this.gameObject.transform.
        for (int i = 0; i < BoomCube.Length; i++)
        {
            centerpos += BoomCube[i].transform.transform.position;
        }
        centerpos /= BoomCube.Length;
        for (int i = 0; i < BoomCube.Length; i++)
        {
            if (Vector3.SqrMagnitude(BoomCube[i].transform.position - centerpos) > MAXDISTANCE)
            {
                MAXDISTANCE = Vector3.SqrMagnitude(BoomCube[i].transform.position - centerpos);
            }
        }
       // Boom__ ();
        //	BoomCube[0].GetComponent<Rigidbody>().

    }
    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Weapon") 
        {
            hp -= 25;
            animator.SetBool("IsAttack",true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hp <= 0) 
        {
            Boom__();
        }
    }

    public void Boom__()
    {
        animator.enabled = false;
        for (int i = 0; i < BoomCube.Length; i++)
        {
            BoomCube[i].GetComponent<Rigidbody>().AddExplosionForce(BoomForce, centerpos, MAXDISTANCE, BoomUp);
            BoomCube[i].GetComponent<Rigidbody>().useGravity = true;
            BoomCube[i].GetComponent<MeshCollider>().enabled = true;
        }
        Invoke("Destroy__", BoomTime);
    }
    void Destroy__()
    {
        for (int i = 0; i < BoomCube.Length; i++)
        {
            Destroy(BoomCube[i]);
        }
        this.gameObject.SetActive(false);
    }
}
