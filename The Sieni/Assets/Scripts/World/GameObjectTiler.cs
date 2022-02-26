using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTiler : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int count;

    [SerializeField]
    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        setupClones();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setupClones() {

        for (var i = 0; i < count; i++) {
            var obj = Instantiate(prefab, transform);
            var sign = i % 2 == 0 ? 1 : -1;
            var objOffset = offset * (i / 2) * sign;
            obj.transform.position = transform.position + (Vector3)objOffset;
        }
    }
}
