using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectChildCollider : MonoBehaviour
{
    WorldMoveObject parentMoveObject;

    private void FetchParentMoveObject()
    {
        if (parentMoveObject == null)
        {
            parentMoveObject = transform.GetComponentInParent<WorldMoveObject>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FetchParentMoveObject();
        parentMoveObject.OnTriggerEnter2DFromChild(other);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        FetchParentMoveObject();
        parentMoveObject.OnCollisionEnter2DFromChild(other);
    }
}
