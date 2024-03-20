using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Vars")]
    public float moveSpeed = 500f;
    public float jumpPower = 300f;
    public float airSpeedModifier = 0.5f;
    [Tooltip("The modifier for how much more gravity is applied while falling downward or the descent after a jump")]
    public float gravityModifier = 2f;
    public float rotationSpeed = 5f;
    public bool mouseInvert = false;
    public float mouseSensitivity = 1f;
    float camRotationX = 0f;

    Vector3 snapShotVelocity = Vector3.zero;
    Vector3 airVelocity;


    //public LineRenderer lineRenderer;
    //public Vector3 trajectoryLineOffsetFromCamera = new Vector3(1, 1, 1);

    Animator _anim;
    public Animator Animator { get {return _anim; } }

    [SerializeField] AudioSource _audioSource1;
    [SerializeField] AudioSource _audioSource2;
    [SerializeField] AudioSource _audioSource3;
    [SerializeField] AudioClip _jumpStartClip;
    [SerializeField] AudioClip _jumpLandClip;

    bool isWalking = false;

    float _baseGravity = -9.81f;

    private float _invert = 1f;
    private float _lastYPos;

    private Rigidbody _rb;
    [SerializeField] private Camera _camera;


    [Header("Disc Variables")]
    public GameObject discPrefab;
    public Transform launchPoint;

    [Tooltip("Delay constrained on the player before Throw or Return can be executed again")]
    public float discActionDelayOnThrow = 0.5f;
    public float discActionDelayOnReturn = 0.2f;
    bool _discInputAllowed = true;

    [Header("Disc Return Voicelines")]
    public List<AudioClip> returnQuips = new List<AudioClip>();
    [Range(0f,1f)]
    public float quipChancePercent = 0.1f;
    public float quipSpeakDelaySec = 0.5f;
    [SerializeField] AudioSource quipSource;
    Coroutine _quipCoroutine;

    [Header("Player Disc Mesh Vars")]
    public GameObject discMesh;
    public bool discShowOnStart = true;



    [Header("GroundCheck")]
    public bool castHit;
    [SerializeField] float castDepth = 0.1f;
    [SerializeField] float yOffset = 1.12f;
    [SerializeField] float jumpCooldownSec = 0.5f;
    public bool canJump = true;
    public bool jumpRaycastOn = true;
    bool justLanded = false;

    [Header("FOR DEVS TO ENABLE SOME LOGS")]
    public bool enablePlayerThrowPosition = false;
    public bool enableThrowLookRotation = false;


    float vertical = 0f;
    float horizontal = 0f;

    

    struct ActiveDisc
    {

        public GameObject gameObject;
        public Disc disc;

        
    }
    private ActiveDisc _activeDisc;
    private bool _discExist = false;

    

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();   

        if (!mouseInvert)
            _invert = -1f;

        ToggleDiscMesh(discShowOnStart);


            

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        _rb.velocity = (gameObject.transform.forward * vertical * moveSpeed)
            + (gameObject.transform.right * horizontal * moveSpeed)
            + new Vector3(0, _rb.velocity.y, 0);
        

        if (jumpRaycastOn)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z), transform.up * -1f, castDepth))
            {
                canJump = true;

                //Landing sfx
                if(!justLanded)
                {
                    _audioSource2.Stop();
                    _audioSource2.clip = _jumpLandClip;
                    _audioSource2.Play();

                    justLanded = true;
                }
            }
        }
            



        if (_rb.velocity.y < 0 && _lastYPos != transform.position.y) //Check to make sure the last yPosition is not the same as last frame. This checks if the player is currently grounded.
        {
            //Debug.Log("Falling!");
            _rb.AddForce(0, _baseGravity * gravityModifier, 0, ForceMode.Acceleration);
            _lastYPos = transform.position.y;

            
            

        }


    }


    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        //TrajectoryHandler.RenderLine(lineRenderer, _camera.transform.position, trajectoryLineOffsetFromCamera, launchPoint.transform.position);

        if (canJump && Input.GetButtonDown("Jump"))
        {
            justLanded = false;
            _audioSource2.Stop();
            _audioSource2.clip = _jumpStartClip;
            _audioSource2.Play();

            //Reset vertical velocity to 0, THEN Add Force
            jumpRaycastOn = false;
            canJump = false;          
            _rb.velocity = new Vector3(_rb.velocity.x, 0.1f, _rb.velocity.z);
            _rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.VelocityChange);
            StartCoroutine(JumpCooldown());

        }

        //Walking sfx
        if (canJump && !isWalking && (Input.GetAxis("Horizontal") > 0f
            || Input.GetAxis("Vertical") > 0f))
        {
            isWalking = true;
            _audioSource1.Play();
        }
        else if (!canJump || (isWalking && Input.GetAxis("Horizontal") == 0f
                && Input.GetAxis("Vertical") == 0f))
        {
            isWalking = false;
            _audioSource1.Stop();
        }


        //Get camera to face mouse position

        //These are the velocities of the mouse pos transform when it is moving
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        //Debug.Log($"MX: {mouseX}    MY: {mouseY}");

        if (mouseX != 0)
        {
            //Get it
            camRotationX += mouseY * _invert * mouseSensitivity;

            camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

            //Then clamp it!
            _camera.transform.localEulerAngles = new Vector3(camRotationX, 0);

            gameObject.transform.localEulerAngles += new Vector3(0, mouseX * rotationSpeed * mouseSensitivity, 0);
        }
        

        //Throw disc
        if (!_anim.GetBool("ThrowLock") && _discInputAllowed && !_discExist && Input.GetButton("Fire1"))
        {
            if (_quipCoroutine != null)
            {
                StopCoroutine(_quipCoroutine);
                _anim.SetBool("Talk", false);
            }
            quipSource.Stop();
            _discExist = true;
            
            StartCoroutine(DiscActionCooldown(discActionDelayOnThrow));

            _anim.SetTrigger("Throw");
            _activeDisc.gameObject = Instantiate(discPrefab, launchPoint.transform.position, Quaternion.identity);

            _activeDisc.gameObject.transform.rotation = Quaternion.Euler(new Vector3(_camera.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z));

            if (enablePlayerThrowPosition)
            {
                Debug.Log($"Player Throw Pos: <color=magenta>{transform.position}</color>");
            }
            if (enableThrowLookRotation) 
            {
                Debug.Log($"Player Look Rotation: <color=cyan>{_activeDisc.gameObject.transform.rotation}</color>");
            }

            _activeDisc.disc = _activeDisc.gameObject.GetComponent<Disc>();
            _activeDisc.disc.Launch();


        }

        if (_discInputAllowed && _discExist && Input.GetButton("Fire2"))
        {
          
            if (_quipCoroutine != null)
            {
                StopCoroutine(_quipCoroutine);
            }

            
            StartCoroutine(DiscActionCooldown(discActionDelayOnReturn));

            //Disc return sfx
            _audioSource3.Play();

            _activeDisc.gameObject = null;
            _activeDisc.disc.Return();
            _anim.SetTrigger("Return");

            _quipCoroutine = StartCoroutine(Quip());

        }



        if (_discExist && _activeDisc.gameObject == null)
        {
            //Disc return sfx
            _audioSource3.Play();

            _discExist = false;
            _anim.SetTrigger("Return");
        }

    }

    public void OnAirLaunch()
    {
        justLanded = false;
        canJump = false;
        jumpRaycastOn = false;
        StartCoroutine(JumpCooldown());
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z), transform.up * -1f * castDepth);

    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldownSec);
        jumpRaycastOn = true;
    }

    IEnumerator DiscActionCooldown(float delay)
    {
        _discInputAllowed = false;
        yield return new WaitForSeconds(delay);
        _discInputAllowed = true;
    }


    IEnumerator Quip()
    {
        yield return new WaitForSeconds(quipSpeakDelaySec);
        DetermineQuip();
    }

    IEnumerator CurrentQuip(float quipDuration)
    {
        yield return new WaitForSeconds(quipDuration);
        _anim.SetBool("Talk", false);
    }

    void DetermineQuip()
    {
        float chance = Random.Range(0f, 1f);
        //Debug.Log(chance);
        if (chance <= quipChancePercent)
        {
            _anim.SetBool("Talk", true);
            Random.InitState((int)Time.realtimeSinceStartup);
            int index = Random.Range(0, returnQuips.Count);
            quipSource.clip = returnQuips[index];
            quipSource.Play();
            _quipCoroutine = StartCoroutine(CurrentQuip(quipSource.clip.length));


        }
    }

    //Used by Animation Behavior when animation changes to idle
    public void RemoteQuipSourceStop()
    {
        quipSource.Stop();
    }

    public bool ToggleDiscMesh()
    {
        MeshRenderer _mr = discMesh.GetComponent<MeshRenderer>();
        _mr.enabled = !_mr.enabled;
        _anim.SetBool("ThrowLock", !_mr.enabled);
        return _mr.enabled;

    }
    public bool ToggleDiscMesh(bool value)
    {
        MeshRenderer _mr = discMesh.GetComponent<MeshRenderer>();
        _mr.enabled = value;
        _anim.SetBool("ThrowLock", !value);
        return _mr.enabled;

    }


}
