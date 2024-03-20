using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    AudioSource _audioSource;
    
    LineRenderer _lineRenderer;
    int _LineRendPosSize = 0;

    [Header("Bounces")]
    [SerializeField] short _maxBounces = 3;
    [SerializeField] short _bounces = 0;

    [Header("Flight")]
    [SerializeField] float _flightSpeed = 8f;
    [SerializeField] float _noCollideFlightDuration = 2f;

    [Header("Audio")]
    [SerializeField] AudioClip _bounceClip;
    [SerializeField] AudioClip _throwClip;

    Collider _lastCollider = null;


    Coroutine _flightDuration;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        _rb.velocity = _rb.velocity.normalized * _flightSpeed;
        transform.forward = _rb.velocity;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position); //Always update this one with current pos

    }

    public void Launch()
    {
        _bounces = _maxBounces;

        _audioSource.Stop();
        _audioSource.clip = _throwClip;
        _audioSource.Play();

        _LineRendPosSize = 2;
        _lineRenderer.positionCount = _LineRendPosSize;
        _lineRenderer.SetPosition(0, transform.position); //The launch point
        _lineRenderer.SetPosition(_LineRendPosSize - 1, transform.position); //Always update this one with current pos

        _rb.velocity = gameObject.transform.forward * _flightSpeed;

        //Debug.Log($"Start Velocity of Disc: <color=green>{_rb.velocity}</color>");

        _flightDuration = StartCoroutine(flightTimer());
        

    }

    public void Return()
    {
        //Debug.Log("Early Return");
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (_bounces > 0)
        {
            if (!_lastCollider || _lastCollider != collision.collider) //if there is no last collider OR the current collider isn't the last collider hit
            {
                if (_lastCollider != null)
                {
                    _lastCollider.excludeLayers -= 7;
                }

                _lastCollider = collision.collider;
                _lastCollider.excludeLayers += 7;
                _audioSource.clip = _bounceClip;
                _audioSource.Play();

                //Debug.Log($"Bounce");
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 2, transform.position);
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
                _bounces--;

                StopCoroutine(_flightDuration);
                _flightDuration = StartCoroutine(flightTimer());

                

                //Debug.Log($"Normal of <color=yellow>Bounce</color> Collision: {collision.contacts[0].normal}");
                //Debug.Log($"<color=yellow>New Velocity</color> of Disc: {_rb.velocity}");

            }
            
        }
        else if (!_lastCollider || _lastCollider != collision.collider)
        {
            //Debug.Log($"<color=red>Destroy</color>");
            Destroy(gameObject);

        }
        
    }


    IEnumerator flightTimer()
    {
        yield return new WaitForSeconds(_noCollideFlightDuration);
        //Debug.Log("Flight duration met, destroying disc");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward);
    }



}
