using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    [SerializeField]
    private WorldMoveObjectConfig moveConfig;

    public MoveObjectType ObjectType { get { return moveConfig.ObjectType; } }

    private string originalName;

    [SerializeField]
    GameObject container;

    [SerializeField]
    [Tooltip("Leave empty if just a single style!")]
    List<GameObject> possibleStyles = new List<GameObject>();

    public void Initialize(string newName)
    {
        originalName = newName;
    }

    public void Sleep()
    {
        container.SetActive(false);
        name = $"{originalName} (*sleepy*)";
    }

    public void Wakeup()
    {
        if (possibleStyles.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleStyles.Count);
            for (int index = 0; index < possibleStyles.Count; index += 1)
            {
                if (randomIndex != index)
                {
                    possibleStyles[index].SetActive(false);
                }
                else
                {
                    possibleStyles[index].SetActive(true);
                }
            }
        }
        container.SetActive(true);
        name = $"{originalName} [active]";
    }

    public void Kill()
    {
        if (moveConfig.ObjectType == MoveObjectType.Bird)
        {
            foreach (GameObject style in possibleStyles)
            {
                foreach (Transform child in style.transform)
                {
                    if (child.TryGetComponent<Bird>(out Bird bird))
                    {
                        bird.Die();
                    }
                }
            }
        }
        WorldMover.main.Sleep(this);
    }

    public void OnTriggerEnter2DFromChild(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.main.CollectWorldObject(this);
        }
    }

    public void OnCollisionEnter2DFromChild(Collision2D other)
    {

    }
}
