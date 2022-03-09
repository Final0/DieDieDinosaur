using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        DetectMouseClick();
    }

    private void DetectMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Debug.Log(hit.transform.name);
    }
}