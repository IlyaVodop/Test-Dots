using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelPropeties", order = 1)]
public class LevelProperties : ScriptableObject
{
   // int length1, int length2, float xoffSet, float yoffSet, float offSet, int y1, int x1, GameObject prefab

   public GameObject Prefab;
   public int LengthVertical;
   public int LengthHorizontal;
   public float OffsetHorizontal;
   public float OffsetVertical;
   public float Multiplier;
   public float ZPosition;


}
