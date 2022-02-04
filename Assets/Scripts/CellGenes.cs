using UnityEngine;

public class CellGenes : MonoBehaviour {
    public double moveCost = 0.01f ;
    public double photosynthesisReward = 0.2d;
    public double strengthCost = 0.025d;
    public double speed = 5d;
    public double stamina = 0.5d;
    public double visionModifier = 3d;
    public double visionCost = 0.00000005d;
    public double reproductionDelay = 15.0d;
    public double mutationRate = 0.1d;
    public double fearModifier = 1.0d;
    public double hungerModifier = 1.0d;
    public double strengthModifier = 1.0d;
    public Color color = new Color(0.9f, 0.1f, 0.1f);
    public int generation = 1;

    public void Mutate() {
        float lowerMutationBound = 1.0f - (float) mutationRate;
        float upperMutationBound = 1.0f + (float) mutationRate;

        // determine new color
        float[] colorVals = {
            color.r * Random.Range(lowerMutationBound, upperMutationBound),
            color.g * Random.Range(lowerMutationBound, upperMutationBound),
            color.b * Random.Range(lowerMutationBound, upperMutationBound),
        };

        for (var i = 0; i < colorVals.Length; i++) {
            if (colorVals[i] > 0.96f) {
                colorVals[i] = 0.96f;
            } else if (colorVals[i] < 0.04f) {
                colorVals[i] = 0.04f;
            }
        }

        color = new Color(colorVals[0], colorVals[1], colorVals[2]);

        // calculate trade offs and update other vals
        double movePhotoSynthesisTradeOff = Random.Range(lowerMutationBound, upperMutationBound);
        double visionVisionCostTradeOff = Random.Range(lowerMutationBound, upperMutationBound);
        double reproductiveTimeMutationTradeOff = Random.Range(lowerMutationBound, upperMutationBound);
        double fearHungerTradeOff = Random.Range(lowerMutationBound, upperMutationBound);
        double strengthMutation = Random.Range(lowerMutationBound, upperMutationBound);

        mutationRate *= 1 / reproductiveTimeMutationTradeOff;
        if (mutationRate < 0) {
            mutationRate = 0;
        } else if (mutationRate > 1) {
            mutationRate = 1;
        }

        moveCost *= movePhotoSynthesisTradeOff;
        photosynthesisReward *= movePhotoSynthesisTradeOff;
        strengthCost *= strengthMutation;
        speed *= Random.Range(lowerMutationBound, upperMutationBound);
        stamina *= Random.Range(lowerMutationBound, upperMutationBound);
        visionModifier *= visionVisionCostTradeOff;
        visionCost *= 1 / visionVisionCostTradeOff;
        reproductionDelay *= reproductiveTimeMutationTradeOff;
        fearModifier *= fearHungerTradeOff;
        hungerModifier *= 1 / fearHungerTradeOff;
        strengthModifier *= strengthMutation;
    }

    public void Clone(CellGenes oldGenes) {
        moveCost = oldGenes.moveCost;
        photosynthesisReward = oldGenes.photosynthesisReward;
        strengthCost = oldGenes.strengthCost;
        speed = oldGenes.speed;
        stamina = oldGenes.stamina;
        visionModifier = oldGenes.visionModifier;
        visionCost = oldGenes.visionCost;
        reproductionDelay = oldGenes.reproductionDelay;
        mutationRate = oldGenes.mutationRate;
        fearModifier = oldGenes.fearModifier;
        hungerModifier = oldGenes.hungerModifier;
        strengthModifier = oldGenes.strengthModifier;
        color = oldGenes.color;
        generation = oldGenes.generation + 1;
    }
}

