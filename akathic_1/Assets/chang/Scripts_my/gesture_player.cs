using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.VR.Input;

namespace Edwon.VR.Gesture.Examples
{
    public class gesture_player : MonoBehaviour
    {
        //定义出的四种魔法手势  喷火 喷冰 拉近 使敌人飞天

        public GameObject fire;
        public GameObject air;
        public GameObject ice;

        VRGestureRig rig;
        IInput input;

        Transform playerHead;
        Transform playerHandL;
        Transform playerHandR;

        public BattleSystem battlesystem;
        void Start()
        {
            rig = VRGestureManager.Instance.rig;

            playerHead = rig.head;
            playerHandR = rig.handRight;
            playerHandL = rig.handLeft;

            input = rig.GetInput(VRGestureManager.Instance.gestureHand);
        }

        void Update()
        {

        }

        void OnEnable()
        {
            VRGestureManager.GestureDetectedEvent += OnGestureDetected;

        }

        void OnDisable()
        {
            VRGestureManager.GestureDetectedEvent -= OnGestureDetected;

        }

        void OnGestureDetected(string gestureName, double confidence)
        {
            //string confidenceString = confidence.ToString().Substring(0, 4);
            //Debug.Log("detected gesture: " + gestureName + " with confidence: " + confidenceString);

            switch (gestureName)
            {
                case "Circle":
                    DoIce();//有寒冰喷射出来
                    break;
                case "Triangle":
                    DoFire();
                    break;
                case "Line_ping":
                    DoPull();
                    break;
                case "Rainbow":
                    DoAir();
                    break;
            }
        }

        //喷出火来
        void DoFire()
        {
            RaycastHit hit;
            Vector3 fwd = playerHead.transform.position;
            if (Physics.Raycast(playerHead.transform.position, fwd, out hit, 20)) 
            {
                if(hit.collider.tag=="AI")
                {
                    battlesystem.attack("player",hit.collider.gameObject,20);
                }
            }
            Quaternion rotation = Quaternion.LookRotation(playerHandR.forward, Vector3.up);
            Vector3 betweenHandsPos = (playerHandL.position + playerHandR.position) / 2;
            GameObject fireInstance = GameObject.Instantiate(fire, betweenHandsPos, rotation) as GameObject;//关于火的粒子效果实例
            StartCoroutine(IEDoFire(fireInstance));//开启一个关于释放火的协程
        }

        IEnumerator IEDoFire(GameObject fireInstance)
        {
            yield return new WaitForSeconds(.1f);
            fireInstance.GetComponent<Collider>().enabled = true;
        }


        //有寒冰喷射出来
        void DoIce()
        {
            RaycastHit hit;
            Vector3 fwd = playerHead.transform.position;
            if (Physics.Raycast(playerHead.transform.position, fwd, out hit, 20))
            {
                if (hit.collider.tag == "AI")
                {
                    battlesystem.attack("player", hit.collider.gameObject, 20);
                }
            }
            GameObject.Instantiate(ice, playerHandR.position, playerHandR.rotation);
        }

        //将小怪拉过来  
        ///只对小怪有作用（？？？
        void DoPull()
        {
            //让物体在布娃娃状态下被拖拽过来
            float pullForce = -300f;
            float floorY = 2.65f;
            Vector3 earthSpawnPosition = new Vector3(playerHead.position.x, floorY, playerHead.position.z);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("AI");
            foreach (GameObject enemy in enemies)
            {

                if (enemy.GetComponent<Ragdoll>() != null)
                {//已经有布娃娃组件的情况
                    Ragdoll ragdoll = enemy.GetComponent<Ragdoll>();
                    ragdoll.TriggerWarning();
                    foreach (Rigidbody rb in ragdoll.myParts)
                    {//获取布娃娃中各个部分的刚体
                        rb.AddExplosionForce(pullForce, earthSpawnPosition, 100000f);
                    }
                }

                else if (enemy.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = enemy.GetComponent<Rigidbody>();
                    rb.AddExplosionForce(pullForce, earthSpawnPosition, 100000f);
                }
            }
        }


        //让小怪飞天
        void DoAir()
        {
            float explosionForce = 6f;

            Ray headRay = new Ray(playerHead.position, playerHead.forward);
            float sphereCastRadius = 1f;
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(headRay, sphereCastRadius);
            int hitCounter = 0;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("AI"))
                {

                    Transform enemy = hit.collider.transform;
                    if (hitCounter == 0)
                    {
                        Vector3 airSpawnPosition = enemy.transform.position;
                        GameObject.Instantiate(air, airSpawnPosition, Quaternion.identity);
                    }

                    hitCounter += 1;

                    if (enemy.GetComponent<Ragdoll>() != null)
                    {
                        Ragdoll ragdoll = enemy.GetComponent<Ragdoll>();
                        ragdoll.TriggerWarning();

                        foreach (Rigidbody rb in ragdoll.myParts)
                        {

                            rb.AddForce(new Vector3(.3f, explosionForce * 2, .1f), ForceMode.Impulse);
                            StartCoroutine(IEDoAir(rb));

                            //rb.useGravity = true;
                            rb.AddForce(new Vector3(0, -100, 0), ForceMode.Impulse);
                        }
                    }
                    else if (enemy.GetComponent<Rigidbody>() != null)
                    {

                        Rigidbody rb = enemy.GetComponent<Rigidbody>();
                        rb.AddForce(new Vector3(.3f, explosionForce, .1f), ForceMode.Impulse);
                        StartCoroutine(IEDoAir(rb));
                        // rb.useGravity = true;

                        rb.AddForce(new Vector3(0, -100, 0), ForceMode.Impulse);
                    }

                }
            }

        }

        IEnumerator IEDoAir(Rigidbody rb)
        {
            yield return new WaitForSeconds(1);
            rb.useGravity = false;

        }
        //没有手势输入的情况
        IEnumerator AnimateShape(GameObject shape)
        {
            yield return null;
        }

    }
}
