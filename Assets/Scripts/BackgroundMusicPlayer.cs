using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip introMusic;
    public AudioClip ghostNormalMusic;
    private AudioSource audioSource;
    private float timer = 0.0f;     
    private float switchTime; 
    private bool hasSwitched = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        switchTime = Mathf.Min(3.0f, introMusic.length);
        
        audioSource.clip = introMusic;
        audioSource.loop = false;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasSwitched)
        {
            return;
        }
        
        timer = timer + Time.deltaTime;
 
        if (timer >= switchTime)
        {
            hasSwitched = true;
            
            audioSource.clip = ghostNormalMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
    }
}
