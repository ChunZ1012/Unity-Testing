using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCollider : MonoBehaviour
{
    private BoxCollider _boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"OnCollisionEnter called");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"OnCollisionExit called");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionEnter2D called");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionExit2D called");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"OnCollisionStay called");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionStay2D called");
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"OnParticleCollision called");
    }
}
