using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Edwon.VR.Gesture
{
    public class Utils
    {
        private static Utils instance;

        //constructor
        private Utils() { }

        public static Utils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Utils();
                }
                return instance;
            }
        }

        public float FindMaxAxis(List<Vector3> capturedLine)
        {
            //find min and max for X,Y,Z
            float minX, maxX, minY, maxY, minZ, maxZ;
            //init all defaults to first point.
            Vector3 firstPoint = capturedLine[0];
            minX = maxX = firstPoint.x;
            minY = maxY = firstPoint.y;
            minZ = maxZ = firstPoint.z;

            foreach (Vector3 point in capturedLine)
            {
                minX = getMin(minX, point.x);
                maxX = getMax(maxX, point.x);

                minY = getMin(minY, point.y);
                maxY = getMax(maxY, point.y);

                minZ = getMin(minZ, point.z);
                maxZ = getMax(maxZ, point.z);
            }

            //we now have all of our mins and max
            float distX = Mathf.Abs(maxX - minX);
            float distY = Mathf.Abs(maxY - minY);
            float distZ = Mathf.Abs(maxZ - minZ);

            //FIND THE AXIS MAX. This will be the length for all of our AXIS.
            float axisMax = distX;
            axisMax = getMax(axisMax, distY);
            axisMax = getMax(axisMax, distZ);
            return axisMax;
        }

        //Same as DownRes line but this will scale, rather than skew.
        public List<Vector3> DownScaleLine(List<Vector3> capturedLine)
        {
            //find min and max for X,Y,Z
            float minX, maxX, minY, maxY, minZ, maxZ;
            //init all defaults to first point.
            Vector3 firstPoint = capturedLine[0];
            minX = maxX = firstPoint.x;
            minY = maxY = firstPoint.y;
            minZ = maxZ = firstPoint.z;

            foreach (Vector3 point in capturedLine)
            {
                minX = getMin(minX, point.x);
                maxX = getMax(maxX, point.x);

                minY = getMin(minY, point.y);
                maxY = getMax(maxY, point.y);

                minZ = getMin(minZ, point.z);
                maxZ = getMax(maxZ, point.z);
            }

            //we now have all of our mins and max
            float distX = Mathf.Abs(maxX - minX);
            float distY = Mathf.Abs(maxY - minY);
            float distZ = Mathf.Abs(maxZ - minZ);

            //FIND THE AXIS MAX. This will be the length for all of our AXIS.
            float axisMax = distX;
            axisMax = getMax(axisMax, distY);
            axisMax = getMax(axisMax, distZ);

            //This should make all of our lowest points start at the origin.
            Matrix4x4 translate = Matrix4x4.identity;
            translate[0, 3] = -minX;
            translate[1, 3] = -minY;
            translate[2, 3] = -minZ;

            Matrix4x4 scale = Matrix4x4.identity;
            scale[0, 0] = 1 / axisMax;
            scale[1, 1] = 1 / axisMax;
            scale[2, 2] = 1 / axisMax;


            List<Vector3> localizedLine = new List<Vector3>();
            foreach (Vector3 point in capturedLine)
            {
                //we translate, but maybe we also divide each by the dist?
                Vector3 newPoint = translate.MultiplyPoint3x4(point);
                newPoint = scale.MultiplyPoint3x4(newPoint);
                localizedLine.Add(newPoint);
            }
            //capture way less points
            return localizedLine;
            // ok now we need to create a matrix to move all of these points to a normalized space.
        }

        //This is warping gestures to be on a scale of (0-1) in every axis
        //It stretches the gesture along each axis for how far it goes.
        public List<Vector3> DownResLine(List<Vector3> capturedLine)
        {
            //find min and max for X,Y,Z
            float minX, maxX, minY, maxY, minZ, maxZ;
            //init all defaults to first point.
            Vector3 firstPoint = capturedLine[0];
            minX = maxX = firstPoint.x;
            minY = maxY = firstPoint.y;
            minZ = maxZ = firstPoint.z;

            foreach (Vector3 point in capturedLine)
            {
                minX = getMin(minX, point.x);
                maxX = getMax(maxX, point.x);

                minY = getMin(minY, point.y);
                maxY = getMax(maxY, point.y);

                minZ = getMin(minZ, point.z);
                maxZ = getMax(maxZ, point.z);
            }

            //we now have all of our mins and max
            float distX = Mathf.Abs(maxX - minX);
            float distY = Mathf.Abs(maxY - minY);
            float distZ = Mathf.Abs(maxZ - minZ);

            //This should make all of our lowest points start at the origin.
            Matrix4x4 translate = Matrix4x4.identity;
            translate[0, 3] = -minX;
            translate[1, 3] = -minY;
            translate[2, 3] = -minZ;

            Matrix4x4 scale = Matrix4x4.identity;
            scale[0, 0] = 1 / distX;
            scale[1, 1] = 1 / distY;
            scale[2, 2] = 1 / distZ;


            List<Vector3> localizedLine = new List<Vector3>();
            foreach (Vector3 point in capturedLine)
            {
                //we translate, but maybe we also divide each by the dist?
                Vector3 newPoint = translate.MultiplyPoint3x4(point);
                newPoint = scale.MultiplyPoint3x4(newPoint);
                localizedLine.Add(newPoint);
            }
            //capture way less points
            return localizedLine;
            // ok now we need to create a matrix to move all of these points to a normalized space.
        }

        public float getMin(float min, float newMin)
        {
            if (newMin < min) { min = newMin;}
            return min;
        }

        public float getMax(float max, float newMax)
        {
            if (newMax > max) { max = newMax;}
            return max;
        }

        public List<Vector3> SubDivideLine(List<Vector3> capturedLine)
        {
            //Make sure list is longer than 11.
            int outputLength = Config.FIDELITY;

            float intervalFloat = Mathf.Round((capturedLine.Count * 1f) / (outputLength * 1f));
            int interval = (int)intervalFloat;
            List<Vector3> output = new List<Vector3>();

            for (int i = capturedLine.Count - 1; output.Count < outputLength; i -= interval)
            {
                if (i > 0) { output.Add(capturedLine[i]);}
                else { output.Add(capturedLine[0]); }
            }
            return output;
        }

        //Format line for NeuralNetwork
        //Might want to pull this out from saving lines.
        //Save huge raw vector lines.
        //Run formatting on them during training.
        //Allow users to changes and train different formats on
        //the same data set.
        public double[] FormatLine(List<Vector3> capturedLine)
        {
            capturedLine = SubDivideLine(capturedLine);
            if (Config.USE_RAW_DATA)
            {
                capturedLine = DownScaleLine(capturedLine);
            }
            else
            {
                capturedLine = DownResLine(capturedLine);
            }
            
            List<double> tmpLine = new List<double>();
            foreach (Vector3 cVector in capturedLine)
            {
                tmpLine.Add(cVector.x);
                tmpLine.Add(cVector.y);
                tmpLine.Add(cVector.z);
            }
            return tmpLine.ToArray();
        }

        public void ReadWeights()
        {

        }

        public void SaveFile()
        {

        }

        public NeuralNetworkStub ReadNeuralNetworkStub(string networkName)
        {
            string path = Config.SAVE_FILE_PATH + networkName + "/" + networkName + ".txt";
            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                ////System.IO.File.
                string inputLine = lines[0];

                

                NeuralNetworkStub stub = JsonUtility.FromJson<NeuralNetworkStub>(inputLine);
                return stub;
            }
            else
            {
                NeuralNetworkStub stub = new NeuralNetworkStub();
                stub.gestures = new List<string>();
                return stub;
            }
        }

        public List<string> GetNetworksFromFile()
        {
            List<string> networkList = new List<string>();
            string networkPath = Config.SAVE_FILE_PATH;
            //string[] files = System.IO.Directory.GetFiles(gesturesPath, "*.txt");
            string[] files = System.IO.Directory.GetDirectories(networkPath);
            if (files.Length == 0)
            {
                Debug.Log("no gestures files (recorded data) yet");
                return null;
            }
            foreach (string path in files)
            {
                //paramschar[] sep = { '/'};
                char[] stringSeparators = new char[] { '/' };
                string[] exploded = path.Split(stringSeparators);
                string finalString = exploded[exploded.Length - 1];
                networkList.Add(finalString);
            }
            return networkList;
        }

        public List<string> GetGestureBank(string networkName)
        {
            List<string> gestureBank = new List<string>();
            string gesturesPath = Config.SAVE_FILE_PATH + networkName + "/gestures/";
            //Check if path exists
            if (System.IO.Directory.Exists(gesturesPath))
            {
                string[] files = System.IO.Directory.GetFiles(gesturesPath, "*.txt");
                if (files.Length == 0)
                {
                    return null;
                }
                foreach (string path in files)
                {
                    //paramschar[] sep = { '/'};
                    char[] stringSeparators = new char[] { '/' };
                    string[] exploded = path.Split(stringSeparators);
                    string iCareAbout = exploded[exploded.Length - 1];
                    //scrub file extension
                    int substrIndex = iCareAbout.LastIndexOf('.');
                    string finalString = iCareAbout.Substring(0, substrIndex);
                    gestureBank.Add(finalString);
                }
            }
            return gestureBank;
        }

		public List<int> GetGestureBankTotalExamples(List<string> gestureList)
		{
			List<int> totals = new List<int>();
			foreach(string gesture in gestureList)
			{
				int total = GetGestureExamplesTotal(gesture);
				totals.Add(total);
			}
			return totals;
		}

        public void CreateGestureFile(string gestureName, string networkName)
        {
            string gestureFileLocation = Config.SAVE_FILE_PATH + networkName + "/Gestures/";
			// if no gestures folder already
			if (!System.IO.Directory.Exists(gestureFileLocation))
			{
				// create gestures folder
				System.IO.Directory.CreateDirectory(gestureFileLocation);
			}

			// create the gesture file
            string fullPath = gestureFileLocation + gestureName + ".txt";
        	System.IO.StreamWriter file = new System.IO.StreamWriter(fullPath, true);
            file.Dispose();
            
#if UNITY_EDITOR
            AssetDatabase.ImportAsset(fullPath);
#endif
        }

		public void DeleteGestureFile(string gestureName, string networkName)
		{
			string gestureFileLocation = Config.SAVE_FILE_PATH + networkName + "/Gestures/" + gestureName + ".txt";
#if UNITY_EDITOR
			FileUtil.DeleteFileOrDirectory(gestureFileLocation);
            AssetDatabase.Refresh();
#endif
        }

        public void DeleteGestureExample(string neuralNetwork, string gesture, int lineNumber)
        {
            string gestureFileLocation = Config.SAVE_FILE_PATH + neuralNetwork + "/Gestures/" + gesture + ".txt"; ;
            List<string> tmpLines = new List<string>();
            tmpLines.AddRange(System.IO.File.ReadAllLines(gestureFileLocation));
            tmpLines.RemoveAt(lineNumber);
            string[] lines = tmpLines.ToArray();

            System.IO.File.WriteAllLines(gestureFileLocation, lines);
        }

        public void RenameGestureFile(string gestureOldName, string gestureNewName, string networkName)
        {
            string oldPath = Config.SAVE_FILE_PATH + networkName + "/Gestures/" + gestureOldName + ".txt";
            string newPath = Config.SAVE_FILE_PATH + networkName + "/Gestures/" + gestureNewName + ".txt";
            //get all them old gesture
            string[] oldLines = System.IO.File.ReadAllLines(oldPath);
            List<string> newLines = new List<string>();
            foreach(string line in oldLines)
            {
                GestureExample currentGest = JsonUtility.FromJson<GestureExample>(line);
                currentGest.name = gestureNewName;
                newLines.Add(JsonUtility.ToJson(currentGest));
            }
            System.IO.File.WriteAllLines(newPath, newLines.ToArray());
            DeleteGestureFile(gestureOldName, networkName);
        }

		public void ChangeGestureName(string gestureNameOld, string gestureNameNew, string networkName)
		{
			string path = Config.SAVE_FILE_PATH + networkName + "/Gestures/" + gestureNameOld + ".txt";
#if UNITY_EDITOR
            AssetDatabase.RenameAsset(path, gestureNameNew);
			string pathUpdated = Config.SAVE_FILE_PATH + networkName + "/Gestures/" + gestureNameNew + ".txt";
//			AssetDatabase.ImportAsset(pathUpdated);
			AssetDatabase.Refresh();
#endif
        }

        public List<string> GetGestureFiles(string networkName)
        {
            string gesturesFilePath = Config.SAVE_FILE_PATH + networkName + "/Gestures/";
            string[] files = System.IO.Directory.GetFiles(gesturesFilePath, "*.txt");
            return files.ToList<string>();
        }

		// get all the examples of this gesture and store in a GestureExample list
		public List<GestureExample> GetGestureExamples(string gesture)
		{
			string[] lines = GetGestureLines(gesture);
			List<GestureExample> gestures = new List<GestureExample>();
			foreach (string currentLine in lines)
			{
				gestures.Add(JsonUtility.FromJson<GestureExample>(currentLine));
			}
			return gestures;
		}

		// get the total amount of examples of this gesture
		public int GetGestureExamplesTotal(string gesture)
		{
			string[] lines = GetGestureLines(gesture);
			return lines.Length;
		}

		private string[] GetGestureLines(string gesture)
		{
			//read in the file
			string filePath = Config.SAVE_FILE_PATH + VRGestureManager.Instance.currentNeuralNet + "/Gestures/";
			string fileName = gesture + ".txt";
			string[] lines = System.IO.File.ReadAllLines(filePath + fileName);
			return lines;
		}

		public void DeleteNeuralNetFiles(string networkName)
		{
			string path = Config.SAVE_FILE_PATH + networkName + "/";
			if (System.IO.Directory.Exists(path))
			{
#if UNITY_EDITOR
                FileUtil.DeleteFileOrDirectory(path);
                AssetDatabase.DeleteAsset(path);
#endif
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        // create a folder in the save file path
        // return true if successful, false if not
        public bool CreateFolder (string path)
        {
            string folderPathNew = Config.SAVE_FILE_PATH + path;
            System.IO.Directory.CreateDirectory(folderPathNew);
#if UNITY_EDITOR
            AssetDatabase.ImportAsset(folderPathNew);
#endif
            return true;
        }
    }

}



[Serializable]
public class GestureExample
{
    public string name;
    public List<Vector3> data;
    public bool trained;
    public bool raw;

    public double[] GetAsArray()
    {
        List<double> tmpLine = new List<double>();
        //gestures.Add(JsonUtility.FromJson<GestureExample>(currentLine));
        foreach (Vector3 currentPoint in data)
        {
            tmpLine.Add(currentPoint.x);
            tmpLine.Add(currentPoint.y);
            tmpLine.Add(currentPoint.z);
        }
        return tmpLine.ToArray();
    }
}

[Serializable]
public class NeuralNetworkStub
{
    public int numInput;
    public int numHidden;
    public int numOutput;
    public List<string> gestures;
    public double[] weights;
}


