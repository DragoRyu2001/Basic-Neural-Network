using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/NeuralNetwork")]
public class NeuralNetwork:ScriptableObject
{
    private int[] layers;//List of size of each layer
    private float[][] neurons;//which neuron of which layer
    private float[][][] weights;//Which weight of which neuron of which Layers 
    private float fitness;
    public bool savedState;
    public int generation;
    public NeuralNetwork(int[] _layers)
    {
        this.layers = new int[_layers.Length];
        for(int i = 0; i< _layers.Length; i++)
        {
            this.layers[i] = _layers[i];
        }
        InitNeurons();
        InitWeights();
    }
    public void NetworkInit(int[] _layers)
    {
        this.layers = null;
        this.neurons = null;
        this.weights = null;
        this.layers = new int[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
        {
            this.layers[i] = _layers[i];
        }
        InitNeurons();
        InitWeights();
    }
    public void NetworkCopy(NeuralNetwork copyNetwork)
    {
        this.layers = null;
        this.neurons = null;
        this.weights = null;
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }
        InitNeurons();
        InitWeights();
        copyWeights(copyNetwork.weights);
    }
    //Deep Copy of the Neural network
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }
        InitNeurons();
        InitWeights();
        //Now to copy Weights from old network to the new one
        copyWeights(copyNetwork.weights);
    }
    private void copyWeights(float [][][] copyWeights)
    {
        for(int i = 0; i< weights.Length; i++)
        {
            for(int j = 0; j< weights[i].Length; j++)
            {
                for(int k = 0; k< weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for(int i = 0; i< layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }
    private void InitWeights()
    {
        List<float[][]> WeightList = new List<float[][]>();//List of Layers
        for(int i = 1; i< layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();//List of Neurons in each Layer
            
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];//Number of Weights in each neuron
                for(int k = 0; k<neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);//Generate Random Weights for NeuralNetwork
                }
                layerWeightList.Add(neuronWeights);
            }
            WeightList.Add(layerWeightList.ToArray());
        }
        weights = WeightList.ToArray();
    }
    public float[] FeedForward(float[] inputs)
    {
        //The First layer in the Network are the Inputs
        for(int i = 0; i< inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for(int i =1; i< layers.Length; i++)//Loop through every layer from layer 2
        {
            for(int j =0; j< neurons[i].Length; j++)//Loop through every neuron in That layer
            {
                float value = 0.25f;//Constant Bias given by me;
                for(int k  = 0; k< neurons[i-1].Length; k++)//Loop through every neuron in the previous layer as they become inputs for our current neuron
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }
        return neurons[neurons.Length-1];
    }
    public void Mutate()
    {
        for(int i = 0; i< weights.Length; i++)
        {
            for(int j = 0; j< weights[i].Length; j++)
            {
                for(int k =  0; k< weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];
                    //Mutate
                    float factor;
                    int operation = UnityEngine.Random.Range(1, 5);
                    switch(operation)
                    {
                        case 1:
                            weight = -1 * weight;
                            break;
                        case 2:
                            factor = UnityEngine.Random.Range(0, 1f) + 1f;//Increase by 0% to 100% randomly
                            weight *= factor;
                            break;
                        case 3:
                            factor = UnityEngine.Random.Range(0, 1f);//Decrease by 0% to 100% randomly
                            weight *= factor;
                            break;
                        default:
                            weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                            break;
                    }
                    weights[i][j][k] = weight;
                }
            }
        }
    }
    #region Fitness
    public void SetFitness(float fit)
    {
        fitness = fit;
    }
    public float GetFitness()
    {
        return fitness;
    }

    #endregion
}
