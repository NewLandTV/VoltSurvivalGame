using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    // �÷��̾� �ӵ� ����
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    // �÷��̾� ���� �Ŀ� ����
    [SerializeField]
    private float jumpForce;

    // �ΰ���
    [SerializeField]
    private float lookSensitivity;

    // ī�޶� ȸ�� ���Ѱ� ���� ī�޶� ȸ�� ����
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX;

    // ��Ÿ ������Ʈ
    [SerializeField]
    private Text TextNickName;
    [SerializeField]
    private Image ImageHealthGauge;
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Rigidbody rigid;
    [SerializeField]
    private PhotonView PV;
    private Camera cam;

    // ���� ����
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

    // �޸��� �Լ�
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

    // ���� �Լ�
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
