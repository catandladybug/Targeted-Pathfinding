using UnityEngine;

public class FollowPath : Seek
{
    public Path path;
    public float threshold;
    public bool loop = false;
    public bool reverse = false;

    private int index = 0;

    protected override Vector3 getTargetPosition()
    {
        Vector3 directionToTarget = path.PathObjects[index].position - character.transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < threshold)
        {
            index++;
            if (index >= path.PathObjects.Length)
            {
                if (loop || reverse)
                    index = 0;
                if (reverse) path.Reverse();
            }
        }


        return path.PathObjects[index].position;
    }
}