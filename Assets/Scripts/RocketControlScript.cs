using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControlScript : MonoBehaviour
{
    private int MissleNum;
    private GameObject Missle;
    private int direction;
    private PlayerMovementScript PlayerMovement;
    void Start()
    {
        MissleNum = 1;
        Missle = GameObject.Find("Missle");
        PlayerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
        direction = PlayerMovement.GetDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(MissleNum > 0)
            {
                Vector3 current = Missle.transform.position;
                direction = PlayerMovement.GetDirection();
                //print("当前方向为：");
                //print(direction);
                current[Mathf.Abs(direction) - 1] += 3 * direction / Mathf.Abs(direction);
                Missle.transform.position = current;
            }
        }
    }
}
