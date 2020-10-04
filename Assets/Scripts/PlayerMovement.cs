using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float MovementSpeed = 1f;
    [SerializeField] float JumpForce = 1f;
    [SerializeField] GameObject Ghosts;
    [SerializeField] Animator anim;
    [SerializeField] GameManager GameManagerScript;

    Vector3 pos;
    Rigidbody2D _rigidbody;
    List<Movement> saveMove;
    float delay;
    float teleportDelay;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        saveMove = new List<Movement>();
        saveMove.Add( new Movement(0, false, 0));
        delay = 0;
        teleportDelay = 0;
        pos = transform.position;
        Physics2D.IgnoreLayerCollision(8, 8);
    }

    void FixedUpdate()
    {
        if (teleportDelay > 0)
            teleportDelay -= Time.deltaTime;

        bool jump = false;

        int movement = 0;
        if (Input.GetKey(KeyCode.A))
        {
            movement = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement = 1;
        }

        RaycastHit2D ray1 = Physics2D.Raycast(transform.position - new Vector3(.2f, .3f, 0), Vector2.down, 0.1f, LayerMask.GetMask("Default"));
        RaycastHit2D ray2 = Physics2D.Raycast(transform.position - new Vector3(-.2f, .3f, 0), Vector2.down, 0.1f, LayerMask.GetMask("Default"));
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0, 0.15f, 0), Vector2.right * movement, 0.3f, LayerMask.GetMask("Default"));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(0, 0.3f, 0), Vector2.right * movement, 0.3f, LayerMask.GetMask("Default"));
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position, Vector2.right * movement, 0.3f, LayerMask.GetMask("Default"));

        if (Input.GetKeyDown(KeyCode.Space) && (ray1 || ray2))
        {
            jump = true;
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }

        if (!hit1 && !hit2 && !hit3)
        {
            transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;
        }
        Movement mv = saveMove[saveMove.Count - 1];

        if (mv.movement != movement || mv.jump != jump)
        {
            saveMove[saveMove.Count - 1].time = delay;
            delay = 0;
            saveMove.Add(new Movement(movement, jump, 0));
        }
        delay += Time.deltaTime;


        if (!(ray1 || ray2))
        {
            anim.Play("Jump");
        }
        else if (movement == 1)
        {
            anim.Play("RunRight");
        }
        else if (movement == -1)
        {
            anim.Play("RunLeft");
        }
        else
        {
            anim.Play("Stay");
        }
    }

    public class Movement
    {
        public float movement;
        public bool jump;
        public float time;

        public Movement(float movement, bool jump, float time)
        {
            this.movement = movement;
            this.jump = jump;
            this.time = time;
        }

        public virtual string ToString()
        {
            return "Ruch: " + movement.ToString() + " Skok " + jump.ToString() + " Czas: " + time.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Finish")
        {
            for (int i = 0; i < Ghosts.transform.childCount; i++)
            {
                Ghosts.transform.GetChild(i).GetComponent<Repeate>().Restart();
            }
            saveMove[saveMove.Count - 1].time = delay;

            GameObject go = Instantiate(Resources.Load("Ghost") as GameObject, Ghosts.transform);
            go.transform.position = pos;
            go.GetComponent<Repeate>().MovementSpeed = MovementSpeed;
            go.GetComponent<Repeate>().JumpForce = JumpForce;
            go.GetComponent<Repeate>().MovementToRepeate = new List<Movement>(saveMove);

            saveMove.Clear();
            saveMove.Add(new Movement(0, false, 0));
            delay = 0;
            transform.position = pos;
            GameManagerScript.Build();
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
