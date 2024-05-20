using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static BelbinStatus;
using UnityEngine.SceneManagement;

public class GroupsStats : MonoBehaviour
{
    public static UILogManager uiManager;
    public static int maxAmount =30;
    public static int maxMemAmount =11;//Стандарт - 11
    public static int BelbinBest = 3;//Стандарт - 3
    public int MemberId { get; set; }
    public static Text uiF;
    public static Text uiS;
    public InputField idMemField;
    public Text[] textStats;
    public Button myButton;
    public static List<Group> Groups { get; private set; }
    public List<BelbinType> belbinTypes { get; set; }
    public static int groupsCounter = 0;

    public static int peopleAmount;
    public GameObject person;
    public static GameObject personSt;
    public static BelbinStatus belbin;

    public GameObject groupPrefub;
    public static GameObject groupLocation;
    [System.Serializable]
    public class Group
    {
        public int Id { get; set; } //id группы
        public int membersAmount { get; set; } //количество участников в группе
        public Vector3 pos { get; set; } //позиция группы в пространстве
        public List<int> membersId { get; set; } //id участникав игровом мире
        public List<int> mStatus { get; set; } //позиция в группе
        public List<BelbinType> belbinTypes { get; set; }  // Типы Белбина участников

        public float groupStress; //уровень стресса группы
        public int mostStressed; //наиболее напряженный участник
        public int lessStressed; //наиболее счастливый участник
        public int notStressed; //участник более нейтральный



    }
    void Awake()
    {
        uiF = textStats[0];
        uiS = textStats[1];
        personSt = person;
        Groups = new List<Group>();
        belbin = GetComponent<BelbinStatus>();
        groupLocation = groupPrefub;
    }


    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UILogManager>();



        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName== "10TIMESTEST")
        {
            //набор параметров групп для теста 10 стрессовых ситуаций
            //инициализация групп через id группы и id участников, согласно их позиции в группе
            StartCoroutine(DoSomethingWithDelay(0, new List<int> { 0, 1, 2, 3 }));
            StartCoroutine(DoSomethingWithDelay(1, new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 }));
            StartCoroutine(DoSomethingWithDelay(2, new List<int> { 8, 9, 10, 11, 15, 19, 7, 12 }));
            StartCoroutine(DoSomethingWithDelay(3, new List<int> { 12, 13, 9, 15, 16 }));
            StartCoroutine(DoSomethingWithDelay(4, new List<int> { 14, 4, 15, 16, 17, 1, 7, 18 }));
            StartCoroutine(DoSomethingWithDelay(5, new List<int> { 18, 17, 19, 14, 6, 5, 9, 11 }));
            StartCoroutine(DoSomethingWithDelay(6, new List<int> { 6, 10, 8, 13, 0, 18, 16, 12 }));
        }
        else {
            //инициализация групп через количество участников в группе

            AddNewGroup(4);
            StartCoroutine(DoSomethingWithDelay(5));
            StartCoroutine(DoSomethingWithDelay(3));
            StartCoroutine(DoSomethingWithDelay(3));
            StartCoroutine(DoSomethingWithDelay(5));
            StartCoroutine(DoSomethingWithDelay(4));
            StartCoroutine(DoSomethingWithDelay(3)); 
        }


        myButton = GameObject.Find("ButtonStress").GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(ButtonStress);
        }
    }
    //инициализация группы через количество участников в группе
    IEnumerator DoSomethingWithDelay(int i)
    {
        yield return new WaitForSeconds(1f); 
        AddNewGroup(i);
    }
    //инициализация групп через id группы и id участников, согласно их позиции в группе
    IEnumerator DoSomethingWithDelay(int i, List<int> memberIds)
    {
        yield return new WaitForSeconds(1f); 
        AddNewGroup(i, memberIds);
    }


    //ОТОБРАЖЕНИЕ СПИСКА ГРУПП
    public static void ShowGroupInfo()
    {
        Text show;
        uiF.text = "";
        uiS.text = "";

        for (int groupid = 0; groupid < Groups.Count; groupid++)
        {
            if (groupid % 2 == 0)
            {
                show = uiF;
            }
            else
            {
                show = uiS;
            }

            show.text += "Group:" + (Groups[groupid].Id + 1) + " (" + Groups[groupid].membersAmount + ") " + "\n";

            for (int i = 0; i < Groups[groupid].membersAmount; i++)
            {
                string typeText = Groups[groupid].belbinTypes[i] + "";
                if (typeText.Length >= 5)
                {
                    show.text += Groups[groupid].mStatus[i] + ") id " + Groups[groupid].membersId[i] + " " + typeText.Substring(0, 5) + "\n";
                }
                else
                {
                    show.text += Groups[groupid].mStatus[i] + ") id " + Groups[groupid].membersId[i] + " " + typeText + "\n";
                }
            }
            show.text += "~~~~~~" + "\n";

        }
        ChangeStress.changeStats = true;
    }

    //ПОКАЗ ЗНАЧЕНИЙ В ГРУППАХ
    public static void ShowGroupInfo(int id)
    {
        int groupid = GetGroupIndexById(id);
        Text show;
        if (Groups[groupid].Id % 2 == 0)
        {
            show = uiF;
        }
        else
        {
            show = uiS;
        }
        show.text += "Group:" + (Groups[groupid].Id + 1) + " (" + Groups[groupid].membersAmount + ") " + "\n";

        for (int i = 0; i < Groups[groupid].membersAmount; i++)
        {
            show.text += Groups[groupid].mStatus[i] + ") id " + Groups[groupid].membersId[i] + " " + Groups[groupid].membersId[i] + "\n";
        }
        show.text += "~~~~~~" + "\n";
    }

    //ДОБАВИТЬ УЧАСТНИКА В ГРУППУ (КОНКРЕТНЫЕ УЧАСТНИКИ-СТАТУСЫ, КОНКРЕТНЫЕ УЧАСТНИКИ ИЗ ПЕРСОНАЛИИ)
    public static void AddMember(int groupId, int id, int status, BelbinType type)
    {
        int groupid = GetGroupIndexById(groupId);
        if (Groups[groupid] != null)
        {
            Groups[groupid].membersAmount++;
            Groups[groupid].membersId.Add(id);
            Groups[groupid].mStatus.Add(status);
            Groups[groupid].belbinTypes.Add(type);
        }
    }
    //ДОБАВИТЬ УЧАСТНИКА В ГРУППУ 
    public static void AddMember(int groupId, int id, GameObject person)
    {
        int groupid = GetGroupIndexById(groupId);
        if (groupid != -1 && Groups[groupid] != null) {

            if (Groups[groupid].membersId == null)
            {
                Groups[groupid].membersId = new List<int>();
            }
            Groups[groupid].membersAmount++;
            Groups[groupid].membersId.Add(id);
            if (Groups[groupid].mStatus == null)
            {
                Groups[groupid].mStatus = new List<int>();
            }

            if (Groups[groupid].membersAmount < maxMemAmount)
            {
                Groups[groupid].mStatus.Add(Groups[groupid].membersAmount - 1);
            }
            else {
                Groups[groupid].mStatus.Add(UnityEngine.Random.Range(9, 11));
            }
            if (Groups[groupid].belbinTypes == null)
            {
                Groups[groupid].belbinTypes = new List<BelbinType>();
            }
            Groups[groupid].belbinTypes.Add(person.GetComponent<PersonalData>().type);
            

            person.GetComponent<PersonalData>().Groups.Add(Groups[groupid].Id); 
            ShowGroupInfo();

        }
    }
    //СОЗДАНИЕ ГРУППЫ ИЗ ПОТЕРЯШЕК
    public static void CreateGroupForLostMembers(int memberId)
    {
        List<int> memberIds = new List<int> { memberId };
        foreach (GameObject person in FindObjectsOfType<GameObject>())
        {
            PersonalData data = person.GetComponent<PersonalData>();
            if (data != null && data.searchForNewGroup)
            {
                memberIds.Add(data.id);
            }
        }
        AddNewGroup(memberIds);
    }

    //УДАЛЕНИЕ ГРУППЫ И ПЕРЕМЕШИВАНИЕ
    public static void DeleteMember(int groupid, int id)
    {
        if (Groups[groupid] != null && Groups[groupid].membersId.IndexOf(id) != -1)
        {
            MakeGroupOfLeavers(groupid,id, Groups[groupid].belbinTypes[Groups[groupid].membersId.IndexOf(id)]);
            for (int i = Groups[groupid].membersId.IndexOf(id); i < Groups[groupid].membersAmount - 1; i++)
            {
                Groups[groupid].membersId[i] = Groups[groupid].membersId[i + 1];
                Groups[groupid].belbinTypes[i] = Groups[groupid].belbinTypes[i + 1];
            }
            Groups[groupid].membersId.RemoveAt(Groups[groupid].membersId.Count - 1);
            Groups[groupid].belbinTypes.RemoveAt(Groups[groupid].belbinTypes.Count - 1);
            Groups[groupid].mStatus.RemoveAt(Groups[groupid].mStatus.Count - 1);
            Groups[groupid].membersAmount--;
            GameObject gO = GameObject.Find("Person" + id);
            gO.GetComponent<PersonalData>().Groups.RemoveAt(gO.GetComponent<PersonalData>().Groups.IndexOf(Groups[groupid].Id));
            gO.GetComponent<PersonalData>().groupPos = new Vector3(0,0,0);
            gO.GetComponent<PersonalData>().searchForNewGroup = true;


            if (Groups[groupid].membersAmount < 2)
            {
                DeleteGroup(groupid);
            }
            else
            {
                ShuffleMembers(groupid);
            }
            ShowGroupInfo();
        }
    }
    //ПЕРЕМЕШИВАНИЕ УЧАСТНИКОВ, СОГЛАСНО ИХ ТИПУ ПО БЕЛБИНУ, ПОСЛЕ ВЫХОДА УЧАСТНИКА ИЗ ГРУППЫ 
    public static void ShuffleMembers(int groupId)
    {
        List<int> sortedIndices = Groups[groupId].belbinTypes
               .Select((type, index) => new { Index = index, LeaderId = belbin.belbinTypeAttributes[type].leaderId })
               .OrderBy(x => x.LeaderId)
               .Select(x => x.Index)
               .ToList();

        List<int> newMembersId = new List<int>();
        List<BelbinType> newBelbinTypes = new List<BelbinType>();

        foreach (int sortedIndex in sortedIndices)
        {
            newMembersId.Add(Groups[groupId].membersId[sortedIndex]);
            newBelbinTypes.Add(Groups[groupId].belbinTypes[sortedIndex]);
        }

        Groups[groupId].membersId = newMembersId;
        Groups[groupId].belbinTypes = newBelbinTypes;
    }
    //СОЗДАНИЕ ГРУППЫ ИЗ ВЫХОДЯЩИХ ИЗ ГРУППЫ
    public static void MakeGroupOfLeavers(int groupId, int leavingId, BelbinType leavingType)
    {
        bool highLeaderLeft = belbin.belbinTypeAttributes[leavingType].leaderId <= BelbinBest;


        int seekingNewGroupCount = Groups[groupId].membersId.Count(id => {
            var personObject = GameObject.Find("Person" + id);
            if (personObject == null)
            {
                return false;
            }
            var personalData = personObject.GetComponent<PersonalData>();
            if (personalData == null)
            {
                return false;
            }
            return personalData.searchForNewGroup;
        });
        bool multipleLeavings = seekingNewGroupCount > 1;

        if (highLeaderLeft && multipleLeavings)
        {
            List<int> leavingMembers = Groups[groupId].membersId
                .Where(id => GameObject.Find("Person" + id).GetComponent<PersonalData>().searchForNewGroup)
                .ToList();

            AddNewGroup(leavingMembers);
        }
    }
    //УДАЛЕНИЕ ГРУППЫ С МАЛЫМ КОЛИЧЕСТВОМ УЧАСТНИКОВ
    public static void DeleteGroup(int groupid)
    {
        if (Groups[groupid] != null)
        {
            if (Groups[groupid].membersAmount != 0)
            {
                    GameObject gO = GameObject.Find("Person" + Groups[groupid].membersId[0]);
                    gO.GetComponent<PersonalData>().Groups.Remove(groupid); // Удаление инфы о группе 
                    gO.GetComponent<PersonalData>().groupPos = new Vector3(0,0,0); // Поиск группы
                    gO.GetComponent<PersonalData>().searchForNewGroup = true; // Поиск группы
                    Groups[groupid].membersAmount--;
            }
            uiManager.LogGroup("- - Group id " + (Groups[groupid].Id + 1) + " deleted");
            Destroy(GameObject.Find("--Group" + (Groups[groupid].Id + 1)));
            Groups.RemoveAt(groupid); 
        }
    }

    //СОЗДАНИЕ НОВОЙ ЧИСТОЙ ГРУППЫ (РАНДОМНОЕ КОЛИЧЕСТВО, КОЛИЧЕСТВО, СЛУЧАЙНОЕ ДОБАВЛЕНИЕ УЧАСТНИКОВ) 
    public static void AddNewGroup(List<int> memberIds) 
    {

        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0, UnityEngine.Random.Range(-40f, 40f));
        Group newGroup = new Group
        {
            Id = groupsCounter++,
            membersAmount = 0,
            pos = randomPosition,
            membersId = new List<int>(),
            mStatus = new List<int>(),
            belbinTypes = new List<BelbinType>()
        };
        Groups.Add(newGroup);
        GameObject groupPrefab = Instantiate(groupLocation, randomPosition, Quaternion.identity);
        groupPrefab.name = "--Group" + groupsCounter;


        foreach (var memberId in memberIds)
        {
            GameObject person = GameObject.Find("Person" + memberId);
            AddMember(newGroup.Id, memberId, person);
            person.GetComponent<PersonalData>().searchForNewGroup = false;
        }
        uiManager.LogGroup("~~New group created id: " + groupsCounter);
    }

    //СОЗДАНИЕ НОВОЙ ГРУППЫ С ПЕРЕЧНЕМ УЧАСТНИКОВ
    public static void AddNewGroup(int id,List<int> memberIds) //запись вышедших из группы
    {

        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0, UnityEngine.Random.Range(-40f, 40f));
        Group newGroup = new Group
        {
            Id = id,
            membersAmount = 0,
            pos = randomPosition,
            membersId = new List<int>(),
            mStatus = new List<int>(),
            belbinTypes = new List<BelbinType>()
        }; groupsCounter++;
        Groups.Add(newGroup);
        GameObject groupPrefab = Instantiate(groupLocation, randomPosition, Quaternion.identity);
        groupPrefab.name = "--Group" + (id+1);


        foreach (var memberId in memberIds)
        {
            GameObject person = GameObject.Find("Person" + memberId);
            AddMember(newGroup.Id, memberId, person);
        }
        uiManager.LogGroup("~~New group created id: " + (id + 1));
    }

    //создание группы случайного размера
    public static void AddNewGroup() 
    {
        groupsCounter++;
        int amount = UnityEngine.Random.Range(3, maxMemAmount);
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-40f,40f), 0, UnityEngine.Random.Range(-40f,40f));
        Group newGroup = new Group { Id = groupsCounter - 1, pos = randomPosition, membersAmount = 0 };
        Groups.Add(newGroup);
        GameObject groupPrefab = Instantiate(groupLocation, randomPosition, Quaternion.identity);
        groupPrefab.name = "--Group" + groupsCounter;

        if (peopleAmount < amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int memberNum = UnityEngine.Random.Range(0, amount);
                SearchNext(newGroup, memberNum, amount);
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                int memberNum = UnityEngine.Random.Range(0, peopleAmount);
                SearchNext(newGroup, memberNum, peopleAmount);
            }
        }
        uiManager.LogGroup("~~New group created id: " + groupsCounter);
    }
    //создание группы заданного размера
    public static void AddNewGroup(int amount) 
    {
        groupsCounter++;
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-40f,40f), 0, UnityEngine.Random.Range(-40f,40f));
        Group newGroup = new Group { Id = groupsCounter - 1, pos = randomPosition, membersAmount = 0 };
        Groups.Add(newGroup);
        GameObject groupPrefab = Instantiate(groupLocation, randomPosition, Quaternion.identity);
        groupPrefab.name = "--Group" + groupsCounter;

        if (peopleAmount < amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SearchNext(newGroup, i, amount);
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                int memberNum = UnityEngine.Random.Range(0, maxAmount+1);
                SearchNext(newGroup, memberNum, peopleAmount);
            }
        }
        uiManager.LogGroup("~~New group created id: " + groupsCounter);
    }


    //ПОИСК УЧАСТНИКА НА ДОБАВЛЕНИЕ В ГРУППУ
    public static void SearchNext(Group newGroup, int id, int amount) {

        if (id < peopleAmount)
        {
            GameObject person = (GameObject.Find("Person" + id));
            if (newGroup.membersId == null || newGroup.membersId.IndexOf(id) == -1)
            {
                AddMember(newGroup.Id, id, person);
            }
            else
            {
                id = UnityEngine.Random.Range(0, amount);
                SearchNext(newGroup, id, amount);
            }
        }
        else
        {
            if (peopleAmount < maxAmount)
            {
                GameObject newPerson = Instantiate(personSt, new Vector3(UnityEngine.Random.Range(-24, 25), 0, UnityEngine.Random.Range(-24, 25)), Quaternion.identity);
                newPerson.GetComponent<PersonalData>().id = peopleAmount;
                newPerson.GetComponent<PersonalData>().gameObject.name = "Person" + peopleAmount;
                newPerson.GetComponent<PersonalData>().Groups = new List<int>();
                AddMember(newGroup.Id, peopleAmount, newPerson);
                peopleAmount++;
            }
            else
            {
                id = UnityEngine.Random.Range(0, amount);
                SearchNext(newGroup, id, amount);
            }
        }

    }









    //ПОСЛАТЬ СТРЕСС
   public void ButtonStress()
    {
        if (ChangeStress.StressGroups != null)
        {
            int id = GetGroupIndexById(int.Parse(ChangeStress.StressGroups) - 1);

            if (Groups[id] != null)
            {
                Groups[id].groupStress = 0;
                Groups[id].lessStressed = -1;
                Groups[id].mostStressed = -1;
                Groups[id].notStressed = -1;

                for (int i = 0; i < Groups[id].membersAmount; i++)
                {
                    PersonalData person = GameObject.Find("Person" + Groups[id].membersId[i]).GetComponent<PersonalData>();
                    int stressPerson = person.stress;
                    person.groupPos = Groups[id].pos;
                    //поиск id с наибольшиим статусами стресса и счастья
                    if (Groups[id].mostStressed == -1 && stressPerson == 1)
                    {
                        Groups[id].mostStressed = i;
                    }
                    if (Groups[id].lessStressed == -1 && stressPerson == -1)
                    {
                        Groups[id].lessStressed = i;
                    }
                    if (Groups[id].notStressed == -1 && stressPerson == 0)
                    {
                        Groups[id].notStressed = i;
                    }
                    Groups[id].groupStress += stressPerson;
                    //выяснение стресса участника с наибольшим статусом

                }
                Groups[id].groupStress = Groups[id].groupStress / Groups[id].membersAmount;
            }

        }
    }

    public static void GetStress(int Id, int mId, int stress)
    {
        int groupId = GetGroupIndexById(Id);
        if (groupId != -1)
        {
            if ((Groups[groupId].groupStress == -1 && stress == -1) || (Groups[groupId] == null))
            {
                // Если стресс группы уже -1 и пытаемся применить -1 снова, пропускаем изменение
                return;
            }

            float Z;
            int status = Groups[groupId].membersId.IndexOf(mId);
            int gR;
            if (stress > 0)
            {
                gR = Groups[groupId].lessStressed;
                if (gR == -1)
                {
                    gR = Groups[groupId].notStressed;
                }
            }
            else
            {
                gR = Groups[groupId].mostStressed;
                if (gR == -1)
                {
                    gR = Groups[groupId].notStressed;
                }
            }

            // Обработка ситуации, когда не найден индекс группы
            if (gR == -1 || gR >= Groups[groupId].membersId.Count)
            {
                Debug.Log("All group is in stress!!!");
                return; // Досрочный выход из метода, если индекс не действителен
            }
            BelbinType belbinType = Groups[groupId].belbinTypes[status];
            var attributes = belbin.belbinTypeAttributes[belbinType];
            BelbinType enemyType = Groups[groupId].belbinTypes[gR];
            var enemyAttr = belbin.belbinTypeAttributes[enemyType];

            Z = (attributes.M * attributes.C * status - attributes.E * (11 - gR) * enemyAttr.C) * Math.Abs(Groups[groupId].groupStress - stress);
            if (attributes.line <= Z)
            {
             //   Debug.Log(mId + " (" + Z + ")  --->  " + " stay O O O    in" + Groups[groupId].Id);
            }
            else
            {
                //****Debug.Log(mId + " (" + Z + ")  -X->  " + " leave X X X    from" + Groups[groupId].Id);
                if (Groups[groupId] != null)
                {
                    DeleteMember(groupId, mId);
                }
            }
        }
    }

    public static int GetStressTime(int Id, int mId, int stress, int stressTime)
    {
        int groupId = GetGroupIndexById(Id);
        if (groupId != -1)
        {
            if ((Groups[groupId].groupStress == -1 && stress == -1) || (Groups[groupId] == null))
            {
                return 0;
            }

            float Z;
            int status = Groups[groupId].membersId.IndexOf(mId);


            int gR;

            if (stress > 0)
            {
                gR = Groups[groupId].lessStressed;
                if (gR == -1)
                {
                    gR = Groups[groupId].notStressed;
                }
            }
            else
            {
                gR = Groups[groupId].mostStressed;
                if (gR == -1)
                {
                    gR = Groups[groupId].notStressed;
                }
            }

            if (gR == -1 || gR >= Groups[groupId].membersId.Count)
            {
                return 0; 
            }

            BelbinType belbinType = Groups[groupId].belbinTypes[status];
            var attributes = belbin.belbinTypeAttributes[belbinType];
            BelbinType enemyType = Groups[groupId].belbinTypes[gR];
            var enemyAttr = belbin.belbinTypeAttributes[enemyType];

            Z = (attributes.M * attributes.C * status - attributes.E * (11 - gR) * enemyAttr.C) * Math.Abs(Groups[groupId].groupStress - stress);

            if (attributes.line <= Z)
            {
                //   Debug.Log(mId + " (" + Z + ")  --->  " + " stay O O O    in" + Groups[groupId].Id);
            }
            else
            {
                //****Debug.Log(mId + " (" + Z + ")  -X->  " + " leave X X X    from" + Groups[groupId].Id);
                if (Groups[groupId] != null)
                {
                    DeleteMember(groupId, mId);
                }
            }
        }
        return stress;
    }

    //ПОЛУЧЕНИЕ ИНФОРМАЦИИ О ТИПЕ БЕЛБИНА
    public static void SetBelbin(int mId, BelbinType type, int groupId) {
        int id = Groups[groupId].membersId.IndexOf(mId);
        Groups[groupId].belbinTypes[id] = type;
        ShowGroupInfo();
    }

    //УДАЛИТЬ УЧАСТНИКА
    public void ButtonDelete()
    {
        int id = int.Parse(idMemField.text);
       DeleteMember(int.Parse(ChangeStress.StressGroups)-1,id);
    }

    // ПОИСК ИНДЕКСА ГРУППЫ по Id в списке Groups
    public static int GetGroupIndexById(int groupId)
    {
        int index = Groups.FindIndex(g => g.Id == groupId);
        if (index == -1)
        {
            return -1; 
        }
        return index; 
    }
    // ПОИСК ЦЕНТРА ГРУППЫ ЧЕРЕЗ ИНДЕКС ГРУППЫ
    public static Vector3 GetGroupPositionById(int groupId)
    {
        int index = Groups.FindIndex(g => g.Id == groupId);
        if (index == -1)
        {
            return new Vector3(0,0,0);

        }
        return Groups[index].pos; 
    }

}
