using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    [SerializeField] float speed;

    private Vector3 origin;
    private float slideDistance = 1;
    private bool isOpen;
    private bool opening;

    [SerializeField] private Vector3 direction;


    public virtual void Awake()
    {
        speed = 2;
        origin = transform.position;
        slideDistance = (GetComponent<Collider>().bounds.size).x;
        isOpen = false;
        gameObject.layer = 3;
    }

    public void OnFocus()
    {
        Debug.Log("Being looked at");
    }

    public void OnInteract()
    {
        Debug.Log("Interacted with");

        if (!isOpen)
        {
            opening = true;
            StartCoroutine(SlideGameObject());
        }

        if (isOpen)
        {
            opening = false;
            StartCoroutine(SlideGameObject());
        }
    }

    public void OnLoseFocus() 
    {
        Debug.Log("Stopped being looked at");
    }

    private void MoveDoor()
    {
        Vector3 goal;

        if (opening)
        {
            goal = origin + slideDistance * direction;
        }
        else
        {
            goal = origin;
        }

        transform.Translate(slideDistance * speed * Time.deltaTime * direction);

        if (Vector3.Distance(transform.position, goal) < 0.05f)
        {
            isOpen = !isOpen;
        }
    }

    private IEnumerator SlideGameObject()
    {
        while (opening ? !isOpen : isOpen)
        {
            MoveDoor();
            yield return null;
        }
    }
}
