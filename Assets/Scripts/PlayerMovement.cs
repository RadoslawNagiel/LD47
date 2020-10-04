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
    float lostDelay;
    bool dead = false;
    bool move = false;
    bool points = false;

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
        if(move)
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
                GetComponent<AudioSource>().Play();
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
        if(dead && !points)
        {
            if (lostDelay < 0) 
            {
                GameManagerScript.GetComponent<GameManager>().GameOver();
                gameObject.transform.Find("Character").gameObject.SetActive(false);
                points = true;
            }
            else
            {
                lostDelay -= Time.deltaTime;
            }
        }
    }

    public void LetsGo()
    {
        move = true;
        for (int i = 0; i < Ghosts.transform.childCount; i++)
        {
            Ghosts.transform.GetChild(i).GetComponent<Repeate>().LetsGo();
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
    }

    public void Restart()
    {
        _rigidbody.simulated = true;
        transform.position = pos;
        dead = false;
        gameObject.transform.Find("Character").gameObject.SetActive(true);
        for (int i = 0; i < Ghosts.transform.childCount; i++)
        {
            Ghosts.transform.GetChild(i).gameObject.SetActive(true);
            Ghosts.transform.GetChild(i).GetComponent<Repeate>().Restart();
        }
        gameObject.SetActive(true);
    }

    public void lost()
    {
        if(!dead)
        {
            lostDelay = 2f;
            points = false;
            move = false;
            dead = true;
        }
    }

    public void GenerateGhost()
    {
        saveMove[saveMove.Count - 1].time = delay+0.2f;

        GameObject go = Instantiate(Resources.Load("Ghost") as GameObject, Ghosts.transform);
        go.GetComponent<Repeate>().MovementSpeed = MovementSpeed;
        go.GetComponent<Repeate>().pos = pos;
        go.GetComponent<Repeate>().JumpForce = JumpForce;
        go.GetComponent<Repeate>().MovementToRepeate = new List<Movement>(saveMove);

        saveMove.Clear();
        saveMove.Add(new Movement(0, false, 0));
        delay = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Finish")
        {
            gameObject.transform.Find("Character").gameObject.SetActive(false);
            collision.GetComponent<Finish>().PlayerEnter();
            move = false;
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
            _rigidbody.simulated = false;
            collision.gameObject.GetComponent<Bomb>().Boom();
            GetComponent<ParticleSystem>().Play();
            lost();
        }
        else if (collision.gameObject.tag == "Spike")
        {
            collision.gameObject.GetComponent<AudioSource>().Play();
            GetComponent<ParticleSystem>().Play();
            lost();
        }
    }
}
