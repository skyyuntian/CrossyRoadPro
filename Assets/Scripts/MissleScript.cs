using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleScript : MonoBehaviour
{
    public float speedX;
    private Rigidbody playerBody;
    private int direction;
    private GameObject player;
    private PlayerMovementScript PlayerMovement;
    private bool fired;

    public GameObject explodePrefab;
    public float explodeLifetime;

    public GameObject flamePrefab;
    private bool justFire;

    public GameObject flame;

    void Start()
    {
        speedX = 100.0f;
        fired = false;
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement = player.GetComponent<PlayerMovementScript>();
        direction = 3;
        justFire = false;
        flame = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fired = true;
            speedX = 20.0f;
        }
        if (!fired)
        {
            direction = PlayerMovement.GetDirection();
            //print(direction);
            switch (direction)
            {
                case 3:
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case -3:
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                case -1:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 1:
                    transform.rotation = Quaternion.Euler(0,180, 0);
                    break;
                default:
                    break;
            }
            float x1 = Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * speedX);
            float y1 = Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * speedX);
            float z1 = Mathf.Lerp(transform.position.z, player.transform.position.z, Time.deltaTime * speedX);
            transform.position = new Vector3(x1, y1, z1);

        }
        if (fired)
        {
            if (!justFire)
            {
                flame = (GameObject)Instantiate(flamePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                justFire = true;
            }
            if (Mathf.Abs(direction) == 3)
            {
                transform.position += new Vector3(0.0f, 0.0f, 
                    speedX * Time.deltaTime * direction/Mathf.Abs(direction));
                if (flame)
                {
                    flame.transform.position = transform.position;
                }
            }
            else if(Mathf.Abs(direction) == 1)
            {
                transform.position += new Vector3(speedX * Time.deltaTime * direction / Mathf.Abs(direction)
                    , 0.0f,0.0f);
                if (flame) {
                    flame.transform.position = transform.position;
                }
            }
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // When collide with player, flatten it!
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Environment")
        {
            //other.gameObject.SetActive(false);
            var o = (GameObject)Instantiate(explodePrefab, other.transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(other.gameObject);

            Destroy(flame);
            Destroy(this.gameObject);

            Destroy(o, explodeLifetime);
        }
    }
}
