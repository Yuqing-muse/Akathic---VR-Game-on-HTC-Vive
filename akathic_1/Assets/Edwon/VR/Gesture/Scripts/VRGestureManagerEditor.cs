#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace Edwon.VR.Gesture
{
    [CustomEditor(typeof(VRGestureManager)), CanEditMultipleObjects]
    public class VRGestureManagerEditor : Editor
    {
        static VRGestureManager vrGestureManager;

        // neural net gui helpers
        int selectedNeuralNetIndex = 0;
        string newNeuralNetName;

        public enum VRGestureRenameState { Good, NoChange, Duplicate };
        string selectedFocus = "";

        #region GUI CONTENT VARIABLES

        public enum EditorListOption
        {
            None = 0,
            ListSize = 1,
            ListLabel = 2,
            ElementLabels = 4,
            Buttons = 8,
            Default = ListSize | ListLabel | ElementLabels,
            NoElementLabels = ListSize | ListLabel,
            ListLabelButtons = ListLabel | Buttons,
            All = Default | Buttons
        }

        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);


        private static GUIContent
        useToggleContent = new GUIContent("", "use this gesture"),
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete"),
        addButtonContent = new GUIContent("+", "add element"),
        neuralNetNoneButtonContent = new GUIContent("+", "click to create a new neural net"),
        trainButtonContent = new GUIContent("TRAIN", "press to train the neural network with the recorded gesture data"),
        detectButtonContent = new GUIContent("DETECT", "press to begin detecting gestures");

        Texture2D bg1;
        Texture2D bg2;

		int tabIndex;

        #endregion

        // GUI MODES
        private bool showSettingsGUI = false;
        enum MenuTabs {Train, Detect, Settings};
        MenuTabs selectedTab = MenuTabs.Train;
        enum NeuralNetGUIMode { None, EnterNewNetName, ShowPopup };
        NeuralNetGUIMode neuralNetGUIMode;

        public void OnEnable()
        {
            vrGestureManager = (VRGestureManager)target;
            if(vrGestureManager == null)
            {
                vrGestureManager = VRGestureManager.Instance;
            }
        }
			
        public void OnDisable()
        {
            vrGestureManager.RefreshGestureBank(false);
        }

        public override void OnInspectorGUI()
        {
            // TEXTURE SETUP
            bg1 = AssetDatabase.LoadAssetAtPath<Texture2D>("");
            bg2 = AssetDatabase.LoadAssetAtPath<Texture2D>("");

            serializedObject.Update();

            ShowToolbar();
            ToolbarUpdate();
            FocusAndClickUpdate();
            serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(target);
        }
			
        void ShowToolbar()
        {
            GUILayout.BeginHorizontal();

			string[] tabs = new string[] { "Neural Network", "Settings" };
			tabIndex = GUILayout.Toolbar(tabIndex, tabs);
            switch (tabIndex)
            {
                case 0:
//                    vrGestureManager.stateInitial = VRGestureManagerState.Idle;
                    showSettingsGUI = false;
                    break;
//                case 1:
//                    vrGestureManager.stateInitial = VRGestureManagerState.ReadyToDetect;
//                    showSettingsGUI = false;
//                    break;
                case 1:
                    showSettingsGUI = true;
                    break;
            }

            GUILayout.EndHorizontal();
        }

        void ToolbarUpdate()
        {
            if (showSettingsGUI)
                ShowSettings();
            else
                ShowTrain();
        }

        void FocusAndClickUpdate()
        {
            //Enter click
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp && IfGestureControl(GUI.GetNameOfFocusedControl()))
            {
                //On return key press
                ChangeGestureName(GUI.GetNameOfFocusedControl());
            }

            //Focus change
            if (GUI.GetNameOfFocusedControl() != selectedFocus)
            {
                //Focus has changed from a Gesture Control.
                if (IfGestureControl(selectedFocus))
                {
                    ChangeGestureName(selectedFocus);
                }
                selectedFocus = GUI.GetNameOfFocusedControl();
            }
        }

        void ChangeGestureName(string controlName)
        {
            int listIndex = Int32.Parse(controlName.Substring(16));
            VRGestureRenameState checkState = vrGestureManager.RenameGesture(listIndex);
            if (checkState == VRGestureRenameState.Duplicate)
            {
                EditorUtility.DisplayDialog("Hey, listen!",
                        "You can't have duplicate gesture names.", "ok");
            }
        }

        bool IfGestureControl(string controlName)
        {
            return (controlName.Length > 15 && controlName.Substring(0, 15) == "Gesture Control");
        }

        #region SHOW GUI SECTIONS

        void ShowTrain()
        {
            // TRAINING SETUP UI
            if (vrGestureManager.state != VRGestureManagerState.Training)
            {
                // BACKGROUND / STYLE SETUP
                GUIStyle neuralSectionStyle = new GUIStyle();
                neuralSectionStyle.normal.background = bg1;
                GUIStyle gesturesSectionStyle = new GUIStyle();
                gesturesSectionStyle.normal.background = bg1;
                GUIStyle separatorStyle = new GUIStyle();
                //separatorStyle.normal.background = bg2;

                // SEPARATOR
                GUILayout.BeginHorizontal(separatorStyle);
                EditorGUILayout.Separator(); // a little space between sections
                GUILayout.EndHorizontal();

                // NEURAL NET SECTION
                GUILayout.BeginVertical(neuralSectionStyle);
                ShowNeuralNets();
                GUILayout.EndVertical();

                // SEPARATOR
                GUILayout.BeginVertical(separatorStyle);
                EditorGUILayout.Separator(); // a little space between sections
                GUILayout.EndVertical();

                // GESTURE SECTION
                GUILayout.BeginVertical(gesturesSectionStyle);
                // if a neural net is selected
                if (neuralNetGUIMode == NeuralNetGUIMode.ShowPopup)
                {
                    ShowGestures();
                }
                GUILayout.EndVertical();

                // SEPARATOR
                GUILayout.BeginHorizontal(separatorStyle);
                EditorGUILayout.Separator(); // a little space between sections
                GUILayout.EndHorizontal();

                // TRAIN BUTTON
                if (vrGestureManager.readyToTrain && neuralNetGUIMode == NeuralNetGUIMode.ShowPopup)
				{
                    ShowTrainButton();
				}

            }
            // TRAINING IS PROCESSING UI
            else if (vrGestureManager.state == VRGestureManagerState.Training)
            {
                ShowTrainingMode();
            }
        }

        void ShowSettings()
        {
            EditorGUILayout.Separator();
            SerializedProperty beginInDetectMode = serializedObject.FindProperty("beginInDetectMode");
            EditorGUILayout.PropertyField(beginInDetectMode);
            if (beginInDetectMode.boolValue == true)
            {
                if (vrGestureManager.neuralNets.Count > 0)
                {
					vrGestureManager.stateInitial = VRGestureManagerState.ReadyToDetect;
                    EditorGUILayout.LabelField("Choose the neural network to detect with");
                    ShowNeuralNetPopup(GetNeuralNetsList());
                }
                else
                {
                    EditorGUILayout.LabelField("You must create and process a neural network before using this option");
                }
            }
			else
			{
				vrGestureManager.stateInitial = VRGestureManagerState.Idle;
			}
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayGestureTrail"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gestureHand"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gestureButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("confidenceThreshold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumGestureAxisLength"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("vrGestureDetectType"));
        }

        string[] GetNeuralNetsList()
        {
            vrGestureManager.RefreshNeuralNetList();

            string[] stringArray = new string[0];
            if (vrGestureManager.neuralNets.Count > 0)
            {
                stringArray = ConvertStringListPropertyToStringArray("neuralNets");
            }
            return stringArray;
        }

        void ShowNeuralNets()
        {
            EditorGUILayout.LabelField("NEURAL NETWORK");

            string[] neuralNetsArray = GetNeuralNetsList();

            // STATE CONTROL
            if (neuralNetGUIMode == NeuralNetGUIMode.EnterNewNetName)
            {
                ShowNeuralNetCreateNewOptions();
            }
            else if (neuralNetsArray.Length == 0) // if the neural nets list is empty show a big + button
            {
                neuralNetGUIMode = NeuralNetGUIMode.None;
            }
            else // draw the popup and little plus and minus buttons
            {
                neuralNetGUIMode = NeuralNetGUIMode.ShowPopup;
            }

            // RENDER
            GUILayout.BeginHorizontal();
            switch (neuralNetGUIMode)
            {
                case (NeuralNetGUIMode.None):
                    // PLUS + BUTTON
                    if (GUILayout.Button(neuralNetNoneButtonContent))
                    {
                        newNeuralNetName = "";
                        GUI.FocusControl("Clear");
                        neuralNetGUIMode = NeuralNetGUIMode.EnterNewNetName;
                        newNeuralNetName = "";
                        GUILayout.EndHorizontal();

                    }
                    break;
                // NEURAL NET POPUP
                case (NeuralNetGUIMode.ShowPopup):
                    ShowNeuralNetPopupGroup(neuralNetsArray);
                    GUILayout.EndHorizontal();
                    ShowNeuralNetTrainedGestures();
                    break;
            }
        }

        void ShowNeuralNetTrainedGestures()
        {
            GUIStyle style = EditorStyles.whiteLabel;
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("PROCESSED GESTURES");
            GUILayout.EndVertical();
            GUILayout.BeginVertical(style);
            foreach (string gesture in vrGestureManager.Gestures)
            {
                EditorGUILayout.LabelField(gesture, style);
            }
            GUILayout.EndVertical();
        }

        void ShowNeuralNetCreateNewOptions()
        {
            newNeuralNetName = EditorGUILayout.TextField(newNeuralNetName);
            if (GUILayout.Button("Create Network"))
            {
                if (string.IsNullOrEmpty(newNeuralNetName))
                {
                    EditorUtility.DisplayDialog("Please give the new neural network a name", " ", "ok");
                }
                else if (vrGestureManager.CheckForDuplicateNeuralNetName(newNeuralNetName))
                {
                    EditorUtility.DisplayDialog(
                        "The name " + newNeuralNetName + " is already being used, " +
                        "please name it something else.", " ", "ok"
                    );
                }
                else
                {
                    vrGestureManager.CreateNewNeuralNet(newNeuralNetName);
                    //This is incorrect because the list will always be sorted alphabetically
                    //We need to find the list in alphbetical order.
                    List<string> sortedList = new List<string>(vrGestureManager.neuralNets);
                    sortedList.Sort(
                        delegate (String s1, String s2)
                        {
                            return s1.CompareTo(s2);
                        }
                    );
                    selectedNeuralNetIndex = sortedList.IndexOf(newNeuralNetName);
                    neuralNetGUIMode = NeuralNetGUIMode.ShowPopup;
                }
            }

        }

        void ShowNeuralNetPopupGroup(string[] neuralNetsArray)
        {
            ShowNeuralNetPopup(neuralNetsArray);

            // + button
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                newNeuralNetName = "";
                GUI.FocusControl("Clear");
                neuralNetGUIMode = NeuralNetGUIMode.EnterNewNetName;

            }

            // - button
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                if (ShowNeuralNetDeleteDialog(vrGestureManager.currentNeuralNet))
                {
                    vrGestureManager.DeleteNeuralNet(vrGestureManager.currentNeuralNet);
                    if (vrGestureManager.neuralNets.Count > 0)
                        selectedNeuralNetIndex = 0;
                }
            }
        }

        /// <summary>
        /// The Neural Network Dropdown List
        /// For selecting the current neural network
        /// </summary>
        /// <param name="neuralNetsArray"></param>
        void ShowNeuralNetPopup (string[] neuralNetsArray)
        {

			selectedNeuralNetIndex = Array.IndexOf(neuralNetsArray, vrGestureManager.currentNeuralNet);

			// If the choice is not in the array then the _choiceIndex will be -1 so set back to 0
			if (selectedNeuralNetIndex < 0)
				selectedNeuralNetIndex = 0;

            if (Event.current.type == EventType.ExecuteCommand)
                vrGestureManager.RefreshGestureBank(false);
            selectedNeuralNetIndex = EditorGUILayout.Popup(selectedNeuralNetIndex, neuralNetsArray);

			// Update the selected choice in the underlying object
			if (neuralNetsArray.Length > 0)
			{
                vrGestureManager.SelectNeuralNet( neuralNetsArray[selectedNeuralNetIndex] );
			}
			else
			{
                vrGestureManager.gestureBank = null;
				vrGestureManager.currentNeuralNet = null;
			}
        }

        bool ShowNeuralNetDeleteDialog(string neuralNetName)
        {
            return EditorUtility.DisplayDialog("Delete the " + neuralNetName + " neural network?",
                "This cannot be undone.",
                "ok",
                "cancel"
            );
        }

        void ShowGestures()
        {
            // first update the gesture bank
            vrGestureManager.RefreshGestureBank(true);

            EditorGUILayout.LabelField("RECORDED GESTURES");

            // then get the gesture bank
            SerializedProperty gesturesList = serializedObject.FindProperty("gestureBank");
            SerializedProperty size = gesturesList.FindPropertyRelative("Array.size");

            // and finally draw the list
            ShowGestureList(gesturesList, EditorListOption.Buttons);

        }

        void ShowTrainButton()
        {
            if (GUILayout.Button("PROCESS \n" + vrGestureManager.currentNeuralNet, GUILayout.Height(40f)))
            {
                EventType eventType = Event.current.type;
                if (eventType == EventType.used)
                {
                    vrGestureManager.BeginTraining(OnFinishedTraining);
                }
            }
        }

        void ShowTrainingMode()
        {
            string trainingInfo = "Training " + vrGestureManager.currentNeuralNet + " is in progress. \n HOLD ON TO YOUR BUTS";

            GUILayout.Label(trainingInfo, EditorStyles.centeredGreyMiniLabel, GUILayout.Height(50f));
            if (GUILayout.Button("QUIT TRAINING"))
            {
                EventType eventType = Event.current.type;
                if (eventType == EventType.used)
                {
                    vrGestureManager.EndTraining(OnQuitTraining);
                }
            }
        }

        #endregion

        // callback that VRGestureManager should call upon training finished
        void OnFinishedTraining(string neuralNetName)
        {
        }

        void OnQuitTraining(string neuralNetName)
        {

        }

        string[] ConvertStringListPropertyToStringArray(string listName)
        {
            SerializedProperty sp = serializedObject.FindProperty(listName).Copy();
            if (sp.isArray)
            {
                int arrayLength = 0;
                sp.Next(true); // skip generic field
                sp.Next(true); // advance to array size field

                // get array size
                arrayLength = sp.intValue;

                sp.Next(true); // advance to first array index

                // write values to list
                string[] values = new string[arrayLength];
                int lastIndex = arrayLength - 1;
                for (int i = 0; i < arrayLength; i++)
                {
                    values[i] = sp.stringValue; // copy the value to the array
                    if (i < lastIndex)
                        sp.Next(false); // advance without drilling into children
                }
                return values;
            }
            return null;
        }

        void ShowGestureList(SerializedProperty list, EditorListOption options = EditorListOption.Default)
        {

            bool showListLabel = (options & EditorListOption.ListLabel) != 0;
            bool showListSize = (options & EditorListOption.ListSize) != 0;
            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list);
                EditorGUI.indentLevel += 1;
            }
            if (!showListLabel || list.isExpanded)
            {
                SerializedProperty size = list.FindPropertyRelative("Array.size");
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                if (size.hasMultipleDifferentValues)
                {
                    EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
                }
                else
                {
                    ShowGestureListElements(list, options);
                }
            }
            if (showListLabel)
                EditorGUI.indentLevel -= 1;
        }

        private void ShowGestureListElements(SerializedProperty list, EditorListOption options)
        {
            if (!list.isArray)
            {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list", MessageType.Error);
                return;
            }

            bool showElementLabels = (options & EditorListOption.ElementLabels) != 0;
            bool showButtons = (options & EditorListOption.Buttons) != 0;

            // render the list
            for (int i = 0; i < list.arraySize; i++)
            {
                string controlName = "Gesture Control " + i;

                if (showButtons)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                if (showElementLabels)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
                else
                {
                    //Was is this one?
                    GUI.SetNextControlName(controlName);
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                }
                if (showButtons)
                {
					ShowGestureListTotalExamples(list, i);
                    ShowGestureListButtons(list, i);
                    EditorGUILayout.EndHorizontal();

                }

            }

            // if the list is empty show the plus + button
            if (showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
            {
                vrGestureManager.CreateGesture("Gesture 1");
            }
        }

		private void ShowGestureListTotalExamples(SerializedProperty list, int index)
		{
            //Sometimes on the first repaint this will still be looking at the previous gestureBank
            //this means we will be checking the index intended for a different array.
            if (index < vrGestureManager.gestureBankTotalExamples.Count)
            {
                int totalExamples = vrGestureManager.gestureBankTotalExamples[index];
                GUILayout.Label(totalExamples.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(35f));
            }
		}

        private void ShowGestureListButtons(SerializedProperty list, int index)
        {
            // plus button
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                //list.InsertArrayElementAtIndex(index);
 
                //Focus has changed from a Gesture Control.
                if (IfGestureControl(selectedFocus))
                {
                    ChangeGestureName(selectedFocus);
                }
                selectedFocus = "";

                int size = list.arraySize + 1;
                vrGestureManager.CreateGesture("Gesture " + size);
            }
            // minus button
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                // new way to delete using vrGestureManager directly
                string gestureName = list.GetArrayElementAtIndex(index).stringValue;
                vrGestureManager.DeleteGesture(gestureName);

                // old way to delete from property
                //int oldSize = list.arraySize;
                //list.DeleteArrayElementAtIndex(index);
                //if (list.arraySize == oldSize)
                //    list.DeleteArrayElementAtIndex(index);
            }
        }

    }

}

#endif
