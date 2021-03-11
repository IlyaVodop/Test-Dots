using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public class CoreGamePlay : MonoBehaviour
{

    public static event Action<int> ScoreUpdated;

#pragma warning disable 649
    [SerializeField] private Material _lineMaterial;
#pragma warning restore 649


    private LineRenderer _line;


    private List<Vector3> _pointsList;
    private List<CircleInfo> _figuresInfoList;
    private List<CircleInfo> _usedCircles;

    private Vector3 _mousePos;
    private GameObject[] _figuresObjects;
    private LevelGenerator _levelGenerator;
    int _figuresCount;
    private int _score;
    private Color initialColor = Color.black;
    private bool _isMousePressed;



    private int unburnableIndex = 0;




    void Awake()
    {
        _line = gameObject.GetComponent<LineRenderer>();
        _pointsList = new List<Vector3>();
        _figuresObjects = new GameObject[100];
        _figuresInfoList = new List<CircleInfo>();
        _levelGenerator = new LevelGenerator();
        _figuresCount = 0;
        _usedCircles = new List<CircleInfo>();

        _figuresObjects = _levelGenerator.GenerateLevel(1);
        _figuresCount = _levelGenerator.FiguresCount;



        for (int i = 1; i <= _figuresCount; i++)
        {
            _figuresInfoList.Add(new CircleInfo());
            _figuresInfoList[i - 1] = new CircleInfo();
            _figuresInfoList[i - 1].Radius = _figuresObjects[i - 1].GetComponent<CircleCollider2D>().radius * _figuresObjects[i - 1].transform.localScale.x;
            _figuresInfoList[i - 1].X = _figuresObjects[i - 1].GetComponent<Renderer>().bounds.center.x + _figuresObjects[i - 1].GetComponent<CircleCollider2D>().offset.x * _figuresObjects[i - 1].transform.localScale.x;
            _figuresInfoList[i - 1].Y = _figuresObjects[i - 1].GetComponent<Renderer>().bounds.center.y + _figuresObjects[i - 1].GetComponent<CircleCollider2D>().offset.y * _figuresObjects[i - 1].transform.localScale.y;
            _figuresInfoList[i - 1].Color = _figuresObjects[i - 1].GetComponent<Renderer>().material.color = GetRandomColor();
        }


    }


    private void CheckAndAddScore()
    {
        _line.positionCount = 0;
        if (_usedCircles.Count > 1)
        {
            for (var index = 0; index < _usedCircles.Count; index++)
            {
                var circle = _usedCircles[index];
                circle.Color = _figuresObjects[_figuresInfoList.IndexOf(circle)].GetComponent<Renderer>().material.color = GetRandomColor();
            }

            _score += _usedCircles.Count * 10;
            ScoreUpdated?.Invoke(_score);
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



    private void Update()
    {
        GameControl();
    }

    private void GameControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isMousePressed = true;
            _pointsList.RemoveRange(0, _pointsList.Count);
        }


        if (Input.GetMouseButtonUp(0))
        {
            CheckAndAddScore();
            _isMousePressed = false;
            _pointsList.Clear();
            _usedCircles.Clear();
            _line.positionCount = 0;
            unburnableIndex = 0;
        }




        if (_isMousePressed)
        {

            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = 0;
            for (int i = 1; i <= _figuresCount; i++)
            {
                if (IsMouseOnCircle(i))
                {

                    var pos = new Vector3(_figuresInfoList[i - 1].X, _figuresInfoList[i - 1].Y, 0);


                    // Первое нажатие
                    if (!_pointsList.Contains(pos) && _pointsList.Count == 0)
                    {

                        _pointsList.Add(pos);
                        _usedCircles.Add(_figuresInfoList[i - 1]);
                        _line.startColor = _figuresInfoList[i - 1].Color;
                        _line.endColor = _figuresInfoList[i - 1].Color;
                        _line.positionCount = _pointsList.Count;
                        _line.SetPosition(_pointsList.Count - 1, (Vector3)_pointsList[_pointsList.Count - 1]);
                        initialColor = _figuresInfoList[i - 1].Color;
                        return;

                    }

                    // Находимся в круге, к которому можем привязаться
                    if (!_pointsList.Contains(pos) && _pointsList.Count > 0 && !_usedCircles.Contains(_figuresInfoList[i - 1]) && _figuresInfoList[i - 1].Color == initialColor)
                    {
                        if (IsStraightNeighbor(_figuresInfoList[i - 1], _levelGenerator.LevelProperties))
                        {
                            if (_pointsList.Count > unburnableIndex)
                            {
                                _pointsList.RemoveRange(unburnableIndex, _pointsList.Count - (unburnableIndex + 1));
                            }


                            _usedCircles.Add(_figuresInfoList[i - 1]);
                            unburnableIndex++;
                            _pointsList.Add(pos);
                            _line.positionCount = _pointsList.Count;

                            _line.SetPosition(_pointsList.Count - 1, (Vector3)_pointsList[_pointsList.Count - 1]);
                            return;
                        }
                    }


                    // Находимся в предыдущем круге
                    if (_usedCircles.Count > 1 && _figuresInfoList[i - 1] == _usedCircles[_usedCircles.Count - 2])
                    {


                        _usedCircles.Remove(_usedCircles.Last());
                        unburnableIndex--;
                        return;
                    }

                }


            }

            if (!_pointsList.Contains(_mousePos) && _pointsList.Count > 0 && !Input.GetMouseButtonDown(0))
            {
                // Debug.LogError("Находимся ВНЕ круга");
                if (_pointsList.Count > unburnableIndex)
                {
                    _pointsList.RemoveRange(unburnableIndex, _pointsList.Count - (unburnableIndex + 1));
                }
                _pointsList.Add(_mousePos);
                _line.positionCount = _pointsList.Count;
                _line.SetPosition(_pointsList.Count - 1, (Vector3)_pointsList[_pointsList.Count - 1]);

            }



        }

    }


    private bool IsStraightNeighbor(CircleInfo info, LevelProperties properties)
    {
        var lastSuccess = _usedCircles.Last();
        var i = _figuresInfoList.IndexOf(lastSuccess);

        if (_figuresInfoList.Count > i + properties.LengthHorizontal)
        {
            var topNeighbor = _figuresInfoList[i + properties.LengthHorizontal];
            if (info == topNeighbor)
            {

                return true;
            }
        }

        if (i - properties.LengthHorizontal >= 0)
        {
            var botNeighbor = _figuresInfoList[i - properties.LengthHorizontal];
            if (info == botNeighbor)
            {

                return true;
            }
        }

        if (_figuresInfoList.Count > i + 1)
        {
            var rightNeighbor = _figuresInfoList[i + 1];
            if (info == rightNeighbor)
            {

                return true;
            }
        }

        if (i - 1 >= 0)
        {
            var leftNeighbor = _figuresInfoList[i - 1];
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
        if ((_figuresInfoList[i - 1].X - _mousePos.x) *
            (_figuresInfoList[i - 1].X - _mousePos.x) +
            (_figuresInfoList[i - 1].Y - _mousePos.y) *
            (_figuresInfoList[i - 1].Y - _mousePos.y) <
            _figuresInfoList[i - 1].Radius * _figuresInfoList[i - 1].Radius)
        {
            return true;
        }

        return false;
    }
}