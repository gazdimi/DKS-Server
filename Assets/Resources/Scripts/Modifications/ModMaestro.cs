

using System.Collections.Generic;
using UnityEngine;

public class ModMaestro
{
    #region Singleton
    private static ModMaestro instance = null;

    public static ModMaestro GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            return new ModMaestro();
        }

    }
    private ModMaestro()
    {
        instance = this;
        meleeModificationType = new List<Possibility>(){
            new Possibility("Attack_Damage_Modification", 0f),
            new Possibility("Attack_Speed_Modification", 0f),
            new Possibility("Movement_Speed_Modification", 0f)
        };
        rangedModificationType = new List<Possibility>(){
            new Possibility("Attack_Damage_Modification", 0f),
            new Possibility("Attack_Range_Modification", 0f),
            new Possibility("Attack_Speed_Modification", 0f),
            new Possibility("Movement_Speed_Modification", 0f)
        };
        modLevel = new List<Possibility>()
        {
            new Possibility("_1", 0f),
            new Possibility("_2", 0f),
            new Possibility("_3", 0f),
            new Possibility("_4", 0f),
            new Possibility("_5", 0f)

        };
        RefreshProbabilities();
        RefreshModLevelProbabilities();
    }
    #endregion

    private List<Possibility> meleeModificationType;
    private List<Possibility> rangedModificationType;
    private List<Possibility> modLevel;
    /// <summary>
    /// Refreshes melee and ranged mod probabilites.
    /// </summary>
    public void RefreshProbabilities()
    {
        (float certainty,float extraversion,float neurotisism) = PenStatsManager.GetInstance().GetMeanPen(PlayerManager.GetInstance().GetActivePlayers());

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                               Ranged Modification Probabilities.
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        float value = (float)ErrorFunction.Erf(Difficulty_Manager.GetInstance().GetDifficultyFactor() / 100)*100;
        rangedModificationType.Find(x => x.GetItem().Equals("Attack_Damage_Modification")).SetValue(value);//Attack Damage Probability.
        List<Possibility> Basepos = new List<Possibility>();
        //Base Probabilities for other Mods.
        float rBaseAR = (100 - value) / (rangedModificationType.Count - 1);
        float rBaseMS = (100 - value) / (rangedModificationType.Count - 1);
        float rBaseAS = (100 - value) / (rangedModificationType.Count - 1);

        float a = 10 / 2;
        float t = 1 / 2 + (extraversion/200);
        float l = 1 / 2 + (neurotisism / 200);
        float pAR= (1 + certainty / 100) * (rBaseAR - a * certainty / 100) * t + (1 - t) - (2 * a * certainty / 100);//Calculate Attack Range Probability.
        float pMS=2*rBaseMS-pAR;//Calculate Movement Speed Probability.
        float pAS;
        float minPas = 10;
        float maxPas = 90;
        //Calculate Attack Speed Probability.
        if (l <= 0.5)
        {
            pAS = (1 - 2*l) * minPas + 2 * l * rBaseAS;
        }
        else
        {
            pAS = 2 * (l - 1) * rBaseAS + (1 - 2 * (l - 1)) * maxPas;
        }
        pAS = rBaseAS * (1 - certainty / 100) + pAS * certainty / 100;
        float diff = rBaseAS - pAS;//Calculate difference between Base Pas and new Pas.
        float bonusPar = pAR * diff / (100 - rBaseAS);
        float bonusPms = pMS * diff / (100 - rBaseAS);
        rangedModificationType.Find(x => x.GetItem().Equals("Attack_Range_Modification")).SetValue( pAR+bonusPar);//Attack Range Probability.
        rangedModificationType.Find(x => x.GetItem().Equals("Movement_Speed_Modification")).SetValue(pMS + bonusPms);//Movement Speed Probability.
        rangedModificationType.Find(x => x.GetItem().Equals("Attack_Speed_Modification")).SetValue(pAS);//Attack Speed Probability.

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                               Melee Modification Probabilities.
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        float mBaseAT= (float)ErrorFunction.Erf(Difficulty_Manager.GetInstance().GetDifficultyFactor()/100)*100;//Attack Damage Base Probability.
        float mBaseMS = (100 - meleeModificationType.Find(x => x.GetItem().Equals("Attack_Damage_Modification")).GetValue()) / (meleeModificationType.Count - 1);//Movement Speed Base Probability.
        float mBaseAS = (100 - meleeModificationType.Find(x => x.GetItem().Equals("Attack_Damage_Modification")).GetValue()) / (meleeModificationType.Count - 1);//Attack Speed Base Probability.
        float pMSmax = 33;//Movement Speed  Max Probability.
        float pASmax = 33;//Attack Speed  Max Probability.
        float pADmax = 66;//Attack Damage  Max Probability.
        pMS = mBaseMS - t * (pMSmax - mBaseMS);
        pAS = mBaseAS + l * (pASmax - mBaseAS);
        float pAD = mBaseAT * (certainty / 100) + (1 - certainty / 100) * pADmax;
        meleeModificationType.Find(x => x.GetItem().Equals("Attack_Damage_Modification")).SetValue(pAD);//Attack Damage Probability.
        meleeModificationType.Find(x => x.GetItem().Equals("Movement_Speed_Modification")).SetValue(pMS);//Movement Speed Probability.
        meleeModificationType.Find(x => x.GetItem().Equals("Attack_Speed_Modification")).SetValue(pAS);//Attack Speed Probability.
    }
    /// <summary>
    /// Refreshes the probabilities of the mod levels.
    /// </summary>
    public void RefreshModLevelProbabilities()
    {
        float[] chunks = new float[modLevel.Count];
        float chunkSize = 100 / modLevel.Count;
        chunks[0] = chunkSize;
        int index = 0;
        bool set = false;
        for(int i = 1; i < chunks.Length; i++)
        {
            chunks[i] = chunks[i - 1] + chunkSize;
            if (Difficulty_Manager.GetInstance().GetDifficultyFactor() <= chunks[i]&&!set)
            {
                index = i;
                set = true;
            }
        }
        float prev = 0;
        float next = 0;
        float upper = 0;
        if (index > 0)
        {
            prev = 10f;
            modLevel[index - 1].SetValue(prev);
        }
        if (index + 1 <= modLevel.Count -1)
        {
            next = 25f;
            modLevel[index + 1].SetValue(next);
        }
        if (index + 2 <= modLevel.Count - 1)
        {
            upper = 10f;
            modLevel[index + 2].SetValue(upper);
        }
        modLevel[index].SetValue(100-prev-next-upper);

        
    }
    /// <summary>
    /// Chooses a random melee modification depending on the melee mod probabilities.
    /// </summary>
    /// <returns></returns>
    public string ChooseMeleeModification()
    {
        return RandomProbability.Choose(meleeModificationType) + RandomProbability.Choose(modLevel);
    }
    /// <summary>
    /// Chooses a random ranged modification depending on the ranged mod probabilities.
    /// </summary>
    /// <returns></returns>
    public string ChooseRangedModification()
    {
        return RandomProbability.Choose(rangedModificationType) + RandomProbability.Choose(modLevel);
    }
}
