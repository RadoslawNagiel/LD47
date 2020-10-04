using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;

public class Repeate : MonoBehaviour
{
    public float MovementSpeed = 1f;
    public float JumpForce = 1f;
    public Animator anim;

    public List<Movement> MovementToRepeate;

    Rigidbody2D _rigidbody;
    float delay;
    Movement mv;
    int index;
    int indexjump;
    public Vector3 pos;
    float teleportDelay;
    float lostDelay;
    bool dead = false;
    bool move = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Restart();
    }

    void FixedUpdate()
    {
        if (dead)
        {
            if (lostDelay < 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                lostDelay -= Time.deltaTime;
            }
        }
        else if(move)
        {
            if (teleportDelay > 0)
                teleportDelay -= Time.deltaTime;

            if (mv.time < delay)
            {
                delay -= mv.time;
                index++;
                if (index < MovementToRepeate.Count)
                {
                    mv = MovementToRepeate[index];
                }
                else
                    mv = new Movement(0, false, float.MaxValue);
            }
            RaycastHit2D ray1 = Physics2D.Raycast(transform.position - new Vector3(.2f, .3f, 0), Vector2.down, 0.1f, LayerMask.GetMask("Default"));
            RaycastHit2D ray2 = Physics2D.Raycast(transform.position - new Vector3(-.2f, .3f, 0), Vector2.down, 0.1f, LayerMask.GetMask("Default"));
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0, 0.15f, 0), Vector2.right * mv.movement, 0.3f, LayerMask.GetMask("Default"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(0, 0.3f, 0), Vector2.right * mv.movement, 0.3f, LayerMask.GetMask("Default"));
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position, Vector2.right * mv.movement, 0.3f, LayerMask.GetMask("Default"));

            if (mv.jump && (ray1 || ray2))
            {
                if (indexjump != index)
                {
                    _rigidbody.velocity = new Vector2(0, 0);
                    _rigidbody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
                    indexjump = index;
                }
            }
            if (!hit1 && !hit2 && !hit3)
            {
                transform.position += new Vector3(mv.movement, 0, 0) * Time.deltaTime * MovementSpeed;
            }

            if (!(ray1 || ray2))
            {
                anim.Play("Jump");
            }
            else if (mv.movement == 1)
            {
                anim.Play("RunRight");
            }
            else if (mv.movement == -1)
            {
                anim.Play("RunLeft");
            }
            else
            {
                anim.Play("Stay");
            }

            delay += Time.deltaTime;
        }
    }

    public void Restart()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.simulated = true;
        transform.position = pos;
        mv = MovementToRepeate[0];
        index = 0;
        delay = 0;
        indexjump = 0;
        dead = false;
        move = false;
    }

    public void LetsGo()
    {
        move = true;
    }

    void lost()
    {
        _rigidbody.simulated = false;
        lostDelay = 1f;
        dead = true;
        move = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Finish")
        {
            collision.GetComponent<Finish>().BotEnter();
        }
        else if (collision.gameObject.tag == "Teleport")
        {
            if (teleportDelay <= 0)
            {
                if (collision.gameObject == collision.gameObject.GetComponentInParent<Teleport>().point1)
                {
                    transform.position = collision.gameObject.GetComponentInParent<Teleport>().point2.transform.position;
                }
                else
                {
                    transform.position = collision.gameObject.GetComponentInParent<Teleport>().point1.transform.position;
                }
                teleportDelay = 1f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
            GetComponent<ParticleSystem>().Play();
            collision.gameObject.GetComponent<Bomb>().Boom();
            lost();
        }
        else if (collision.gameObject.tag == "Spike")
        {
            GetComponent<ParticleSystem>().Play();
            lost();
        }
    }
}
