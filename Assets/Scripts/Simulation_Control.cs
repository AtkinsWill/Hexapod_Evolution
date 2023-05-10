using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            GameObject hexapodController = Instantiate(hexapodControllerPrefab, new Vector3(0, 15, (float)i * 25), Quaternion.identity) as GameObject;
            hexapodControllers[i] = hexapodController;
            Hexapod_Control hexapod_control = hexapodController.GetComponent<Hexapod_Control>();
            GameObject newGoal = Instantiate(goalPrefab, new Vector3(40, 3, (float)i * 25), Quaternion.identity) as GameObject;
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
    System.Random random = new System.Random();
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
                    //tempIntraNeuronWeights[j, k, l] = random.NextDouble() * 2 - 1;
                    //tempInputWeights[j, k, l] = random.NextDouble() * 2 - 1;
                    tempIntraNeuronWeights[j, k, l] = Random.Range(-1f, 1f);
                    tempInputWeights[j, k, l] = Random.Range(-1f, 1f);
                }
                tempExoNeuronWeights[j, k] = Random.Range(-1f, 1f);
                //tempExoNeuronWeights[j, k] = random.NextDouble() * 2 - 1;
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
        PrintWeightsCSV(intraNW, "CSVdataOutput/HexapodID" + controllerID + "_step7_SetControllerWeights.csv");
        hexapod_control.setIntraNeuronWeights(intraNW);
        hexapod_control.setInputWeights(inputW);
        hexapod_control.setExoNeuronWeights(exoNW);
        hexapod_control.getIntraNeuronWeights();
        hexapod_control.resetTime();
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
            GameObject newHexapod = Instantiate(hexapodPrefab, new Vector3(0, 1.01f, (float)i * 25), Quaternion.identity) as GameObject;
            newHexapod.GetComponent<ArticulationBody>().velocity = Vector3.zero;
            newHexapod.GetComponent<ArticulationBody>().angularVelocity= Vector3.zero;
            newHexapod.GetComponent<ArticulationBody>().immovable = true;


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
            hexapodFitnesses[i] = (hexapod_control.getNormalisedDistanceToGoal());
            totalFitness += hexapodFitnesses[i];
        }
        //print(totalFitness);
        print(Mathf.Max(hexapodFitnesses));
        print(totalFitness / numHexapods);
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
        foreach (int x in rouletteWheel)
        {
            print(string.Format("gen {0} controller {1}", gencounter, x));
        }
        print(rouletteWheel.Count);

        for (int i = 0; i < numHexapods; i++)
        {
            
            //PrintWeightsCSV4D(newAllIntraNW, i, "CSVdataOutput/HexapodID" + i + "_step2_PreRoulette.csv");

            if (rouletteWheel.Count < 1)
            {
                newControllers[i] = Random.Range(0, numHexapods);
            }
            else
            {
                newControllers[i] = rouletteWheel[Random.Range(0, rouletteWheel.Count - 1)];
            }
            print(string.Format("gen {0} controller {1} takes {2}", gencounter ,i, newControllers[i]));

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
            //PrintWeightsCSV4D(newAllIntraNW, i, "CSVdataOutput/HexapodID" + i + "_step3_PostRoulette.csv");

        }
        for (int i = 0; i <numHexapods; i++)
        {
            //PrintWeightsCSV4D(newAllIntraNW, i, "CSVdataOutput/HexapodID" + i + "_step4_PostAllRoulette.csv");
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

                        // print(string.Format("Crossing intra i:{0}, j:{1}, k{2} with i+1", i, j, k));
                        for (int l = 0; l < num_neuron_per_set; l++)
                        {
                            if (Random.Range(0f, 1f) < crossoverRate)
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
                        // print(string.Format("Crossing input i:{0}, j:{1}, k{2} with i+1", i, j, k));
                        for (int l = 0; l < num_neuron_per_set; l++)
                        {
                            if (Random.Range(0f, 1f) < crossoverRate)
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
                           // print(string.Format("Crossing exo i:{0}, j:{1}, k{2} with i+1", i, j, k));
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
                           // print(string.Format("Mutating intra i:{0}, j:{1}, k{2}, l:{3} ", i, j, k, l));

                            newAllIntraNW[i, j, k, l] += Random.Range(-mutationAmount, mutationAmount);
                        }
                    }
                }
            }
            //PrintWeightsCSV4D(newAllIntraNW, i, "CSVdataOutput/HexapodID" + i + "_step5_PostMutation.csv");

            // Mutate input weights
            for (int j = 0; j < num_sets; j++)
            {
                for (int k = 0; k < num_neuron_per_set; k++)
                {
                    for (int l = 0; l < num_neuron_per_set; l++)
                    {
                        if (Random.Range(0f, 1f) < mutationRate)
                        {
                           // print(string.Format("Mutating input i:{0}, j:{1}, k{2}, l:{3}", i, j, k, l));
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
                      //  print(string.Format("Mutating exo  i:{0}, j:{1}, k{2}", i, j, k));

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
            PrintWeightsCSV4D(newAllIntraNW, i, "CSVdataOutput/HexapodID" + i + "_step5_BeforeAssignment.csv");
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
            PrintWeightsCSV(newIntraNW, "CSVdataOutput/HexapodID" + i + "_step6_PostAssignment.csv");
            setControllerWeights(i, newIntraNW, newExoNW, newInputW);
        }

        spawnHexapods();

    }

    void PrintWeights(double[,,] weights)
    {
        int dim1 = weights.GetLength(0);
        int dim2 = weights.GetLength(1);
        int dim3 = weights.GetLength(2);

        for (int i = 0; i < dim1; i++)
        {
            Debug.Log($"Set {i + 1}:");

            for (int j = 0; j < dim2; j++)
            {
                string row = "Row " + (j + 1) + ": ";

                for (int k = 0; k < dim3; k++)
                {
                    row += weights[i, j, k].ToString("F2") + " ";

                    // Uncomment the line below to add a separator between values for easier reading
                    // row += (k < dim3 - 1) ? ", " : "";
                }

                Debug.Log(row);
            }

            Debug.Log("");
        }
    }


    void PrintWeightsCSV(double[,,] weights, string fileName)
    {
        int dim1 = weights.GetLength(0);
        int dim2 = weights.GetLength(1);
        int dim3 = weights.GetLength(2);

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            for (int i = 0; i < dim1; i++)
            {
                writer.WriteLine($"Set {i + 1}:");

                for (int j = 0; j < dim2; j++)
                {
                    string row = "";

                    for (int k = 0; k < dim3; k++)
                    {
                        row += weights[i, j, k].ToString("F2");

                        // Add a comma separator between values, except for the last value in the row
                        row += (k < dim3 - 1) ? "," : "";
                    }

                    writer.WriteLine(row);
                }

                writer.WriteLine("");
            }
        }
    }

    void PrintWeightsCSV4D(double[,,,] weights, int firstDimensionIndex, string fileName)
    {
        int dim2 = weights.GetLength(1);
        int dim3 = weights.GetLength(2);
        int dim4 = weights.GetLength(3);

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            for (int i = 0; i < dim2; i++)
            {
                writer.WriteLine($"Set {i + 1}:");

                for (int j = 0; j < dim3; j++)
                {
                    string row = "";

                    for (int k = 0; k < dim4; k++)
                    {
                        row += weights[firstDimensionIndex, i, j, k].ToString("F2");

                        // Add a comma separator between values, except for the last value in the row
                        row += (k < dim4 - 1) ? "," : "";
                    }

                    writer.WriteLine(row);
                }

                writer.WriteLine("");
            }
        }
    }

    void mutatetester()
    {
        double[,,] beforearray = new double[6, 6, 6];
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < 6; k++)
                {

                    beforearray[i, j, k] = 1.0;
                }
            }
        }
        double[,,] afterarray = beforearray;
        PrintWeights(afterarray);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < 6; k++)
                {
                    for (int l = 0; l < 6; l++)
                    {
                        if (Random.Range(0f, 1f) < mutationRate)
                        {
                            afterarray[i, j, k] += Random.Range(-mutationAmount, mutationAmount);
                        }
                    }
                }

            }
        }
        PrintWeights(afterarray);

    }

}
