using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Edwon.VR.Input;

namespace Edwon.VR.Gesture
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(VRControllerUIInput))]
    public class VRGestureUI : MonoBehaviour
    {
        VRGestureRig myAvatar;

        bool uiVisible;

        [HideInInspector]
        public HandType menuHandedness;
        VRGestureUIPanelManager panelManager;
        Transform vrMenuHand; // the hand to attach the hand ui to
        Transform vrHandUIPanel; // the actual ui
        Transform vrCam;
        VRGestureGallery vrGestureGallery;
        public float offsetZ;

        //public VRGestureManager VRGestureManagerInstance; // the VRGestureManager script we want to interact with
        public RectTransform mainMenu; // the top level transform of the main menu
        public RectTransform recordMenu; // the top level transform of the recordMenu where we will generate gesture buttons'
        public RectTransform editMenu; // the top level transform of the eidtMenu
        public RectTransform selectNeuralNetMenu; // the top level transform of the select neural net menu where we will generate buttons
        public RectTransform detectMenu;
        public GameObject buttonPrefab;

        // PARENT
        Canvas rootCanvas; // the canvas on the main VRGestureUI object

        // RECORD MENU
        private List<Button> gestureButtons;
        [Tooltip("the title of the gesture list on the record menu")]
        public CanvasRenderer recordListTitle;
        public CanvasRenderer editListTitle;
        public CanvasRenderer newGestureButton;

        // RECORDING MENU
        [Tooltip("the now recording indicator in the recording menu")]
        public Text nowRecordingLabel;
        public Image nowRecordingBackground;
        [Tooltip("the label that tells you what gesture your recording currently")]
        public Text nowRecordingGestureLabel;
        [Tooltip("the label that tells you how many examples you've recorded")]
        public Text nowRecordingTotalExamplesLabel;

        // EDITING MENU
        [Tooltip("the label that tells you what gesture your editing currently")]
        public Text nowEditingGestureLabel;
        [Tooltip("the button that begins delete gesture in the Recording Menu")]
        public Button deleteGestureButton;

        // DELETE CONFIRM MENU
        public Text deleteGestureConfirmLabel;
        [Tooltip("the button that actually deletes a gesture in the Delete Confirm Menu")]
        public Button deleteGestureConfirmButton;

        // SELECT NEURAL NET MENU
        [Tooltip("the panel of the Select Neural Net Menu")]
        public RectTransform neuralNetTitle;

        // DETECT MENU
        private Slider thresholdSlider;
        private Text thresholdLog;

        // TRAINING MENU
        [Tooltip("the text feedback for the currently training neural net")]
        public Text neuralNetTraining;

        // default settings
        private Vector3 buttonRectScale; // new Vector3(0.6666f, 1, 0.2f);

        void Start()
        {
            panelManager = GetComponentInChildren<VRGestureUIPanelManager>();

            menuHandedness = (VRGestureManager.Instance.gestureHand == HandType.Left)? HandType.Right : HandType.Left;

            rootCanvas = GetComponent<Canvas>();
            vrHandUIPanel = transform.Find("Panels");
            
            // start with hand UI visible
            uiVisible = true;
            ToggleCanvasGroup(panelManager.canvasGroup, uiVisible);

            vrGestureGallery = transform.GetComponentInChildren<VRGestureGallery>(true);

            buttonRectScale = new Vector3(0.6666f, 1, 0.2f);

            // get vr player hand and camera
            myAvatar = VRGestureManager.Instance.rig;
            vrMenuHand = myAvatar.GetHand(menuHandedness);
            vrCam = VRGestureManager.Instance.rig.head;
      
            GenerateRecordMenuButtons();
            GenerateEditMenuButtons();
            GenerateNeuralNetMenuButtons();

        }

        void Update()
        {
            // if press Button1 on menu hand toggle menu on off
            HandType oppositeHand = VRGestureManager.Instance.gestureHand == HandType.Left ? HandType.Right : HandType.Left;
            if (myAvatar.GetInput(oppositeHand) != null && myAvatar.GetInput(oppositeHand).GetButtonDown(InputOptions.Button.Button1))
                ToggleVRGestureUI();

            Vector3 handToCamVector = vrCam.position - vrMenuHand.position;
            vrHandUIPanel.position = vrMenuHand.position + (offsetZ * handToCamVector);
            if (-handToCamVector != Vector3.zero)
                vrHandUIPanel.rotation = Quaternion.LookRotation(-handToCamVector, Vector3.up);

            if(VRGestureManager.Instance.state == VRGestureManagerState.Detecting)
                UpdateDetectMenu();

            UpdateCurrentNeuralNetworkText();
            UpdateNowRecordingStatus();

        }

        // toggles this UI's visibility on/off
        public void ToggleVRGestureUI ()
        {
            uiVisible = !uiVisible;

            if (vrGestureGallery != null)
                ToggleCanvasGroup(vrGestureGallery.canvasGroup, uiVisible);

            if (vrHandUIPanel != null)
                ToggleCanvasGroup(vrHandUIPanel.GetComponent<CanvasGroup>(), uiVisible);
        }

        #region CALLED BY BUTTON METHODS

        // called every time main menu is entered
        public void BeginMainMenu()
        {
            // GET ALL THE BUTTONS IN MAIN MENU
            CanvasGroup recordButton = new CanvasGroup();
            CanvasGroup editButton = new CanvasGroup();
            CanvasGroup trainButton = new CanvasGroup();
            CanvasGroup detectButton = new CanvasGroup();
            CanvasGroup[] cgs = mainMenu.GetComponentsInChildren<CanvasGroup>();
            List<CanvasGroup> buttons = new List<CanvasGroup>();
            for (int i = 0; i < cgs.Length; i++)
            {
                if (cgs[i].name == "Record Button")
                {
                    recordButton = cgs[i];
                    buttons.Add(recordButton);
                }
                if (cgs[i].name == "Edit Button")
                {
                    editButton = cgs[i];
                    buttons.Add(editButton);
                }
                if (cgs[i].name == "Train Button")
                {
                    trainButton = cgs[i];
                    buttons.Add(trainButton);
                }
                if (cgs[i].name == "Detect Button")
                {
                    detectButton = cgs[i];
                    buttons.Add(detectButton);
                }
            }

            // DISABLE ALL BUTTONS FIRST
            foreach (CanvasGroup c in buttons)
            {
                c.interactable = false;
                c.blocksRaycasts = true;
                c.alpha = .5f;
            }

            // ENABLE BUTTONS DEPENDING ON TRAINING STATE
            // enable record button because always need it
            ToggleCanvasGroup(recordButton, true, 1f);

            if (VRGestureManager.Instance.gestureBank.Count > 0)
            {
                // some gestures recorded, show edit and train buttons
                ToggleCanvasGroup(editButton, true, 1f);
                ToggleCanvasGroup(trainButton, true, 1f);
            }
            if (VRGestureManager.Instance.Gestures.Count > 0)
            {
                // some gestures trained, show detect button
                ToggleCanvasGroup(detectButton, true, 1f);
            }
        }

        public void BeginDetectMenu()
        {
            RefreshDetectLogs("begin", true, 0, "");
        }

        // called when detect mode begins
        public void BeginDetectMode()
        {
            VRGestureManager.Instance.BeginDetect("");
        }

        // called when entering recording menu
        public void BeginRecordingMenu(string gestureName)
        {
            nowRecordingGestureLabel.text = gestureName;
            VRGestureManager.Instance.BeginReadyToRecord(gestureName);
            RefreshTotalExamplesLabel();
        }

        public void BeginEditGesture(string gestureName)
        {
            nowEditingGestureLabel.text = gestureName;
            deleteGestureButton.onClick.RemoveAllListeners();
            deleteGestureButton.onClick.AddListener(() => panelManager.FocusPanel("Delete Confirm Menu")); // go to confirm delete menu
            deleteGestureButton.onClick.AddListener(() => BeginDeleteConfirm(gestureName));
            VRGestureManager.Instance.BeginEditing(gestureName);
        }

        public void BeginDeleteConfirm(string gestureName)
        {
            deleteGestureConfirmLabel.text = gestureName;
            deleteGestureConfirmButton.onClick.RemoveAllListeners();
            deleteGestureConfirmButton.onClick.AddListener(() => DeleteGesture(gestureName));
            deleteGestureConfirmButton.onClick.AddListener(() => panelManager.FocusPanel("Edit Menu")); 
        }

        public void SelectNeuralNet(string neuralNetName)
        {
            VRGestureManager.Instance.SelectNeuralNet(neuralNetName);
        }

        public void BeginTraining()
        {
            panelManager.FocusPanel("Training Menu");
            neuralNetTraining.text = VRGestureManager.Instance.currentNeuralNet;
            VRGestureManager.Instance.BeginTraining(OnFinishedTraining);
        }

        public void QuitTraining()
        {
            VRGestureManager.Instance.EndTraining(OnQuitTraining);
        }

        void OnFinishedTraining(string neuralNetName)
        {
            StartCoroutine(TrainingMenuDelay(1f));
        }

        void OnQuitTraining(string neuralNetName)
        {
            StartCoroutine(TrainingMenuDelay(1f));
        }

        public void CreateGesture()
        {
            string newGestureName = "Gesture " + (VRGestureManager.Instance.gestureBank.Count + 1);
            VRGestureManager.Instance.CreateGesture(newGestureName);
            GenerateRecordMenuButtons();
        }

        public void DeleteGesture(string gestureName)
        {
            VRGestureManager.Instance.DeleteGesture(gestureName);
        }

        void UpdateDetectMenu()
        {
            if (thresholdSlider != null)
            {
                VRGestureManager.Instance.confidenceThreshold = thresholdSlider.value;
                thresholdSlider.value = (float)VRGestureManager.Instance.confidenceThreshold;
            }
            if (thresholdLog != null)
            {
                thresholdLog.text = VRGestureManager.Instance.confidenceThreshold.ToString("F3");
            }
        }

        IEnumerator RefreshDetectLogs(string gestureName, bool isNull, double confidence, string info)
        {
            float clearDelay = 1f;

            // get all the elements
            Image bigFeedbackImage = detectMenu.Find("Detect Big Feedback").GetComponent<Image>();
            Text bigFeedbackText = bigFeedbackImage.transform.GetChild(0).GetComponent<Text>();
            Text gestureLog = detectMenu.Find("Detect Log Gesture").GetChild(0).GetComponent<Text>();
            Text confidenceLog = detectMenu.Find("Detect Log Confidence").GetChild(0).GetComponent<Text>();
            Text infoLog = detectMenu.Find("Detect Log Info").GetChild(0).GetComponent<Text>();
            thresholdLog = detectMenu.Find("Detect Log Threshold").GetChild(0).GetComponent<Text>();
            thresholdSlider = detectMenu.Find("Threshold Slider").GetComponent<Slider>();

            // set the log text
            gestureLog.text = gestureName;
            confidenceLog.text = confidence.ToString("F3");
            infoLog.text = info;

            // set the big feedback
            if (isNull)
            {
                bigFeedbackImage.color = Color.red;
                bigFeedbackText.text = "REJECTED";
            }
            else
            {
                bigFeedbackImage.color = Color.green;
                bigFeedbackText.text = "DETECTED";
            }

            if (gestureName == "begin")
            {
                bigFeedbackImage.color = Color.white;
                bigFeedbackText.text = "";
            }

            // wait a second, then clear everything visually
            yield return new WaitForSeconds(clearDelay);
            bigFeedbackImage.color = Color.white;
            bigFeedbackText.text = "";

            yield return null;
        }

        IEnumerator TrainingMenuDelay(float delay)
        {
            // after training complete and a short delay go back to main menu
            yield return new WaitForSeconds(delay);
            panelManager.FocusPanel("Main Menu");
        }

        #endregion

        #region GENERATIVE BUTTONS

        void GenerateRecordMenuButtons()
        {
            GenerateGestureButtons(VRGestureManager.Instance.gestureBank, recordMenu.transform, GestureButtonsType.Record);

        }

        void GenerateEditMenuButtons()
        {
            GenerateGestureButtons(VRGestureManager.Instance.gestureBank, editMenu.transform, GestureButtonsType.Edit);
        }

        enum GestureButtonsType { Record, Edit };

        void GenerateGestureButtons(List<string> gesturesToGenerate, Transform buttonsParent, GestureButtonsType gestureButtonsType)
        {
            // first destroy the old gesture buttons if they are there
            if (gestureButtons != null)
            {
                if (gestureButtons.Count > 0)
                {
                    foreach (Button button in gestureButtons)
                    {
                        Destroy(button.gameObject);
                    }
                    gestureButtons.Clear();
                }
            }

            float gestureButtonHeight = 30;

            gestureButtons = GenerateButtonsFromList(gesturesToGenerate, buttonsParent, buttonPrefab, gestureButtonHeight);

            // set the functions that the button will call when pressed
            for (int i = 0; i < gestureButtons.Count; i++)
            {
                string gestureName = VRGestureManager.Instance.gestureBank[i];
                if (gestureButtonsType == GestureButtonsType.Record)
                {
                    gestureButtons[i].onClick.AddListener(() => BeginRecordingMenu(gestureName));
                    gestureButtons[i].onClick.AddListener(() => panelManager.FocusPanel("Recording Menu"));
                }
                else if (gestureButtonsType == GestureButtonsType.Edit)
                {
                    gestureButtons[i].onClick.AddListener(() => BeginEditGesture(gestureName));
                    gestureButtons[i].onClick.AddListener(() => panelManager.FocusPanel("Editing Menu"));
                }
            }

            if (gestureButtonsType == GestureButtonsType.Record)
                AdjustListTitlePosition(recordListTitle.transform, gestureButtons.Count, gestureButtonHeight);
            else if (gestureButtonsType == GestureButtonsType.Edit)
                AdjustListTitlePosition(editListTitle.transform, gestureButtons.Count, gestureButtonHeight);

            if (gestureButtonsType == GestureButtonsType.Record)
            {
                // adjust new gesture button position
                float totalHeight = gestureButtons.Count * gestureButtonHeight;
                float y = -(totalHeight / 2);
                newGestureButton.transform.localPosition = new Vector3(0, y, 0);
            }
        }

        void GenerateNeuralNetMenuButtons()
        {
            int neuralNetMenuButtonHeight = 30;

            List<Button> buttons = GenerateButtonsFromList(VRGestureManager.Instance.neuralNets, selectNeuralNetMenu.transform, buttonPrefab, neuralNetMenuButtonHeight);

            // set the functions that the button will call when pressed
            for (int i = 0; i < buttons.Count; i++)
            {
                string neuralNetName = VRGestureManager.Instance.neuralNets[i];
                buttons[i].onClick.AddListener(() => panelManager.FocusPanel("Main Menu"));
                buttons[i].onClick.AddListener(() => SelectNeuralNet(neuralNetName));
            }

            AdjustListTitlePosition(neuralNetTitle.transform, buttons.Count, neuralNetMenuButtonHeight);
        }

        List<Button> GenerateButtonsFromList(List<string> list, Transform parent, GameObject prefab, float buttonHeight)
        {
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < list.Count; i++)
            {
                // instantiate the button
                GameObject button = GameObject.Instantiate(prefab);
                button.transform.parent = parent;
                button.transform.localPosition = Vector3.zero;
                button.transform.localRotation = Quaternion.identity;
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                buttonRect.localScale = buttonRectScale;
                button.transform.name = list[i] + " Button";
                // set the button y position
                float totalHeight = list.Count * buttonHeight;
                float y = 0f;
                if (i == 0)
                {
                    y = totalHeight / 2;
                }
                y = (totalHeight / 2) - (i * buttonHeight);
                buttonRect.localPosition = new Vector3(0, y, 0);
                // set the button text
                Text buttonText = button.transform.GetComponentInChildren<Text>(true);
                buttonText.text = list[i];
                buttons.Add(button.GetComponent<Button>());
            }
            return buttons;
        }

        #endregion

        void AdjustListTitlePosition(Transform title, int totalButtons, float buttonHeight)
        {
            if (title != null)
            {
                float totalHeight = totalButtons * buttonHeight;
                float y = (totalHeight / 2) + buttonHeight;
                title.localPosition = new Vector3(0, y, 0);
            }
        }

        void OnEnable()
        {
            VRGestureManager.GestureDetectedEvent += OnGestureDetected;
            VRGestureManager.GestureRejectedEvent += OnGestureRejected;
            VRGestureUIPanelManager.OnPanelFocusChanged += PanelFocusChanged;
            VRControllerUIInput.OnVRGuiHitChanged += VRGuiHitChanged;
        }

        void OnDisable()
        {
            VRGestureManager.GestureDetectedEvent -= OnGestureDetected;
            VRGestureManager.GestureRejectedEvent -= OnGestureRejected;
            VRGestureUIPanelManager.OnPanelFocusChanged -= PanelFocusChanged;
            VRControllerUIInput.OnVRGuiHitChanged -= VRGuiHitChanged;
        }

        void OnGestureDetected (string gestureName, double confidence)
        {
            StartCoroutine(RefreshDetectLogs(gestureName, false, confidence, "Gesture Detected" ));
            //detectLog.text = gestureName + "\n" + confidence.ToString("F3");
        }

        void OnGestureRejected(string error, string gestureName = null, double confidence = 0)
        {
            StartCoroutine(RefreshDetectLogs(gestureName, true,confidence, error));
            //detectLog.text = "null" + "\n" + error;
        }

        void VRGuiHitChanged(bool hitBool)
        {
            if (hitBool)
            {
                if (VRGestureManager.Instance.state == VRGestureManagerState.ReadyToRecord)
                {
                    TogglePanelAlpha("Recording Menu", 1f);
                    TogglePanelInteractivity("Recording Menu", true);
                }
            }
            else if (!hitBool)
            {
                if (VRGestureManager.Instance.state == VRGestureManagerState.ReadyToRecord || VRGestureManager.Instance.state == VRGestureManagerState.Recording)
                {
                    TogglePanelAlpha("Recording Menu", .35f);
                    TogglePanelInteractivity("Recording Menu", false);
                }
            }
        }

        void TogglePanelAlpha(string panelName, float toAlpha)
        {
            CanvasRenderer[] canvasRenderers = vrHandUIPanel.GetComponentsInChildren<CanvasRenderer>();
            foreach (CanvasRenderer cr in canvasRenderers)
            {
                cr.SetAlpha(toAlpha);
            }
        }

        void TogglePanelInteractivity(string panelName, bool interactive)
        {
            Button[] buttons = vrHandUIPanel.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                button.interactable = interactive;
            }
        }

        void PanelFocusChanged(string panelName)
        {
            if (panelName == "Main Menu")
            {
                VRGestureManager.Instance.state = VRGestureManagerState.Idle;
                BeginMainMenu();
            }
            if (panelName == "Select Neural Net Menu")
            {
                VRGestureManager.Instance.RefreshNeuralNetList();
                VRGestureManager.Instance.state = VRGestureManagerState.Idle;
            }
            if (panelName == "Record Menu")
            {
                VRGestureManager.Instance.state = VRGestureManagerState.Idle;
                GenerateRecordMenuButtons();
            }
            if (panelName == "Recording Menu")
            {
                //vrGestureManager.state = VRGestureManagerState.ReadyToRecord;
                //Not sure why this is in here. This re-introduced the sticky button bug.
            }
            if (panelName == "Edit Menu")
            {
                VRGestureManager.Instance.state = VRGestureManagerState.Edit;
                GenerateEditMenuButtons();
            }
            if (panelName == "Editing Menu")
            {
                VRGestureManager.Instance.state = VRGestureManagerState.Editing;
            }
            if (panelName == "Delete Confirm Menu")
            {
                VRGestureManager.Instance.state = VRGestureManagerState.Editing;
            }
            if (panelName == "Detect Menu")
            {

            }
        }

        void UpdateCurrentNeuralNetworkText()
        {
            if (GetCurrentNeuralNetworkText() == null)
                return;

            Text title = GetCurrentNeuralNetworkText();
            title.text = VRGestureManager.Instance.currentNeuralNet;
        }

        void UpdateNowRecordingStatus()
        {

            if (VRGestureManager.Instance.state == VRGestureManagerState.ReadyToRecord
                || VRGestureManager.Instance.state == VRGestureManagerState.EnteringRecord)
            {
                nowRecordingBackground.color = Color.grey;
                nowRecordingLabel.text = "ready to record";
            }
            else if (VRGestureManager.Instance.state == VRGestureManagerState.Recording)
            {
                nowRecordingBackground.color = Color.red;
                nowRecordingLabel.text = "RECORDING";
            }
            // update gesture example count in UI if gesture just finished recording
            if (VRGestureManager.Instance.state != VRGestureManager.Instance.stateLast)
            {
                if (VRGestureManager.Instance.stateLast == VRGestureManagerState.Recording)
                {
                    RefreshTotalExamplesLabel();
                }
            }
        }

        // refresh the label that says how many examples recorded
        void RefreshTotalExamplesLabel ()
        {
            string gesture = VRGestureManager.Instance.gestureToRecord;
            int totalExamples = Utils.Instance.GetGestureExamplesTotal(gesture);
            nowRecordingTotalExamplesLabel.text = totalExamples.ToString();
        }

        Text GetCurrentNeuralNetworkText()
        {
            // update current neural network name on each currentNeuralNetworkTitle UI thingy
            if (panelManager == null)
                return null;
            if (vrHandUIPanel == null)
                return null;
            if (vrHandUIPanel.Find(panelManager.currentPanel) == null)
                return null;
            Transform currentPanelParent = vrHandUIPanel.Find(panelManager.currentPanel);
            if (currentPanelParent == null)
                return null;
            Transform currentNeuralNetworkTitle = currentPanelParent.Find("Current Neural Network");
            if (currentNeuralNetworkTitle == null)
                return null;

            Text title = currentNeuralNetworkTitle.Find("neural network name").GetComponent<Text>();
            return title;
        }

        public static void ToggleCanvasGroup(CanvasGroup cg, bool on)
        {
            if (on)
            {
                // turn panel on
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                // turn panel off
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

        public static void ToggleCanvasGroup(CanvasGroup cg, bool on, float alpha)
        {
            if (on)
            {
                // turn panel on
                cg.alpha = alpha;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                // turn panel off
                cg.alpha = alpha;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }
}
