using System;
using UnityEngine;

public class Draggable : MonoBehaviour {
    private Vector3 offset;
    public Vector3 originalPos; // hmm
    private float mousePosZ;

    private bool lastTarget = false;

    private Camera mainCamera;
    private Rigidbody myRigidbody;

    public bool isXEnabled = true;
    public bool isYEnabled = true;
    public bool isZEnabled = true;

    public bool isKinematic = true;

    // [Range( 0, 9 )]
    // public int mouseButtonNr = 0;

    [Range( 0, float.MaxValue )]
    public float distanceLimit = float.MaxValue;

    public IDragListener dragListener;

    public static T clamp<T>( T value, T min, T max ) where T : IComparable<T> {
        return ( value.CompareTo( min ) < 0 ) ? min
            : ( value.CompareTo( max ) > 0 ) ? max
            : value;
    }

    public Draggable() {
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
        dragListener?.onDragStart( this );
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

        dragListener?.onDrag( this, dst );
    }

    private void onDragStop() {
        if ( isKinematic && myRigidbody != null && !myRigidbody.isKinematic ) {
            myRigidbody.isKinematic = false;
            myRigidbody.detectCollisions = true;
            // myRigidbody.useGravity = true;
        }

        dragListener?.onDragStop( this );
    }

    private void handleDrag( bool isTriggerActive ) {
        bool isTargeted = isTriggerActive
            && Physics.Raycast( mainCamera.ScreenPointToRay( Input.mousePosition ), out RaycastHit hit )
            && hit.rigidbody == myRigidbody;

        if ( !lastTarget ) {
            if ( !isTargeted ) {
                return;
            }

            lastTarget = true;
            // Debug.Log( $"onDragStart {name}" );
            onDragStart();

            return;
        }

        if ( !isTargeted ) {
            lastTarget = false;
            // Debug.Log( $"onDragStop {name}" );
            onDragStop();
            return;
        }

        // Debug.Log( $"onDrag {name}" );
        onDrag();
    }

    private void Start() {
        // Debug.Log( String.Join<Component>( ",", GetComponents<Component>() ) );
        myRigidbody = GetComponent<Rigidbody>();

        // originalScale: assume 1,1,1 now, TODO
        // originalRot: assume 0,0,0, TODO
    }

    void Update() {
        mainCamera = Camera.main;
        if ( mainCamera == null ) {
            return;
        }

        // handleDrag( Input.GetMouseButton( mouseButtonNr ) );
        handleDrag( Input.GetMouseButton( 0 ) || Input.GetMouseButton( 1 ) );
    }
}

public interface IDragListener {
    public void onDragStart( Draggable draggable );
    public void onDrag( Draggable draggable, Vector3 dst );
    public void onDragStop( Draggable draggable );
}

