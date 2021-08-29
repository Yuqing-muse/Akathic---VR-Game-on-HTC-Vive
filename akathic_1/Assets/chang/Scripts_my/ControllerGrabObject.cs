using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerGrabObject : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;


    private GameObject collidingObject;//用于保存当前与之碰撞的触发器（trigger），这样你才能抓住这个对象
    private GameObject objectInHand;//用于保存玩家当前抓住的对象

    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }


    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    //将 collidingObject 设为 null 以删除目标对象。
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }


    /// <summary>
    /// 如果玩家已经抓着某些东西了，或者这个对象没有一个刚性体，则不要将这个 GameObject 作为可以抓取目标;将这个对象作为可以抓取的目标。
    ///接受一个碰撞体作为参数，并将它的 GameObject 保存到 collidingObject 变量，以便抓住和释放这个对象
    /// </summary>
    /// <param name="col"></param>
    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }

    void Update()
    {
        if (device.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        if (device.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    //将手柄和手中正在拿取的物体联系在一起
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            //想象抛出物体时的情况，物体获得手柄当时的速度和角速度
            objectInHand.GetComponent<Rigidbody>().velocity = device.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = device.angularVelocity;
        }

        objectInHand = null;
    }
}
