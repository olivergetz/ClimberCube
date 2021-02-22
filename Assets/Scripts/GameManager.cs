using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    bool isPaused = false;

    readonly uint pauseStateGroupID = 3092587493U;
    readonly uint pauseStateID = 319258907U;
    readonly uint unPauseStateID = 1365518790U;

    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject victoryMenu;
    [SerializeField] public GameObject screenOverlay;
    [SerializeField] public GameObject fadeScreen;

    Scene scene;

    Image img;

    [SerializeField] Color blackScreen;
    [SerializeField] Color transparentScreen;

    void Awake()
    {
        //img = fadeScreen.GetComponent<Image>();
        Time.timeScale = 0;
        //img.tintColor = blackScreen;
    }

    private void Start()
    {
        //StartCoroutine(FadeIn());
        //Make sure the game is not paused
        Time.timeScale = 1;
        UnPause(pauseMenu, screenOverlay);
        isPaused = false;
        AkSoundEngine.SetState(pauseStateGroupID, unPauseStateID);

        scene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (!isPaused && Input.GetButtonDown("Pause"))
        {
            isPaused = true;
            Pause(pauseMenu, screenOverlay);
            //Update Audio
            AkSoundEngine.SetState(pauseStateGroupID, pauseStateID);
        }
        else if (isPaused && Input.GetButtonDown("Pause"))
        {
            isPaused = false;
            UnPause(pauseMenu, screenOverlay);
            //Update Audio
            AkSoundEngine.SetState(pauseStateGroupID, unPauseStateID);
        }     
    }

    public static void Pause(GameObject pMenu, GameObject overlay)
    {
        Time.timeScale = 0;
        pMenu.SetActive(true);
        overlay.SetActive(true);
    }

    public static void UnPause(GameObject pMenu, GameObject overlay)
    {
        Time.timeScale = 1;
        pMenu.SetActive(false);
        overlay.SetActive(false);
    }

    public static void WinGame(GameObject winMenu, GameObject overlay)
    {
        Time.timeScale = 0;
        if (!winMenu.activeInHierarchy)
        {
            winMenu.SetActive(true);
        }

        if (!overlay.activeInHierarchy)
        {
            overlay.SetActive(true);
        }
        
    }

    public void ResumeGame()
    {
        UnPause(pauseMenu, screenOverlay);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(scene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeIn()
    {
        for (float f = 1; f > 0.0f; f = f - 0.01f)
        {
            img.tintColor = Color.Lerp(transparentScreen, blackScreen, f);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
