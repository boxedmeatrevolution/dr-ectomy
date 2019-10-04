using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCounterComponent : MonoBehaviour
{
    private GameComponent gameComponent;
    private TextMeshPro textMesh;

    void Start() {
        this.gameComponent = GameObject.FindObjectOfType<GameComponent>();
        this.textMesh = this.GetComponentInChildren<TextMeshPro>();
    }
    
    void Update() {
        int moveCount = this.gameComponent.moveCount;
        this.textMesh.text = "Moves: " + moveCount.ToString();
    }
}
