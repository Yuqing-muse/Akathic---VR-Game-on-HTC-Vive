using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Edwon.VR.Gesture
{
    public class GestureRecognizer
    {
        public double currentConfidenceValue;

        List<string> outputs;
        NeuralNetwork neuralNet;
        //save the array of gestures
        //This should always require a name to load.
        public GestureRecognizer(string filename)
        {
            Load(filename);
        }

        //Load a SavedRecognizer from a file
        public void Load(string filename)
        {
            NeuralNetworkStub stub = Utils.Instance.ReadNeuralNetworkStub(filename);
            outputs = stub.gestures;
            neuralNet = new NeuralNetwork(stub.numInput, stub.numHidden, stub.numOutput);
            neuralNet.SetWeights(stub.weights);
        }

        public string GetGesture(double[] input)
        {
            double[] output = neuralNet.ComputeOutputs(input);

            string actualDebugOutput = "[";
            for(int i=0; i<output.Length; i++)
            {
                actualDebugOutput += output[i];
                actualDebugOutput += ", ";
            }
            actualDebugOutput = actualDebugOutput.Substring(0, actualDebugOutput.Length - 2);
            actualDebugOutput += "]";

            //Debug.Log(actualDebugOutput);
            return GetGestureFromVector(output);
        }

        public string GetGestureFromVector(double[] outputVector)
        {
            //find max index
            int maxIndex = 0;
            double maxVal = 0;
            for (int i = 0; i < outputVector.Length; i++)
            {
                if (outputVector[i] > maxVal)
                {
                    maxIndex = i;
                    maxVal = outputVector[i];
                }
            }
            //maxVal is the confidence value.
            //Debug.Log("Confidence Value: "+ maxVal);
            currentConfidenceValue = maxVal;
            return outputs[maxIndex];
        }

        public double[] ConvertGestureToVector(string gestureName)
        {
            int vectorIndex = outputs.IndexOf(gestureName);
            double[] outputVector = new double[outputs.Count];
            for(int i = 0; i < outputVector.Length; i++)
            {
                outputVector[i] = 0;
            }
            outputVector[vectorIndex] = 1;
            return outputVector;
        }

        public string ConvertVectorToGesture(double[] outputVector)
        {
            //Find maxIndex
            int maxIndex = 0;
            double maxValue = 0;
            for (int i = 0; i < outputVector.Length; i++)
            {
                if(outputVector[i] > maxValue)
                {
                    maxValue = outputVector[i];
                    maxIndex = i;
                }
            }
            return outputs[maxIndex];
        }

    }

}

