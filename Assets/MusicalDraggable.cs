using UnityEngine;

public class MusicalDraggable : Draggable, IDragListener {
    [Range( 10, 22000 )]
    public int lpMin = 10;

    [Range( 10, 22000 )]
    public int lpMax = 22000;

    [Range( 0, 10 )]
    public float mouseSensitivity = 2.0f;

    private float yRotStart;
    private float yScaleStart;

    private AudioSource myAudioSource;
    private AudioLowPassFilter myLowPassFilter;

    public MusicalDraggable() {
        dragListener = this;
    }

    public void onDragStart( Draggable draggable ) {
        myAudioSource = draggable.GetComponent<AudioSource>();
        myLowPassFilter = draggable.GetComponent<AudioLowPassFilter>();
        Transform tf = draggable.transform;
        yRotStart = tf.eulerAngles.y;
        yScaleStart = tf.localScale.y;
    }

    public void onDrag( Draggable draggable, Vector3 dst ) {
        float mag = clamp( ( dst - draggable.originalPos ).sqrMagnitude * mouseSensitivity, 0, 1 );
        if ( Input.GetMouseButton( 1 ) ) {
            mag *= -1;
        }

        Transform tf = draggable.transform;

        if ( Input.GetKey( KeyCode.LeftShift ) ) {
            mag = clamp( ( yScaleStart - 1 ) + mag, 0, 1 );
            myAudioSource.volume = mag;
            tf.localScale = Vector3.one * ( mag + 1 );
        } else if ( Input.GetKey( KeyCode.LeftControl ) ) {
            mag += yRotStart / 90f;
            mag = clamp( mag, 0, 1 );
            myLowPassFilter.cutoffFrequency = Mathf.Lerp( lpMax, lpMin, mag );
            tf.eulerAngles = new Vector3( 0, 90.0f * mag, 0 );
        } else {
            // myAudioSource.pitch = dst.y;
            tf.position = dst;
        }
    }

    public void onDragStop( Draggable draggable ) {
    }
}
