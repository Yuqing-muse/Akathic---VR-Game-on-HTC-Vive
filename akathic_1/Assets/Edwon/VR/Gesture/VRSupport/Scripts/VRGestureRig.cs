using UnityEngine;
using System.Collections;
using System;
using Edwon.VR.Gesture;
using Edwon.VR.Input;

namespace Edwon.VR
{
    public class VRGestureRig : MonoBehaviour
    {

        //public VRRigAnchors vrRigAnchors;
        [SerializeField]
        public Transform head;
        [SerializeField]
        public Transform handLeft;
        [SerializeField]
        public Transform handRight;

        [SerializeField]
        GameObject leftController;
        [SerializeField]
        GameObject rightController;

        [SerializeField]
        public bool spawnControllerModels = false;

        [SerializeField]
        public GameObject handLeftModel;
        [SerializeField]
        public GameObject handRightModel;

        IInput inputLeft = null;
        IInput inputRight = null;

        void Awake()
        {
            CreateInputHelper();
        }

        public void AutoSetup()
        {
#if EDWON_VR_OCULUS
            if (GetComponent<OVRCameraRig>() != null)
            {
                OVRCameraRig ovrCameraRig = GetComponent<OVRCameraRig>();
                head = ovrCameraRig.centerEyeAnchor;
                handLeft = ovrCameraRig.leftHandAnchor;
                handRight = ovrCameraRig.rightHandAnchor;
            }
            else
            {
                Debug.Log(
                    "Could not setup OculusVR rig, is this script on the top level of your OVRCameraRig?\nDid you define EDWON_VR_OCULUS in Project Settings > Player ?"
                    );
            }
#endif
#if EDWON_VR_STEAM
            if (GetComponent<SteamVR_ControllerManager>() != null)
            {
                SteamVR_ControllerManager steamVRControllerManager = GetComponent<SteamVR_ControllerManager>();
                head = GetComponentInChildren<SteamVR_GameView>().transform;
                handLeft = steamVRControllerManager.left.transform;
                handRight = steamVRControllerManager.right.transform;
            }
            else
            {
                Debug.Log(
                    "Could not setup SteamVR rig, is this script on the top level of your SteamVR camera prefab?\nDid you define EDWON_VR_STEAM in Project Settings > Player ?"
                    );
            }
#endif
        }

        public Transform GetHand(HandType handedness)
        {
            if (handedness == HandType.Left)
            {
                return handLeft;
            }
            else
            {
                return handRight;
            }
        }

        //These are still expecting a proper HAND.
        //They will not find them because Inputs are not connected to HANDS.
        //These are now part of VR GestureManager maybe.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handedness"></param>
        /// <returns></returns>
        public IInput GetInput(HandType handedness)
        {
            if (handedness == HandType.Left)
            {
                return inputLeft;
            }
            else
            {
                return inputRight;
            }
        }

        /// <summary>
        /// This will check to see if an IInput interface is attached to the controller.
        /// If not it will attach the default VRControllerInput from EdwonVR
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public void CreateInputHelper()
        {
#if EDWON_VR_STEAM
            SteamVR_ControllerManager[] steamVR_cm = FindObjectsOfType<SteamVR_ControllerManager>();
            leftController = steamVR_cm[0].left;
            rightController = steamVR_cm[0].right;

            inputLeft = gameObject.AddComponent<VRControllerInputSteam>().Init(HandType.Left, leftController);
            inputRight = gameObject.AddComponent<VRControllerInputSteam>().Init(HandType.Right, rightController);
            if (spawnControllerModels)
                SpawnControllerModels();
#endif

#if EDWON_VR_OCULUS
            inputLeft = handLeft.gameObject.AddComponent<VRControllerInputOculus>().Init(HandType.Left);
            inputRight = handRight.gameObject.AddComponent<VRControllerInputOculus>().Init(HandType.Right);
            if (spawnControllerModels)
                SpawnControllerModels();
#endif
        }

        public void SpawnControllerModels ()
        {
            Transform leftModel = GameObject.Instantiate(handLeftModel).transform;
            Transform rightModel = GameObject.Instantiate(handRightModel).transform;
            leftModel.parent = handLeft;
            rightModel.parent = handRight;
            leftModel.localPosition = Vector3.zero;
            rightModel.localPosition = Vector3.zero;
            leftModel.localRotation = Quaternion.identity;
            rightModel.localRotation = Quaternion.identity;
        }
    }
}