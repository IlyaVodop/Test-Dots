using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public class lineDraw : MonoBehaviour
{

    public static event Action<int> ScoreUpdated;

    [SerializeField] private Material _lineMaterial;


    private LineRenderer line;
    private bool isMousePressed;
    private List<Vector3> pointsList;
    private Vector3 mousePos;
    private List<CircleInfo> circleList;
    private GameObject[] circleClone;
    private levelScript ls;
    private bool isGamePlayeble = true;
    int dots;
    private int score;



    private List<CircleInfo> usedCircles = new List<CircleInfo>();

    private int unburnableIndex = 0;


    class CircleInfo
    {
        public float x;
        public float y;
        public float rad;
        public Color color;

    };
    void Awake()
    {
        // Create line renderer component and set its property

        line = gameObject.AddComponent<LineRenderer>();
        line.material = _lineMaterial;
        line.positionCount = 0;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.useWorldSpace = true;
        line.sortingOrder = 1;
        isMousePressed = false;
        pointsList = new List<Vector3>();
        circleClone = new GameObject[100];
        circleList = new List<CircleInfo>(100);
        ls = new levelScript();
        dots = 0;


        circleClone = ls.instantiate();
        dots = ls.arrayLength();



        for (int i = 1; i <= dots; i++)
        {
            circleList.Add(new CircleInfo());
            circleList[i - 1] = new CircleInfo();
            circleList[i - 1].rad = circleClone[i - 1].GetComponent<CircleCollider2D>().radius * circleClone[i - 1].transform.localScale.x;
            circleList[i - 1].x = circleClone[i - 1].GetComponent<Renderer>().bounds.center.x + circleClone[i - 1].GetComponent<CircleCollider2D>().offset.x * circleClone[i - 1].transform.localScale.x;
            circleList[i - 1].y = circleClone[i - 1].GetComponent<Renderer>().bounds.center.y + circleClone[i - 1].GetComponent<CircleCollider2D>().offset.y * circleClone[i - 1].transform.localScale.y;
            circleList[i - 1].color = circleClone[i - 1].GetComponent<Renderer>().material.color = GetRandomColor();
        }


    }


    void AddScore()
    {
        line.positionCount = 0;
        if (usedCircles.Count > 1)
        {
            for (var index = 0; index < usedCircles.Count; index++)
            {
                var circle = usedCircles[index];
                circle.color = circleClone[circleList.IndexOf(circle)].GetComponent<Renderer>().material.color = GetRandomColor();
            }

            score += usedCircles.Count * 10;
            ScoreUpdated?.Invoke(score);
        }
    }


    private Color GetRandomColor()
    {
        Color[] colors = new Color[]
        {
            Color.blue, Color.red, Color.green, Color.yellow, Color.magenta, Color.black
        };
        return colors[Random.Range(0, colors.Length - 1)];
    }



    void Update()
    {
        if (isGamePlayeble == true)
            GameControl();
    }

    private Color initialColor = Color.black;
    void GameControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            pointsList.RemoveRange(0, pointsList.Count);
        }


        if (Input.GetMouseButtonUp(0))
        {
            AddScore();
            isMousePressed = false;
            pointsList.Clear();
            usedCircles.Clear();
            line.positionCount = 0;
            unburnableIndex = 0;
        }




        // Drawing line when mouse is moving(presses)
        if (isMousePressed)
        {

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            for (int i = 1; i <= dots; i++)
            {
                if (IsMouseOnCircle(i))
                {

                    var pos = new Vector3(circleList[i - 1].x, circleList[i - 1].y, 0);


                    // Первое нажатие
                    if (!pointsList.Contains(pos) && pointsList.Count == 0)
                    {
                    
                        pointsList.Add(pos);
                        usedCircles.Add(circleList[i - 1]);
                        line.startColor = circleList[i - 1].color;
                        line.endColor = circleList[i - 1].color;
                        line.positionCount = pointsList.Count;
                        line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                        initialColor = circleList[i - 1].color;
                        return;

                    }

                    // Находимся в круге, к которому можем привязаться
                    if (!pointsList.Contains(pos) && pointsList.Count > 0 && !usedCircles.Contains(circleList[i - 1]) && circleList[i - 1].color == initialColor)
                    {
                        if (IsStraightNeighbor(circleList[i - 1]))
                        {
                            if (pointsList.Count > unburnableIndex)
                            {
                                pointsList.RemoveRange(unburnableIndex, pointsList.Count - (unburnableIndex + 1));
                            }
                         

                            usedCircles.Add(circleList[i - 1]);
                            unburnableIndex++;
                            pointsList.Add(pos);
                            line.positionCount = pointsList.Count;

                            line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                            return;
                        }
                    }


                    // Находимся в предыдущем круге
                    if (usedCircles.Count > 1 && circleList[i - 1] == usedCircles[usedCircles.Count - 2])
                    {
                      

                        usedCircles.Remove(usedCircles.Last());
                        unburnableIndex--;
                        return;
                    }

                }


            }

            if (!pointsList.Contains(mousePos) && pointsList.Count > 0 && !Input.GetMouseButtonDown(0))
            {
                // Debug.LogError("Находимся ВНЕ круга");
                if (pointsList.Count > unburnableIndex)
                {
                    pointsList.RemoveRange(unburnableIndex, pointsList.Count - (unburnableIndex + 1));
                }
                pointsList.Add(mousePos);
                line.positionCount = pointsList.Count;
                line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);

            }



        }

    }


    private bool IsStraightNeighbor(CircleInfo info)
    {
        var lastSuccess = usedCircles.Last();
        var i = circleList.IndexOf(lastSuccess);

        if (circleList.Count > i + 6)
        {
            var topNeighbor = circleList[i + 6];
            if (info == topNeighbor)
            {
              
                return true;
            }
        }

        if (i - 6 >= 0)
        {
            var botNeighbor = circleList[i - 6];
            if (info == botNeighbor)
            {
               
                return true;
            }
        }

        if (circleList.Count > i + 1)
        {
            var rightNeighbor = circleList[i + 1];
            if (info == rightNeighbor)
            {
               
                return true;
            }
        }

        if (i - 1 >= 0)
        {
            var leftNeighbor = circleList[i - 1];
            if (info == leftNeighbor)
            {
               ;
                return true;
            }
        }

        return false;
    }


    private bool IsMouseOnCircle(int i)
    {
        if ((circleList[i - 1].x - mousePos.x) * (circleList[i - 1].x - mousePos.x) + (circleList[i - 1].y - mousePos.y) * (circleList[i - 1].y - mousePos.y) < circleList[i - 1].rad * circleList[i - 1].rad)
            return true;
        else
            return false;

    }



}