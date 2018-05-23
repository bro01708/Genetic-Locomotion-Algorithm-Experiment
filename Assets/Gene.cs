using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class Gene
    {
        /// <summary>
        /// This Class does not use a constructor , to make it easier during instatiation 
        /// to use an iterated loop to assign each DNA value in the "setter".
        /// </summary>
        private int[] dnaSequence = new int[9];
        private int[] possInts = { -1, 0, 1 };
        public Gene()
        {

        }

        public int[] DnaSequence
        {
            get
            {
                return dnaSequence;
            }

            set
            {
                dnaSequence = value;
            }
        }

        public void Scramble(System.Random r)
        {

            for (int i = 0; i < dnaSequence.Length; i++)
            {
                dnaSequence[i] = possInts[r.Next(possInts.Length)];
            }
        }

    }
}
