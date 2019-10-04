using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButtonComponent : MonoBehaviour
{

    public GameObject previousLevelPrefab;
    public GameObject nextLevelPrefab;

    void Start() {
        GameObject.Instantiate(nextLevelPrefab, this.transform.localPosition - new Vector3(1.5f, 0.0f), Quaternion.identity);
        GameObject.Instantiate(previousLevelPrefab, this.transform.localPosition - new Vector3(3.0f, 0.0f), Quaternion.identity);
    }
    
    void Update() {
        
    }
}
