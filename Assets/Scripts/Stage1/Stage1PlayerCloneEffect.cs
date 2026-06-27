using UnityEngine;

public class Stage1PlayerCloneEffect : MonoBehaviour
{
    [SerializeField] private int maxProjectileMultiplier = 12;
    [SerializeField] private int maxCloneCount = 12;

    private int baseProjectileCount = 1;
    private int projectileMultiplier = 1;

    public int ActiveCloneCount => Mathf.Max(0, baseProjectileCount - 1);
    public int ProjectileMultiplier => Mathf.Max(1, projectileMultiplier);
    public int BaseProjectileCount => Mathf.Max(1, baseProjectileCount);
    public int TotalShooterCount => BaseProjectileCount;

    private void Start()
    {
        ClearClones();
    }

    public void SpawnClones(int cloneCount)
    {
        SetBaseProjectileCount(cloneCount + 1);
    }

    public void AddClones(int extraCloneCount)
    {
        IncreaseBaseProjectileCount(extraCloneCount);
    }

    public void SetBaseProjectileCount(int count)
    {
        baseProjectileCount = Mathf.Clamp(Mathf.Max(1, count), 1, maxCloneCount);
    }

    public void IncreaseBaseProjectileCount(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        baseProjectileCount = Mathf.Clamp(baseProjectileCount + amount, 1, maxCloneCount);
    }

    public void MultiplyShooters(int multiplier)
    {
        if (multiplier <= 1)
        {
            return;
        }

        projectileMultiplier = Mathf.Clamp(projectileMultiplier * multiplier, 1, maxProjectileMultiplier);
    }

    public void AddProjectileMultiplier(int multiplier)
    {
        if (multiplier <= 1)
        {
            return;
        }

        projectileMultiplier = Mathf.Clamp(projectileMultiplier + multiplier - 1, 1, maxProjectileMultiplier);
    }

    public void SetProjectileMultiplier(int multiplier)
    {
        projectileMultiplier = Mathf.Clamp(Mathf.Max(1, multiplier), 1, maxProjectileMultiplier);
    }

    public void ClearClones()
    {
        baseProjectileCount = 1;
        projectileMultiplier = 1;
    }
}
