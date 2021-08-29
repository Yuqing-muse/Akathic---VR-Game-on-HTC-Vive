using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Edwon.VR.Input;

namespace Edwon.VR.Gesture
{

    public enum VRGestureManagerState { Idle, Edit, Editing, EnteringRecord, ReadyToRecord, Recording, Training, EnteringDetect, ReadyToDetect, Detecting };
    public enum VRGestureDetectType { Button, Continious };

    public class VRGestureManager : MonoBehaviour
    {
        #region SINGLETON
        static VRGestureManager instance;

        public static VRGestureManager Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = FindObjectOfType<VRGestureManager>();
                    VRGestureManager[] instances = FindObjectsOfType<VRGestureManager>();
                    if (instances.Length == 1)
                    {
                        instance = instances[0];
                    }
                    else if (instances.Length == 0)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        instance = obj.AddComponent<VRGestureManager>();
                    }
                    else
                    {
                        Debug.LogError("There are too many VRGestureManagers added to your scene. VRGestureManager behaves as a signleton. Please remove any extra VRGestureManager components.");
                    }

                    instance.Init();
                }
                return instance;
            }
        }
        #endregion

        #region SETTINGS VARIABLES
        // which hand to track
        [SerializeField]
        [Tooltip("which hand to track using the gesture")]
        public HandType gestureHand = HandType.Right; // the hand to track
        [Tooltip("display default gesture trails")]
        public bool displayGestureTrail = true;
        [Tooltip("the button that triggers gesture recognition")]
        public InputOptions.Button gestureButton = InputOptions.Button.Trigger1;
        [Tooltip("the threshold over wich a gesture is considered correctly classified")]
        public double confidenceThreshold = 0.98;
        [Tooltip("Your gesture must have one axis longer than this length in world size")]
        public float minimumGestureAxisLength = 0.10f;
        [Tooltip("use this option for builds when you don't want users to see the VR UI from this plugin")]
        public bool beginInDetectMode = false;
        // whether to track when pressing trigger or all the time
        // continious mode is not supported yet
        // though you're welcome to try it out
        [HideInInspector]
        public VRGestureDetectType vrGestureDetectType;
        #endregion

        #region STATE VARIABLES
        public VRGestureManagerState state;
        [SerializeField]
        public VRGestureManagerState stateInitial;
        [SerializeField]
        public VRGestureManagerState stateLast;

        public bool readyToTrain
        {
            get
            {
                if (gestureBank != null)
                {
                    if (gestureBank.Count > 0)
                    {
                        foreach (int total in gestureBankTotalExamples)
                        {
                            if (total <= 0)
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
        }

        #endregion

        #region AVATAR VARIABLES
        public VRGestureRig rig;
        IInput input;
        Transform playerHead;
        Transform playerHand;
        Transform perpTransform;
        #endregion

        #region LINE CAPTURE VARIABLES
        GestureTrail myTrail;
        List<Vector3> currentCapturedLine;
        public string gestureToRecord;

        float nextRenderTime = 0;
        float renderRateLimit = Config.CAPTURE_RATE;
        float nextTestTime = 0;
        float testRateLimit = 500;
        #endregion

        #region NEURAL NET VARIABLES

        [Tooltip("the neural net that I am using")]
        [SerializeField]
        public string currentNeuralNet;
        [SerializeField]
        public string lastNeuralNet; // used to know when to refresh gesture bank
        [SerializeField]
        public List<string> neuralNets;
        private List<string> gestures;  // list of gestures already trained in currentNeuralNet
        public List<string> Gestures
        {
            get
            {
                NeuralNetworkStub stub = Utils.Instance.ReadNeuralNetworkStub(currentNeuralNet);
                return stub.gestures;
            }
            set
            {
                value = gestures;
            }
        }
        public List<string> gestureBank; // list of recorded gesture for current neural net
        public List<int> gestureBankTotalExamples;

        Trainer currentTrainer;
        GestureRecognizer currentRecognizer;

        #endregion

        #region EVENTS VARIABLES
        public delegate void GestureDetected(string gestureName, double confidence);
        public static event GestureDetected GestureDetectedEvent;
        public delegate void GestureRejected(string error, string gestureName = null, double confidence = 0);
        public static event GestureRejected GestureRejectedEvent;
        public delegate void StartCapture();
        public static event StartCapture StartCaptureEvent;
        public delegate void ContinueCapture(Vector3 capturePoint);
        public static event ContinueCapture ContinueCaptureEvent;
        public delegate void StopCapture();
        public static event StopCapture StopCaptureEvent;

        #endregion

        #region DEBUG VARIABLES
        public string debugString;
        #endregion

        #region INITIALIZE

        public virtual void Awake()
        {

            DontDestroyOnLoad(this.gameObject);
            if (instance == null)
            {
                instance = this;
                instance.Init();
            }

        }

        void Init()
        {
            if (FindObjectOfType<VRGestureRig>() != null)
            {
                rig = FindObjectOfType<VRGestureRig>();
                playerHead = rig.head;
                playerHand = rig.GetHand(gestureHand);
                input = rig.GetInput(gestureHand);
            }
        }

        void Start()
        {
            if (stateInitial == VRGestureManagerState.ReadyToDetect)
            {
                BeginDetect("");
            }
            else if (FindObjectOfType<VRGestureUI>() == null)
            {
                Debug.LogError("Cannot find VRGestureUI in scene. Please add it or select Begin In Detect Mode in the VR Gesture Manager Settings");
            }
                

            state = stateInitial;
            stateLast = state;
            gestureToRecord = "";

            input = rig.GetInput(gestureHand);

            //create a new Trainer
            currentTrainer = new Trainer(Gestures, currentNeuralNet);

            currentCapturedLine = new List<Vector3>();
            if (displayGestureTrail)
            {
                myTrail = gameObject.AddComponent<GestureTrail>();
            }
            

            perpTransform = new GameObject("Perpindicular Head").transform;
            perpTransform.parent = this.transform;
        }

		#endregion
			
		#region LINE CAPTURE



        public void LineCaught(List<Vector3> capturedLine)
        {
            if (state == VRGestureManagerState.Recording || state == VRGestureManagerState.ReadyToRecord)
            {

                TrainLine(gestureToRecord, capturedLine);
            }
            else if (state == VRGestureManagerState.Detecting || state == VRGestureManagerState.ReadyToDetect)
            {
                RecognizeLine(capturedLine);
            }
        }

        public void TrainLine(string gesture, List<Vector3> capturedLine)
        {

            currentTrainer.AddGestureToTrainingExamples(gesture, capturedLine);
            debugString = "trained : " + gesture;
        }

        public void RecognizeLine(List<Vector3> capturedLine)
        {
            if (IsGestureBigEnough(capturedLine))
            {
                //Detect if the captured line meets minimum gesture size requirements
                double[] networkInput = Utils.Instance.FormatLine(capturedLine);
                string gesture = currentRecognizer.GetGesture(networkInput);
                string confidenceValue = currentRecognizer.currentConfidenceValue.ToString("N3");

                // broadcast gesture detected event
                if (currentRecognizer.currentConfidenceValue > VRGestureManager.Instance.confidenceThreshold)
                {
                    debugString = gesture + " " + confidenceValue;
                    if (GestureDetectedEvent != null)
                        GestureDetectedEvent(gesture, currentRecognizer.currentConfidenceValue);
                }
                else
                {
                    debugString = "Null \n" + gesture + " " + confidenceValue;
                    if (GestureRejectedEvent != null)
                        GestureRejectedEvent("Confidence Too Low", gesture, currentRecognizer.currentConfidenceValue);
                }
            }
            else
            {
                //broadcast that a gesture is too small??
                debugString = "Gesture is too small!";
                if (GestureRejectedEvent != null)
                    GestureRejectedEvent("Gesture is too small");
                
            }
        }

        public bool IsGestureBigEnough(List<Vector3> capturedLine)
        {
            float check = Utils.Instance.FindMaxAxis(capturedLine);
            return (check > minimumGestureAxisLength);
        }
		

		//This will get points in relation to a users head.
		public Vector3 getLocalizedPoint(Vector3 myDumbPoint)
		{
			perpTransform.position = playerHead.position;
			perpTransform.rotation = Quaternion.Euler(0, playerHead.eulerAngles.y, 0);
			return perpTransform.InverseTransformPoint(myDumbPoint);
		}



		#endregion

		#region UPDATE
        // Update is called once per frame
        void Update()
        {
            stateLast = state;
            //get the position from the left anchor.
            //draw a point.
            if (rig != null)
            {
                if (state == VRGestureManagerState.ReadyToRecord ||
                    state == VRGestureManagerState.EnteringRecord ||
                    state == VRGestureManagerState.Recording)
                {
                    UpdateRecord();
                }
                else if (state == VRGestureManagerState.Detecting ||
                         state == VRGestureManagerState.EnteringDetect ||
                            state == VRGestureManagerState.ReadyToDetect)
                {
                    if (VRGestureManager.Instance.vrGestureDetectType == VRGestureDetectType.Continious)
                    {
                        UpdateContinual();
                    }
                    else
                    {
                        UpdateDetectWithButtons();
                    }
                }
            }
        }

        void UpdateRecord()
        {
            if (input.GetButtonUp(gestureButton))
            {
                state = VRGestureManagerState.ReadyToRecord;
                StopRecording();
            }

            if (input.GetButtonDown(gestureButton) && state == VRGestureManagerState.ReadyToRecord)
            {
                state = VRGestureManagerState.Recording;
                StartRecording();
            }

            if (state == VRGestureManagerState.Recording)
            {
                CapturePoint();
            }
        }

        void UpdateDetectWithButtons()
        {
            if (input.GetButtonUp(gestureButton))
            {
                state = VRGestureManagerState.ReadyToDetect;
                StopRecording();
            }

            if (input.GetButtonDown(gestureButton) && state == VRGestureManagerState.ReadyToDetect)
            {
                state = VRGestureManagerState.Detecting;
                StartRecording();
            }

            if (state == VRGestureManagerState.Detecting)
            {
                CapturePoint();
            }
        }

        void StartRecording()
        {
            nextRenderTime = Time.time + renderRateLimit / 1000;
            if (StartCaptureEvent != null)
                StartCaptureEvent();
            CapturePoint();
            

        }

        void CapturePoint()
        {
            Vector3 rightHandPoint = playerHand.position;
            Vector3 localizedPoint = getLocalizedPoint(rightHandPoint);
            currentCapturedLine.Add(localizedPoint);
            if(ContinueCaptureEvent != null)
                ContinueCaptureEvent(rightHandPoint);
        }

        void StopRecording()
        {

            if (currentCapturedLine.Count > 0)
            {
                LineCaught(currentCapturedLine);
                currentCapturedLine.RemoveRange(0, currentCapturedLine.Count);
                currentCapturedLine.Clear();

                if(StopCaptureEvent != null)
                    StopCaptureEvent();
            }

        }

        /// <summary>
        /// This is an experimental form of gesture detection. It will always attempt to
        /// detect a gesture while it is running. This would allow a user to constantly 
        /// be inputting various gesture in a more natural way.
        /// </summary>
        void UpdateContinual()
        {
            //		state = VRGestureManagerState.Detecting;
            if (Time.time > nextRenderTime)
            {
                Vector3 rightHandPoint = playerHand.position;

                nextRenderTime = Time.time + renderRateLimit / 1000;
                //myTrail.CapturePoint(rightHandPoint, rightCapturedLine, lengthOfLineRenderer);
                //IF currentCapturedLine is length greater than renderRateLimit v testRateLimit
                //  30 / 1000 = every 0.03 seconds
                // 100 / 1000 = every 0.10 seconds this will have only logged 3 points of data.
                // 500 / 1000 = every 0.5 second this will always have 16 points of data. 
                int maxLineLength = (int)testRateLimit / (int)renderRateLimit;
                myTrail.CapturePoint(getLocalizedPoint(rightHandPoint), currentCapturedLine, maxLineLength);
            }
            //myTrail.RenderTrail(rightLineRenderer, rightCapturedLine);

            //On Release
            //@TODO: fix this magic number 14.
            if (Time.time > nextTestTime && currentCapturedLine.Count > 14)
            {
                nextTestTime = Time.time + testRateLimit / 1000;
                LineCaught(currentCapturedLine);
                //currentRenderer.SetVertexCount(currentCapturedLine.Count);
                //currentRenderer.SetPositions(currentCapturedLine.ToArray());
            }


        }

		#endregion

		#region HIGH LEVEL METHODS

        //This should be called directly from UIController via instance
        public void BeginReadyToRecord(string gesture)
        {
            currentTrainer = new Trainer(gestureBank, currentNeuralNet);
            gestureToRecord = gesture;
            state = VRGestureManagerState.EnteringRecord;
        }

        public void BeginEditing(string gesture)
        {
            gestureToRecord = gesture;
        }

        public void BeginDetect(string ignoreThisString)
        {
            gestureToRecord = "";
            state = VRGestureManagerState.EnteringDetect;
            currentRecognizer = new GestureRecognizer(currentNeuralNet);
        }

        [ExecuteInEditMode]
        public void BeginTraining(Action<string> callback)
        {
            state = VRGestureManagerState.Training;
            currentTrainer = new Trainer(gestureBank, currentNeuralNet);
            currentTrainer.TrainRecognizer();
            // finish training
            state = VRGestureManagerState.Idle;
            callback(currentNeuralNet);
        }

        [ExecuteInEditMode]
        public void EndTraining(Action<string> callback)
        {
            state = VRGestureManagerState.Idle;
            callback(currentNeuralNet);
        }

        [ExecuteInEditMode]
        public bool CheckForDuplicateNeuralNetName(string neuralNetName)
        {
            // if neuralNetName already exists return true
            if (neuralNets.Contains(neuralNetName))
                return true;
            else
                return false;
        }

        [ExecuteInEditMode]
        public void CreateNewNeuralNet(string neuralNetName)
        {
            // create new neural net folder
            Utils.Instance.CreateFolder(neuralNetName);
            // create a gestures folder
            Utils.Instance.CreateFolder(neuralNetName + "/Gestures/");

            neuralNets.Add(neuralNetName);
            gestures = new List<string>();
            gestureBank = new List<string>();
            gestureBankPreEdit = new List<string>();
            gestureBankTotalExamples = new List<int>();

            // select the new neural net
            SelectNeuralNet(neuralNetName);
        }

        [ExecuteInEditMode]
        public void RefreshNeuralNetList()
        {
            neuralNets = new List<string>();
            string path = Config.SAVE_FILE_PATH;
            foreach (string directoryPath in System.IO.Directory.GetDirectories(path))
            {
                string directoryName = Path.GetFileName(directoryPath);
                if (!neuralNets.Contains(directoryName))
                {
                    neuralNets.Add(directoryName);
                }
            }
        }

        [ExecuteInEditMode]
        public void RefreshGestureBank(bool checkNeuralNetChanged)
        {
            if (checkNeuralNetChanged)
            {
                if (currentNeuralNet == lastNeuralNet)
                {
                    return;
                }
            }
            if (currentNeuralNet != null && currentNeuralNet != "" && Utils.Instance.GetGestureBank(currentNeuralNet) != null)
            {
                gestureBank = Utils.Instance.GetGestureBank(currentNeuralNet);
                gestureBankPreEdit = new List<string>(gestureBank);
                gestureBankTotalExamples = Utils.Instance.GetGestureBankTotalExamples(gestureBank);
            }
            else
            {
                gestureBank = new List<string>();
                gestureBankPreEdit = new List<string>();
                gestureBankTotalExamples = new List<int>();
            }
        }

        [ExecuteInEditMode]
        public void DeleteNeuralNet(string neuralNetName)
        {
            // get this neural nets index so we know which net to select next
            int deletedNetIndex = neuralNets.IndexOf(neuralNetName);

            // delete the net and gestures
            neuralNets.Remove(neuralNetName); // remove from list
            gestureBank.Clear(); // clear the gestures list
            gestureBankPreEdit.Clear();
			gestureBankTotalExamples.Clear();
            Utils.Instance.DeleteNeuralNetFiles(neuralNetName); // delete all the files

            if (neuralNets.Count > 0)
                SelectNeuralNet(neuralNets[0]);
        }

        [ExecuteInEditMode]
        public void SelectNeuralNet(string neuralNetName)
        {
            lastNeuralNet = currentNeuralNet;
            currentNeuralNet = neuralNetName;
            RefreshGestureBank(true);
        }

        [ExecuteInEditMode]
        public void CreateGesture(string gestureName)
        {
            gestureBank.Add(gestureName);
			gestureBankTotalExamples.Add(0);
            Utils.Instance.CreateGestureFile(gestureName, currentNeuralNet);
            gestureBankPreEdit = new List<string>(gestureBank);
        }

        [ExecuteInEditMode]
        public void DeleteGesture(string gestureName)
        {
			int index = gestureBank.IndexOf(gestureName);
            gestureBank.Remove(gestureName);
			gestureBankTotalExamples.RemoveAt(index);
            Utils.Instance.DeleteGestureFile(gestureName, currentNeuralNet);
            gestureBankPreEdit = new List<string>(gestureBank);
        }
			
        List<string> gestureBankPreEdit;

        bool CheckForDuplicateGestures(string newName)
        {
            bool dupeCheck = true;
            int dupeCount = -1;
            foreach (string gesture in gestureBank)
            {
                if (newName == gesture)
                {
                    dupeCount++;
                }
            }
            if (dupeCount > 0)
            {
                dupeCheck = false;
            }

            return dupeCheck;
        }

		#endregion


#if UNITY_EDITOR
        [ExecuteInEditMode]
        public VRGestureManagerEditor.VRGestureRenameState RenameGesture(int gestureIndex)
        {
            //check to make sure the name has actually changed.
            string newName = gestureBank[gestureIndex];
            string oldName = gestureBankPreEdit[gestureIndex];
            VRGestureManagerEditor.VRGestureRenameState renameState = VRGestureManagerEditor.VRGestureRenameState.Good;

            if (oldName != newName)
            {
                if (CheckForDuplicateGestures(newName))
                {
                    //ACTUALLY RENAME THAT SHIZZ
                    Utils.Instance.RenameGestureFile(oldName, newName, currentNeuralNet);
                    gestureBankPreEdit = new List<string>(gestureBank);

                }
                else
                {
                    //reset gestureBank
                    gestureBank = new List<string>(gestureBankPreEdit);
                    renameState = VRGestureManagerEditor.VRGestureRenameState.Duplicate;
                }
            }
            else
            {
                renameState = VRGestureManagerEditor.VRGestureRenameState.NoChange;
            }

            return renameState;
        }
#endif

    }
}
