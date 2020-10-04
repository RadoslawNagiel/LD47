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
    Vector3 pos;
    float teleportDelay;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        pos = transform.position;
        Restart();
    }

    void FixedUpdate()
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

    public void Restart()
    {
        transform.position = pos;
        mv = MovementToRepeate[0];
        index = 0;
        delay = 0;
        indexjump = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Finish")
        {
            ;
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
}
