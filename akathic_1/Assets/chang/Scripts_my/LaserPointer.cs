using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LaserPointer : MonoBehaviour
{
    public Transform cameraRigTransform;// [CameraRig] 的 transform 组件
    public Transform headTransform; 
    public Vector3 teleportReticleOffset; //标记距离地板的偏移，以防止和其他平面发生“z-缓冲冲突”
    public LayerMask teleportMask; //层遮罩，用于过滤这个地方允许什么东西传送

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab; 
    private GameObject laser; 
    private Transform laserTransform;

    public GameObject teleportReticlePrefab; //对传送标记预制件的引用
    private GameObject reticle; //传送标记实例的引用
    private Transform teleportReticleTransform;
    private Vector3 hitPoint; //激光击中的位置
    private bool shouldTeleport; //如果为 true，表明找到一个有效的传送点。

    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

 
    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
    }

    void Update()
    {
        //按下touchpad
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            //teleportMask确保激光只能点到你能够传送过去的 GameObjects 上
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;

                ShowLaser(hit);

                
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;//添加一个偏移以免 z 缓冲冲突

                shouldTeleport = true;
            }
        }
        else 
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); 
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        shouldTeleport = false; 
        reticle.SetActive(false); 
        Vector3 difference = cameraRigTransform.position - headTransform.position; 
        difference.y = 0; 

        cameraRigTransform.position = hitPoint + difference; 
    }
}
