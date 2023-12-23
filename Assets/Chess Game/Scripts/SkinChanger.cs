using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    public static SkinChanger Instance;

    [SerializeField]
    private Material []Board1Materials1,Board1Materials2;

    [SerializeField]
    private Material Board2Materials1,Board4Materials1;

    [SerializeField]
    private Material[] Board3Materials1, Board3Materials2;

    [SerializeField]
    private GameObject[] Board;

    private void Awake()
    {
        Instance = this;
    }

    public Material GetMaterial1(int id)
    {
        if (id >= 0 && id <= 10)
        {
            return Board1Materials1[id-1];
        }
        else if (id >= 11 && id <= 20)
        {
            return Board1Materials1[0];
        }
        else if (id >= 21 && id <= 30)
        {
            return Board3Materials1[id-21];
        }
        else if(id==31)
        {
            return Board1Materials1[0];
        }

        return Board1Materials1[0];
    }

    public Material GetMaterial2(int id)
    {
        if (id >= 0 && id <= 10)
        {
            return Board1Materials2[id-1];
        }
        else if (id >= 11 && id <= 20)
        {
            return Board2Materials1;
        }
        else if (id >= 21 && id <= 30)
        {
            return Board3Materials2[id-21];
        }
        else if(id==31)
        {
            return Board4Materials1;
        }

        return Board1Materials2[0];
    }

    public void SetBoard(int id)
    {
        if(id>=0 && id<=10)
        {
            Material[] materials = Board[0].GetComponent<MeshRenderer>().materials;

            materials[0] = Board1Materials1[id-1];
            materials[1] = Board1Materials2[id-1];

            Board[0].GetComponent<MeshRenderer>().materials = materials;
            Board[0].SetActive(true);
        }
        else if(id>=11 && id<=20)
        {
            Board[1].SetActive(true);
        }
        else if(id>=21 && id<=30)
        {
            Material[] materials = Board[2].GetComponent<MeshRenderer>().materials;

            materials[1] = Board3Materials1[id - 21];
            materials[2] = Board3Materials2[id - 21];

            Board[2].GetComponent<MeshRenderer>().materials = materials;
            Board[2].SetActive(true);
        }
        else if(id==31)
        {
            Board[3].SetActive(true);
        }
    }

}
