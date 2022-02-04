using System;
using UnityEngine;

public class CellSimulation : MonoBehaviour {
    public float brightness = 0.5f;
    public float photosynthesisScaleFactor = 1.95f;
    public float minCellSize = 150;
    public float restThreshold = 0.95f;
    private SpriteRenderer background;
    public static int CellCounter = 1;

    private void Start() {
        background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
    }

    private void Update() {
        background.color = Color.Lerp(new Color(0.067f, .094f, .165f), Color.white, brightness);
    }
}
