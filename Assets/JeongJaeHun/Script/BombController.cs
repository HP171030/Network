using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    private Camera mainCamera; //메인 카메라
    
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float throwPower; //수류탄 투척력

    [SerializeField] private Bomb currentBomb; //현재 들고 있는 폭탄 --> 수류탄 프리팹이나 마찬가지 아니냐?

    [SerializeField] private Rigidbody bombRigidbody; // 자식의 폭탄의 리지드바디 가져와서 kinematic 으로 해주고 던지면 해제. 

    private GameObject instanceBomb; //실제 날리는게 아니라 생성해서 날려줄 폭탄. 


    public Bomb GetBomb() { return currentBomb; }

    // 붐이 지금 준비가 되어 있는 상태라면... 궤적을 보여줘야함. 

    private void OnEnable() //기본적으로 꺼져있다가 켜질 거기 때문에 궤적 여기서 그려주기.
    {

        // currentBomb을 찾으려면?? --> 자기 자신의 자식들 중에서 (active 되어 있는 것을 찾아서 넣어주자.)
        int numOfChild = this.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf == true)
            {
                currentBomb = transform.GetChild(i).GetComponent<Bomb>();

            }
        }

        Rigidbody bombRigidbody = transform.GetChild(0).gameObject.GetComponent<Rigidbody>();
        bombRigidbody.isKinematic = true;
        mainCamera = Camera.main;
        lineRenderer = currentBomb.GetComponent<LineRenderer>();
        lineRenderer.enabled = true; // 렌더러 켜주기. 
        ShowTrajectory();
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Throw(); // 임시로 폭탄 발사. 
        }
    }

    

    private void Start()
    {
        
    }

    private void Throw() //투척 함수 
    {
        if(currentBomb.currentBombNumber<=0)
        {
            Debug.Log("폭탄숫자 부족");
            return; //발사불가능
        }

        


        // 이게 손에 들어가 있는 상황에서는 transform을 어떻게 잡아줘야 될지 고민되네
        // local 인지 전역 position인지.. 
        Vector3 position = transform.position;
        Vector3 forward = mainCamera.transform.forward; //카메라의 전방으로 라인을 그릴 거기 때문에 
        Vector3 startVelocity = throwPower * forward; // 전방으로 던지는 힘. 
        Vector3 startPosition = transform.position;

        instanceBomb = Instantiate(currentBomb.gameObject, startPosition,Quaternion.identity);



        //Vector3 startPosition = transform.localPosition;  --> 이거 실험해보자. 

        //아무튼 실제로는 던져도 실물을 던지는게 아니라 복사본? 약간 그런걸 던지면됨. 
        Rigidbody rigidGrenade = instanceBomb.GetComponent<Rigidbody>();
        rigidGrenade.isKinematic = false; //키네마틱 해제. 
        rigidGrenade.AddForce(startVelocity, ForceMode.Impulse);

        currentBomb.currentBombNumber--;
       
        if(currentBomb.currentBombNumber<=0) //이거 폭탄 컨트롤러가 아니라 폭탄 자체를 꺼주면 아마 못찾을거야 체크를 그렇게하니까
        {
            currentBomb.gameObject.SetActive(false);    
        }
        




    }

    private void ShowTrajectory() //궤도를 보여줌 (라인렌더러)
    {
        // 궤적 표시 관련 변수 초기화
        int linePoint = 1;
        float timePoint = 0.1f;

        //라인 렌더러 점 개수 설정
        lineRenderer.positionCount=Mathf.CeilToInt(linePoint / timePoint) + 1;

        // 초기 위치 및 방향 설정
        Vector3 forward = mainCamera.transform.forward;
        Vector3 startVelocity = throwPower * forward;
        Vector3 startPosition = transform.position; //홀더 위치? --> 홀더도 자식이니까 실제 홀더면 local인가?
        

        //초기 위치 설정
        lineRenderer.SetPosition(0, startPosition);

        for(int i=0;i<lineRenderer.positionCount;i++) // 1/0.1 이므로 10개의 선으로 이루어져 곡선을 그림
        {
            float time = i * timePoint;

            // 시간에 따른 궤적 위치 계산 
            Vector3 point = startPosition + (time * startVelocity);
            point.y=startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            // 라인 렌더러에 궤적 위치 설정
            lineRenderer.SetPosition(i, point);
        }

    }


}
