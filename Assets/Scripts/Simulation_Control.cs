using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation_Control : MonoBehaviour
{
    public int seed = 0;
    public int numHexapods = 1;
    int num_sets = 6;
    int num_neuron_per_set = 5;
    public GameObject[] hexapods;
    public GameObject hexapodPrefab;
    float angle;
    public float simulationTime;
    public float currentTime;
    private double[,,,] intraNeuronWeights;
    private double[,,] exoNeuronWeights;
    private double[,,,] inputWeights;


    // Start is called before the first frame update
    void Start()
    {
        intraNeuronWeights = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        inputWeights = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        exoNeuronWeights = new double[numHexapods, 3, num_neuron_per_set];
        InitaliseRandomWeights();
        spawnHexapods();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;

        if (currentTime > simulationTime)
        {
            evaluate();
        }
        else
        {
            runIteration();
        }


    }

    void runIteration()
    {
        angle += 0.0001f;
        int i = 0;
        foreach (GameObject hexapod in hexapods)
        {
            //hexapod.GetComponent<Body_Control>().setJointTargets(angle * weights[i]);
            i++;
        }
    }

    void evaluate()
    {
        destroyHexapods();
        for (int i = 0; i < numHexapods; i++)
        {
            //Debug.Log(weights);
            //weights[i] = Random.Range(1f, 10);
        }

        spawnHexapods();

    }

    void spawnHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            //preferable if in this loop weights get assigned to robots?
            //Debug.Log(Random.Range(-1f, 1f));
            GameObject hexapod = Instantiate(hexapodPrefab, new Vector3(0, 3, (float)i * 15), Quaternion.identity) as GameObject;
            hexapods[i] = hexapod;
            Body_Control body_control = hexapod.GetComponent<Body_Control>();
            body_control.intraNeuronWeights[,,] = intraNeuronWeights[i,,];
            body_control.exoNeuronWeights[,] = exoNeuronWeights[i,];
            body_control.inputWeights[,,] = inputWeights[i,,];
        }
        angle = 0;
        currentTime = 0;

    }

    void destroyHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            Destroy(hexapods[i]);
        }
    }


    void InitaliseRandomWeights()
    {
        int counter = 0;
        //initialise random weights with seed.
        Random.InitState(seed);
        hexapods = new GameObject[numHexapods];
        for (int i = 0; i < numHexapods; i++)
        {
            for(int j = 0; j < num_sets; j++)
            {
                for(int k = 0; k < num_neuron_per_set; k++)
                {
                    for(int l = 0; l < num_neuron_per_set; l++)
                    {
                        intraNeuronWeights[i,j,k,l] = Random.Range(-1f, 1f);
                        inputWeights[i,j,k,l] = Random.Range(-1f, 1f);
                    }

                }
            }
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    exoNeuronWeights[i, j, k] = Random.Range(-1f, 1f);
                }
            }
        }
    }
}