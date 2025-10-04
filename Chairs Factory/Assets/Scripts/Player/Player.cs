using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Damageable
{
    protected override void Die()
    {
        print("����� �����");
        FindAnyObjectByType<BuildingSaveManager>().SaveBuildings();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
