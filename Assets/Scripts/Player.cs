using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int score;
    private float actualSpeed;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostedSpeed = 10.0f;
    [SerializeField] private float fireRate = 0.25f;
    private float canFire = 0.0f;
    [SerializeField] private int ammoCount = 15;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private bool isStarBurstActive = false;
    [SerializeField] private GameObject starBurstPrefab1;
    [SerializeField] private GameObject starBurstPrefab2;
    [SerializeField] private GameObject starBurstPrefab3;
    [SerializeField] private int lives = 3;
    private SpawnManager spawnManager;
    private UIManager uiManager;
    private bool isSpeedBoostActive = false;
    private bool isTripleShotActive = false;
    private bool isShieldActive = false;
    private int shieldStrength;
    private int maxShieldStrength = 3;
    [SerializeField] private GameObject tripleShotPrefab;
    [SerializeField] private GameObject shieldsObject;
    [SerializeField] private GameObject leftEngineFire;
    [SerializeField] private GameObject rightEngineFire;
    private AudioSource audioSource;
    [SerializeField] private AudioClip laserSoundClip;
    [SerializeField] private GameObject explosionAnim;
    [SerializeField] private GameObject mainCamera;
    private Vector3 initialCameraPosition;
    private float remainingShakeTime;
    [SerializeField] private float shakeDuration = 1.0f;
    [SerializeField] private float shakeStrength = 1.0f;
    private bool shouldCameraShake;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0f, -3.5f, 0f);

        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(spawnManager == null)
            Debug.LogError("The Spawn Manager is NULL!!");

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null)
            Debug.LogError("The UI Manager is null!");

        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.Log("Player audio source is null!");
        }
        else
        {
            audioSource.clip = laserSoundClip;
        }

        initialCameraPosition = mainCamera.transform.position;
        shouldCameraShake = false;
    }


    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            FireLaser();
        }

        //move at faster speed when left shift is held down
        if (Input.GetKey(KeyCode.LeftShift))
        {
            actualSpeed = boostedSpeed;
        }
        else
        {
            actualSpeed = baseSpeed;
        }

        //shake camera if needed
        if(remainingShakeTime <= 0)
        {
            mainCamera.transform.position = initialCameraPosition;
            shouldCameraShake = false;
        }
        else
        {
            mainCamera.transform.Translate(Random.insideUnitCircle * shakeStrength);
            remainingShakeTime -= Time.deltaTime;
        }
    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0f);

        if (isSpeedBoostActive)
        {
            transform.Translate(direction * actualSpeed * 2.0f * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * actualSpeed * Time.deltaTime);
        }
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 0f), 0);
     

        if (transform.position.x <= -11.25)
        {
            transform.position = new Vector3(11.25f, transform.position.y, 0f);
        }
        else if (transform.position.x >= 11.25)
        {
            transform.position = new Vector3(-11.25f, transform.position.y, 0f);
        }
    }


    void FireLaser()
    {
        if(ammoCount < 1 && isStarBurstActive == false)
        {
            return;
        }

        canFire = Time.time + fireRate;

        if (isStarBurstActive == true)
        {
            Instantiate(starBurstPrefab1, transform.position + new Vector3(-1f, 1.1f, 0f), Quaternion.identity);
            Instantiate(starBurstPrefab2, transform.position + new Vector3(0f, 1.1f, 0f), Quaternion.identity);
            Instantiate(starBurstPrefab3, transform.position + new Vector3(1f, 1.1f, 0f), Quaternion.identity);
            audioSource.Play();
        }
        else
        {
            if (isTripleShotActive == true)
            {
                Instantiate(tripleShotPrefab, transform.position + new Vector3(0f, 1.05f, 0f), Quaternion.identity);
            }
            else
            {
                Instantiate(laserPrefab, transform.position + new Vector3(0f, 1.05f, 0f), Quaternion.identity);
            }
            audioSource.Play();

            ammoCount--;
            uiManager.UpdateAmmoCount(ammoCount);
        }
    }


    public void Damage()
    {
        //give the shields 3 hits before removing them
        if (isShieldActive == true)
        {
            shieldStrength -= 1;
            DisplayShields();
            return;
        }
                
        lives--;
        shouldCameraShake = true;
        shakeStrength = ( 3f - lives ) / 10f;
        remainingShakeTime = shakeDuration;
        uiManager.UpdateLivesImage(lives);

        if(lives == 2)
        {
            leftEngineFire.SetActive(true);
        }

        if (lives == 1)
        {
            rightEngineFire.SetActive(true);
        }

        if (lives < 1)
            {
                spawnManager.OnPlayerDeath();
                Instantiate(explosionAnim, transform.position, Quaternion.identity);
                Destroy(this.gameObject, 0.25f);
            }       
    }

    public void AddLife()
    {
        lives++;
        if(lives > 3)
        {
            lives = 3;
        }

        uiManager.UpdateLivesImage(lives);

        //update visual damage
        leftEngineFire.SetActive(false);
        rightEngineFire.SetActive(false);

        if (lives == 2)
        {
            leftEngineFire.SetActive(true);
        }
    }

    public void EnableTripleShot()
    {
        isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        isTripleShotActive = false;
    }


    public void EnableSpeedBoost()
    {
        isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        isSpeedBoostActive = false;
    }


    public void EnableShield()
    {
        shieldStrength = maxShieldStrength;
        DisplayShields();
    }

    public void AddAmmo()
    {
        ammoCount = 15;
        uiManager.UpdateAmmoCount(ammoCount);
    }

    public void ActivateStarburst()
    {
        isStarBurstActive = true;
        StartCoroutine(StarburstPowerDown());
    }

    IEnumerator StarburstPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        isStarBurstActive = false;
    }

    public void IncreaseScore(int pointValue)
    {
        score += pointValue;
        uiManager.UpdateScoreText(score);
    }

    void DisplayShields()
    {
        isShieldActive = true;
        shieldsObject.SetActive(true);
        Color temp = shieldsObject.GetComponent<SpriteRenderer>().color;

        if(shieldStrength == 3)
        {
            temp.a = 1f;
        }
        else if(shieldStrength == 2)
        {
            temp.a = 0.30f;
        }
        else
        {
            temp.a = 0.075f;
        }

        shieldsObject.GetComponent<SpriteRenderer>().color = temp;

        if(shieldStrength < 1)
        {
            isShieldActive = false;
            shieldsObject.SetActive(false);
        }
    }
}
