using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera BuildCamera;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] Canvas GameCanvas;
    [SerializeField] Canvas BuildCanvas;
    [SerializeField] Canvas EndCanvas;
    [SerializeField] Text EndScore;
    [SerializeField] Text Score;
    [SerializeField] Transform Objects;
    [SerializeField] PlayerMovement Player;
    [SerializeField] GameObject finish;
    [SerializeField] GameObject BuildText;
    [SerializeField] GameObject Frame;
    [SerializeField] GameObject Ghosts;

    float delay = 0;
    bool build = true;
    int points = 0;

    private void Start()
    {
        Build();
    }

    private void Update()
    {
        if (delay > 0)
            delay -= Time.unscaledDeltaTime;
        else if (delay <= 0 && !build)
        {
            GameCanvas.GetComponent<Animator>().Play("Nothing");
            build = true;
            Player.LetsGo();
        }
    }

    public void Build()
    {
        for (int i = 0; i < Ghosts.transform.childCount-1; i++)
        {
            Ghosts.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (points > 0)
        {
            BuildText.SetActive(true);
            Frame.SetActive(true);
            int x = Random.Range(0, 3);
            GameObject go;
            Vector3 pos = Frame.transform.position;
            pos.z = 0;
            switch (x)

            {
                case 0:
                    go = Instantiate(Resources.Load("bomb") as GameObject, Objects);
                    go.transform.position = pos;
                    break;
                case 1:
                    go = Instantiate(Resources.Load("spike") as GameObject, Objects);
                    go.transform.position = pos;
                    break;
                case 2:
                    go = Instantiate(Resources.Load("Wood") as GameObject, Objects);
                    go.transform.position = pos;
                    break;
            }
        }
        else
        {
            BuildText.SetActive(false);
            Frame.SetActive(false);
        }
        BuildCanvas.gameObject.SetActive(true);
        BuildCamera.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);
        foreach (Transform child in Objects)
        {
            child.GetComponent<MouseMove>().enabled = true;
            if (child.tag == "Bomb")
                child.GetComponent<Bomb>().Restart();
        }
    }

    public void Game()
    {
        Player.Restart();
        BuildCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(true);
        BuildCamera.gameObject.SetActive(false);
        delay = 3.1f;
        build = false;
        finish.GetComponent<Animator>().Play("Nothing");
        GameCanvas.GetComponent<Animator>().Play("Start");
        foreach (Transform child in Objects)
        {
            child.GetComponent<MouseMove>().enabled = false;
        }
    }

    public void GameOver()
    {
        int BestScore = 0;
        if (PlayerPrefs.HasKey("dwamkgrkmklvsc"))
        {
            BestScore = PlayerPrefs.GetInt("dwamkgrkmklvsc");
        }
        if (points > BestScore)
        {
            BestScore = points;
            PlayerPrefs.SetInt("dwamkgrkmklvsc", points);
            PlayerPrefs.Save();

        }
        EndScore.text = "SCORE: " + points + "\n BEST SCORE: " + BestScore;
        points = 0;
        EndCanvas.gameObject.SetActive(true);
        BuildCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(false);
        BuildCamera.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);
    }

    public void AddPoint()
    {
        points++;
        Score.text = "SCORE: " + points;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
