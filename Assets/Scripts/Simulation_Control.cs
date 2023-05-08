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


    private double[,,] tempIntraNeuronWeights;
    private double[,] tempExoNeuronWeights;
    private double[,,] tempInputWeights;

    public GameObject[] hexapodControllers;
    public GameObject hexapodControllerPrefab;

    public GameObject[] hexapods;
    public GameObject hexapodPrefab;

    public GameObject[] goals;
    public GameObject goalPrefab;


    public float simulationTime;
    public float currentTime;



    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        tempIntraNeuronWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        tempInputWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        tempExoNeuronWeights = new double[num_sets, num_neuron_per_set];

        hexapodControllers = new GameObject[numHexapods];
        hexapods = new GameObject[numHexapods];
        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = Instantiate(hexapodControllerPrefab, new Vector3(0, 15, (float)i * 15), Quaternion.identity) as GameObject;
            hexapodControllers[i] = hexapodController;
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
            GameObject newGoal = Instantiate(goalPrefab, new Vector3(20, 3, (float)i * 15), Quaternion.identity) as GameObject;
            hexapod_control.goal = newGoal;
            print("Hexapod ID of bot " + i); 
            print(hexapodControllers[i].GetInstanceID());
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
    for (int i = 0; i < numHexapods; i++)
    {
        double[,,] tempIntraNeuronWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,] tempInputWeights = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,] tempExoNeuronWeights = new double[num_sets, num_neuron_per_set];

        for (int j = 0; j < num_sets; j++)
        {
            for (int k = 0; k < num_neuron_per_set; k++)
            {
                for (int l = 0; l < num_neuron_per_set; l++)
                {
                    tempIntraNeuronWeights[j, k, l] = Random.Range(-1f, 1f);
                    tempInputWeights[j, k, l] = Random.Range(-1f, 1f);
                }
            }
        }
        for (int j = 0; j < 3; j++)
        {
            for (int k = 0; k < num_neuron_per_set; k++)
            {
                tempExoNeuronWeights[j, k] = Random.Range(-1f, 1f);
            }
        }
        setControllerWeights(i, tempIntraNeuronWeights, tempExoNeuronWeights, tempInputWeights);
    }
}

    void setControllerWeights(int controllerID, double[,,] intraNW, double[,] exoNW, double[,,] inputW)
    {
        
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        Debug.Log("Setcontrollerweight controller ID");
        Debug.Log(controllerID);
        Debug.Log(intraNW[2, 2, 2]);
        hexapod_control.setIntraNeuronWeights(intraNW);
        hexapod_control.setInputWeights(inputW);
        hexapod_control.setExoNeuronWeights(exoNW);
        hexapod_control.getIntraNeuronWeights();
    }

    double[,,] getControllerIntraNeuronWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.getIntraNeuronWeights();
    }

    double[,] getControllerExoNeuronWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.getExoNeuronWeights();
    }

    double[,,] getControllerInputWeights(int controllerID)
    {
        GameObject hexapodController = hexapodControllers[controllerID];
        Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
        return hexapod_control.getInputWeights();
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
        //destroyHexapods();
        for (int i = 0; i < numHexapods; i++)
        {
            //output old weights to CSV
            double[,,] tempWeights = getControllerInputWeights(i);

            //get fitnesses
                //do this with a loop for i (each hexapod)
                //calc fitnesse
                //store all fitnesses to array with controller ID (fitness array)
                //output that CSV
            
            //destroy hexapod

            //LEAVE I BASED FOR LOOP

            //with the fitness array do roulette (spin num hexapod times, prob based on percentage of ind. fitness based on total fit)
            //crossover (look into standards) (if nothing found just next in line)
            
            //loop through selected controllers
            //mutate
            //setcontroller

            
            setControllerWeights(i, tempIntraNeuronWeights, tempExoNeuronWeights, tempWeights);
            

            //weights[i] = Random.Range(1f, 10);
        }

        spawnHexapods();

    }

    


}