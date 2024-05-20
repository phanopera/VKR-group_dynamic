using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BelbinStatus;


public class PersonalData : MonoBehaviour
{
    public int maxMembersAmount = 10;

    public int id;
    public enum CityType { none, FirsTown, Secondity, ThirDown };
    public enum SexType { none, Male, Female };

    public int Age;
    public CityType cType;
    public SexType sType;

    public List<int> Groups;
    public Transform cityPos;
    public Vector3 groupPos;

    public TextMesh stats;
    public TextMeshPro ACT;//age,city,type
    public TextMeshPro GL;//group list
    public GameObject sexPic;//êàğòèíêà ïîëà
    public Sprite[] sexTypePic;//âàğèàíòû êàğòèíêè äëÿ îòîáğàæåíèÿ ïîëà

    private float speed = 5f;
    public float wanderRadius = 5f;

    private Rigidbody rb;
    private bool inWanderMode = false;
    private bool groupStress = false;
    public bool searchForNewGroup = false;
    private Vector3 wanderTarget;

    private CityType parsedCType;
    private SexType parsedSType;

    public int stress;
    public int stressTime;

    public Material[] stressMat;

    private Button myButton;
    private float timeSinceLastGroupSearch = 0f;
    private const float groupSearchInterval = 3f; // Èíòåğâàë ïîèñêà íîâîé ãğóïïû â ñåêóíäàõ

   
    public BelbinType type;
    public TextMesh statsBelbin;
    BelbinStatus myBelbin;

