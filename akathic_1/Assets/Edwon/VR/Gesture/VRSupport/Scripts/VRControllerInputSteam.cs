#if EDWON_VR_STEAM

using UnityEngine;
using System.Collections;
using Edwon.VR.Gesture;

namespace Edwon.VR.Input
{
    public class VRControllerInputSteam : VRController
    {
        [Header("SteamVR Options")]
        public int deviceIndex;
        bool properlySetIndex = false;
        public GameObject _hand;

        public IInput Init(HandType handy, GameObject hand)
        {
            handedness = handy;
            _hand = hand;
            //gameObject.SetActive(true);
            StartCoroutine(RegisterIndex());
            return this;
        }

        IEnumerator RegisterIndex()
        {
            //yield return new WaitForSeconds(.2f);
            while (true)
            {
                deviceIndex = (int)_hand.GetComponent<SteamVR_TrackedObject>().index;
                if (deviceIndex > -1)
                {
                    //Debug.Log("I just got registered. Index: " + deviceIndex);
                    yield break;
                }
                yield return null;
            }
        }

        void OnDestroy()
        {
            //Debug.Log("I am being destroyed");
        }

        void LateUpdate()
        {
            //Only attempt this if Device Index != -1
            if (deviceIndex > -1)
            {
                directional1 = SteamVR_Controller.Input(deviceIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

                button1 = SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.Touchpad);
                button1Down = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
                button2 = SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu);
                button2Down = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);
                trigger1Button = SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.Trigger);
                trigger1ButtonDown = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
                trigger1ButtonUp = SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
                trigger2Button = SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.Grip);
                trigger2ButtonDown = SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Grip);
                trigger2ButtonUp = SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Grip);
            }

        }
    }
}

#endif