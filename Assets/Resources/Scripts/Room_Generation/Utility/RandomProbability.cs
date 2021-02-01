using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RandomProbability
{
    /// <summary>
    /// Random choice given some probabilities.
    /// </summary>
    /// <param name="probabilities"></param>
    /// <returns></returns>
 public static string Choose(List<Possibility> probabilities)
    {
        probabilities.Sort((p, q) => p.GetValue().CompareTo(q.GetValue()));
        float maxprob = probabilities.Sum(s => s.GetValue());
        double prob =UnityEngine.Random.Range(0,maxprob);
        int item=-1;
        double counter =0;
        while (prob > counter)
        {
            item++;
            counter += probabilities[item].GetValue();
            
        }
        return probabilities[item].GetItem();


    }
}
