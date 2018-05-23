using UnityEngine;
using Assets;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Assets
{

    public class Main : MonoBehaviour
    {
        //instantiate GA
        GeneticAlgorithm ga = GeneticAlgorithm.Instance;
        //Collection of biped controllers for each agent
        private JointController[] controllers;
        //track game camera
        private CameraController cc;
        //generate random
        private System.Random random = new System.Random();

        //Variables to be viewed/edited in the unity inspector are REQUIRED to be public.

        //Ensure users cannot enter values that will break it
        [Range(1, 100)]
        public int populationSize;
        //set which agent to base the automata from
        public GameObject agentPrefab;
        //camera to track positions, not necessary but useful.
        public GameObject cameraPrefab;
        //Variable to handle time for epoch termination
        public float timeElapsed = 0;
        public float timeLimit = 20;
        //track which generation we are on
        public int generation = 0;

        //for viewing the progress in the inspector:
        //see the highest value ever seen and by which chromosome
        public float historicFittest = -10f;
        public string historicBestChrom = "";
        //see the highest value from this generation(used in debug to ensure these matched)
        public string GenBestChrom = "";
        public float GenFittest = 0;
        //view entire array of fitnesses
        public float[] fitnesses;


        // Use this for initialization as called upon by unity during instantiation
        void Start()
        {
            InstantiateAgents();
            InstantiateCamera();
            ga.InitPopulation(populationSize,random);
            InsertChromosomes();
        }

        //regularly called irrelevant of framerate, use this for physics
        void FixedUpdate()
        {
            //manually simulate physics as auto-sim is disabled
            //parameter = amount of time to simulate
            Physics2D.Simulate(0.04f);
            //pause the simulation if max epochs reached
            if (generation == 200)
            {
                EditorApplication.isPaused = true;
            }
            //increase timer by fixedUpdate interval (Found in unity project settings)
            timeElapsed += 0.04f;
            //reset level if epoch timer expired
            if (timeElapsed >= timeLimit)
            {
                ResetLevel();
            }
        }

        //Create camera object and assign
        public void InstantiateCamera()
        {
            GameObject camera = Instantiate(cameraPrefab);
            cc = camera.GetComponent<CameraController>();
        }
        /// <summary>
        /// loop through population size, instantiate prefabs, set names,
        /// tags and positions accordingly, have to set parents to null otherwise
        /// they become children in the hierarchy
        /// Assign the controller for each instance to the collection of biped controllers
        /// </summary>
        public void InstantiateAgents()
        {
            for (int i = 0; i < populationSize; i++)
            {
                GameObject newAgent = Instantiate(agentPrefab, agentPrefab.transform);
                newAgent.transform.Translate(0, 0, (i * 5));
                newAgent.transform.parent = null;
                newAgent.name = "Agent " + i.ToString();
            }

            GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent").OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
            controllers = new JointController[agents.Length];
            for (int i = 0; i < agents.Length; ++i)
            {
                controllers[i] = agents[i].GetComponentInChildren<JointController>();
            }
        }

        /// <summary>
        /// for every agent , check if its previous chromosome was an elite,
        /// if so, give it back(used to check if assigning chromosomes to different
        /// agents affected results).
        /// If not get a new chromosome from the standard ones
        /// </summary>
        public void InsertChromosomes()
        {
            int populationIndex=0;
            for (int i = 0; i < controllers.Length; i++)
            {
                if (ga.Elites.Exists(x => x.HostAgent == i))
                {
                    controllers[i].agentIndex = i;
                    Chromosome c = ga.Elites.Find(x => x.HostAgent == i);
                    controllers[i].GetCurrentChromosome = c;
                }
                else
                {
                    controllers[i].agentIndex = i;
                    ga.population[populationIndex].HostAgent = i;
                    controllers[i].GetCurrentChromosome = ga.population[populationIndex];
                    populationIndex++;
                }
            }
        }

        /// <summary>
        /// request new generation
        /// update public inspector vars
        /// debug log winner
        /// 
        /// </summary>
        public void ResetLevel()
        {
            ga.NewGeneration(random);
            GenBestChrom = ga.BestChromosome;
            GenFittest = ga.BestFitness;
            if (GenFittest > historicFittest)
            {
                historicFittest = GenFittest;
                historicBestChrom = GenBestChrom.ToString();
            }
            fitnesses = ga.Fitnesses;
            Debug.Log("Winner From Gen " + generation + " = " + "Fitness = " + fitnesses[0]);
            GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent").OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
            ///NOTE - Unity will not actually destroy the object until the frame
            ///after , need to change tag to stop "destroyed objects" being searched
            ///through when looking for the controllers.
            for (var i = 0; i < agents.Length; i++)
            {
                agents[i].tag = "Destroyed";
                Destroy(agents[i]);
            }
            //repeat instantiation
            timeElapsed = 0;
            InstantiateAgents();
            cc.AssignAgents();
            InsertChromosomes();
            generation++;
        }

    }
}


