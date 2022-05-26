using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

public class ShellCase : MonoBehaviour, IPoolObject
{
    public Rigidbody _rigidbody;
    public float returnTime = 4;
    private float _time = 0;
    private bool isThrow = false;
    private Vector3 pos = Vector3.right;
    private Vector3 force = Vector3.up;
    private float rotateForce = 10;
    public ObjectPool pool { get; set; }

    public void OnGetObjectFromPool()
    {
    }

    public void ReturnToPool()
    {
        isThrow = false;
        _time = 0;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.useGravity = false;
    }

    public void ThrowShellCase(Vector3 force, Vector3 pos)
    {
        this.force = force;
        this.pos = pos;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(force);
        rotateForce = Random.Range(30f, 200f);
        isThrow = true;
    }

    private void FixedUpdate()
    {
        if (isThrow)
        {
            _rigidbody.AddRelativeTorque(new Vector3(-1, 0, 0) * rotateForce);
        }
        if (_time >= returnTime)
        {
            isThrow = false;
            ObjectPool.ReturnToPool(this);
        }
        if (gameObject.activeInHierarchy)
            _time += TimeHelper.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrow)
        {
            GameDebug.Log("ShellCase Collision Enter");
            isThrow = false;
        }
    }
}

