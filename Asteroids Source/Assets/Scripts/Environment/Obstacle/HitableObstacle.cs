using UnityEngine;

public abstract class HitableObstacle : Obstacle, IHitable
{
    public abstract void GetHit();

    protected abstract void OnOtherObstacleEnter();

    protected override void OnHitableEnter(Collider2D entered, IHitable hitable)
    {
        if (entered.GetComponent<Obstacle>()) OnOtherObstacleEnter();
    }
}

