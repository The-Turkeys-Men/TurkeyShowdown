
using UnityEngine;

public static class RaycastUtils
{
    public static RaycastHit2D RaycastFirstEnnemy(int teamID, Vector2 origin, Vector2 direction, float distance, int layerMask)
    {
        RaycastHit2D[] raycastResults = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        
        foreach (RaycastHit2D hit in raycastResults)
        {
            if (hit.collider.TryGetComponent(out HealthComponent healthComponent))
            {
                if (healthComponent.TryGetComponent(out TeamComponent teamComponent) &&
                    teamComponent.TeamID.Value == teamID)
                {
                    continue;
                }

                return hit;
            }
            else
            {
                return hit;
            }
        }

        return default;
    }
}