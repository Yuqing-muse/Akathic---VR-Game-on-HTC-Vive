using UnityEngine;
using System.Collections;
using VRTK;
using Valve.VR;

public class VRInput : MonoBehaviour
{

	[HideInInspector]
	public Vector3 rightVelocity;
	[HideInInspector]
	public Vector3 leftVelocity;
	[HideInInspector]
	public Vector3 rightPos;
	[HideInInspector]
	public Vector3 leftPos;
	[HideInInspector]
	public Vector3 topPos;
	[HideInInspector]
	public bool TouchPanelDown;
	//[HideInInspector]
	//public bool TouchPanelUp;
	[HideInInspector]
	public bool gripDown;
    [HideInInspector]
    public bool TriggerDown_left;
    [HideInInspector]
    public bool gripDown_left;
    [HideInInspector]
    public bool TriggerDown;
	// Use this for initialization
	private SteamVR_TrackedObject trackedObj;
	public GameObject right;
	public GameObject left;
	public GameObject hmd;
	private SteamVR_TrackedObject hmdTrackedObject ;//ͷ��

	public VRTK_ControllerEvents vrtk_right;
	public VRTK_ControllerEvents vrtk_left;

	void Start()
	{


	}

	private void Update()
	{

		//�ҵ�ͷ�����豸
		if (hmdTrackedObject == null)
		{
			SteamVR_TrackedObject[] trackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();
			foreach (SteamVR_TrackedObject tracked in trackedObjects)
			{
				if (tracked.index == SteamVR_TrackedObject.EIndex.Hmd)
				{
					hmdTrackedObject = tracked;
					break;
				}
			}
		}

		if (hmdTrackedObject)
		{

		}
	}
	void LateUpdate()
	{
		var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);//����
		var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);//����
		//var deviceIndex0 = hmdTrackedObject.index;//ͷ��

		rightVelocity = SteamVR_Controller.Input(deviceIndex1).velocity;
		//Debug.Log(rightVelocity);
		leftVelocity = SteamVR_Controller.Input(deviceIndex2).velocity;
		//Debug.Log(leftVelocity);

		rightPos = right.transform.position;
		leftPos = left.transform.position;
		topPos = hmd.transform.position;
		//Debug.Log(leftPos);
		//���´���
		if (vrtk_right.touchpadPressed)
		{
		//	Debug.Log("������ ��Touchpad�� �� ��");
			TouchPanelDown = true;
		}
		else
			TouchPanelDown = false;



		if (vrtk_right.gripPressed)//û�а����׾Ϳ���ʶ��
		{//�Ƿ���Ҫ�ᰴ������ȵ� GetHairTrigger/GetHairTriggerDown/GetHairTriggerUp
		//	Debug.Log("��press������ ��Grip�� �� ��");
			gripDown = true;
		}
		else
			gripDown = false;
        if (vrtk_left.gripPressed)
        {
            gripDown_left = true;
        }
        else 
        {
            gripDown_left = false;
        }
        if (vrtk_left.triggerClicked)
        {
            TriggerDown_left = true;
        }
        else 
        {
            TriggerDown_left = false;
        }
        if (vrtk_right.triggerClicked)
        {
            TriggerDown = true;
        }
        else
        {
            TriggerDown = false;
        }
	}
}