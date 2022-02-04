using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellSpawner : MonoBehaviour {
    public static void SpawnCell(float x = 0, float y = 0, float energy = 0, CellGenes parentGenes = null) {
        // create new cell object
        GameObject newCell = new GameObject("Cell" + CellSimulation.CellCounter);
        CellSimulation.CellCounter++;
        newCell.tag = "Cell";
        
        // create new cell's dna
        var newGenes = newCell.AddComponent<CellGenes>();
        
        // if cell has parent genes, use them to get the new cell's genes. Otherwise just do random mutations 10 times
        if (parentGenes != null) {
            newGenes.Clone((parentGenes));
            newGenes.Mutate();
        }
        else {
            for (int i = 0; i < 10; i++) {
                newGenes.Mutate();
            }

            newGenes.color = Random.ColorHSV();
        }

        // create the sprite for the new cell to use as its visual representation
        var newSprite = newCell.AddComponent<SpriteRenderer>();
        newSprite.sprite = GameObject.FindGameObjectWithTag("Cell").GetComponent<SpriteRenderer>().sprite;
        newSprite.color = newGenes.color;

        // add the cell's collider and rigid body components
        var newCollider = newCell.AddComponent<CircleCollider2D>();
        var newRb = newCell.AddComponent<Rigidbody2D>();
        
        // if the cell was given a specific location, set its position to that. Otherwise generate a random position
        if (x == 0 && y == 0) {
            Vector3 backgroundScale = GameObject.Find("Background").transform.localScale;
            newRb.position = new Vector2(
                (Random.value - 0.5f) * backgroundScale.x,
                (Random.value - 0.5f) * backgroundScale.y
            );
            newRb.MovePosition(
                new Vector2((Random.value - 0.5f) * backgroundScale.x * .9f, (Random.value - 0.5f) * backgroundScale.y) * .9f
            );
        }
        else {
            newRb.position = new Vector2(x, y);
            newRb.MovePosition(new Vector2(x, y));
        }
        
        // in the future the cells should be kinematic. But need to figure out how to handle collisions with the barrier
        //newRb.isKinematic = true;
        
        // create the cell's controller and set its energy
        var newController = newCell.AddComponent<CellController>();
        if (energy == 0) {
            newController.energy = Random.value * 1000f + 500f;
        } else {
            newController.energy = energy;
        }
    }
}