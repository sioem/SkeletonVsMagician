using Meta.WitAi;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PcPlayer : MonoBehaviour
{
    public enum ePcEventType
    {
        GrabImpact = 11, ThrowObjectImpact = 12, GrabObjectImpact = 13, ThrowPlayer = 14, PCDIE = 15, PCHIT = 16
    }
    #region Property

    private int maxItemCount = 5;
    [SerializeField] private int itemCount = 5;
    [Header("Network")]
    public PhotonView photonView;

    [Header("Rocket")]
    public Transform fireTrans;

    [Header("Shield")]
    [SerializeField] private GameObject shieldGo;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float swapRange;
    [SerializeField] private float swapSpeed;

    [SerializeField] private float elapsedTime = 0.0f;
    private Rigidbody rbody;
    private Player player;

    [Header("Bool State")]
    [SerializeField] public bool isGround = true;
    [SerializeField] private bool isSuperJumped = false;
    [SerializeField] private bool isHit = false; //Lerp로 이동 중에, 충돌이 발생하면 바로 Lerp 종료하기 위함 
   // [SerializeField] private bool isWireWalk = false;

    [Header("LineRender")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("CrossHair")]
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] private Image[] crossHairGos;
    [SerializeField] private float shootPower;

    [SerializeField] public SkinnedMeshRenderer bodyRenderer;

    [Header("Animator")]
    [SerializeField] private Animator uiAnim;
    [SerializeField] private Animator pcAnim;

    [SerializeField] private PlayerNetworkGrabbable networkGrabbable;
    [SerializeField] private PhotonAnimatorView animatorView;

    [SerializeField] private TurnPCMP turnMp;
    [SerializeField] private TurnPCHP turnHp;

    [SerializeField] private Collider col;

    private int maxHp = 100;
    private int hp;

    private Vector3 moveDir;

    private Camera mainCam;
    #endregion

    public Action onImpactAction;
    public Action onPlayerRequestAction;
    private float h => Input.GetAxis("Horizontal");
    private float v => Input.GetAxis("Vertical");

    public Material damageMaterial;

    public bool isGameOver = true;
    private bool isCoroutineRunning = false;
    private Action onDie;

    private Material myBodyMaterial;

    private int uniqueNumber;

    public int UniqueNumber { get => uniqueNumber; }
    public Player Player { get => player;  }

    private Coroutine coroutine;
    private void Awake()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.rbody = this.GetComponent<Rigidbody>();
        this.mainCam = Camera.main;
        this.rbody.useGravity = false;
        this.col.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.onDie = () =>
        {
            this.StopPcPlayer();
            this.pcAnim.Play("Die");
            EventDispatcher.instance.SendEvent((short)ePcEventType.PCDIE, this.player);
        };
        if (this.photonView.IsMine)
        {
            //조준점 가져오기
            this.uiAnim = GameObject.Find("Crosshair").GetComponent<Animator>();
            this.crossHairGos[0] = GameObject.Find("AimCircle").GetComponent<Image>();
            this.crossHairGos[1] = GameObject.Find("Crosshair").GetComponent<Image>();
            this.crossHairGos[1].gameObject.SetActive(false);
            this.canvasRectTransform = GameObject.Find("PcAimCanvas").GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (!this.isGameOver)
        {
            this.Movement();
            this.JumpAnimation();
            this.SwapMovement();
            this.DistanceCheck();
            this.Defend();
            this.CrossHair();
        }
    }

    [PunRPC]
    public void Init(Player player)
    {
        this.player = player;

        this.hp = this.maxHp; //피 초기화 

        //고유 번호 저장 
        this.SetUniqueNumber();
        //고유 번호로 색 변경 
        this.ChangePcPlayerBody(this.uniqueNumber);

    }

    #region SwapMove
    private Vector3 hitPoint;
    private void SwapMovement()
    {
        if (this.photonView.IsMine)
        {
            //Ray ray = new Ray(this.mainCam.transform.position, this.mainCam.transform.forward);
            Ray ray = new Ray(this.fireTrans.transform.position, this.mainCam.transform.forward);
            //Swap
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

                if (Physics.Raycast(ray.origin, ray.direction, out hit, this.swapRange))
                {
                    this.hitPoint = hit.point;
                }
                else
                {
                    this.hitPoint = Vector3.zero;
                }
            }

            if (Input.GetMouseButton(1)) //마우스 우클릭
            {
                if (isHit == false) //IsHit는 닿았을 때 중력작용 및 라인렌더러 끊기
                {
                    DrawLine();
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > 0.2f && this.hitPoint != Vector3.zero && this.isHit == false)
                    {
                        this.rbody.useGravity = false;
                        this.transform.position = Vector3.Lerp(this.transform.position, this.hitPoint, Time.deltaTime * swapSpeed);
                       // this.isWireWalk = true;
                        this.FastMoveAnimation(0); // 하늘나는 애니메이션          
                    }
                }
                else
                {
                    this.SwapSettingDefault();
                }

            }
            else if (Input.GetMouseButtonUp(1))
            {
                this.isHit = false;
               // this.isWireWalk = false;
                this.SwapSettingDefault();
                this.photonView.RPC("SwapSettingDefault", RpcTarget.Others);
                this.FastMoveAnimation(2); // Idle 애니메이션
            }
        }
    }

    private void DistanceCheck()
    {
        if (photonView.IsMine)
        {
            if (Vector3.Distance(this.transform.position, hitPoint) < 0.1f)
            {
                this.photonView.RPC("SwapSettingDefault", RpcTarget.AllViaServer);
            }
        }

    }
    [PunRPC]
    private void SwapSettingDefault()
    {
        this.rbody.useGravity = true;
        this.elapsedTime = 0;
        this.lineRenderer.enabled = false;
    }
    [PunRPC]
    private void DrawLine()
    {
        this.lineRenderer.enabled = true;
        this.lineRenderer.SetPosition(0, this.transform.position);
        if (hitPoint == Vector3.zero)
        {
            this.lineRenderer.SetPosition(1, this.transform.position);
        }
        else this.lineRenderer.SetPosition(1, hitPoint);

    }
    #endregion

    #region PlayerMove

    //이름 바꾸기 
    private void Movement()
    {
        //이동
        if (this.photonView.IsMine)
        {
            //this.photonView.RPC("Move", this.player);
            Move();

            if (Input.GetKeyDown(KeyCode.Space) && this.isGround)
            {
                //일반 점프 
                this.photonView.RPC("Jump", RpcTarget.All);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && this.isGround == false && this.itemCount > 0 && this.isSuperJumped == false)
            {
                //슈퍼점프
                this.photonView.RPC("Jump", RpcTarget.All);
                this.UseItem();
                this.photonView.RPC("UseItem", RpcTarget.Others);
                //마법공격 이펙트 및 애니메이션 실행
                this.isSuperJumped = true;
            }
            this.photonView.RPC("Attack", RpcTarget.All);
        }
        //속도 제한
        if (this.rbody.velocity.magnitude >= maxSpeed)
        {
            this.rbody.velocity = this.rbody.velocity.normalized * maxSpeed;
        }
    }

    private void Move()
    {
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        this.moveDir = (forward * v) + (right * h);
        moveDir.Set(moveDir.x, 0f, moveDir.z);

        if (!PhotonNetwork.IsMasterClient)
        {
            Turn();
        }

        this.transform.Translate(moveDir * this.moveSpeed * Time.deltaTime);
        this.MoveAnimation(this.moveDir);
    }

    [PunRPC]
    private void Jump()
    {
        this.rbody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        this.isGround = false;
    }

    private void Turn()
    {
        Vector3 lookDir = this.mainCam.transform.forward;
        lookDir.Set(lookDir.x, 0, lookDir.z);

        this.transform.localRotation = Quaternion.LookRotation(lookDir);

    }

    #endregion

    #region PlayerAnimation

    private void FastMoveAnimation(int num)
    {
        this.pcAnim.SetInteger("isFastMove", num);
    }

    private void JumpAnimation()
    {
        //점프 애니메이션
        if (!this.isGround) // 캐릭터 y축 위치 수정 -> y축으로 기준 하지말고 다른 기준 만들기 
        {
            this.pcAnim.SetBool("isJump", true);
        }
        else
        {
            this.pcAnim.SetBool("isJump", false);
        }
    }

    [PunRPC]
    private void MoveAnimation(Vector3 dir)
    {
        //이동 애니메이션
        if (dir.z > 0)
        {
            this.pcAnim.SetInteger("isWalk", 1);
        }
        else if (dir.x < 0)
        {
            this.pcAnim.SetInteger("isWalk", 2);
        }
        else if (dir.x > 0)
        {
            this.pcAnim.SetInteger("isWalk", 3);
        }
        else if (dir.z < 0)
        {
            this.pcAnim.SetInteger("isWalk", 4);
        }
        else
        {
            this.pcAnim.SetInteger("isWalk", 0); //Idle 애니메이션 
        }
    }


    private void DefendAnimation(int num)
    {
        this.pcAnim.SetInteger("isDefend", num);
    }

    private void AttackAnimation(bool isAttack)
    {
        this.pcAnim.SetBool("isAttack", isAttack); // 공격 발사 애니메이션
    }
    #endregion

    #region OnImpact
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (this.photonView.IsMine)
            {
                this.photonView.RPC("GetItem", RpcTarget.AllViaServer);
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (this.networkGrabbable.isGrab && PhotonNetwork.IsMasterClient && this.networkGrabbable.ControllerSpeed > 1.0f)
        {
            //이벤트 호출 11번
            EventDispatcher.instance.SendEvent((short)ePcEventType.GrabImpact, this);
        }

        if (this.networkGrabbable.isGrabbed && PhotonNetwork.IsMasterClient && this.networkGrabbable.ThrowSpeed > 2.5f)
        {
            EventDispatcher.instance.SendEvent((short)ePcEventType.ThrowPlayer, this);
            this.networkGrabbable.isGrabbed = false;
        }

        if (collision.rigidbody != null)
        {
            this.DamageEvent(collision);
        }

        if (!this.photonView.IsMine && collision.gameObject.CompareTag("Ground"))
        {
            this.onPlayerRequestAction();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isGround = true;
            this.isSuperJumped = false;
        }
        #region HighWallJump
        //else if (collision.gameObject.CompareTag("HighWall") && this.isWireWalk == true)
        //{
        //    Debug.LogFormat("<color=red>높게 점프함</color>");
        //    this.rbody.AddRelativeForce(new Vector3(0, this.transform.localPosition.y * 3f, this.transform.localPosition.z * 3f), ForceMode.Impulse);
        //    this.isWireWalk = false;
        //    this.isHit = true;
        //}
        //else if (collision.gameObject.CompareTag("NormalWall") && this.isWireWalk == true)
        //{
        //    Debug.LogFormat("<color=red>그냥 점프함</color>");
        //    this.rbody.AddForce(new Vector3(0, this.transform.localPosition.y * 4f, 0), ForceMode.Impulse);
        //    this.isWireWalk = false;
        //    this.isHit = true;
        //}
        #endregion

        if (collision.gameObject.CompareTag("DeadZone"))
        {
            this.onDie();
        }
    }

    private void DamageEvent(Collision collision)
    {
        var colVelocity = collision.rigidbody.velocity.magnitude;
        if (collision.gameObject.CompareTag("Rigid")) //잡은 물체로 치는 거 13번
        {
            if (colVelocity > 2.5f)
            {
                EventDispatcher.instance.SendEvent((short)ePcEventType.GrabObjectImpact, this);
            }
            this.onImpactAction();
            isHit = true;
        }
        else if (!collision.gameObject.CompareTag("Player") && colVelocity > 1.3f && this.photonView.Owner == this.player) //던지는 거 12번
        {
            EventDispatcher.instance.SendEvent((short)ePcEventType.ThrowObjectImpact, this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rigid"))
        {
            var colVelocity = collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if (colVelocity < 1f)
            {
                this.onPlayerRequestAction();
            }
        }
    }
    #endregion

    #region Attack
    [PunRPC]
    private void Attack()
    {
        if (this.itemCount > 0 && this.photonView.IsMine)
        {
            if (Input.GetMouseButtonUp(0) && this.itemCount > 0)
            {
                //Debug.Log("Attack!");
                this.Turn();
                this.UseItem();
                this.photonView.RPC("UseItem", RpcTarget.OthersBuffered);
                this.ShootMissile(this.mainCam.transform.forward);
                this.AttackAnimation(true);
            }
        }
    }

    private void CrossHair()
    {
        if (photonView.IsMine && !PhotonNetwork.IsMasterClient)
        {
            //아이템이 있을경우
            Ray ray = new Ray(this.mainCam.transform.position, this.mainCam.transform.forward);

            if (Input.GetMouseButton(0))
            {
                this.ChangeAim(false);
                int layerMask = 1 << LayerMask.NameToLayer("VR");
                RaycastHit hit;
                if (Physics.Raycast(ray.origin, ray.direction, out hit, this.swapRange, layerMask))
                {
                    LockOnAnim(true);
                }
                else
                {
                    LockOnAnim(false);
                }
            }

            if (Input.GetMouseButtonUp(0) && this.itemCount >= 0)
            {
                this.crossHairGos[1].rectTransform.sizeDelta = new Vector2(100, 100); // 크로스헤어 사이즈조절
                this.ChangeAim(true);
            }
        }
        else if(photonView.IsMine && PhotonNetwork.IsMasterClient) //잡히는 순간 동작 
        {
            this.photonView.RPC("SetAnim", this.player);
        }
    }
    [PunRPC]
    private void SetAnim()
    {
        if (this.crossHairGos[0] != null && this.crossHairGos[1] != null)
        {
            this.crossHairGos[1].rectTransform.sizeDelta = new Vector2(100, 100); // 크로스헤어 사이즈조절
            this.ChangeAim(true);
        }

    }

    private void ChangeAim(bool state)
    {
        if (this.crossHairGos[0] != null && this.crossHairGos[1] != null)
        {
            this.crossHairGos[0].gameObject.SetActive(state);
            this.crossHairGos[1].gameObject.SetActive(!state);
        }
    }

    private void LockOnAnim(bool state)
    {
        if(this.uiAnim != null)
        {
            this.uiAnim.SetBool("LockOn", state);
        }
    }

    [PunRPC]
    private void GetItem()
    {
        if (this.itemCount >= this.maxItemCount)
        {
            this.itemCount = this.maxItemCount - 1;
        }
        if (this.itemCount <= 0)
        {
            this.itemCount = 0;
        }
        this.itemCount++;
        this.turnMp.AddItem();
    }

    [PunRPC]
    private void UseItem()
    {
        if (this.itemCount >= this.maxItemCount)
        {
            this.itemCount = this.maxItemCount;
        }
        if (this.itemCount <= 0)
        {
            this.itemCount = 0;
        }
        this.itemCount--;
        this.turnMp.UseItem();
    }

    private void ShootRocket(Vector3 direction)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, crossHairGos[0].transform.position, this.mainCam, out pos);
        var rot = Quaternion.LookRotation(direction);
        //공격
        GameObject itemGo = PhotonNetwork.Instantiate("Rocket", pos, rot);
        itemGo.GetComponent<Rigidbody>().AddForce(direction * shootPower, ForceMode.Impulse);
    }

    private void ShootMissile(Vector3 direction)
    {
        Vector3 pos;
        Vector2 screenCenter = this.mainCam.ViewportToScreenPoint(this.mainCam.rect.center);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, screenCenter, this.mainCam, out pos);
        var rot = Quaternion.LookRotation(direction);
        //공격
        GameObject itemGo = PhotonNetwork.Instantiate("MagicMissileProjectile", pos, rot);
        itemGo.GetComponent<Rigidbody>().AddForce(direction * shootPower, ForceMode.Impulse);
    }
    #endregion

    #region Defend
    private void Defend()
    {
        if (this.photonView.IsMine)
        {
            if (itemCount <= 0)
            {
                return;
            }

            if (Input.GetMouseButtonDown(2))
            {
                this.UseItem();
                this.photonView.RPC("UseItem", RpcTarget.OthersBuffered);
            }
            if (Input.GetMouseButton(2))
            {
                //마우스 가운데 버튼을 누르고 있는 경우 [방어하고있어야함]
                this.DefendAnimation(0);
                this.DefendRPC(false);
                this.photonView.RPC("DefendRPC", RpcTarget.OthersBuffered, false);
                //마나 소모 [몇초간 누르고 있으면 아이템 소모]
                this.UseMp();
            }
            else if (Input.GetMouseButtonUp(2))
            {
                //마우스 가운데 버튼을 뗀 경우 [방어 해제해야함]
                this.DefendAnimation(2);
                this.DefendRPC(true);
                this.photonView.RPC("DefendRPC", RpcTarget.OthersBuffered, true);
            }
        }
    }

    [PunRPC]
    private void DefendRPC(bool isUp)
    {
        if (isUp)
        {
            //this.DefendAnimation(2); //방어 끝 애니메이션 [Idle]
            this.shieldGo.SetActive(false); //쉴드 끝
        }
        else
        {
            //this.DefendAnimation(0); // 방어 시작 애니메이션
            this.shieldGo.SetActive(true); //쉴드 시작
        }
    }

    //누르고 있는 동안 몇 초 지나면 MP 소모
    private void UseMp()
    {
        if (!isCoroutineRunning)
        {
            this.StartCoroutine(this.CoUseMp());
        }
    }

    private IEnumerator CoUseMp()
    {
        this.isCoroutineRunning = true; //코루틴이 돌아가는지 확인

        float elapsedTime = 0f;

        while (true)
        {
            if (Input.GetMouseButton(2))
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 2.0f && itemCount > 0)
                {
                    elapsedTime = 0f;
                    this.UseItem();
                    this.photonView.RPC("UseItem", RpcTarget.OthersBuffered);
                }

                if (elapsedTime >= 2.0f && itemCount <= 0)
                {
                    this.DefendAnimation(2);
                    this.DefendRPC(true);
                    this.photonView.RPC("DefendRPC", RpcTarget.OthersBuffered, true);
                    break;
                }
            }
            else if (Input.GetMouseButtonUp(2))
            {
                if (elapsedTime <= 2.0f && itemCount <= 0)
                {
                    this.DefendAnimation(2);
                    this.DefendRPC(true);
                    this.photonView.RPC("DefendRPC", RpcTarget.OthersBuffered, true);
                    break;
                }
            }
            else
            {
                break;
            }
            yield return null;
        }
        this.isCoroutineRunning = false;
    }

    #endregion
    [PunRPC]
    public void HitDamage(int damage, Player player)
    {
        if (!this.isGameOver)
        {
            this.hp -= damage;
            this.TurnOffHp(this.hp);
            //Debug.LogFormat("{0} {1} 데미지", this.player.NickName, damage);
            this.coroutine = this.StartCoroutine(SetDamageMaterial(0.5f));
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer == player) EventDispatcher.instance.SendEvent((short)ePcEventType.PCHIT);
            if (this.hp <= 0)
            {
                this.hp = 0;
                //Debug.LogFormat("{0} 사망", this.player.NickName);
                this.onDie();
            }
            //Debug.LogFormat("<color=cyan>{2}의 hp {0} / {1}</color>", this.hp, this.maxHp, this.player.NickName);
        }
    }

    private void TurnOffHp(int hp)
    {
        if(hp <= 0) //0
        {
            this.turnHp.goCount = 0;
        }
        else if (hp <= 20) //1~20
        {
            this.turnHp.goCount = 1;
        }
        else if (hp <= 40) //21~40
        {
            this.turnHp.goCount = 2;
        }
        else if (hp <= 60) //41~60
        {
            this.turnHp.goCount = 3;
        }
        else if (hp <= 80) //61~80
        {
            this.turnHp.goCount = 4;
        }
        else if (hp <= 100) //81 ~ 100
        {
            this.turnHp.goCount = 5;
        }
        this.turnHp.UpdateGoActive();
        this.turnHp.UpdateGoPosition();
    }

    private IEnumerator SetDamageMaterial(float time)
    {
        Material[] materialsArray = bodyRenderer.materials;
        materialsArray[1] = damageMaterial;
        bodyRenderer.materials = materialsArray;

        yield return new WaitForSeconds(time);

        materialsArray[1] = null;
        bodyRenderer.materials = materialsArray;
    }

    private void SetUniqueNumber()
    {
        for (int i = 0; i < PcSettingManager.instance.arrPcPlayers.Length; i++)
        {
            if (PcSettingManager.instance.arrPcPlayers[i] != null)
            {
                if (PcSettingManager.instance.arrPcPlayers[i].ActorNumber == player.ActorNumber)
                {
                    this.uniqueNumber = i;
                }
            }
        }
    }

    [PunRPC]
    private void StopPcPlayer()
    {
        //Debug.Log("StopPcPlayer");
        this.pcAnim.SetInteger("isWalk", 0); //Idle 애니메이션
        this.isGameOver = true;
        this.col.enabled = false;
        this.rbody.velocity = new Vector3 (0, 0, 0);
        this.rbody.angularVelocity = new Vector3(0,0,0);
        this.rbody.useGravity = false;
    }

    [PunRPC]
    private void StartPcPlayer()
    {
        this.isGameOver = false;
        this.rbody.useGravity = true;
        this.col.enabled = true;
    }

    public void ChangePcPlayerBody(int num)
    { 

        //Debug.LogFormat("ChangePcPlayerBody!!!! {0}", num);
        switch (num)
        {
            case 0:
                this.bodyRenderer.material = Resources.Load<Material>("Material/Wizard1");
                break;
            case 1:
                this.bodyRenderer.material = Resources.Load<Material>("Material/Wizard2");
                break;
            case 2:
                this.bodyRenderer.material = Resources.Load<Material>("Material/Wizard3");
                break;
            case 3:
                this.bodyRenderer.material = Resources.Load<Material>("Material/Wizard4");
                break;

        }
    }

}
