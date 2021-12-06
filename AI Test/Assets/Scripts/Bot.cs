using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    //TODO: 
    //How to save
    //Current progress: Created a reference Network which is a Scriptable object
    //Refer the Scriptable object if it has any content
    //Problems:
    //  1)The Array is not getting transferred from GM to Bot
    [SerializeField]GameManager GM;
    int[] arr = null;
    GameObject[] food;
    GameObject target;
    NeuralNetwork brain;
    int energy;
    int foodPoints;
    int count;
    [SerializeField]bool canThink;
    public void SetLayers(int[] _arr)
    {
        arr = _arr;
    }
    public void SetBrain(NeuralNetwork network = null)
    {
        count = 15;
        StartCoroutine(PulsingVision());
        Debug.Log("BrainSet");
        try
        {
            if(network==null)
            {
                brain = new NeuralNetwork(arr);
            }
            else
            {
                brain = new NeuralNetwork(network);//ERROR
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }
    void Start()
    {
        GM = FindObjectOfType<GameManager>();
        energy = 1;
        foodPoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
        if (transform.position.y <= -10)
        {
            KillBot();
            
        }
        if(count<=0)
        {
            KillBot();
        }
    }
    private void FixedUpdate()
    {
        if(canThink)
        {
            try
            {
                //float x2 = target.transform.position.x;
                //float z2 = target.transform.position.z;
                //float x1 = transform.position.x;
                //float z1 = transform.position.z;
                float dist = Vector3.Distance(target.transform.position, this.transform.position);
                float ang = Vector3.Angle(this.transform.position, target.transform.position);

                float[] pos = { dist, ang };
                float[] instructions = brain.FeedForward(pos);
                if (instructions[0] > 0.5f)
                {
                    transform.Translate(new Vector3(0, 0, 0.25f));
                    energy += 1;
                }
                if (instructions[1] > 0.5f)
                {
                    transform.Rotate(new Vector3(0, -1, 0));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 1, 0));
                }
                
            }
            catch(Exception e)
            {
                throw(e);
            }
        }
    }
    IEnumerator PulsingVision()//This Updates the Food List every 5 seconds
    {
        while(true)
        {
            Debug.Log("Pulse");
            food = GameObject.FindGameObjectsWithTag("Food");
            float temp = 10000f;
            for(int i = 0; i< food.Length; i++)
            {
                float foundDist = Vector3.Distance(this.transform.position, food[i].transform.position);
                if(foundDist<temp)
                {
                    target = food[i];
                    temp = foundDist;
                }
            }
            canThink = true;
            count -= 1;
            yield return new WaitForSeconds(1f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject==target)
        {
            Destroy(other.gameObject);
            Debug.Log("Ate Food");
            foodPoints += 10;
            GM.CreateFood();
        }
    }
    void KillBot()
    {
        Debug.Log("Bot Died");
        float fitness = (float)energy/1000 + (foodPoints*1000);
        brain.SetFitness(fitness);
        GM.BotDied(brain);
        Destroy(this.gameObject);
    }

}
