using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingDoorScript : MonoBehaviour {
    const string PLAYER_TAG = "Player";
    
    public float doorOpenAngle = -90.0f;
    public float openSpeed = 2.0f;

    private bool open = false;
    private bool canUse = false;

    private float defaultRotationAngle;
    private float currentRotationAngle;
    private float openTime = 0;

    private KeyCode keyCode = KeyCode.F;

    void Start() {
        defaultRotationAngle = transform.localEulerAngles.y;
        currentRotationAngle = defaultRotationAngle;
    }

    // Main function
    void Update() {
        if ( openTime < 1 ) {
            openTime += Time.deltaTime * openSpeed;
        }

        transform.localEulerAngles = new Vector3( transform.localEulerAngles.x,
                                                  Mathf.LerpAngle( currentRotationAngle,
                                                                   defaultRotationAngle + ( open ? doorOpenAngle : 0 ),
                                                                   openTime ), transform.localEulerAngles.z );

        if ( Input.GetKeyDown( keyCode ) && canUse ) {
            GetComponent<AudioSource>().Play();
            open = !open;
            currentRotationAngle = transform.localEulerAngles.y;
            openTime = 0;
        }
    }

    void OnGUI() {
        if ( canUse ) {
            GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height - 100, 150, 30 ),
                       $@"Press '{keyCode.ToString()}' to use the door" );
        }
    }

    void OnTriggerEnter( Collider other ) {
        if ( other.CompareTag( PLAYER_TAG ) ) {
            canUse = true;
        }
    }

    void OnTriggerExit( Collider other ) {
        if ( other.CompareTag( PLAYER_TAG ) ) {
            canUse = false;
        }
    }
}