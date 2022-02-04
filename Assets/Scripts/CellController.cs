using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Diagnostics;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CellController : MonoBehaviour {
    private Rigidbody2D rb;
    private CellGenes genes;
    private CellSimulation sim;

    public float energy;
    public float strength;
    public float reproductionTimer;
    public float exhaustion;
    public bool tired;
    
    
    // Start is called before the first frame update
    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        //rb.isKinematic = true;
        genes = gameObject.GetComponent<CellGenes>();
        sim = GameObject.Find("Simulation").GetComponent<CellSimulation>();
    }

    private void Update() {
        // if cell energy is too small, it dies
        if (energy < sim.minCellSize) {
            Destroy(gameObject);
        }
        
        // set size of cell to be equal to the square root of its energy
        var size = (float) Math.Sqrt(energy) / 10;
        transform.localScale = new Vector3(size, size, size);
        
        // determine the direction to travel in and move in that direction
        var direction = Think();
        rb.velocity = direction * (float) genes.speed;
        if (!direction.Equals(Vector2.zero)) {
            energy -= GetMoveCost((float) genes.speed * Time.deltaTime);
        }
        
        // gain energy from photosynthesis
        energy += (float) (sim.brightness * genes.photosynthesisReward * 3.14f * Math.Pow((size * 5), sim.photosynthesisScaleFactor)) * Time.deltaTime;
        
        // maintain strength
        energy -= (float) genes.strengthCost * energy * Time.deltaTime;
        strength = (float) genes.strengthModifier * energy;
        
        // try to reproduce
        Reproduce();
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.tag.Equals("Cell"))
            return;
        
        var otherCell = collision.gameObject.GetComponent<CellController>();

        if (otherCell.strength > strength)
            return;
        
        energy += otherCell.energy;
        Destroy(collision.gameObject);
    }

    // returns a normalized vector corresponding to the direction the cell should travel in
    private Vector2 Think() {
        // spend energy using vision
        var vision = (float) genes.visionModifier * (float) Math.Sqrt(energy);
        energy -= (float) (genes.visionCost * vision * energy) * Time.deltaTime;

        if (energy < sim.minCellSize * 1.25f) {
            return Vector2.zero;
        }

        Transform scariestTransform = null;
        var scariestScore = 0.0f;

        Transform tastiestTransform = null;
        var tastiestScore = 0.0f;

        Collider2D[] cellColliders = Physics2D.OverlapCircleAll(transform.position, vision);

        float selfSize = transform.localScale.x;
        var selfPosition = transform.position;
        var selfCollider = GetComponent<Collider2D>();
        

        for (var i = 0; i < cellColliders.Length; i++) {
            if (cellColliders[i] == selfCollider) {
                continue;
            }
            
            var otherCell = cellColliders[i].gameObject.GetComponent<CellController>();
            if (otherCell == null) {
                continue;
            }
            var otherTransform = cellColliders[i].transform;
            
            var distance = DistanceToOtherCell(selfPosition, selfSize, otherTransform);

            if (otherCell.strength > strength) {
                var score = GetMoveCost(vision - distance) * (float) genes.fearModifier;
                if (score > scariestScore) {
                    scariestTransform = otherTransform;
                    scariestScore = score;
                }
            } else {
                var score = (otherCell.energy - GetMoveCost(distance)) * (float) genes.hungerModifier;
                if (score > tastiestScore) {
                    tastiestTransform = otherTransform;
                    tastiestScore = score;
                }
            }
        }
        if (tastiestScore > 0 && tastiestScore > scariestScore && tastiestTransform != null) {
            if (tired) {
                if (exhaustion > genes.stamina * sim.restThreshold) {
                    Rest();
                    return Vector2.zero;
                }
                else {
                    tired = false;
                }
            }

            if (exhaustion > genes.stamina) {
                tired = true;
                Rest();
                return Vector2.zero;
            }

            var otherPosition = tastiestTransform.position;
            exhaustion += GetMoveCost((float) genes.speed * Time.deltaTime) / energy;
            return new Vector2(otherPosition.x - transform.position.x, otherPosition.y - rb.position.y).normalized;
        } else if (scariestScore > 0.0f && scariestTransform != null) {
            Rest();
            var otherPosition = scariestTransform.transform.position;
            return new Vector2(transform.position.x - otherPosition.x, rb.position.y - otherPosition.y).normalized;
        } else {
            Rest();
            return Vector2.zero;
        }
    }

    private float DistanceToOtherCell(Vector3 selfPosition, float selfSize, Transform otherTransform) {
        return Vector3.Distance(selfPosition, otherTransform.position) - otherTransform.localScale.x - selfSize;
    }

    private void Rest() {
        if (exhaustion < 0.0f) {
            exhaustion = 0.0f;
        }
        else {
            exhaustion -= (float) (genes.hungerModifier * genes.speed * genes.moveCost) / energy;
        }
    }

    private float GetMoveCost(float distance) {
        return (float) (distance * genes.moveCost * energy);
    }

    private void Reproduce() {
        reproductionTimer += Time.deltaTime;
        if (energy > sim.minCellSize * 3) {
            if (reproductionTimer > genes.reproductionDelay) {
                energy /= 2.0f;
                CellSpawner.SpawnCell(energy:energy, parentGenes:genes);
                
                reproductionTimer = 0;
            } 
        } else if (reproductionTimer > genes.reproductionDelay * 2) {
            Destroy(gameObject);
        }
    }
}
