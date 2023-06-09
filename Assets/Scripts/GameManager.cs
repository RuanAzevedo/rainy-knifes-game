using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int LEVEL_COUNT = 10;

    [SerializeField]
    private float movementSpeed = 3.0f;

    [SerializeField]
    private float maxSpawnCooldown = 0.2f;

    public int score;
    public int level = 1;

    private bool canMove = true;
    private readonly string horizontalAxis = "Horizontal";
    private readonly string Walk_Animation_Name = "Walk";
    private readonly string Knife_Tag = "Knife";
    private readonly float min_X = -2.5f, max_X = 2.5f;

    private SpriteRenderer spriteRenderer;
    private Animator playerAnimator;
    private Spawner Spawner;

    [SerializeField] private AudioClip ouchSound;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    private AudioSource playerAudio;

    private void Awake()
    {
        score = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Spawner = FindObjectOfType<Spawner>();
    }

    void Start()
    {
        StartCoroutine(Spawner.SpawnKnife());
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerBounds();
    }

    void PlayerMovement()
    {
        if (!canMove)
            return;

        float h = Input.GetAxisRaw(horizontalAxis);

        Vector2 tempPos = transform.position;        

        if (h > 0)
        {
            // we are going in the right side
            tempPos.x += movementSpeed * Time.deltaTime;
            spriteRenderer.flipX = false;

            playerAnimator.SetBool(Walk_Animation_Name, true);
        }
        else if (h < 0)
        {
            tempPos.x -= movementSpeed * Time.deltaTime;
            spriteRenderer.flipX = true;

            playerAnimator.SetBool(Walk_Animation_Name, true);
        }
        else
        {
            playerAnimator.SetBool(Walk_Animation_Name, false);
        }

        transform.position = tempPos;
        
    }

    void PlayerBounds()
    {
        Vector2 tempPos = transform.position;

        if(tempPos.x > max_X)
            tempPos.x = max_X;
        
        else if (tempPos.x < min_X)
            tempPos.x = min_X;
        
        transform.position = tempPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Knife_Tag))
        {
            playerAudio.PlayOneShot(ouchSound, 1.0f);
            Time.timeScale = 0f;
            canMove = false;

            StartCoroutine(RestartGame());
        }
    }

    public void CheckScore()
    {
        if (score % LEVEL_COUNT == 0)
        {
            IncrementLevel();
            IncrementMoveSpeed();
        }
    }

    public void IncrementScore()
    {
        score += 1;
        scoreText.text = "Score: " + score;
    }

    public void IncrementLevel()
    {
        level += 1;
        levelText.text = "Level: " + level;

        if(Spawner.spawnCooldown > maxSpawnCooldown)
        {
            Spawner.spawnCooldown -= 0.1f;
        }
    }

    public void IncrementMoveSpeed()
    {
        movementSpeed += 1;
    }

    public IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
}
