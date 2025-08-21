using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIButtonScript : MonoBehaviour,IPointerEnterHandler
{
    //public GameObject pauseGameObject;
    //public GameObject pauseText;
    //private bool isPaused = false;
    
    
    //public GameObject cam;
    //private CameraMovement camMove;
    
    private void Start()
    {
        //sound = AudioManager.Instance;
        //camMove = cam.GetComponent<CameraMovement>();
        
    }

    public void LoadTitleMenu()
    {
        Debug.Log("LoadTitleMenu");
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene(0);
    }

    public void StartGameScene()
    {
        Debug.Log("StartGame");
        
    }
    public void LoadSceneByIndex(int i)
    {   
        Debug.Log("LoadSlantris");
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene(i);
    }
    
    
    // public void RestartGame()
    // {
    //     Debug.Log("RestartGame");
    //     sound.PlayClickSound();
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }


    // public void TogglePause()
    // {
    //     isPaused = !isPaused;
    //     if (isPaused)
    //     {
    //         Time.timeScale = 0f;
    //         pauseText.gameObject.SetActive(true);
    //         for (int i = 0; i < pauseGameObject.transform.childCount; i++)
    //         {
    //             pauseGameObject.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
    //         }
    //     }
    //     else
    //     {
    //         Time.timeScale = 1f;
    //         pauseText.gameObject.SetActive(false);
    //         for (int i = 0; i < pauseGameObject.transform.childCount; i++)
    //         {
    //             pauseGameObject.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
    //         }
    //     }
    // }
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        AudioManager.Instance.PlayButtonSound();
    }
}
