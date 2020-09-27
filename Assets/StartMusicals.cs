using System.Linq;
using UnityEngine;

public class StartMusicals : MonoBehaviour {
    private AudioSource[] musicalAudioSources;
    
    // Start is called before the first frame update
    void Start() {
        musicalAudioSources = GameObject
                              .FindGameObjectsWithTag( "Musical" )
                              .Select( x => x.GetComponent<AudioSource>() )
                              .ToArray();
    }

    void Update()
    {
        for ( int i = 1; i < musicalAudioSources.Length; i++ ) {
            // musicalAudioSources[i].timeSamples = musicalAudioSources[i - 1].timeSamples;
        }
    }

    void OnMouseDown() {
        foreach ( AudioSource audioSource in musicalAudioSources ) {
            audioSource.Play();
        }
        
    }

    void OnTriggerEnter( Collider other ) {
        // foreach ( AudioSource audioSource in musicalAudioSources ) {
        //     audioSource.Play();
        // }
    }
}
