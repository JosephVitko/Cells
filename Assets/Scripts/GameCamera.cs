using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameCamera : MonoBehaviour {
    private Camera camera;
    private Transform transform;
    public float cameraMoveSpeed = 0.001f;

    private void Start() {
        camera = gameObject.GetComponent<Camera>();
        transform = gameObject.GetComponent<Transform>();
    }

    private void Update() {
        // use arrow keys to navigate
            
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(-transform.right * camera.orthographicSize * cameraMoveSpeed);
        }
            
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(transform.right * camera.orthographicSize * cameraMoveSpeed);
        } 
            
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(transform.up * camera.orthographicSize * cameraMoveSpeed);
        }
            
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(-transform.up * camera.orthographicSize * cameraMoveSpeed);
        }
            
        // use scroll wheel to move in and out
            
        if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
            camera.orthographicSize++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
            if (camera.orthographicSize > 1)
                camera.orthographicSize--;
        }
        
        // use space bar to spawn a new cell
        if (Input.GetKeyDown(KeyCode.Space)) {
            CellSpawner.SpawnCell(x:transform.position.x, y:transform.position.y);
        }
        
        // use p key to populate scene with 100 random cells.
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("P pressed");
            for (int i = 0; i < 100; i++) {
                CellSpawner.SpawnCell();
            }
        }
    }
}