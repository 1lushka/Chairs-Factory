using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Damageable
{
    protected override void Die()
    {
        print("игрок погиб");
        FindAnyObjectByType<BuildingSaveManager>().SaveBuildings();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
