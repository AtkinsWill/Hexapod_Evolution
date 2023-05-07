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


    private double[,,] intraNeuronWeights;
    private double[,] exoNeuronWeights;
    private double[,,] inputWeights;

    public GameObject[] hexapodControllers;
    public GameObject hexapodControllerPrefab;

    public GameObject[] hexapods;
    public GameObject hexapodPrefab;

    public GameObject[] objectives;
    public GameObject objectivePrefab;


    public float simulationTime;
    public float currentTime;



    // Start is called before the first frame update
    void Start()
    {
        intraNeuronWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        inputWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        exoNeuronWeights = new double[5, num_neuron_per_set];

        hexapodControllers = new GameObject[numHexapods];
        hexapods = new GameObject[numHexapods];
        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = Instantiate(hexapodControllerPrefab, new Vector3(0, 15, (float)i * 15), Quaternion.identity) as GameObject;
            hexapodControllers[i] = hexapodController;
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
            GameObject newObjective = Instantiate(objectivePrefab, new Vector3(20, 3, (float)i * 15), Quaternion.identity) as GameObject;
            hexapod_control.objective = newObjective;
        }

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

    // ===========================================================================================
    // Functions to set up and utilise the persistent hexapod controllers. These controllers are 
    // created at the start of the simulation and are updated between iterations.
    // They are never deleted.
    // Each one stores the weights for one hexapod for each iteration.
    void InitaliseRandomWeights()
    {
        int counter = 0;
        //initialise random weights with seed.
        Random.InitState(seed);

        for (int i = 0; i < numHexapods; i++)
        {
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        intraNeuronWeights[j, k, l] = Random.Range(-1f, 1f);
                        inputWeights[j, k, l] = Random.Range(-1f, 1f);
                    }

                }
            }
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    exoNeuronWeights[j, k] = Random.Range(-1f, 1f);
                }
            }

            setControllerWeights(i, intraNeuronWeights, exoNeuronWeights, inputWeights);

        }
    }

    void setControllerWeights(int controllerID, double[,,] intraNW, double[,] exoNW, double[,,] inputW)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        hexapod_control.intraNeuronWeights = intraNeuronWeights;
        hexapod_control.inputWeights = inputWeights;
        hexapod_control.exoNeuronWeights = exoNeuronWeights;
    }

    double[,,] getControllerIntraNeuronWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.intraNeuronWeights;
    }

    double[,] getControllerExoNeuronWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.exoNeuronWeights;
    }

    double[,,] getControllerInputWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.inputWeights;
    }
    //===========================================================================================

    //-------------------------------------------------------------------------------------------
    // Functions to set up and utilise the temporary hexapods, which are created and destroyed with
    // each iteration.
    void spawnHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            //preferable if in this loop weights get assigned to robots?
            GameObject newHexapod = Instantiate(hexapodPrefab, new Vector3(0, 3, (float)i * 15), Quaternion.identity) as GameObject;
            hexapods[i] = newHexapod;

            GameObject hexapodController = hexapodControllers[i];
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
            hexapod_control.hexapod = newHexapod;


        }
        currentTime = 0;

    }

    void destroyHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            Destroy(hexapods[i]);
        }
    }
    //-------------------------------------------------------------------------------------------

    

    void runIteration()
    {
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
            double[,,] tempWeights = getControllerInputWeights(i);
            Debug.Log(i);
            Debug.Log(tempWeights[i, 0, 0]);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    tempWeights[i, j, k] += 0.1f;
                }
                
            }
            if (i == 0)
            {
                Debug.Log("This one: ");
                Debug.Log(tempWeights[0, 0, 0]);
            }
            setControllerWeights(i, intraNeuronWeights, exoNeuronWeights, tempWeights);
            

            //weights[i] = Random.Range(1f, 10);
        }

        spawnHexapods();

    }

    


}