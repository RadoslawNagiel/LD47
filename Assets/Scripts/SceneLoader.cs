using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject CreditsText;

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowCredits()
    {
        CreditsText.SetActive(true);
    }
}
