using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public float iFrame = 0f;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    { 
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    public void Restart()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            TakeDamage(1);
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            IncreaseHealth(1);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            IncreaseHealth(-1);
        }
    }

    private void FixedUpdate()
    {
        if (iFrame > 0)
        {
            gameObject.layer = 7;
            iFrame -= Time.fixedDeltaTime;
        }
        else gameObject.layer = 8;
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, 2f);
    }

    public void TakeDamage(int damage, float iFrame)
    {
        if (this.iFrame <= 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //Destroy(gameObject);
            }
            healthBar.SetHealth(currentHealth);
            this.iFrame = iFrame;
        }
    }

    public void IncreaseHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        if (maxHealth < 1) maxHealth = 1;
        healthBar.SetMaxHealth(maxHealth);
    }
}
