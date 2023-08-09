using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private enum Destination { None, Level1, Level2 };
    [SerializeField] private Destination destination;
    [SerializeField] private Transform spawnPosition;
    private string sceneToLoad;
    private bool inZone;

    void Start()
    {
        sceneToLoad = destination.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(inZone)
        {
            if(Input.GetButtonDown("Interact"))
            {
                SingletonManager.Instance.HasTravelled = true;
                SceneManager.LoadScene(sceneToLoad);               
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inZone = false;
        }
    }
}
