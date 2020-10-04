using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera BuildCamera;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] Canvas GameCanvas;
    [SerializeField] Canvas BuildCanvas;

    float delay = 0;
    bool build = true;

    private void Start()
    {
        Build();
    }

    private void Update()
    {
        if (delay > 0)
            delay -= Time.unscaledDeltaTime;
        if (delay <= 0 && !build)
        {
            GameCanvas.GetComponent<Animator>().Play("Nothing");
            Time.timeScale = 1;
            GameCanvas.gameObject.SetActive(false);
            build = true;
        }
    }

    public void Build()
    {
        BuildCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        BuildCamera.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);
    }

    public void Game()
    {
        BuildCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(true);
        BuildCamera.gameObject.SetActive(false);
        delay = 3.1f;
        build = false;
        GameCanvas.GetComponent<Animator>().Play("Start");
    }


}
