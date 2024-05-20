using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BelbinStatus : MonoBehaviour
{
    public enum BelbinType
    { 
        none,
        Strateg, 
        GeneratorIdey, 
        Hozyaistvennik,
        Ekspert,
        DushaKompanii,
        Kritik,
        Peregovorshik,
        Analitik,
        Organizator
    };

    public BelbinType type;

    /* int M - самомотивация
     int C - контроль эмоций
     int E - эмпатия
     float line - минимум для выхода из группы
     int leaderId - рейтинг лидерских способностей
    */
    public Dictionary<BelbinType, (int M, int C, int E, float line, int leaderId)> belbinTypeAttributes = new Dictionary<BelbinType, (int, int, int, float, int)>()
    {
        { BelbinType.GeneratorIdey, (9, 3, 8, -275.5f, 1) },
        { BelbinType.Peregovorshik, (13, 3, 9, -397.5f, 2) },
        { BelbinType.Hozyaistvennik, (10, 3, 9, -312f, 4) },
        { BelbinType.Kritik, (10, 6, 8, -94f, 3) },
        { BelbinType.Analitik, (11, 6, 10, -217f, 5) },
        { BelbinType.Ekspert, (12, 9, 10, -36f, 6) },
        { BelbinType.Organizator, (10, 4, 8, -204f, 7) },
        { BelbinType.Strateg, (12, 6, 10, -234f, 8) },
        { BelbinType.DushaKompanii, (10, 3, 6, -153f, 9) }
    };
}
