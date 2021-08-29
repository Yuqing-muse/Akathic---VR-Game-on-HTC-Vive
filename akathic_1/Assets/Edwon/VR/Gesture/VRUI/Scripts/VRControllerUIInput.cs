/*
The MIT License(MIT) only covers this script named VRControllerUIInput that is based on the ViveUGUIModule script
created by VREALITY and is posted on github at https://github.com/VREALITY/ViveUGUIModule

Copyright(c) 2015 VREAL INC.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Edwon.VR.Input;

namespace Edwon.VR.Gesture
{
    [RequireComponent(typeof(VRGestureUI))]
    public class VRControllerUIInput : BaseInputModule
    {
        public static VRControllerUIInput Instance;

        [Header(" [Cursor setup]")]
        public Sprite CursorSprite;
        public Material CursorMaterial;
        public float CursorSize = 50f;
        public float NormalCursorScale = 0.00025f;

        [Space(10)]

        [Header(" [Runtime variables]")]
        [Tooltip("Indicates whether or not the gui was hit by any controller this frame")]
        public bool GuiHit;
        private bool GuiHitLast;

        [Tooltip("Indicates whether or not a button was used this frame")]
        public bool ButtonUsed;

        [Tooltip("Generated cursors")]
        public RectTransform[] Cursors;

        private GameObject[] CurrentPoint;
        private GameObject[] CurrentPressed;
        private GameObject[] CurrentDragging;

        private PointerEventData[] PointEvents;

        private bool Initialized = false;

        [Tooltip("Generated non rendering camera (used for raycasting ui)")]
        public Camera ControllerCamera;

        private VRGestureRig rig;
        private IInput ControllerInputLeft;
        private IInput ControllerInputRight;
        private Transform[] Controllers;

        void SetInputModule()
        {
            MethodInfo changeEventModuleMethod = eventSystem.GetType().GetMethod("ChangeEventModule", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(BaseInputModule) }, null);
            changeEventModuleMethod.Invoke(eventSystem, new object[] { this });
            eventSystem.UpdateModules();
        }

        protected override void Start()
        {
            base.Start();

            SetInputModule();
            rig = VRGestureManager.Instance.rig;
            ControllerInputLeft = rig.GetInput(HandType.Left);
            ControllerInputRight = rig.GetInput(HandType.Right);

            if (Initialized == false)
            {
                Instance = this;

                ControllerCamera = new GameObject("Controller UI Camera").AddComponent<Camera>();
                ControllerCamera.transform.parent = this.transform;
                ControllerCamera.clearFlags = CameraClearFlags.Nothing; //CameraClearFlags.Depth;
                ControllerCamera.cullingMask = 0; // 1 << LayerMask.NameToLayer("UI"); 
                ControllerCamera.nearClipPlane = 0.0001f;

                Controllers = new Transform[] { rig.handLeft, rig.handRight };
                Cursors = new RectTransform[2];                

                for (int index = 0; index < Cursors.Length; index++)
                {
                    GameObject cursor = new GameObject("Cursor " + index);
                    Canvas canvas = cursor.AddComponent<Canvas>();
                    //cursor.transform.parent = this.transform;
                    cursor.AddComponent<CanvasRenderer>();
                    cursor.AddComponent<CanvasScaler>();
                    cursor.AddComponent<UIIgnoreRaycast>();
                    cursor.AddComponent<GraphicRaycaster>();

                    // set cursor size
                    cursor.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CursorSize);
                    cursor.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CursorSize);

                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.sortingOrder = 1000; //set to be on top of everything

                    Image image = cursor.AddComponent<Image>();
                    image.sprite = CursorSprite;
                    image.material = CursorMaterial;


                    if (CursorSprite == null)
                        Debug.LogError("Set CursorSprite on " + this.gameObject.name + " to the sprite you want to use as your cursor.", this.gameObject);

                    Cursors[index] = cursor.GetComponent<RectTransform>();
                }

                CurrentPoint = new GameObject[Cursors.Length];
                CurrentPressed = new GameObject[Cursors.Length];
                CurrentDragging = new GameObject[Cursors.Length];
                PointEvents = new PointerEventData[Cursors.Length];

                Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in canvases)
                {
                    canvas.worldCamera = ControllerCamera;
                }

                Initialized = true;
            }
        }

        // use screen midpoint as locked pointer location, enabling look location to be the "mouse"
        private bool GetLookPointerEventData(int index)
        {
            if (PointEvents[index] == null)
                PointEvents[index] = new PointerEventData(base.eventSystem);
            else
                PointEvents[index].Reset();

            PointEvents[index].delta = Vector2.zero;
            PointEvents[index].position = new Vector2(Screen.width / 2, Screen.height / 2);
            PointEvents[index].scrollDelta = Vector2.zero;

            base.eventSystem.RaycastAll(PointEvents[index], m_RaycastResultCache);
            PointEvents[index].pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
            {
                GuiHit = true; //gets set to false at the beginning of the process event
            }

            m_RaycastResultCache.Clear();

            return true;
        }

        // update the cursor location and whether it is enabled
        // this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
        private void UpdateCursor(int index, PointerEventData pointData)
        {
            if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
            {
                Cursors[index].gameObject.SetActive(true);

                if (pointData.pointerEnter != null)
                {
                    RectTransform draggingPlane = pointData.pointerEnter.GetComponent<RectTransform>();
                    Vector3 globalLookPos;
                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, pointData.position, pointData.enterEventCamera, out globalLookPos))
                    {
                        Cursors[index].position = globalLookPos;
                        Cursors[index].rotation = draggingPlane.rotation;

                        // scale cursor based on distance to camera
                        float lookPointDistance = (Cursors[index].position - Camera.main.transform.position).magnitude;
                        float cursorScale = lookPointDistance * NormalCursorScale;
                        if (cursorScale < NormalCursorScale)
                        {
                            cursorScale = NormalCursorScale;
                        }

                        Cursors[index].localScale = Vector3.one * cursorScale;
                    }
                }
            }
            else
            {
                Cursors[index].gameObject.SetActive(false);
            }
        }

        // clear the current selection
        public void ClearSelection()
        {
            if (base.eventSystem.currentSelectedGameObject)
            {
                base.eventSystem.SetSelectedGameObject(null);
            }
        }

        // select a game object
        private void Select(GameObject go)
        {
            ClearSelection();

            if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
            {
                base.eventSystem.SetSelectedGameObject(go);
            }
        }

        // send update event to selected object
        // needed for InputField to receive keyboard input
        private bool SendUpdateEventToSelectedObject()
        {
            if (base.eventSystem.currentSelectedGameObject == null)
                return false;

            BaseEventData data = GetBaseEventData();

            ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);

            return data.used;
        }

        private void UpdateCameraPosition(int index)
        {
            ControllerCamera.transform.position = Controllers[index].transform.position;
            ControllerCamera.transform.forward = Controllers[index].transform.forward;
        }

        // Process is called by UI system to process events
        public override void Process()
        {
            //Debug.Log("PROCESS IS RUNNING");

            GuiHit = false;
            ButtonUsed = false;

            // send update events if there is a selected object - this is important for InputField to receive keyboard events
            SendUpdateEventToSelectedObject();

            // see if there is a UI element that is currently being looked at
            for (int index = 0; index < Cursors.Length; index++)
            {
                if (Controllers[index].gameObject.activeInHierarchy == false)
                {
                    if (Cursors[index].gameObject.activeInHierarchy == true)
                    {
                        Cursors[index].gameObject.SetActive(false);
                    }
                    continue;
                }

                UpdateCameraPosition(index);

                bool hit = GetLookPointerEventData(index);

                if (hit == false)
                    continue;

                CurrentPoint[index] = PointEvents[index].pointerCurrentRaycast.gameObject;

                // handle enter and exit events (highlight)
                base.HandlePointerExitAndEnter(PointEvents[index], CurrentPoint[index]);

                // update cursor
                UpdateCursor(index, PointEvents[index]);

                if (Controllers[index] != null)
                {
                    if (ButtonDown(index))
                    {
                        ClearSelection();

                        PointEvents[index].pressPosition = PointEvents[index].position;
                        PointEvents[index].pointerPressRaycast = PointEvents[index].pointerCurrentRaycast;
                        PointEvents[index].pointerPress = null;

                        if (CurrentPoint[index] != null)
                        {
                            CurrentPressed[index] = CurrentPoint[index];

                            GameObject newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerDownHandler);

                            if (newPressed == null)
                            {
                                // some UI elements might only have click handler and not pointer down handler
                                newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerClickHandler);
                                if (newPressed != null)
                                {
                                    CurrentPressed[index] = newPressed;
                                }
                            }
                            else
                            {
                                CurrentPressed[index] = newPressed;
                                // we want to do click on button down at same time, unlike regular mouse processing
                                // which does click when mouse goes up over same object it went down on
                                // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                                ExecuteEvents.Execute(newPressed, PointEvents[index], ExecuteEvents.pointerClickHandler);
                            }

                            if (newPressed != null)
                            {
                                PointEvents[index].pointerPress = newPressed;
                                CurrentPressed[index] = newPressed;
                                Select(CurrentPressed[index]);
                                ButtonUsed = true;
                            }

                            ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.beginDragHandler);
                            PointEvents[index].pointerDrag = CurrentPressed[index];
                            CurrentDragging[index] = CurrentPressed[index];
                        }
                    }

                    if (ButtonUp(index))
                    {
                        if (CurrentDragging[index])
                        {
                            ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.endDragHandler);
                            if (CurrentPoint[index] != null)
                            {
                                ExecuteEvents.ExecuteHierarchy(CurrentPoint[index], PointEvents[index], ExecuteEvents.dropHandler);
                            }
                            PointEvents[index].pointerDrag = null;
                            CurrentDragging[index] = null;
                        }
                        if (CurrentPressed[index])
                        {
                            ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerUpHandler);
                            PointEvents[index].rawPointerPress = null;
                            PointEvents[index].pointerPress = null;
                            CurrentPressed[index] = null;
                        }
                    }

                    // drag handling
                    if (CurrentDragging[index] != null)
                    {
                        ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.dragHandler);
                    }
                }
            }

            if (GuiHitLast != GuiHit)
                OnVRGuiHitChanged(GuiHit);

            GuiHitLast = GuiHit;
        }

        public delegate void VRGuiHitChanged(bool guiHitBool);
        public static event VRGuiHitChanged OnVRGuiHitChanged;

        public bool ButtonDown(int index)
        {
            //Debug.Log("Left Trigger Down is: " + tester + "Right Trigger Down is: " + tester2);
            if (index == 0)
            {
                bool pressed = false;
                if (ControllerInputLeft != null)
                {
                    pressed = ControllerInputLeft.GetButtonDown(InputOptions.Button.Trigger1);
                    if (pressed)
                    {
                        //Debug.Log("Left Trigger " + pressed);
                    }
                }
                //This is actually the RIGHT trigger
                return pressed;
            }
            else{
                bool pressed = false;
                if (ControllerInputRight != null)
                {
                    pressed = ControllerInputRight.GetButtonDown(InputOptions.Button.Trigger1);
                    if (pressed)
                    {
                        //Debug.Log("Right Trigger " + pressed);
                    }
                }
                return pressed;
            }
        }

        /// <summary>
        /// This is checking the button up for an INDEX. left or right. 0 or 1?
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ButtonUp(int index)
        {
            if (index == 0)
            {
                bool pressed = false;
                if (ControllerInputLeft != null)
                {
                    pressed =ControllerInputLeft.GetButtonUp(InputOptions.Button.Trigger1);
                }
                return pressed;
            }
                    
            else
            {
                bool pressed = false;
                if(ControllerInputRight != null)
                {
                    pressed =ControllerInputRight.GetButtonUp(InputOptions.Button.Trigger1);
                }
                return pressed;
            }
        }
    }
}