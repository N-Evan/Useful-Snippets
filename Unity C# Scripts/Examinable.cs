using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Examinable : Interactable
{

    private bool _inExamination;
    protected Vector3 _LastFrame;
    private Vector3 _originalPosition;
    private Vector3 _originalRotation;
    private Camera _mainCamera;


    public override void Start()
    {
        _mainCamera = Camera.main;
    }


    public override void Interact()
    {
        if (!IsInteractable) return;
        AudioManager.Instance.PlaySFX(Sfx);
        StartExamination();
    }
    
    private void StartExamination()
    {
        if (_inExamination) return;
        _inExamination = true;

        Time.timeScale = 0f;

        gameObject.layer = LayerMask.NameToLayer("RenderTop");

        _originalPosition = transform.position;
        _originalRotation = transform.rotation.eulerAngles;
    }

    //Convert to InputSystem
    private void Update()
    {
        if (!_inExamination) return;
        
        if (Input.GetMouseButtonDown(0))
            _LastFrame = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - _LastFrame;
            _LastFrame = Input.mousePosition;

            var rotationAxis = Quaternion.AngleAxis(-90f, Vector3.forward) * delta;
            transform.rotation = Quaternion.AngleAxis(delta.magnitude * 0.7f, rotationAxis) * transform.rotation;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.transform.position = _originalPosition;
            gameObject.transform.eulerAngles = _originalRotation;
            ExitExamination();
        }
    }

    private void ExitExamination()
    {
        _inExamination = false;

        Time.timeScale = 1f;

        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}