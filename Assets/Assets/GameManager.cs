using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Enemie> ListEnemys;
    [Header("UI")]
    [SerializeField] GameObject CatchedMenu;
    [SerializeField] GameObject PauseMenu;
   public bool paused;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
    }

    private void Update()
    {
        CheckIfPlayerCatched();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
            {
                PauseMenu.SetActive(false);
                Time.timeScale = 1;
                Cursor.visible = false;
                paused = false;
            }
            else
            {
                PauseMenu.SetActive(true);
                Time.timeScale = 0;
                Cursor.visible = true;
                paused =true;
            }
        }
    

    }

    void CheckIfPlayerCatched()
    {
        foreach(Enemie enemie in ListEnemys)
        {
            if(enemie.Catched == true)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CatchedMenu.SetActive(true);
                break;
            }
        }
    }

    public void RestartGame()
    {
 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      
    }
}
