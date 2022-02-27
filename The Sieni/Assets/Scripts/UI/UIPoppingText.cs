using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoppingText : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Text txtMessage;

    public void Show(Vector2 position, string message)
    {
        transform.position = position;
        txtMessage.text = message;
        animator.Play("Show");
    }

    public void ShowFinished()
    {
        Destroy(gameObject);
    }
}
