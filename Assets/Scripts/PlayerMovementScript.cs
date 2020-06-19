using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour {
    public bool canMove = false;
    public float timeForMove = 0.2f;
    public float jumpHeight = 1.0f;

    public int minX = -8;
    public int maxX = 8;

    public GameObject[] leftSide;
    public GameObject[] rightSide;

    public float leftRotation = -45.0f;
    public float rightRotation = 90.0f;

    private bool moving;
    private float elapsedTime;

    private Vector3 current;
    private Vector3 target;
    private float startY;
    private int direction;

    private Rigidbody body;
    private GameObject mesh;

    private GameStateControllerScript gameStateController;
    private int score;

    //private Animator anim;

    public void Start() {
        mesh = GameObject.FindGameObjectWithTag("Player");
        body = GetComponentInChildren<Rigidbody>();
        direction = 1;
        moving = false;
        startY = mesh.transform.position.y;
        mesh.transform.position = new Vector3(0, 1.5f, 1);
        current = mesh.transform.position;
        score = 0;
        gameStateController = GameObject.Find("GameStateController").GetComponent<GameStateControllerScript>();

        //anim = gameObject.GetComponent<Animator>();
    }

    public void Update() {
        // If player is moving, update the player position, else receive input from user.
        if (moving)
            MovePlayer();
        else {
            // Update current to match integer position (not fractional).
            //print(current);
            current = new Vector3(Mathf.Round(mesh.transform.position.x), mesh.transform.position.y, Mathf.Round(mesh.transform.position.z));
            
            if (canMove)
                HandleInput();
        }

        score = Mathf.Max(score, (int)current.z);
        gameStateController.score = score / 3;
    }
	
	private void HandleMouseClick() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit)) {
			var direction = hit.point - transform.position;
			var x = direction.x;
			var z = direction.z;
			
			if (Mathf.Abs(z) > Mathf.Abs(x)) {
				if (z > 0)
					Move(new Vector3(0, 0, 3));
                else
					Move(new Vector3(0, 0, -3));
			}
            else { // (Mathf.Abs(z) < Mathf.Abs(x))
				if (x > 0) {
					if (Mathf.RoundToInt(current.x) < maxX)
						Move(new Vector3(1, 0, 0));
				}
                else { // (x < 0)
					if (Mathf.RoundToInt(current.x) > minX)
						Move(new Vector3(-1, 0, 0));
				}
			}
        }
	}

    private void HandleInput() {	
		// Handle mouse click
		if (Input.GetMouseButtonDown(0)) {
			HandleMouseClick();
			return;
		}
		
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(new Vector3(0, 0, 3));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(new Vector3(0, 0, -3));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (current.x > minX)
                Move(new Vector3(-2, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (current.x < maxX)
                Move(new Vector3(2, 0, 0));
        }
    }

    private void Move(Vector3 distance) {
        var newPosition = current + distance;
        target = newPosition;
        //print(MoveDirection);
        switch (MoveDirection)
        {
            case "north":
                direction = 3;
                mesh.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case "south":
                direction = -3;
                mesh.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case "east":
                direction = -1;
                mesh.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case "west":
                direction = 1;
                mesh.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            default:
                break;
        }
        // Don't move if blocked by obstacle.
        if (Physics.CheckSphere(newPosition + new Vector3(0.0f, 0.5f, 0.0f), 0.1f) ||
            Physics.CheckSphere(newPosition + new Vector3(0.0f, -0.5f, 0.0f), 0.1f)  ||
            Physics.CheckSphere(newPosition + new Vector3(0.5f, 0.0f, 0.0f), 0.1f) ||
            Physics.CheckSphere(newPosition + new Vector3(-0.5f, 0.5f, 0.0f), 0.1f)) 
            return;

        
        moving = true;
        elapsedTime = 0;
        body.isKinematic = true;
        



        // Rotate arm and leg.
        foreach (var o in leftSide) {
            o.transform.Rotate(leftRotation, 0, 0);
        }

        foreach (var o in rightSide) {
            o.transform.Rotate(rightRotation, 0, 0);
        }
    }

    private void MovePlayer() {
        elapsedTime += Time.deltaTime;

        float weight = (elapsedTime < timeForMove) ? (elapsedTime / timeForMove) : 1;
        float x = Lerp(current.x, target.x, weight);
        float z = Lerp(current.z, target.z, weight);
        float y = Sinerp(current.y, startY + jumpHeight, weight);

        //anim.SetBool("isJumping", true);
        Vector3 result = new Vector3(x, y, z);
        mesh.transform.position = result;
        //print(mesh.transform.position);
        //body.MovePosition(result);

        if (result == target) {
            moving = false;
            //anim.SetBool("isJumping", false);
            current = target;
            body.isKinematic = false;
            body.AddForce(0, -10, 0, ForceMode.VelocityChange);

            // Return arm and leg to original position.
            foreach (var o in leftSide) {
                o.transform.rotation = Quaternion.identity;
            }

            foreach (var o in rightSide) {
                o.transform.rotation = Quaternion.identity;
            }
        }
    }

    private float Lerp(float min, float max, float weight) {
        return min + (max - min) * weight;
    }

    private float Sinerp(float min, float max, float weight) {
        return min + (max - min) * Mathf.Sin(weight * Mathf.PI);
    }

    public bool IsMoving {
        get { return moving; }
    }

    public string MoveDirection {
        get
        {
            /*
            if (moving) {
                float dx = target.x - current.x;
                float dz = target.z - current.z;
                if (dz > 0)
                    return "north";
                else if (dz < 0)
                    return "south";
                else if (dx > 0)
                    return "west";
                else
                    return "east";
            }
            else
                return null;*/

            float dx = target.x - current.x;
            float dz = target.z - current.z;
            if (dz > 0)
                return "north";
            else if (dz < 0)
                return "south";
            else if (dx > 0)
                return "west";
            else
                return "east";
        }
    }

    public int GetDirection()
    {
        return direction;
    }
    public void GameOver() {
        // When game over, disable moving.
        canMove = false;

        // Call GameOver at game state controller (instead of sending messages).
        gameStateController.GameOver();
    }

    public void Reset() {
        // TODO This kind of reset is dirty, refactor might be needed.
        transform.position = new Vector3(0, 1.5f, 1);
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = Quaternion.identity;
        score = 0;
        direction = 1;
    }
}
