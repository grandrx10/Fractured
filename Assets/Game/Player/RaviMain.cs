using System;
using Characters;
using UnityEngine;

public class RaviMain : MonoBehaviour
{
    public Transform playerHead;

    public Transform player;
    public Follower follow;
    private Rigidbody _rb;
    public GameObject normal, small;
    public GameObject bigEffect;
    public static RaviMain instance;
    public float minMoveDelta;
    public Animator animator;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            _rb = GetComponent<Rigidbody>();
            GlobalWorldManager.OnPreLoadNewScene += env =>
            {
                switch (env.raviMode)
                {
                    case RaviMode.Normal:
                        SetBig();
                        break;
                    case RaviMode.Small:
                        SetSmall();
                        break;
                    case RaviMode.Gone:
                        SetGone();
                        break;
                }
            };
        }
    }

    private void SetSmall()
    {
        normal.SetActive(false);
        small.SetActive(true);
        _rb.isKinematic = true;
        Instantiate(bigEffect, transform.position, Quaternion.identity);
        transform.position = playerHead.position;
        transform.parent = playerHead;
        follow.enabled = false;
    }
    
    private void SetBig()
    {
        normal.SetActive(true);
        small.SetActive(false);
        Instantiate(bigEffect, transform.position, Quaternion.identity);
        _rb.isKinematic = false;
        transform.parent = null;
        follow.enabled = true;
    }
    
    private void SetGone()
    {
        Instantiate(bigEffect, transform.position, Quaternion.identity);
        normal.SetActive(false);
        small.SetActive(false);
        _rb.isKinematic = true;
        transform.position = playerHead.position;
        transform.parent = playerHead;
    }
    
    void Update()
    {
        
    }

    private Vector3 _ppos;
    private int stillFrames;
    private void FixedUpdate()
    {
        if (Vector3.Distance(_ppos, transform.position) > minMoveDelta)
        {
            stillFrames++;
            stillFrames = Mathf.Max(-5, stillFrames);
        }
        else
        {
            stillFrames--;
            stillFrames = Mathf.Min(0, stillFrames);
        }
        animator.SetInteger("Frames", stillFrames);
        _ppos = transform.position;
    }
}

public enum RaviMode
{
    Normal,
    Small,
    Gone
}