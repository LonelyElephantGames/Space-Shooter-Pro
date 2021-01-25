using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBurst : MonoBehaviour
{

    [SerializeField] private float speed = 8f;
    [SerializeField] private float direction = 0;

    void Update()
    {
        //for now - single shot straight up
        //final version will have random trajectory right or left
        transform.position += new Vector3(direction * Time.deltaTime, speed * Time.deltaTime, 0f);

        if (transform.position.y > 8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
