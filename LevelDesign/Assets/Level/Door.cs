using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    [SerializeField] float speed;

    private Vector3 origin;
    private float slideDistance;
    private bool isOpen;
    private bool opening;

    void Start()
    {
        origin = transform.position;
        slideDistance = (transform.rotation * GetComponent<Collider>().bounds.size).x;
        isOpen = false;
    }

    public void Open()
    {
        if (!isOpen)
        {
            opening = true;
            StartCoroutine(SlideGameObject());
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            opening = false;
            StartCoroutine(SlideGameObject());
        }
    }

    private void MoveDoor()
    {
        Vector3 direction;
        Vector3 goal;

        if (opening)
        {
            direction = transform.rotation * transform.right;
            goal = origin - slideDistance * transform.right;
        }
        else
        {
            direction = transform.rotation * -transform.right;
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
