using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Countdown, Playing, Dying, GameOver }
    public static GameState gameState;
    public PacStudentController pacStudent;
    public GhostController[] ghosts;
    public LevelGrid levelGrid;
    public HUDManager hud;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip introMusic;
    public AudioClip normalMusic;
    public AudioClip scaredMusic;
    public AudioClip deadMusic;
    public AudioClip wallClip;
    public AudioClip cherryClip;
    public AudioClip ghostEatClip;
    public AudioClip deathClip;

    public string scoreKey = "Level1HighScore";
    public string timeKey = "Level1BestTime";
    private int score = 0;
    private int lives = 3;
    private float gameTime = 0f;
    private float scaredTimer = 0f;
 
    void Start()
    {
        gameState = GameState.Countdown;
        StartCoroutine(Countdown());
    }
 
    void Update()
    {
        if (gameState == GameState.Playing || gameState == GameState.Dying)
        {
            gameTime += Time.deltaTime;
            hud.SetTimer(gameTime);
        }
        if (gameState == GameState.Playing && scaredTimer > 0f)
        {
            UpdateScaredTimer();
        }
    }

    IEnumerator Countdown()
    {
        PlayMusic(introMusic, false);
        string[] steps = { "3", "2", "1", "GO!" };
        for (int i = 0; i < steps.Length; i++)
        {
            hud.ShowMessage(steps[i]);
            yield return new WaitForSeconds(1f);
        }
        hud.HideMessage();
        musicSource.Stop();  
        gameState = GameState.Playing;
        pacStudent.SetCollisions(true);
        UpdateMusic();
    }
    public void PelletEaten()
    {
        AddScore(10);
        CheckAllPelletsEaten();
    }
    public void PowerPelletEaten()
    {
        AddScore(50);
        scaredTimer = 10f;
 
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i].state != GhostController.GhostState.Dead)
            {
                ghosts[i].SetState(GhostController.GhostState.Scared);
            }
        }
        hud.ShowGhostTimer(scaredTimer);
        UpdateMusic();
        CheckAllPelletsEaten();
    }
 
    public void CherryEaten()
    {
        AddScore(100);
        PlaySFX(cherryClip);
    }
 
    public void PlayWallSound()
    {
        PlaySFX(wallClip);
    }
 
    public void GhostCollision(GhostController ghost)
    {
        if (gameState != GameState.Playing)
        {
            return;
        }
        if (ghost.state == GhostController.GhostState.Normal)
        {
            StartCoroutine(PacStudentDies());
        }
        else if (ghost.state == GhostController.GhostState.Scared || ghost.state == GhostController.GhostState.Recovering)
        {
            ghost.SetState(GhostController.GhostState.Dead);
            PlaySFX(ghostEatClip);
            AddScore(300);
            UpdateMusic();
            StartCoroutine(ReviveGhost(ghost));
        }
    }
    public void GhostRevived(GhostController ghost)
    {
        if (scaredTimer <= 0f)
        {
            ghost.SetState(GhostController.GhostState.Normal);
        }
        else if (scaredTimer <= 3f)
        {
            ghost.SetState(GhostController.GhostState.Recovering);
        }
        else
        {
            ghost.SetState(GhostController.GhostState.Scared);
        }
        UpdateMusic();
    }
    void UpdateScaredTimer()
    {
        scaredTimer -= Time.deltaTime;
        if (scaredTimer <= 0f)
        {
            scaredTimer = 0f;
            hud.HideGhostTimer();
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i].state != GhostController.GhostState.Dead)
                {
                    ghosts[i].SetState(GhostController.GhostState.Normal);
                }
            }
            UpdateMusic();
        }
        else
        {
            hud.ShowGhostTimer(scaredTimer);
            if (scaredTimer <= 3f)
            {
                for (int i = 0; i < ghosts.Length; i++)
                {
                    if (ghosts[i].state == GhostController.GhostState.Scared)
                    {
                        ghosts[i].SetState(GhostController.GhostState.Recovering);
                    }
                }
            }
        }
    }

    IEnumerator ReviveGhost(GhostController ghost)
    {
        yield return new WaitForSeconds(3f);
        if (ghost.state == GhostController.GhostState.Dead)
        {
            if (scaredTimer <= 0f)
            {
                ghost.SetState(GhostController.GhostState.Normal);
            }
            else if (scaredTimer <= 3f)
            {
                ghost.SetState(GhostController.GhostState.Recovering);
            }
            else
            {
                ghost.SetState(GhostController.GhostState.Scared);
            }
            UpdateMusic();
        }
    }
    IEnumerator PacStudentDies()
    {
        gameState = GameState.Dying;
        lives--;
        hud.SetLives(lives);
        pacStudent.Die();
        StopMusic();
        PlaySFX(deathClip);
        
        yield return new WaitForSeconds(deathClip.length);
 
        if (lives <= 0)
        {
            StartCoroutine(GameOver());
        }
        else
        {
            scaredTimer = 0f;
            hud.HideGhostTimer();
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].GhostReset();
            }
            pacStudent.Respawn();
            pacStudent.SetCollisions(true);
            gameState = GameState.Playing;
            UpdateMusic();
        }
    }
    void CheckAllPelletsEaten()
    {
        if (levelGrid.GetPelletsLeft() == 0 && gameState == GameState.Playing)
        {
            StartCoroutine(GameOver());
        }
    }
 
    IEnumerator GameOver()
    {
        gameState = GameState.GameOver;
        pacStudent.Freeze();
        StopMusic();
        hud.ShowMessage("Game Over");
        hud.DisableButtons();
        SaveHighScore();
 
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("StartScene");
    }
    void SaveHighScore()
    {
        int bestScore = PlayerPrefs.GetInt(scoreKey);
        float bestTime = PlayerPrefs.GetFloat(timeKey);
        
        if (!PlayerPrefs.HasKey(scoreKey) || score > bestScore || (score == bestScore && gameTime < bestTime))
        {
            PlayerPrefs.SetInt(scoreKey, score);
            PlayerPrefs.SetFloat(timeKey, gameTime);
            PlayerPrefs.Save();
        }
    }
    void AddScore(int points)
    {
        score += points;
        hud.SetScore(score);
    }
    void PlaySFX(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.Play();
    }
    void PlayMusic(AudioClip clip, bool loop)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;  
    }
    void UpdateMusic()
    {
        AudioClip clip = normalMusic;
        if (AnyGhostDead())
        {
            clip = deadMusic;
        }
        else if (scaredTimer > 0f)
        {
            clip = scaredMusic;
        }
        if (musicSource.clip != clip)
        {
            PlayMusic(clip, true);
        }
    }

    void FreezeGhost()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].Freeze();
        }
    }
    bool AnyGhostDead()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i].state == GhostController.GhostState.Dead)
            {
                return true;
            }
        } 
        return false;
    }
}
