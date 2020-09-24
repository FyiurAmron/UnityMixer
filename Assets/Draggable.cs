using System;
using UnityEngine;

public class Draggable : MonoBehaviour {
    private Vector3 offset;
    private Vector3 originalPos;
    private float mousePosZ;

    private bool lastTarget = false;

    private Camera mainCamera;
    private Rigidbody myRigidbody;
    private AudioSource myAudioSource;

    public bool isXEnabled = true;
    public bool isYEnabled = true;
    public bool isZEnabled = true;

    public bool isKinematic = true;

    [Range( 0, float.MaxValue )]
    public float distanceLimit = float.MaxValue;

    public static T clamp<T>( T value, T min, T max ) where T : IComparable<T> {
        return ( value.CompareTo( min ) < 0 ) ? min
            : ( value.CompareTo( max ) > 0 ) ? max
            : value;
    }

    private Vector3 getMouseAsWorldPoint() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mousePosZ;
        return mainCamera.ScreenToWorldPoint( mousePos );
    }

    private void onDragStart() {
        if ( isKinematic && myRigidbody != null && !myRigidbody.isKinematic ) {
            myRigidbody.isKinematic = true;
            myRigidbody.detectCollisions = false;
            // myRigidbody.useGravity = false;
        }

        originalPos = transform.position;
        mousePosZ = mainCamera.WorldToScreenPoint( originalPos ).z;
        offset = originalPos - getMouseAsWorldPoint();
    }

    private void onDrag() {
        Vector3 src = transform.position;
        Vector3 dst = getMouseAsWorldPoint() + offset;

        bool[] axesEnabled = {isXEnabled, isYEnabled, isZEnabled};
        for ( int i = 0; i < 3; i++ ) {
            if ( !axesEnabled[i] ) {
                dst[i] = src[i];
            }
        }

        myAudioSource.volume = dst.x;
        myAudioSource.pitch = dst.y;
        myAudioSource.panStereo = dst.z;

        transform.position = dst;
    }

    private void onDragStop() {
        if ( isKinematic && myRigidbody != null && !myRigidbody.isKinematic ) {
            myRigidbody.isKinematic = false;
            myRigidbody.detectCollisions = true;
            // myRigidbody.useGravity = true;
        }

        // other stuff
    }

    private void handleDrag( bool isTriggerActive ) {
        bool isTargeted = isTriggerActive
            && Physics.Raycast( mainCamera.ScreenPointToRay( Input.mousePosition ), out RaycastHit hit )
            && hit.transform.name != transform.name;

        if ( !lastTarget ) {
            if ( !isTargeted ) {
                return;
            }

            lastTarget = true;
            // Debug.Log( "onDragStart" );
            onDragStart();

            return;
        }

        if ( !isTargeted ) {
            lastTarget = false;
            // Debug.Log( "onDragStop" );
            onDragStop();
            return;
        }

        // Debug.Log( "onDrag" );
        onDrag();
    }

    private void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
        // Debug.Log( String.Join<Component>( ",", GetComponents<Component>() ) );
    }

    void Update() {
        mainCamera = Camera.main;
        if ( mainCamera == null ) {
            return;
        }

        handleDrag( Input.GetMouseButton( 2 ) );
    }
}
