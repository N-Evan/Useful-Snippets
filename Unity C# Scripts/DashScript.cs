using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DashScript : MonoBehavior 
{
	private void Dash(float x, float y)
    {
        isDashing = true;

        rigidBody.velocity = Vector2.zero;
        dir = new Vector2(x, y);
        dir = dir.normalized;

        rigidBody.velocity += dir * dashSpeed;

        hasDashed = true;

        StartCoroutine(DashWait());
    }

    void RigidbodyDrag(float x)
    {
        rigidBody.drag = x;
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        DOVirtual.Float(10, 0, 0.6f, RigidbodyDrag);
        rigidBody.gravityScale = 0f;
        isDashing = true;
        
        yield return new WaitForSeconds(.2f);

        rigidBody.gravityScale = 3.5f;
        isDashing = false;
        StopCoroutine(DashWait());
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (grounded)
        {
            hasDashed = false;
        }
    }
}