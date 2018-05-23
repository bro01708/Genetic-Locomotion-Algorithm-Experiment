using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    ///needs to be serializable as child chromosomes need to reference different
    ///memory block to their parents chromosomes otherwise the mutation function 
    ///affect the parents dna aswell - epescially bad if parent was an elite.
    [Serializable]
    class Chromosome 
    {
        //an array of gene objects
        private Gene[] genes;
        //how many genes to contain
        private int chromosomeLength;
        //fitness ranking
        private float fitness;
        //index of which agent it was inserted in
        private int hostAgent;
        //text representation of the chromosome
        string myString;

        public Chromosome(int _length)
        {
            chromosomeLength = _length;
        }
 
        /// <summary>
        /// randomly assign new size between 4-20 
        /// </summary>
        /// <param name="r"></param>
        public void RandomlyPopulate(System.Random r)
        {
            chromosomeLength = r.Next(4, 20);
            genes = new Gene[chromosomeLength];
            for (int i = 0; i < genes.Length; i++)
            {
                Gene g = new Gene();
                g.Scramble(r);
                genes[i] = g;
            }
        }

        /// <summary>
        /// combines a second chromosome with 'this'
        /// splits them in half and reassembles to return child
        /// </summary>
        /// <param name="_otherParent"></param>
        /// <returns></returns>
        public Chromosome Crossover(Chromosome _otherParent)
        {
            Chromosome a = ObjectCopier.Clone(this);
            Chromosome b = ObjectCopier.Clone(_otherParent);

            // make it so this can handle multiple lengths of chromosome
            int firstHalf = (int)Math.Floor((double)chromosomeLength / 2);
            int secondHalf = (int)Math.Floor((double)_otherParent.chromosomeLength / 2);
            int newLength = firstHalf + secondHalf;

            List<Gene> childList = new List<Gene>();
            List<Gene> parent1 = new List<Gene>();
            List<Gene> parent2 = new List<Gene>();


            for (int i = 0; i < firstHalf; i++)
            {
                
                parent1.Add(a.genes[i]);
            }
            for (int i = 0; i < b.chromosomeLength; i++)
            {
                if (i >= secondHalf)
                {
                    parent2.Add(b.genes[i]);
                }
            }

            childList.AddRange(parent1);
            childList.AddRange(parent2);

            Chromosome child = new Chromosome(newLength);
            child.genes = childList.ToArray();
            return child;
        }

        /// <summary>
        /// loops through each gene in chromosome and has the chance
        /// to run the scramble function.
        /// </summary>
        /// <param name="_mutationRate"></param>
        /// <param name="r"></param>
        public void Mutate(float _mutationRate, System.Random r)
        {
            for (int i = 0; i < genes.Length; i++)
            {
                if (r.NextDouble() < _mutationRate)
                {
                    genes[i].Scramble(r);

                }
            }
        }
        
        /// <summary>
        /// Not implemented yet. Will swap over two genes in the chromosome
        /// </summary>
        /// <param name="_mutationRate"></param>
        /// <param name="r"></param>
        //public void Permutate(float _mutationRate,System.Random r)
        //{
        //    for (int i = 0; i < genes.Length; i++)
        //    {
        //        if (r.NextDouble() < _mutationRate)
        //        {
                   

        //        }
        //    }
        //}

        
        ///string representation of the DNA
        public override string ToString()
        {
            if (myString != null)
            {
                return myString;
            }
            string s = "";
            foreach (Gene g in Genes)
            {
                foreach (int i in g.DnaSequence)
                {
                    s = s + i.ToString();
                }
            }
            myString = s;
            return myString;
        }


        //getters and setters
        public Gene[] Genes
        {
            get
            {
                return genes;
            }

            set
            {
                genes = value;
            }
        }

        public float Fitness
        {
            get
            {
                return fitness;
            }

            set
            {
                fitness = value;
            }
        }

        public int HostAgent
        {
            get
            {
                return hostAgent;
            }

            set
            {
                hostAgent = value;
            }
        }

    }
}
