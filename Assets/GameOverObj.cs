using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverObj : MonoBehaviour
{
    [SerializeField] private BoxCollider box;
    public BoxCollider Box { get => box; }

    private void Start()
    {
        box.enabled = false;
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 180f * Time.deltaTime);
    }
}
