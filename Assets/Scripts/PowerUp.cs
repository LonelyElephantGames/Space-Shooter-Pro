using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int powerupID;  //0-triple shot; 1-speed; 2-shields
    [SerializeField] private AudioClip audioClip;
    

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if(transform.position.y < -6.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            //give the player the power up
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(audioClip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.EnableTripleShot();
                        break;
                    case 1:
                        player.EnableSpeedBoost();
                        break;
                    case 2:
                        player.EnableShield();
                        break;
                    case 3:
                        player.AddAmmo();
                        break;
                    case 4:
                        player.AddLife();
                        break;
                    default:
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
