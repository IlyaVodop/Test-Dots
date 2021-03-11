using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    public int FiguresCount { get; private set; }
    public LevelProperties LevelProperties { get; private set; }

    private GameObject[] _circleObjects = new GameObject[100];

    public GameObject[] GenerateLevel(int levelNumber)
    {
        string configName = "Level_" + levelNumber;
        LevelProperties = (LevelProperties)Resources.Load(configName);

        InstantiateFigures(LevelProperties);
        return _circleObjects;
    }

    private void InstantiateFigures(LevelProperties properties)
    {
        for (int y = 0; y < properties.LengthVertical; y++)
        {
            for (int x = 0; x < properties.LengthHorizontal; x++)
            {
                _circleObjects[FiguresCount++] = Instantiate(properties.Prefab,
                    new Vector3(properties.Multiplier * (x - properties.OffsetHorizontal),
                        properties.Multiplier * (y - properties.OffsetVertical), properties.ZPosition), Quaternion.identity);
            }
        }
    }


}
