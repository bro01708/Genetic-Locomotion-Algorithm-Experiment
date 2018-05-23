using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class GeneticAlgorithm 
    {
        //Only ever going to need 1 GA running, make it singleton
        private static GeneticAlgorithm instance;
        private GeneticAlgorithm() { }
        //variables that should be editable/viewable in inspector
        //size of the population
        public int populationSize = 0;
        //How many elites to use
        public int elitism = 0;
        //chance to mutate (1x)
        public float mutationRate = 0.5f;
        //our current collection of dna
        public List<Chromosome> population = new List<Chromosome>();

        //generation counter
        private int generation;
        //
        private float bestFitness;
        private string bestChromosome;
        private float[] fitnesses;
        //use seperate list for elites for definite access to elites only
        private List<Chromosome> elites = new List<Chromosome>();

        //singleton
        public static GeneticAlgorithm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GeneticAlgorithm();
                }
                return instance;
            }
        }

        /// <summary>
        /// for the size of the Pop, create a new chromosome of default
        /// size then trigger it to randomly populate itself with random genes.
        /// Add it to the population
        /// </summary>
        /// <param name="_size"></param>
        /// <param name="r"></param>
        public void InitPopulation(int _size, System.Random r)
        {
            elitism = (int)Math.Ceiling((double)_size / 10);
            populationSize = _size;
            for (int i = 0; i < _size; i++)
            {
                Chromosome c = new Chromosome(1);
                c.RandomlyPopulate(r);
                population.Add(c);
            }
        }

        /// <summary>
        /// Main genetic functions, before realising to use fixedUpdate for
        /// physics, was experiencing non-deterministic results so split the 
        /// elites from standard to see if assigning them to their previousa
        /// agent made them perform the same
        /// 
        /// Set up collections for : Elites, Regulars , Both Combined
        /// 
        /// Calculate fitnesses
        /// Order by fitness
        /// Update global vars
        /// 
        /// loop through pop, add elites to new pop
        /// for rest of pop generate new child using parents from prev gen
        /// mutate
        /// add to new pop
        /// replace old pop with new
        /// </summary>
        /// <param name="r"></param>
        public void NewGeneration(System.Random r)
        {
            int nonEliteSize = (populationSize - elitism)+1;
            List<Chromosome> newPopulation = new List<Chromosome>();
            List<Chromosome> elitesAndRegs = new List<Chromosome>();
            elitesAndRegs.AddRange(elites);
            elitesAndRegs.AddRange(population);
            elites = new List<Chromosome>();
            CalculateFitness(elitesAndRegs);
            elitesAndRegs.Sort(CompareChromosomes);
            fitnesses = UpdateFitnessArray(elitesAndRegs);
            BestChromosome = elitesAndRegs[0].ToString();
            BestFitness = elitesAndRegs[0].Fitness;
            //Debug.Log("Fittest = Agent " + elitesAndRegs[0].HostAgent + ", With A Fitness of :"+ elitesAndRegs[0].Fitness + " -   " + elitesAndRegs[0].ToString());

            for (int i = 0; i < populationSize; i++)
            {
                if (i < elitism && i < nonEliteSize)
                {
                    elites.Add(elitesAndRegs[i]);
                    //Debug.Log("Elite Added -        " + population[i].Fitness + " - " + population[i].ToString());
                }
                else if (i < nonEliteSize)
                {
                    Chromosome parent1 = SelectParent(r,elitesAndRegs);
                    Chromosome parent2 = SelectParent(r, elitesAndRegs);

                    //make sure it isnt the same parent
                    while (parent1.ToString() == parent2.ToString())
                    {
                        parent2 = SelectParent(r, elitesAndRegs);
                    }

                    Chromosome child = parent1.Crossover(parent2);

                    child.Mutate(mutationRate, r);

                    newPopulation.Add(child);
                }
                else
                {
                    //This fuction can be used to fill out population 
                    //should never get triggered in current state
                    Chromosome temp = new Chromosome(9);
                    temp.RandomlyPopulate(r);
                    newPopulation.Add(temp);
                }

            }
            population = newPopulation;
            //Debug.Log("New pop carryover No.1 is = " + population[0].ToString());
            Generation++;
        
        }

        /// <summary>
        /// check a chromosome against the x position of its hosts 'body'
        /// </summary>
        /// <param name="_selection"></param>
        public void CalculateFitness(List<Chromosome> _selection)
        {
            List<GameObject> go = GameObject.FindGameObjectsWithTag("Agent").OrderBy(g => g.transform.GetSiblingIndex()).ToList();
            //find the tranform position x coord for the agent that matches the chromosomes host.
            for (int i = 0; i < _selection.Count; i++)
            {
                int indexOfCorrespondingObject = _selection[i].HostAgent;
                _selection[i].Fitness = go[indexOfCorrespondingObject].transform.GetChild(0).transform.position.x;
            }
        }

        //Return a random parent from population
        private Chromosome SelectParent(System.Random r, List<Chromosome> _selection)
        {
            int randomIndex = r.Next(0, _selection.Count);
            return _selection[randomIndex];
        }

        //gives the orderby function a var to order on
        private int CompareChromosomes(Chromosome a, Chromosome b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //update the tracker
        public float[] UpdateFitnessArray(List<Chromosome> _selection)
        {
            float[] temp = new float[_selection.Count];
            for (int i = 0; i < _selection.Count; i++)
            {
                temp[i] = _selection[i].Fitness;
            }
            return temp;
        }
        //getters and setters
        public int Generation
        {
            get
            {
                return generation;
            }

            set
            {
                generation = value;
            }
        }

        public float BestFitness
        {
            get
            {
                return bestFitness;
            }

            set
            {
                bestFitness = value;
            }
        }

        public string BestChromosome
        {
            get
            {
                return bestChromosome;
            }

            set
            {
                bestChromosome = value;
            }
        }

        public float[] Fitnesses
        {
            get
            {
                return fitnesses;
            }

            set
            {
                fitnesses = value;
            }
        }

        internal List<Chromosome> Elites
        {
            get
            {
                return elites;
            }

            set
            {
                elites = value;
            }
        }
    }


}

