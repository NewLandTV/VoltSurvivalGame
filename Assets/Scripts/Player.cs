using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    // 플레이어 속도 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    // 플레이어 점프 파워 변수
    [SerializeField]
    private float jumpForce;

    // 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 회전 제한과 현재 카메라 회전 변수
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX;

    // 기타 컴포넌트
    [SerializeField]
    private Text TextNickName;
    [SerializeField]
    private Image ImageHealthGauge;
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    // 필요한 컴포넌트
    [SerializeField]
    private Rigidbody rigid;
    [SerializeField]
    private PhotonView PV;
    private Camera cam;

    // 상태 변수
    private bool isRun;
    private bool isJump;

    private Vector3 currentPosition;

    private void Awake()
    {
        cam = Camera.main;

        TextNickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;

        if (PV.IsMine)
        {
            cam.transform.SetParent(transform);

            cam.transform.position = Vector3.up * 0.9f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            applySpeed = walkSpeed;
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            TryRun();
            Move();
            CheckGround();
            Jump();
            CameraRotation();
            CharacterRotation();
        }
    }

    // 달리기 함수
    #region Run

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            CancelRun();
        }
    }

    private void Run()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void CancelRun()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    #endregion Run

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * x;
        Vector3 moveVertical = transform.forward * z;

        rigid.velocity = (moveHorizontal + Vector3.zero + moveVertical) * applySpeed;
    }

    private void CheckGround()
    {
        isJump = !Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    // 점프 함수
    #region Jump

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;

            PV.RPC(nameof(JumpRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    private void JumpRPC()
    {
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    #endregion Jump

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;

        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        Camera.main.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;

        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(characterRotationY));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            currentPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
