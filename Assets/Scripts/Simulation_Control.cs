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
    public float crossoverRate = 0.4f;
    public float mutationRate = 0.2f;
    public float mutationAmount = 0.2f;
    private int gencounter = 1;


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
           // print("Hexapod ID of bot " + i); 
            //print(hexapodControllers[i].GetInstanceID());
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
            gencounter++;
            print("-----------");
            print("GENERATION: " + gencounter);
            print("-----------");
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
       // Debug.Log("Setcontrollerweight controller ID");
        //Debug.Log(controllerID);
        //Debug.Log(intraNW[2, 2, 2]);
        hexapod_control.setIntraNeuronWeights(intraNW);
        hexapod_control.setInputWeights(inputW);
        hexapod_control.setExoNeuronWeights(exoNW);
        hexapod_control.getIntraNeuronWeights();
    }

    public double[,,,] getControllerIntraNeuronWeights()
    {
        double[,,,] tempIntraNeuronWeights = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];

        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = hexapodControllers[i];
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();

            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        tempIntraNeuronWeights[i, j, k, l] = hexapod_control.getIntraNeuronWeights()[j, k, l];
                    }
                }
            }
        }

        return tempIntraNeuronWeights;

    }

    public double[,,] getControllerExoNeuronWeights()
    {
        double[,,] tempExoNeuronWeights = new double[numHexapods, num_sets, num_neuron_per_set];

        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = hexapodControllers[i];
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();

            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    tempExoNeuronWeights[i, j, k] = hexapod_control.getExoNeuronWeights()[j, k];
                }
            }
        }

        return tempExoNeuronWeights;
    }

    public double[,,,] getControllerInputWeights()
    {
        double[,,,] tempInputWeights = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];

        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = hexapodControllers[i];
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();

            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        tempInputWeights[i, j, k, l] = hexapod_control.getInputWeights()[j, k, l];
                    }
                }
            }
        }

        return tempInputWeights;
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
        //======================= Get the old weights =============================
        double[,,,] tempAllIntraNW = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,,] tempAllInputW = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,] tempAllExoNW = new double[numHexapods, num_sets, num_neuron_per_set];

        tempAllIntraNW = getControllerIntraNeuronWeights();
        tempAllExoNW = getControllerExoNeuronWeights();
        tempAllInputW = getControllerInputWeights();

        //======================= Get the fitnesses =============================
        float[] hexapodFitnesses = new float[numHexapods];
        float totalFitness = 0;

        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapodController = hexapodControllers[i];
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
            //print(hexapod_control.getNormalisedDistanceToGoal());
            hexapodFitnesses[i] = hexapod_control.getNormalisedDistanceToGoal() + 1;
            totalFitness += hexapodFitnesses[i];
        }
        //print(totalFitness);
        print(Mathf.Max(hexapodFitnesses));
        print("-----------");
        destroyHexapods();

        //======================= Build a roulette wheel =============================
        // The number of times each controller ID is added to the wheel depends
        // on the fitness of that ID controller's hexapod's fitness
        List<int> rouletteWheel = new List<int>(0);

        for (int i = 0; i < numHexapods; i++)
        {
            // If fitness of i was 0.36, it will put i on the wheel 36 times.
            // If fitness of i was 0.88, it will put i on the wheel 88 times.
            int integerisedFitness = (int) (hexapodFitnesses[i] * 100);
            for (int j = 0; j < integerisedFitness; j++)
            {
                rouletteWheel.Add(i);
            }
        }

        //======================= Select new controllers using roulette wheel =============================
        // Pick which controller to base each new controller off from the wheel
        // Old controllers that resulted in better fitness are on the wheel more, so will be more likely to be picked.
        // Then, assign the weights from the old controller to the new controller(s).
        int[] newControllers = new int[numHexapods];
        double[,,,] newAllIntraNW = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,,] newAllInputW = new double[numHexapods, num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,] newAllExoNW = new double[numHexapods, num_sets, num_neuron_per_set];

        for (int i = 0; i < numHexapods; i++)
        {

            newControllers[i] = rouletteWheel[Random.Range(0, rouletteWheel.Count - 1)];
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    // The new controller i values are set to the old controller x values, where x has been selected on the roulette wheel
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        newAllIntraNW[i, j, k, l] = tempAllIntraNW[newControllers[i], j, k, l];
                        newAllInputW[i, j, k, l] = tempAllInputW[newControllers[i], j, k, l];
                    }
                    newAllExoNW[i, j, k] = tempAllExoNW[newControllers[i], j, k];
                }
            }
            
        }

        //======================= Crossover new weights with each other =============================
        for (int i = 0; i < numHexapods; i += 2)
        {
            if (i + 1 < numHexapods)
            {
                // Crossover intra-neuron weights
                for (int j = 0; j < num_sets; j++)
                {
                    for (int k = 0; k < num_neuron_per_set; k++)
                    {
                        if (Random.Range(0f, 1f) < crossoverRate)
                        {
                            for (int l = 0; l < num_neuron_per_set; l++)
                            {
                                double tempWeight = newAllIntraNW[i, j, k, l];
                                newAllIntraNW[i, j, k, l] = newAllIntraNW[i + 1, j, k, l];
                                newAllIntraNW[i + 1, j, k, l] = tempWeight;
                            }
                        }
                    }
                }

                // Crossover input weights
                for (int j = 0; j < num_sets; j++)
                {
                    for (int k = 0; k < num_neuron_per_set; k++)
                    {
                        if (Random.Range(0f, 1f) < crossoverRate)
                        {
                            for (int l = 0; l < num_neuron_per_set; l++)
                            {
                                double tempWeight = newAllInputW[i, j, k, l];
                                newAllInputW[i, j, k, l] = newAllInputW[i + 1, j, k, l];
                                newAllInputW[i + 1, j, k, l] = tempWeight;
                            }
                        }
                    }
                }

                // Crossover exo-neuron weights
                for (int j = 0; j < num_sets; j++)
                {
                    for (int k = 0; k < num_neuron_per_set; k++)
                    {
                        if (Random.Range(0f, 1f) < crossoverRate)
                        {
                            double tempWeight = newAllExoNW[i, j, k];
                            newAllExoNW[i, j, k] = newAllExoNW[i + 1, j, k];
                            newAllExoNW[i + 1, j, k] = tempWeight;
                        }
                    }
                }
            }
        }


        //======================= Mutate some of the new weights =============================
        for (int i = 0; i < numHexapods; i++)
        {
            // Mutate intra-neuron weights
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        if (Random.Range(0f, 1f) < mutationRate)
                        {
                            newAllIntraNW[i, j, k, l] += Random.Range(-mutationAmount, mutationAmount);
                        }
                    }
                }
            }

            // Mutate input weights
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        if (Random.Range(0f, 1f) < mutationRate)
                        {
                            newAllInputW[i, j, k, l] += Random.Range(-mutationAmount, mutationAmount);
                        }
                    }
                }
            }

            // Mutate exo-neuron weights
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    if (Random.Range(0f, 1f) < mutationRate)
                    {
                        newAllExoNW[i, j, k] += Random.Range(-mutationAmount, mutationAmount);
                    }
                }
            }
        }



        //======================= Assign weights to the controllers =============================
        // Create a set of weights for a single controller so it can be sent to the right controller
        double[,,] newIntraNW = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,,] newInputW = new double[num_sets, num_neuron_per_set, num_neuron_per_set];
        double[,] newExoNW = new double[num_sets, num_neuron_per_set];
        for (int i = 0; i < numHexapods; i++)
        {
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        newIntraNW[j, k, l] = newAllIntraNW[i, j, k, l];
                        newInputW[j, k, l] = newAllInputW[i, j, k, l];
                    }
                    newExoNW[j, k] = newAllExoNW[i, j, k];
                }
            }
            setControllerWeights(i, newIntraNW, newExoNW, newInputW);
        }

        spawnHexapods();

    }
}
