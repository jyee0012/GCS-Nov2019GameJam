using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    GameObject trail = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trail != null) trail.transform.position = Input.mousePosition;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ClickEnemy(Input.mousePosition);
#endif

#if UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
                ClickEnemy(touch.position);
        }
#endif
    }

    public void ClickEnemy(Vector2 input)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(input);
        RaycastHit2D hitInfo = Physics2D.Raycast(point, Vector2.zero, 30.0f, LayerMask.GetMask("EnemyLayer"));

        if (hitInfo.collider != null)
        {
            EnemyAI enemy = hitInfo.collider.GetComponent<EnemyAI>();

            if (enemy != null && !enemy.isSpawning)
                enemy.KillEnemy();
        }
    }
}
