using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;
    [SerializeField] int maxNum;
    [SerializeField] GameObject botPrefab;
    [SerializeField] int maxBots;
    [SerializeField] int[] networkLayers = { 2, 4, 4, 2 };
    [SerializeField] NeuralNetwork referenceNetwork;
    NeuralNetwork bestNetwork;
    int generation;
    float bestFitness;
    void Start()
    {
        if(referenceNetwork.savedState==false)
        {
            bestNetwork = null;
            generation = 0;
            referenceNetwork.NetworkInit(networkLayers);
            referenceNetwork.savedState = true;
        }
        else
        {
            bestNetwork = referenceNetwork;
            Debug.Log("Best Fitness so Far: " + bestNetwork.GetFitness());
            generation = referenceNetwork.generation;
        }
        StartFood();
        CreateNewGeneration();
        StartCoroutine(PulsingVision());
    }
    public void CreateFood()
    {
        float x = Random.Range(-49, 49);
        float z = Random.Range(-49, 49);
        Instantiate(foodPrefab, new Vector3(x, 1, z), Quaternion.identity);
    }
    void StartFood()
    {
        for(int i =0; i< maxNum; i++)
        {
            CreateFood();
        }
    }
    public void BotDied(NeuralNetwork network)
    {
        float fit = network.GetFitness();
        if(fit>bestFitness)
        {
            bestFitness = fit;
            bestNetwork = network;
            referenceNetwork.NetworkCopy(bestNetwork);
        }
        
    }
    void CreateNewGeneration()
    {
        generation++;
        bestFitness = 0;
        Debug.Log("Generation: " + generation);
        for(int i = 0; i<maxBots; i++)
        {
            float x = Random.Range(-29, 29);
            float z = Random.Range(-29, 29);
            Bot _bot = Instantiate(botPrefab, new Vector3(x, 2f, z), Quaternion.identity).GetComponent<Bot>();
            _bot.SetLayers(networkLayers);
            if(bestNetwork!=null)
            {
                if(i>(maxBots/2+1))
                {
                    bestNetwork.Mutate();
                }
                _bot.SetBrain(bestNetwork);
                Debug.Log("Best Fitness: " + bestNetwork.GetFitness());
            }
            else
            {
                _bot.SetBrain();
            }
        }
    }
    IEnumerator PulsingVision()//This Updates the Food List every 2 seconds
    {
        while (true)
        {
            GameObject[] Bots = GameObject.FindGameObjectsWithTag("Bot");
            Debug.Log("Number of Bots alive: " + (Bots.Length - 1));
            if (Bots.Length <= 1)
            {
                CreateNewGeneration();
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
