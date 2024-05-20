using System.Collections;
using UnityEngine;

public class StressZone : MonoBehaviour
{
    //�������� ���� �������
    public int stressType;
    public int stressDuraction;

    public string StressCity;
    public string StressSex;
    public int StressGroups;

    public TextMesh zoneText;
    public TextMesh timeText;
    public Material[] stressMat;


    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (((StressCity == "None") && (StressSex == "None") && (StressGroups == 0))|| stressType==0) {
            Destroy(gameObject);
        }
        else {
            if (stressDuraction > 0)
            {
                timeText.text = stressDuraction + "";
            }
            else {
                timeText.text = "x";
            }
            if (StressCity != "None")
            {
                zoneText.text = "City: " + StressCity + "\n";
            }
            else {
                zoneText.text = "";
            }
            if (StressSex != "None")
            {
                zoneText.text += "Sex: " + StressSex + "\n";
            }
            else
            {
                zoneText.text += "";
            }
            if (StressGroups != 0)
            {
                zoneText.text += "Group: " + StressGroups;
            }
        }
        CheckStressColor();
    }
    //����� ��������� ���� �������
    void CheckStressColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = stressMat[Mathf.Clamp(stressType, -1, 1) + 1];
    }
    //������ �� ����������� ����
    private IEnumerator StressCountdown()
    {
        while (stressDuraction > 0)
        {
            yield return new WaitForSeconds(1);
            stressDuraction--;
            timeText.text = stressDuraction + "";
        }

        Destroy(gameObject);
    }
    //����������� ����
    void OnMouseDown()
    {
        isDragging = true;
        screenPoint = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }
    void OnMouseDrag()
    {
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = mainCamera.ScreenToWorldPoint(cursorScreenPoint) + offset;
        transform.position = cursorPosition;
    }
    void OnMouseUp()
    {
        isDragging = false;
        if (stressDuraction > 0)
        {
            StartCoroutine(StressCountdown());
        }
    }
    //����������� ���� ������ ������� ����
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
        }
    }
}
