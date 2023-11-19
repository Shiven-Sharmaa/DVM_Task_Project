

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.up;
    private List<Transform> _segments;
    public Transform BodyPrefab;
    public int deaths=0;
    public Text coinCollected;
    public int coins=0;
    public Text lives;

    private void Start(){
        _segments = new List<Transform>();
        _segments.Add(this.transform);
        
        StartCoroutine(WaitForFunction());
    }
    IEnumerator WaitForFunction()
{
    yield return new WaitForSeconds(3);
    Debug.Log("Hello!");  
}
    private void Update(){
        if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _direction!=Vector2.down){
            _direction=Vector2.up;
            transform.eulerAngles = new Vector3(0,0,0);
        }
        if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))&& _direction!=Vector2.right){
            _direction=Vector2.left;
            transform.eulerAngles = new Vector3(0,0,90);
        }
        if((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))&& _direction!=Vector2.left){
            _direction=Vector2.right;
            transform.eulerAngles = new Vector3(0,0,270);
        }
        if((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))&& _direction!=Vector2.up){
            _direction=Vector2.down;
            transform.eulerAngles = new Vector3(0,0,180);
        }
        //Debug.Log("Coins : %d",coinCollected);
        Debug.Log(deaths);
    }
    private void FixedUpdate(){
        for(int i=_segments.Count-1;i>0;i--){
            _segments[i].position=_segments[i-1].position;
        }

        this.transform.position = new Vector3(
            Mathf.RoundToInt(this.transform.position.x) + _direction.x,
            Mathf.RoundToInt(this.transform.position.y) + _direction.y,
            0.0f
        );
        coinCollected.text=coins.ToString();
        switch(deaths){
        default:
        case 0: lives.text="3";break;
        case 1: lives.text="2";break;
        case 2: lives.text="1";break;}
    }

    public void ResetState(){
        for(int i=1;i<_segments.Count;i++){
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        this.transform.position = Vector3.zero;
    }

    private void Grow(){
        Transform body = Instantiate(this.BodyPrefab);
        body.position = _segments[_segments.Count-1].position;
        _segments.Add(body);
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag=="Coin"){
            Grow();
            coins++;
        }
        else if(other.tag=="Obstacle"){
            ResetState();
            deaths++;
            if(deaths==3){
                SceneManager.LoadSceneAsync(2);
            }
        }
    }


}

/*
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate) {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero) {
            direction = input;
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls) {
                Traverse(other.transform);
            } else {
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

}*/