using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float score = 0f;
    private bool isPowerUpActive = false;
    private float powerUpDuration = 1.5f;
    private float powerUpTimer = 0f;
    private float cherry = 0f;
    public GameObject victoryCanvas;
    public Text scoreTotal;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()

    {
        scoreTotal.text = "Score: " + score.ToString();
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        if (movement.x != 0 && movement.y != 0)
        {
            if (Mathf.Abs(moveHorizontal) > Mathf.Abs(moveVertical))
            {
                movement.y = 0;
            }
            else
            {
                movement.x = 0;
            }
        }

        else if (movement == Vector2.zero)
        {
            movement = rb.velocity.normalized;
        }
        rb.velocity = movement * speed;


        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (isPowerUpActive)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                DeactivatePowerUp();
            }
        }
        // Prevent upside down rotation

        if (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270)
        {
            transform.rotation = Quaternion.Euler(180, 0, 180);
        }

        DestroyObjectWithTag();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("money"))
        {
            collision.gameObject.SetActive(false);
            score += 10;
        }
        if (collision.CompareTag("enemies") && !isPowerUpActive)
        {
            Destroy(gameObject);
            GameOver();
        }

        if (score >= 370)
        {
            ActivateVictoryCanvas();
            StartCoroutine(GameOverAfterDelay(3f));
        }

        if (collision.CompareTag("cherry"))
        {
            collision.gameObject.SetActive(false);
            ActivatePowerUp();
            cherry++;
        }
    }

    IEnumerator GameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameOver();
    }

    void ActivateVictoryCanvas()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Victory canvas is not assigned!");
        }
    }

    private void DestroyObjectWithTag()
    {
        GameObject targetObject1 = GameObject.FindWithTag("cherryCanvas");
        GameObject targetObject2 = GameObject.FindWithTag("cherryCanvas1");
        GameObject targetObject3 = GameObject.FindWithTag("cherryCanvas2");

        if (cherry == 1)
        {
            Destroy(targetObject1);
        }
        else if (cherry == 2)
        {
            Destroy(targetObject2);
        }
        else if (cherry == 3)
        {
            Destroy(targetObject3);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPowerUpActive && collision.gameObject.CompareTag("enemies"))
        {
            collision.transform.position = Vector2.zero;
            collision.gameObject.SetActive(false);
            StartCoroutine(ReactivateAfterDelay(collision.gameObject, 5f));
        }
    }

    IEnumerator ReactivateAfterDelay(GameObject obj, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Activate the game object
        obj.SetActive(true);
    }

    private void ActivatePowerUp()
    {
        isPowerUpActive = true;
        powerUpTimer = powerUpDuration;

    }

    private void DeactivatePowerUp()
    {
        isPowerUpActive = false;
    }

    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
