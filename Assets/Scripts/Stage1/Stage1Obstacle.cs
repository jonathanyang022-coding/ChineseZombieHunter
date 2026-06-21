using UnityEngine;

public class Stage1Obstacle : MonoBehaviour
{
    [SerializeField] private int hitPoints = 2;

    public void TakeHit()
    {
        hitPoints--;
        if (hitPoints > 0)
        {
            return;
        }

        Destroy(gameObject);
    }
}
