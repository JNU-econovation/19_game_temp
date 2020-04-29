﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
[System.Serializable]
public class Animal : MonoBehaviour
{
    Rigidbody2D animalrigidbody;
    public Vector2Int bottomLeft, topRight;
    Vector2Int targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;
    public float speed=2f;
    private Animator animator;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    void Start()
    {
        animalrigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        nodeSetting();
        transform.position = new Vector3Int(Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y), 0);
        while (NodeArray[(int)transform.position.x - bottomLeft.x, (int)transform.position.y - bottomLeft.y].isWall == true)
            transform.position = new Vector3Int(Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y), 0);
        for (int i = 0; i < 5; i++)
        {
            randomSetting();
            pathFinding();
            roitering();
        }
    }

    void randomSetting() 
    {
        targetPos = new Vector2Int((int)Random.Range(bottomLeft.x, topRight.x), (int)Random.Range(bottomLeft.y, topRight.y));
        while ((NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y].isWall == true))
            targetPos = new Vector2Int((int)Random.Range(bottomLeft.x, topRight.x), (int)Random.Range(bottomLeft.y, topRight.y));
    }

    private void OnMouseDown()
    {
        //반응
        animator.SetBool("tapAnimal", true);
    }

    private void OnMouseUp()
    {
        //돌아옴
        animator.SetBool("tapAnimal", false);
    }

    private void OnMouseExit()
    {
        animator.SetBool("tapAnimal", false);
    }


    private void roitering()
    {
            for (int i = 0; i < FinalNodeList.Count-1; i++)
            {
                animalrigidbody.velocity = new Vector2((FinalNodeList[i].x- FinalNodeList[i + 1].x) * speed, (FinalNodeList[i].y - FinalNodeList[i+1].y) * speed);
            if ((transform.position.x == FinalNodeList[i + 1].x) && (transform.position.y == FinalNodeList[i + 1].y))
                continue;
            else if ((animalrigidbody.velocity.x == 0) && (animalrigidbody.velocity.y < 0))
                animator.SetInteger("rotate", 0);
            else if ((animalrigidbody.velocity.x < 0) && (animalrigidbody.velocity.y == 0))
                animator.SetInteger("rotate", 1);
            else if ((animalrigidbody.velocity.x == 0) && (animalrigidbody.velocity.y > 0))
                animator.SetInteger("rotate", 2);
            else if ((animalrigidbody.velocity.x > 0) && (animalrigidbody.velocity.y == 0))
                animator.SetInteger("rotate", 3);
            }
    }
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}


    public void nodeSetting()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }
    }

    void pathFinding() { 
        StartNode = NodeArray[(int)(transform.position.x) - bottomLeft.x, (int)(transform.position.y) - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i3 = 1; i3 < OpenList.Count; i3++)
                if (OpenList[i3].F <= CurNode.F && OpenList[i3].H < CurNode.H) CurNode = OpenList[i3];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 최종 노드리스트 설정
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int ii = 0; ii < FinalNodeList.Count; ii++) print(ii + "번째는 " + FinalNodeList[ii].x + ", " + FinalNodeList[ii].y);
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    /*
    void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0)
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
        }
    }
    */
}


