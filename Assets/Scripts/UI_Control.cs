using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Control : MonoBehaviour
{
    public GameObject Simulation_Controller;
    public GameObject Canvas;

    public Simulation_Control sim_control;

    public TMP_InputField crossoverRateInput;
    public TMP_InputField mutationRateInput;
    public TMP_InputField mutationAmountInput;

    public TMP_InputField seedInput;
    public TMP_InputField populationInput;
    public TMP_InputField durationInput;





    // Start is called before the first frame update
    void Start()
    {
        sim_control = Simulation_Controller.GetComponent<Simulation_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void runSim()
    {
        sim_control.run();
        Canvas.SetActive(false);
    }

    public void updateSeed()
    {
        try
        {
            string input = seedInput.text;
            if (input != "")
            {
                int seed = int.Parse(input);
                sim_control.seed = seed;
            }
        }

        catch
        {
            Debug.Log("Invalid input");
        }
    }

    public void updatePopulation()
    {
        try
        {
            string input = populationInput.text;
            if (input != "")
            {
                int population = int.Parse(input);
                sim_control.numHexapods = population;
            }
        }
        
        catch
        {
            Debug.Log("Invalid input");
        }
    }

    public void updateDuration()
    {
        try
        {
            string input = durationInput.text;
            if (input != "")
            {
                float duration = float.Parse(input);
                sim_control.simulationTime = duration;
            }
        }
        catch
        {
            Debug.Log("Invalid input");
        }
    }

    public void updateCrossoverRate()
    {
        try
        {
            string input = crossoverRateInput.text;
            if (input != "")
            {
                float crossoverRate = float.Parse(input);

                if (crossoverRate > 1)
                {
                    crossoverRate = 1;
                }
                if (crossoverRate < 0)
                {
                    crossoverRate = 0;
                }

                sim_control.crossoverRate = crossoverRate;
            }
        }
        catch
        {
            Debug.Log("Invalid input");
        }
    }

    public void updateMutationRate()
    {
        try
        {
            string input = mutationRateInput.text;
            if (input != "")
            {
                float mutationRate = float.Parse(input);
                if (mutationRate > 1)
                {
                    mutationRate = 1;
                }
                if (mutationRate < 0)
                {
                    mutationRate = 0;
                }

                sim_control.mutationRate = mutationRate;
            }
        }
        catch
        {
            Debug.Log("Invalid input");
        }
    }

    public void updateMutationAmount()
    {
        try
        {
            string input = mutationAmountInput.text;
            if (input != "")
            {
                float mutationAmount = float.Parse(input);
                if (mutationAmount > 1)
                {
                    mutationAmount = 1;
                }
                if (mutationAmount < 0)
                {
                    mutationAmount = 0;
                }

                sim_control.mutationAmount = mutationAmount;
            }
        }
        catch
        {
            Debug.Log("Invalid input");
        }
        
    }
}
