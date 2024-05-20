using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogManager : MonoBehaviour
{
    //яхярелю бшбндю яннаыемхи
    public Text gameLogs;
    private Queue<string> logMessages = new Queue<string>();
    private int maxLogCount = 8;

    public void LogStressNR(string message)
    {
        AppendLog(message, Color.red);
    }

    public void LogStressPG(string message)
    {
        AppendLog(message, Color.green);
    }
    public void LogStressNone(string message)
    {
        AppendLog(message, Color.white);
    }
    public void LogGroup(string message)
    {
        AppendLog(message, Color.white);
    }

    private void AppendLog(string message, Color color)
    {
        string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n";

        logMessages.Enqueue(coloredMessage);
        if (logMessages.Count > maxLogCount)
        {
            logMessages.Dequeue();
        }
        gameLogs.text = string.Join("", logMessages.ToArray());
    }
}
