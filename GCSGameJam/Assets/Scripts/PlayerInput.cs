using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(point, Vector2.zero, 30.0f, LayerMask.GetMask("EnemyLayer"));

            if (hitInfo.collider != null)
            {
                EnemyAI enemy = hitInfo.collider.GetComponent<EnemyAI>();

                if (enemy != null && !enemy.isSpawning)
                    enemy.KillEnemy();
            }
        }
#endif

#if UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {

            }
        }
#endif
    }
}
