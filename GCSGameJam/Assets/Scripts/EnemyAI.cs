    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Walking,
    Death
}

public class EnemyAI : MonoBehaviour
{
    Animator anim = null;
    //[HideInInspector]
    public Vector2 areaBounds = new Vector2();

    [SerializeField]
    [Tooltip("The number of times the enemy divides before dying")]
    int divideSize = 0;
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

    [Header("Debugging")]
    [SerializeField]
    Transform locator;

    // Start is called before the first frame update
    void Start()
    {
        switchStateTimeStamp = Time.time + Random.Range(minTimeIdle, maxTimeIdle);
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
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
        currentState = EnemyState.Idle;
        switchStateTimeStamp = Time.time + Random.Range(minTimeIdle, maxTimeIdle);
    }

    public Vector3 FindRandomPoint()
    {
        #region X Bounds Check
        float xMaxRadius = 0.0f;
        float xMinRadius = 0.0f;

        if (transform.position.x + moveRadius > areaBounds.x)
            xMaxRadius = areaBounds.x;
        else
            xMaxRadius = transform.position.x + moveRadius;

        if (transform.position.x - moveRadius < -areaBounds.x)
            xMinRadius = 0;
        else
            xMinRadius = transform.position.x - moveRadius;

        float x = Random.Range(xMinRadius, xMaxRadius);
        #endregion

        #region Y Bounds Check
        float yMaxRadius = 0.0f;
        float yMinRadius = 0.0f;

        if (transform.position.y + moveRadius > areaBounds.y)
            yMaxRadius = areaBounds.y;
        else
            yMaxRadius = transform.position.y + moveRadius;

        if (transform.position.y - moveRadius < -areaBounds.y)
            yMinRadius = 0;
        else
            yMinRadius = transform.position.y - moveRadius;

        float y = Random.Range(yMinRadius, yMaxRadius);
        #endregion

        return new Vector3(x, y, transform.position.z);
    }

    IEnumerator Move(Vector3 destination)
    {
        if (locator != null)
            locator.position = destination;

        bool destinationReached = false;
        Vector3 startLocation = transform.position;

        PickDirection(startLocation, destination);
        float totalDist = Mathf.Abs(Vector3.Distance(startLocation, destination));

        while (!destinationReached)
        {
            float distanceLeft = Mathf.Abs(Vector3.Distance(transform.position, destination));
            float lerpAmount = ((totalDist - distanceLeft + (moveSpeed * Time.deltaTime)) / totalDist);

            Debug.Log(distanceLeft + " / " + totalDist + "\nDestination: " + destination);
            transform.position = Vector3.Lerp(startLocation, destination, lerpAmount);

            if (lerpAmount >= 1)
                destinationReached = true;

            yield return new WaitForEndOfFrame();
        }

        StartIdle();

        yield break;
    }

    public void KillEnemy()
    {
        currentState = EnemyState.Death;

        //set animator to death animation
        if (anim == null)
            OnDeath();
        else
            anim.SetTrigger("DeathTrigger");
    }

    public void OnDeath()
    {
        if (divideSize > 0)
            Split();

        gameObject.SetActive(false);
    }

    public void Split()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject newEnemy = Instantiate(gameObject, transform.position, transform.rotation);

            EnemyAI newAI = newEnemy.GetComponent<EnemyAI>();
            newAI.divideSize = divideSize - 1;
            newAI.currentState = EnemyState.Idle;
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
}
