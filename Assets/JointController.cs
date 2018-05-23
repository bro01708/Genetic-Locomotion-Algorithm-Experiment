using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets;
using System.Linq;

public class JointController : MonoBehaviour
{
    //Record of Hingejoints
    public List<HingeJoint2D> jointList = new List<HingeJoint2D>();
    //Record of Motors
    public List<JointMotor2D> motorList = new List<JointMotor2D>();
    //Reference to gameobjects
    GameObject[] agents;

    //the chromosome that is currently 'plugged in'

    private Chromosome currentChromosome;
    //Inspector vars
    public string chrom = "";
    //current frame refers to physics updates, not graphics frames
    //frames dictate how long a gene interpretation lasts for
    private int currentFrame = 0;
    private int geneIndex = 0;
    private int geneDuration = 20;
    public int[] prevGene;
    public int agentIndex;



    // Use this for initialization
    void Start()
    {
        //Ajust timescale for testing Purposes, appears to make physics non-deterministic
        Time.timeScale = 1f;
        AssignJoints();
       // Debug.Log(jointList.Count);
    }

    /// <summary>
    /// called regularly independant of framerate, used for anything to do with
    /// pysics to keep deterministic results
    /// checks if frame limit expired if so , moves onto next gene in chromosome
    /// if not carry on activating the joints according to the current chromosome
    /// </summary>
    void FixedUpdate()
    {
        chrom = currentChromosome.ToString();
        if (currentFrame < geneDuration-1)
        {
            currentFrame++;
        }
        else
        {
            currentFrame = 0;
            if (geneIndex == currentChromosome.Genes.Length - 1)
            {
                geneIndex = 0;
            }
            else
            {
                geneIndex++;
            }
        }
        
        //Debug.ClearDeveloperConsole();
        //Debug.Log("Frame = " + currentFrame.ToString());
        activateJoints();
        
    }

    /// <summary>
    /// finds its parent agent, and tracks all the 2dhingejoints within it
    /// </summary>
    public void AssignJoints()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent").OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
        jointList.Clear();
        motorList.Clear();

        foreach (GameObject go in agents)
        {
            if (go.name == "Agent " + agentIndex)
            {
                foreach (HingeJoint2D hj in go.GetComponentsInChildren<HingeJoint2D>().OrderBy(g => g.transform.GetSiblingIndex()).ToArray())
                {
                    jointList.Add(hj);
                }
            }
        }
    }

    /// <summary>
    /// interprets each integer of DNA into a state of torque for the appropriate motor
    /// </summary>
    void activateJoints()
    {
        for (int i = 0; i < currentChromosome.Genes[geneIndex].DnaSequence.Length; i++)
        {
            if (currentChromosome.Genes[geneIndex].DnaSequence[i] != prevGene[i])
            {
                JointMotor2D tempMotor = jointList[i].motor;
                switch (currentChromosome.Genes[geneIndex].DnaSequence[i])
                {
                    case 1:
                        tempMotor.motorSpeed = 100;
                        tempMotor.maxMotorTorque = 100;
                        prevGene[i] = 1;
                        break;
                    case 0:
                        tempMotor.motorSpeed = 0;
                        tempMotor.maxMotorTorque = 0;
                        prevGene[i] = 0;
                        break;
                    case -1:
                        tempMotor.motorSpeed = -100;
                        tempMotor.maxMotorTorque = 10;
                        prevGene[i] = -1;
                        break;
                }
                jointList[i].motor = tempMotor;
            }
            else
            {
                //Debug.Log("Input same, skipping changeover");
            }

        }

    }
    //getters and setters
    internal Chromosome GetCurrentChromosome
    {
        get
        {
            return currentChromosome;
        }

        set
        {
            currentChromosome = value;
        }
    }

     
}
