using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    bool start = false;
    float delay = 1f;
    private void Update()
    {
        if(start)
        {
            if (delay > 0)
            {
                if(delay<0.6)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                }
                delay -= Time.deltaTime;
            }
            else
                gameObject.SetActive(false);
        }
    }

    public void Boom()
    {
        if(start == false)
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<CircleCollider2D>().enabled = true;
            start = true;
            GetComponent<AudioSource>().Play();
        }
    }

    public void Restart()
    {
        start = false;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = false;
        gameObject.SetActive(true);
        delay = 1f;
    }
}
