using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexapod_Control : MonoBehaviour
{
    public GameObject hexapod;
    private Body_Control body_control;

    public GameObject objective;

    private const int NumberOfSets = 6;
    private const int NeuronsPerSet = 5;
    private const int AngleInputs = 3;
    //private const int Threshold = 0;
    private double[,] neuronStates;
    public double[,,] intraNeuronWeights;
    public double[,] exoNeuronWeights;
    public double[,,] inputWeights;
    public float[,] angleInputs;

    public bool runSim = false;
    public float[,] actualJointTargets = new float[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };

    public Vector3 distanceToObjective;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    private void Update()
    {
        if (runSim)
        {
            body_control = hexapod.GetComponent<Body_Control>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    actualJointTargets[i, j] += 0.0001f;     
                }

            }
            body_control.actualJointTargets = actualJointTargets;

            distanceToObjective = body_control.transform.position - objective.transform.position;
            Debug.Log(distanceToObjective);
        }
    }

    // Update is called once per frame
    /*
    void Update()
    {

        //iterate through each set
        for (int i = 0; i < NumberOfSets; i++) 
        {
            //get angle input for that leg
            for (int j = 0; j < AngleInputs; j++)
            {
                angleInputs[i,j] = 0;
            }

                //for each neuron in that set
                for (int j = 0; j < NeuronsPerSet; j++)
            {
                //sum set to 0
                double weightedSum = 0;

                

                //Intra connections
                for (int k = 0; k < NeuronsPerSet; k++)
                {
                    //prevents self connections
                    if (j != k)
                    {
                        weightedSum += neuronStates[i,j] * intraNeuronWeights[i,j,k];
                    }
                }

                //Exo connections
                for (int k = 0; k < NumberOfSets; k++)
                {
                    //prevents self connections
                    if (j != k)
                    {
                        weightedSum += neuronStates[k,j] * exoNeuronWeights[k,j];
                    }
                }

                //Input connections
                for (int k = 0; k < AngleInputs; k++)
                {
                weightedSum += angleInputs[j,k] * inputWeights[i,k,j];
                
                }


                // Apply binary step function
                //neuronStates[i,j] = weightedSum >= Threshold ? 1 : 0;

                // Apply tanh activation function
                //this is more closer to what the literature uses.
                neuronStates[i,j] = Math.Tanh(weightedSum);
            }
        }
    }
    */
    public void setJointTargets(double targetJoints)
    {
        for (int i = 0; i < NumberOfSets; i++)
        {
            for (int j = 0; j < NeuronsPerSet; j += 2)
            {
                //body_control.actualJointTargets[i, j] = (float)neuronStates[i][j];
            }
        }

    }
}
