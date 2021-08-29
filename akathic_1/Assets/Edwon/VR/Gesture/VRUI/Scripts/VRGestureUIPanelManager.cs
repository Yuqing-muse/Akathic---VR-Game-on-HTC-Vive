using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Edwon.VR.Gesture
{
    [RequireComponent(typeof(CanvasGroup))]
    // this script should be placed on the panels parent
    public class VRGestureUIPanelManager : MonoBehaviour
    {

        private string initialPanel = "Select Neural Net Menu";
        [HideInInspector]
        public string currentPanel;

        public delegate void PanelFocusChanged(string panelName);
        public static event PanelFocusChanged OnPanelFocusChanged;

        List<CanvasGroup> panels;

        [HideInInspector]
        public CanvasGroup canvasGroup;

        void Start()
        {
            // get the panels below me
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            panels = new List<CanvasGroup>();
            CanvasGroup[] panelsTemp = transform.GetComponentsInChildren<CanvasGroup>();
            for (int i = 0; i < panelsTemp.Length; i++)
            {
                if (panelsTemp[i] != canvasGroup)
                {
                    if (panelsTemp[i].transform.parent == transform)
                        panels.Add(panelsTemp[i]);
                }
            }

            if (VRGestureManager.Instance.stateInitial == VRGestureManagerState.ReadyToDetect)
            {
                initialPanel = "Detect Menu";
            }

            // initialize initial panel focused
            if (VRGestureManager.Instance.neuralNets.Count <= 0)
                FocusPanel("No Neural Net Menu");
            else
                FocusPanel(initialPanel);
        }

        public void FocusPanel(string panelName)
        {

            currentPanel = panelName;

            if (OnPanelFocusChanged != null)
            {
                OnPanelFocusChanged(panelName);
            }

            foreach (CanvasGroup panel in panels)
            {
                if (panel.gameObject.name == panelName)
                {
                    VRGestureUI.ToggleCanvasGroup(panel, true);
                }
                else
                {
                    VRGestureUI.ToggleCanvasGroup(panel, false);
                }
            }
        }

    }
}