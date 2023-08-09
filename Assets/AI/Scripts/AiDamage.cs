using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDamage : MonoBehaviour
{
    private AIController parent;
    private CapsuleCollider fistCollider;

    private void Start()
    {
        parent = transform.root.GetComponent<AIController>();
        fistCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        fistCollider.enabled = parent.IsPunching;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            float damage = Random.Range(0.075f, 0.15f);

        }
    }
}
