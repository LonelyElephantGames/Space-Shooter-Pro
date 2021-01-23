using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    [SerializeField] private int pointValue = 10;
    private Player player;
    private Animator anim;
    private AudioSource audioSource;
    [SerializeField] private GameObject laserPrefab;
    private float fireRate = 3.0f;
    private float canFire = -1.0f;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        if(player == null)
        {
            Debug.Log("Player is null...  oops!");
        }

        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.Log("Animator is null!");
        }

        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.Log("Enemy audiosource is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if(Time.time > canFire)
        {
            fireRate = Random.Range(3.0f, 7.0f);
            canFire = Time.time + fireRate;
            GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);       
        }
    }

    void CalculateMovement()
    {
transform.Translate(Vector3.down * speed * Time.deltaTime);

        if(transform.position.y < -7.0f)
        {
            float xPos = Random.Range(-9.0f, 9.0f);
            transform.position = new Vector3(xPos, 8f, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            anim.SetTrigger("OnEnemyDeath");
            speed = 0.5f;
            PlayExplosionAudio();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }   
        
        if (other.tag == "Laser")
        {
            if (player != null)
            {
                player.IncreaseScore(pointValue);
            }
            Destroy(other.gameObject);

            anim.SetTrigger("OnEnemyDeath");
            speed = 0;
            PlayExplosionAudio();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    void PlayExplosionAudio()
    {
        audioSource.Play();
    }
}
