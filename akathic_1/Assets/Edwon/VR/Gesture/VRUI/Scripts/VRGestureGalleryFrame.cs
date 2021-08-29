using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Edwon.VR.Gesture
{
    public class VRGestureGalleryFrame : MonoBehaviour
    {

        public CanvasGroup trash;

        public void OnPointerEnter()
        {
            VRGestureUI.ToggleCanvasGroup(trash, false, 1);
        }

        public void OnPointerExit()
        {
            VRGestureUI.ToggleCanvasGroup(trash, false, 0);
        }

    }
}