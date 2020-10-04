using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] GameManager gamemanager;
    [SerializeField] PlayerMovement PlayerScript;
    float delay;
    bool start = false;

    void Update()
    {
        if (start == true)
        {
            if (delay > 0)
                delay -= Time.deltaTime;
            else
            {
                GetComponent<Animator>().Play("FlyAnim");
                start = false;
                PlayerScript.GenerateGhost();
                gamemanager.AddPoint();
                gamemanager.Build();
            }
        }
    }

    public void PlayerEnter()
    {
        if(!start)
        {
            delay = 3f;
            start = true;
            GetComponent<Animator>().Play("TimeToEnd");
        }
    }

    public void BotEnter()
    {
        if(!(start && delay <= 0))
        {
            GetComponent<Animator>().Play("Nothing");
            start = false;
            PlayerScript.lost();
        }
    }
}
