using UnityEngine;
using System.Collections;

public class levelScript : MonoBehaviour
{

    // Use this for initialization
    GameObject[] circleClone = new GameObject[100];
    int dots = 0;
    GameObject c;
    public GameObject[] instantiate()
    {
        var prefab = (GameObject)Resources.Load("circle 1");
        GenerateCircles(6, 6, 2.3f, 2.7f, 1f, 0, 0, prefab);
        return circleClone;
    }

    private void GenerateCircles(int length1, int length2, float xoffSet, float yoffSet, float offSet, int y1, int x1, GameObject c)
    {
        for (int y = y1; y < length1 - Mathf.Abs(y1); y++)
        {
            for (int x = x1; x < length2 - Mathf.Abs(x1); x++)
            {
                circleClone[dots++] = Instantiate(c, new Vector3(1.5f * (x - xoffSet) * offSet, 1.5f * (y - yoffSet) * offSet, 0.5f), Quaternion.identity) as GameObject;

            }
        }
    }





    public int arrayLength()
    {
        return dots;
    }

}
