using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeStress : MonoBehaviour
{
    //»«Ã≈Õ≈Õ»≈ —“–≈——¿

    private UILogManager uiManager;
    public Dropdown City;
    public Dropdown Sex;
    public Dropdown Groups;

    public Scrollbar Stress;
    public Scrollbar Duration;
    public CanvasGroup durationGroup;
    public Toggle needTime;
    public Toggle needRandomStress;

    public static bool changeStats;
    public static bool noStress;
    public static bool playerPresses;

    public GameObject stressZOne;
    public static GameObject stressZoneGO;

    public static int StressSt;
    public static int StressStTime;

    public static string StressCity;
    public static string StressSex;
    public static string StressGroups;
    public static Vector3 StressGroupsPos;

    private Button myButton;
    public GameObject GroupHolder;
    void Start()
    {
        myButton = GameObject.Find("ButtonStress").GetComponent<Button>();
        
        uiManager = FindObjectOfType<UILogManager>();
        GroupHolder = GameObject.Find("Main Camera");
        stressZoneGO = stressZOne;
        changeStats = true;
        Stress.value = 0.5f;
        Duration.value = 0.5f;
        
        needTime.isOn = false;
        if (needRandomStress != null)
        {
            needRandomStress.isOn = false;
            noStress = true;
        }


    }
    void Update()
    {
        if (changeStats)
        {
            ChangeGroupsList(Groups);
            changeStats = false;
        }

        StressSt = Stress.value < 0.25 ? -1 : (Stress.value > 0.75 ? 1 : 0);

        if (needTime.isOn)
        {
            StressStTime =Mathf.FloorToInt(Duration.value * (Duration.numberOfSteps - 1)) + 1;
            if (!Duration.enabled)
            {
                Duration.enabled = true;
                durationGroup.alpha = 1;
            }
        }
        else
        {
            StressStTime = 0;
            if (Duration.enabled)
            {
                Duration.enabled = false;
                durationGroup.alpha = 0.5f;
            }
        }
        if (needRandomStress != null)
        {
            if (needRandomStress.isOn && noStress)
            {
                noStress = false;
                StartCoroutine(RandomStress());
            }
        }
    }

    //œ≈–≈ƒ¿“‹ —“–≈——
    public void ButtonStressCheck() {

        if (playerPresses) { 
            SendStress(City, Sex, Groups, StressSt, StressStTime, uiManager);
            Stress.value = 0.5f;
        }
    }
    //—Œ«ƒ¿Õ»≈ «ŒÕ€ —“–≈——¿
    public void ButtonStressCreateZone()
    {
        SendStressIntoZone(City, Sex, Groups, StressSt, StressStTime, uiManager);

        Stress.value = 0.5f;
    }



    //œ≈–≈ƒ¿◊¿ —“–≈——¿ ◊≈–≈« UI
    public static void SendStress(Dropdown city, Dropdown sex, Dropdown group, int stress, int stressTime, UILogManager uiManager)
    {
        StressCity = city.options[city.value].text;
        StressSex = sex.options[sex.value].text;
        if (group.options[group.value].text != "None")
        {
            StressGroups = group.options[group.value].text;
            StressGroupsPos = GroupsStats.GetGroupPositionById(int.Parse(StressGroups) - 1);
        }
        else
        {
            StressGroups = null;
        }
        if (StressCity != "None" || StressSex != "None" || StressGroups != null)
        {
            string textAll = "";
            if (StressCity != "None")
            {
                textAll = "City: " + StressCity + " ";
            }
            if (StressSex != "None")
            {
                textAll += "Sex: " + StressSex + " ";
            }
            if (StressGroups != null)
            {
                textAll += "Group: " + StressGroups;
            }

            switch (stress)
            {
                case -1:
                    uiManager.LogStressPG(textAll);
                    break;
                case 0:
                    uiManager.LogStressNone(textAll);
                    break;
                case 1:
                    uiManager.LogStressNR(textAll);
                    break;
            }
        }
        city.value = 0;
        sex.value = 0;
        group.value = 0;
        ChangeGroupsList(group);
    }
    //œ≈–≈ƒ¿◊¿ —“–≈——¿ ◊≈–≈« «ŒÕ”
    public static void SendStressIntoZone(Dropdown city, Dropdown sex, Dropdown group, int stress, int stressTime, UILogManager uiManager)
    {
        GameObject zonePF = Instantiate(stressZoneGO, new Vector3(-30, 0, -30), Quaternion.identity);
       
        zonePF.GetComponent<StressZone>().StressCity = city.options[city.value].text;
        zonePF.GetComponent<StressZone>().StressSex = sex.options[sex.value].text;
        zonePF.GetComponent<StressZone>().stressType = stress;

        zonePF.GetComponent<StressZone>().stressDuraction = stressTime;
        if (group.options[group.value].text != "None")
        {
            zonePF.GetComponent<StressZone>().StressGroups = int.Parse(group.options[group.value].text);
        }
        else
        {
            zonePF.GetComponent<StressZone>().StressGroups = 0;
        }

        if (city.options[city.value].text != "None" || sex.options[sex.value].text != "None" || group.options[group.value].text != "None")
        {
            string textAll = "";
            if (city.options[city.value].text != "None")
            {
                textAll = "City: " + city.options[city.value].text+" ";
            }
            if (sex.options[sex.value].text != "None")
            {
                textAll += "Sex: " + sex.options[sex.value].text + " ";
            }
            if (group.options[group.value].text != "None")
            {
                textAll += "Group: " + group.options[group.value].text;
            }
            switch (stress)
            {
                case -1:
                    uiManager.LogStressPG(textAll);
                    break;
                case 0:
                    uiManager.LogStressNone(textAll);
                    break;
                case 1:
                    uiManager.LogStressNR(textAll);
                    break;
            }
        }
        city.value = 0;
        sex.value = 0;
        group.value = 0;

    }
    //»«Ã≈Õ»“‹ —œ»—Œ  √–”œœ
    public static void ChangeGroupsList(Dropdown group)
    {
        group.ClearOptions();
        List<string> groupIDs = new List<string>();
        groupIDs.Add("None");
        foreach (var groups in GroupsStats.Groups)
        {
            groupIDs.Add((groups.Id+1).ToString()); 
        }
        group.AddOptions(groupIDs);
    }

    // ŒÔÂ‰ÂÎÂÌËÂ ÍÎ‡ÒÒ‡ StressState
    private class StressState
    {
        public int StressLevel;
        public string City;
        public string Sex;
    }
    //---ƒÀﬂ “≈—“¿ 10 —“–≈——Œ¬€’ —»“”¿÷»…
    private List<StressState> stressStates = new List<StressState>
    {
        new StressState { StressLevel = 1, City = "FirsTown", Sex = "None" },
        new StressState { StressLevel = 1, City = "Secondity", Sex = "Male" },
        new StressState { StressLevel = -1, City = "ThirDown", Sex = "None" },
        new StressState { StressLevel = -1, City = "FirsTown", Sex = "Male" },
        new StressState { StressLevel = 1, City = "ThirDown", Sex = "Female" },
        new StressState { StressLevel = -1, City = "Secondity", Sex = "None" },
        new StressState { StressLevel = 1, City = "ThirDown", Sex = "None" },
        new StressState { StressLevel = -1, City = "ThirDown", Sex = "Female" },
        new StressState { StressLevel = -1, City = "Secondity", Sex = "Male" },
        new StressState { StressLevel = 1, City = "FirsTown", Sex = "Male" },
        new StressState { StressLevel = 1, City = "FirsTown", Sex = "Male" }
        
    };
    public void ButtonStressTenTimes()
    {
        StartCoroutine(SendStressSequence());
    }    
    private IEnumerator SendStressSequence()
    {
        foreach (var state in stressStates)
        {
              UpdateDropdowns(state.City, state.Sex,state.StressLevel);
            playerPresses = false;
            myButton.onClick.Invoke();
            SendStress(state.City, state.Sex, state.StressLevel, uiManager);

            yield return new WaitForSeconds(1);
        }
    }
    private void UpdateDropdowns(string city, string sex,int StressLevel)
    {
        City.value = City.options.FindIndex(option => option.text == city);
        Sex.value = Sex.options.FindIndex(option => option.text == sex);
        Stress.value = StressLevel;
    }
    
        public static void SendStress(string city, string sex, int stress, UILogManager uiManager)
    {
        StressCity = city;
        StressSex = sex;
        if (StressCity != "None" || StressSex != "None" )
        {
            string textAll = "";
            if (StressCity != "None")
            {
                textAll = "City: " + StressCity + " ";
            }
            if (StressSex != "None")
            {
                textAll += "Sex: " + StressSex + " ";
            }

            switch (stress)
            {
                case -1:
                    uiManager.LogStressPG(textAll);
                    break;
                case 0:
                    uiManager.LogStressNone(textAll);
                    break;
                case 1:
                    uiManager.LogStressNR(textAll);
                    break;
            }
        }
        playerPresses = true;
    }
    //---

    //~~~—À”◊¿…Õ€… —“–≈——
    private IEnumerator RandomStress()
    {
        while (needRandomStress.isOn)
        {
            int randomInterval = Random.Range(3, 6);
            int stress = Random.Range(0, 3) - 1; 
            int group;
            City.value = Random.Range(0, City.options.Count);
            Sex.value = Random.Range(0, Sex.options.Count);
            Stress.value = stress;
            if (City.value == 0 && Sex.value == 0)
            {
                Groups.value = Random.Range(1, Groups.options.Count);
                group = int.Parse(Groups.options[Groups.value].text);
            }
            else
            {
                group = 0;
            }
            Debug.Log(stress);
            SendStress(City.options[City.value].text, Sex.options[Sex.value].text, group, stress, uiManager);
            playerPresses = false;
            myButton.onClick.Invoke();
            City.value =0;
            Sex.value = 0;
            Stress.value = 0;
            yield return new WaitForSeconds(randomInterval);
        }
        noStress = true;
    }
    public static void SendStress(string city, string sex, int group, int stress, UILogManager uiManager)
    {
        StressCity = city;
        StressSex = sex;

        string textAll = "";
        if (StressCity != "None" || StressSex != "None")
        {
            if (StressCity != "None")
            {
                textAll = "City: " + StressCity + " ";
            }
            if (StressSex != "None")
            {
                textAll += "Sex: " + StressSex + " ";
            }


            StressGroups = null;
        }
        else {
            if (group != 0)
            {
                StressGroups = group+"";
                StressGroupsPos = GroupsStats.GetGroupPositionById(group - 1);
            }
        }


        switch (stress)
        {
            case -1:
                uiManager.LogStressPG(textAll);
                break;
            case 0:
                uiManager.LogStressNone(textAll);
                break;
            case 1:
                uiManager.LogStressNR(textAll);
                break;
        }
        playerPresses = true;
    }
    //~~~
}
