    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Spawning,
    Idle,
    Walking,
    Death
}

public class EnemyAI : MonoBehaviour
{
    Animator anim = null;
    //[HideInInspector]
    public Vector2 areaBounds = new Vector2();
    public EnemySpawner spawner = null;

    [Tooltip("The number of times the enemy divides before dying")]
    public int divideSize = 0;
    [SerializeField]
    bool sizeDownOnSplit = false;
    [SerializeField]
    bool facingLeft = false;
    [SerializeField]
    float moveSpeed = 0.0f;
    [SerializeField]
    float moveRadius = 0.0f;

    [SerializeField]
    float minTimeIdle = 0.0f;
    [SerializeField]
    float maxTimeIdle = 0.0f;
    float switchStateTimeStamp = 0.0f;
    [SerializeField]
    EnemyState currentState = EnemyState.Idle;
    [HideInInspector]
    public bool isSpawning = false;

    [Header("Debugging")]
    [SerializeField]
    Transform locator;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Spawn()
    {
        currentState = EnemyState.Spawning;
        isSpawning = true;
        StartCoroutine(Move(FindRandomPoint(0.5f), 2.0f));
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Spawning:
                if (!isSpawning)
                    StartIdle();
                break;
            case EnemyState.Idle:
                if (Time.time >= switchStateTimeStamp)
                {
                    currentState = EnemyState.Walking;
                    StartCoroutine(Move(FindRandomPoint()));
                }
                break;
            case EnemyState.Walking:
                break;
            case EnemyState.Death:
                break;
            default:
                break;
        }
    }

    void StartIdle()
    {
        if (isSpawning)
            isSpawning = false;
        currentState = EnemyState.Idle;
        switchStateTimeStamp = Time.time + Random.Range(minTimeIdle, maxTimeIdle);
        GetComponent<Animator>().SetFloat("speed", 0.0f);
        GetComponent<Animator>().SetFloat("IdleIndex", Random.Range(0, 2));
    }

    public Vector3 FindRandomPoint(float radiusFactor = 1.0f)
    {
        float possibleMoveRadius = moveRadius * radiusFactor;

        #region X Bounds Check
        float xMaxRadius = 0.0f;
        float xMinRadius = 0.0f;

        if (transform.position.x + possibleMoveRadius > areaBounds.x)
            xMaxRadius = areaBounds.x;
        else
            xMaxRadius = transform.position.x + possibleMoveRadius;

        if (transform.position.x - possibleMoveRadius < -areaBounds.x)
            xMinRadius = 0;
        else
            xMinRadius = transform.position.x - possibleMoveRadius;

        float x = Random.Range(xMinRadius, xMaxRadius);
        #endregion

        #region Y Bounds Check
        float yMaxRadius = 0.0f;
        float yMinRadius = 0.0f;

        if (transform.position.y + possibleMoveRadius > areaBounds.y)
            yMaxRadius = areaBounds.y;
        else
            yMaxRadius = transform.position.y + possibleMoveRadius;

        if (transform.position.y - possibleMoveRadius < -areaBounds.y)
            yMinRadius = 0;
        else
            yMinRadius = transform.position.y - possibleMoveRadius;

        float y = Random.Range(yMinRadius, yMaxRadius);
        #endregion
        
        return new Vector3(x, y, transform.position.z);
    }

    public void KillEnemy()
    {
        currentState = EnemyState.Death;

        //set animator to death animation
        //if (anim == null)
            OnDeath();
        //else
        //    anim.SetTrigger("DeathTrigger");
    }

    public void OnDeath()
    {
        if (divideSize > 0)
            Split();

        spawner.enemyCount--;
        Destroy(gameObject);
    }

    public void Split()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject newEnemy = Instantiate(gameObject, transform.position, transform.rotation);
            newEnemy.transform.localScale = new Vector3(transform.localScale.x / 1.2f, transform.localScale.y / 1.2f, transform.localScale.z / 1.2f);

            EnemyAI newAI = newEnemy.GetComponent<EnemyAI>();
            newAI.spawner = spawner;
            newAI.divideSize = divideSize - 1;
            newAI.Spawn();

            //Add one to the enemy count
            spawner.enemyCount++;
        }
    }

    public void PickDirection(Vector2 startLocation, Vector2 destination)
    {
        if (destination.x > startLocation.x)
            facingLeft = false;
        else
            facingLeft = true;

        //set animator to left movement
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(moveRadius * 2, moveRadius * 2));
    }

    #region Coroutines
    IEnumerator Move(Vector3 destination, float moveFactor = 1.0f)
    {
        float newMoveSpeed = moveSpeed * moveFactor;
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot.y = (destination.x > transform.position.x) ? 180 : 0;
        transform.rotation = Quaternion.Euler(newRot);
        if (locator != null)
            locator.position = destination;

        bool destinationReached = false;
        Vector3 startLocation = transform.position;

        PickDirection(startLocation, destination);
        float totalDist = Mathf.Abs(Vector3.Distance(startLocation, destination));

        GetComponent<Animator>().SetFloat("speed", 1.0f);
        while (!destinationReached)
        {
            float distanceLeft = Mathf.Abs(Vector3.Distance(transform.position, destination));
            float lerpAmount = ((totalDist - distanceLeft + (newMoveSpeed * Time.deltaTime)) / totalDist);

            //Debug.Log(distanceLeft + " / " + totalDist + "\nDestination: " + destination);
            transform.position = Vector3.Lerp(startLocation, destination, lerpAmount);

            if (lerpAmount >= 1)
                destinationReached = true;

            yield return new WaitForEndOfFrame();
        }

        StartIdle();

        yield break;
    }
    #endregion
}