    private int attempts;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myBelbin = GameObject.Find("Main Camera").GetComponent<BelbinStatus>();
        myButton = GameObject.Find("ButtonStress").GetComponent<Button>();
        myButton.onClick.AddListener(ButtonStress);
        
       
        InitializeData();
        UpdateStats();
    }

    void InitializeData()
    {
        if (cType == CityType.none)
        {
            cType = (CityType)UnityEngine.Random.Range(1, 4);
        }
        if (sType == SexType.none)
        {
            sType = (SexType)UnityEngine.Random.Range(1, 3);
        }
        Age = Age < 18 ? UnityEngine.Random.Range(18, 99) : Age;

        switch (cType)
        {
            case CityType.FirsTown:
                cityPos = GameObject.Find("FirsTown").transform;
                ACT.outlineColor = new Color32(255, 255, 0, 255);
                GL.outlineColor = new Color32(255, 255, 0, 255);
                break;
            case CityType.Secondity:
                cityPos = GameObject.Find("Secondity").transform;
                ACT.outlineColor = new Color32(0, 255, 0, 255);
                GL.outlineColor = new Color32(0, 255, 0, 255);
                break;
            case CityType.ThirDown:
                cityPos = GameObject.Find("ThirDown").transform;
                ACT.outlineColor = new Color32(0, 0, 255, 255);
                GL.outlineColor = new Color32(0, 0, 255, 255);
                break;
        }
        switch (sType)
        {
            case SexType.Male:
                sexPic.GetComponent<SpriteRenderer>().sprite = sexTypePic[0];
                break;
            case SexType.Female:
                sexPic.GetComponent<SpriteRenderer>().sprite = sexTypePic[1];
                break;
        }
        if (type == BelbinType.none)
        {
            MakeStatus();
        }
        ACT.text = id + " / " + ("" + type).Substring(0,6);
    }

    void Update()
    {
        if (searchForNewGroup)
        {
            timeSinceLastGroupSearch += Time.deltaTime;
            if (timeSinceLastGroupSearch >= groupSearchInterval)
            {
                SearchNewGroup();
                searchForNewGroup = false;
                timeSinceLastGroupSearch = 0f;
            }
        }
    }


    void FixedUpdate()
    {
        if (!groupStress)
        {
            if (Time.frameCount % 10 == 0)
            {
                if (Vector3.Distance(transform.position, cityPos.position) <= wanderRadius)
                {
                    inWanderMode = true;
                }
            }

            if (inWanderMode)
            {
                Wander(); 
            }
            else
            {
                MoveTowards(cityPos.position);
            }
        }
        else
        {
            MoveTowards(groupPos);
            if (Vector3.Distance(transform.position, groupPos) <= wanderRadius && stress < 1|| Vector3.Distance(new Vector3(0,0,0), groupPos) <= 1f)
            {
                groupStress = false;
            }
        }

        UpdateStats();

        if (searchForNewGroup)
        {
            searchForNewGroup = false;
            SearchNewGroup(); 
        }
    }
    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }

    void Wander()
    {
        if (wanderTarget == Vector3.zero || Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * wanderRadius;
            wanderTarget = cityPos.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
        MoveTowards(wanderTarget);
    }

    void UpdateStats()
    {
        if (HideStats.isHidden&&!(ACT.text == ""))
        {
            stats.text = "";
            statsBelbin.text = "";
            ACT.text = "";
            GL.text = "";

        }
        else if(!HideStats.isHidden&& (stats.text==""|| stats.text == "Hello World"))
        {
            GL.text = "id "+id+": ";
            ACT.text = id + " / " + (""+type).Substring(0,6);
            for (int index = 0; index < Groups.Count;index++)
            {
                if (index == Groups.Count - 1) {
                    GL.text += (Groups[index]+1);
                }
                else
                {
                    GL.text += (Groups[index] + 1) + ", ";
                }
            }
        }
    }

    //ÎÏĞÅÄÅËÅÍÈÅ ÑËÓ×ÀÉÍÎÃÎ ÒÈÏÀ ÁÅËÁÈÍÀ 
    void MakeStatus()
    {
        // Ïîëó÷àåì âñå çíà÷åíèÿ òèïîâ, êğîìå 'none', â âèäå ìàññèâà
        var types = Enum.GetValues(typeof(BelbinType))
                    .Cast<BelbinType>()
                    .Where(t => t != BelbinType.none)
                    .ToArray();
        type = types[UnityEngine.Random.Range(0, types.Length)];

        for (int i = 0; i < Groups.Count; i++)
        {
            GroupsStats.SetBelbin(id, type, Groups[i]);
        }
    }






    //ÏÎËÓ×ÅÍÈÅ ÑÒĞÅÑÑÀ
    void ButtonStress()
     {
         bool bCity = Enum.TryParse(ChangeStress.StressCity, out parsedCType);
         bool bSex = Enum.TryParse(ChangeStress.StressSex, out parsedSType);
         if (bCity || bSex)
         {
             if (parsedCType == cType && parsedSType == sType)
             {

                 stress += ChangeStress.StressSt;
             }
             if (parsedCType == cType && ChangeStress.StressSex == "None")
             {
                 stress += ChangeStress.StressSt;
             }
             if (parsedSType == sType && ChangeStress.StressCity == "None")
             {
                 stress += ChangeStress.StressSt;
             }

            if (stress != 0)
            {
                if (ChangeStress.StressStTime > stressTime)
                {
                    stressTime = ChangeStress.StressStTime;
                }
                for (int index = 0; index < Groups.Count; index++)
                {
                    GroupsStats.GetStress(Groups[index], id, stress);
                }

            }
        }
        if (ChangeStress.StressGroups != null)
        {
            int index = Groups.IndexOf(int.Parse(ChangeStress.StressGroups) - 1);
            if (index != -1 && index < Groups.Count)
            {
                stress += ChangeStress.StressSt;
                groupStress = true;
                if (stress != 0)
                {
                    if (ChangeStress.StressStTime > stressTime)
                    {
                        stressTime = ChangeStress.StressStTime;
                    }
                    GroupsStats.GetStress(Groups[index], id, stress);
                }
            }
        }


        if (stress > 1)
         {
             stress = 1;
         }
         if (stress < -1)
         {
             stress = -1;
         }

         CheckStressColor();
         if (ChangeStress.StressStTime>0) { 
             StartCoroutine(StressCountdown());
         }

    }
    //ÎÒÑ×ÅÒ ÂĞÅÌÅÍÈ ÎÙÓÙÅÍÈß ÑÒĞÅÑÑÀ
    private IEnumerator StressCountdown()
    {
        while (stressTime > 0)
        {
            yield return new WaitForSeconds(1); 
            stressTime--; 
            Debug.Log("Current stressTime: " + stressTime);
        }
        stress = 0;
        CheckStressColor();
    }
    //ÏĞÎÂÅĞÈÒÜ ÖÂÅÒ ÊÓÁÀ
    void CheckStressColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = stressMat[Mathf.Clamp(stress, -1, 1) + 1];
    }
    //ÑÌÅÍÀ ÒÅÊÑÒÓĞÛ ÊÓÁÀ, ÑÎÃËÀÑÍÎ ÓĞÎÂÍŞ ÑÒĞÅÑÑÀ
    void ChangeMaterials(Transform currentTransform, Material newMat)
    {
       MeshRenderer meshRenderer = currentTransform.GetComponent<MeshRenderer>();

       if (meshRenderer != null)
       {
          meshRenderer.material = newMat;
       }
    }


    //ÏÎÈÑÊ ÍÎÂÎÉ ÃĞÓÏÏÛ
    void SearchNewGroup()
    {
        while (Groups.Count < 3 && attempts < 4)
        {
            int groupId = UnityEngine.Random.Range(0, GroupsStats.Groups.Count);
            int group = GroupsStats.Groups[groupId].Id;
            if (Groups.IndexOf(group) == -1 && GroupsStats.Groups[groupId].membersId.Count() < maxMembersAmount)
            {
                GroupsStats.AddMember(group, id, gameObject);
                return;
            }
            else
            {
                attempts++;
                if (attempts >= 4)
                {
                    GroupsStats.CreateGroupForLostMembers(id);
                    return; 
                }
            }
        }
        attempts = 0; 
    }


    //ĞÅÀÃÈĞÎÂÀÍÈÅ ÍÀ ÂÇÀÈÌÎÄÅÉÑÒÂÈÅ ÑÎ ÑÒĞÅÑÑÎÂÎÉ ÇÎÍÎÉ
    private void OnTriggerEnter(Collider other)
    {
        ZoneStress(other);
    }
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(StressCountdown());
    }
    
    
    void ZoneStress(Collider other)
    {
        StressZone sZ= other.GetComponent<StressZone>();

        if ((sZ.StressCity != "None") || (sZ.StressSex != "None"))
        {
            if (sZ.StressCity == (cType+"") && parsedSType == sType)
            {
                stress += sZ.stressType;
            }
            if (sZ.StressCity == (cType + "") && sZ.StressSex == "None")
            {
                stress += sZ.stressType;
            }
            if (sZ.StressSex == (sType + "") && sZ.StressCity == "None")
            {
                stress += sZ.stressType;
            }

            if (stress != 0)
            {
                for (int index = 0; index < Groups.Count; index++)
                {
                    GroupsStats.GetStress(Groups[index], id, stress);
                }
                if (sZ.stressDuraction > stressTime) {
                    stressTime= sZ.stressDuraction;
                }
            }
        }
        if (sZ.StressGroups!=0)
        {
            int index = Groups.IndexOf(sZ.StressGroups - 1);
            if (index != -1 && index < Groups.Count)
            {
                stress += sZ.stressType;
                groupStress = true;
                if (stress != 0)
                {
                    GroupsStats.GetStress(Groups[index], id, stress); 
                    if (sZ.stressDuraction > stressTime)
                    {
                        stressTime = sZ.stressDuraction;
                    }
                }
            }
        }
        if (stress > 1)
        {
            stress = 1;
        }
        if (stress < -1)
        {
            stress = -1;
        }
        CheckStressColor();
    }
}
