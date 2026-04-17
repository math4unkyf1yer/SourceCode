using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pawnCoordinates : MonoBehaviour
{
    public Vector2Int Coordinate;
    public Transform startPosition;

    private Board board;

    //image 
    Image tileDownIm;
    Image tileUpIm;
    Image tileleftIm;
    Image tileRightIm;
    bool checkOnce;

    private void Start()
    {
    //    Coordinate = new Vector2Int(8,4);
        transform.position = startPosition.position;
        board = FindObjectOfType<Board>();
        ShowAdjecentTiles();
    }

    public void SetPlayer(Transform postion)
    {
        transform.position =postion.position;
    }
    public void movePawn(Vector2Int newPos,Transform parent,Node newnode)
    {
        if (board.player1Turn)
        {
            if (!isAdjecent(Coordinate, newPos))
            {
                Debug.Log("not adjacent");
                return;
            }

            if (!CanMove(Coordinate, newPos))
            {
                Debug.Log("blocked by wall");
                return;
            }

            // Move is valid
            HideAdjecentTiles();
            board.nodes[Coordinate.x, Coordinate.y].pawnOnIt = false;
            Coordinate = newPos;
            newnode.pawnOnIt = true;
            if (newnode.itemOnIt)
            {
                slotPos slotLocation = board.nodes[Coordinate.x, Coordinate.y].tileInNode.GetComponent<slotPos>();
                slotLocation.RemoveItemToSlot();
                newnode.itemOnIt = false;
            }
            transform.position = parent.position;
            board.PlayerTurnEnd();
            if(Coordinate.x == 0)
            {
                board.Win();
            }
        }
    }

    bool isAdjecent(Vector2Int currentPos, Vector2Int newPos)
    {
        //need to check exaclty where adjacent to see if there is a wall blocking the new tile
        int dx = Mathf.Abs(currentPos.x - newPos.x);
        int dy = Mathf.Abs(currentPos.y - newPos.y);

        // Adjacent means 1 step in any cardinal direction
        return (dx + dy == 1);
    }
    public void ShowAdjecentTiles()
    {
        Vector2Int left = Coordinate + Vector2Int.down;
        Vector2Int right = Coordinate + Vector2Int.up;
        Vector2Int up = Coordinate - Vector2Int.right;
        Vector2Int down = Coordinate - Vector2Int.left;
        //get the tiles from the nodes that are adjacent to the player 
        if (IsInsideBoard(left) && !board.nodes[left.x,left.y].wallRight)
        {
            tileDownIm = board.nodes[left.x, left.y].tileInNode.GetComponent<Image>();
            tileDownIm.color = Color.yellow;
        }
        if (IsInsideBoard(right) && !board.nodes[right.x, right.y].wallLeft)
        {
            tileUpIm = board.nodes[right.x, right.y].tileInNode.GetComponent<Image>();
            tileUpIm.color = Color.yellow;
        }
        if (IsInsideBoard(down) && !board.nodes[down.x, down.y].wallUp)
        {
            tileleftIm = board.nodes[down.x, down.y].tileInNode.GetComponent<Image>();
            tileleftIm.color = Color.yellow;
        }
        if(IsInsideBoard(up) && !board.nodes[up.x, up.y].wallDown)
        {
            tileRightIm = board.nodes[up.x, up.y].tileInNode.GetComponent<Image>();
            tileRightIm.color = Color.yellow;
        }
        //call on it in update when is your turn and when its false then after playing we hide them and not your turn
    }
    bool IsInsideBoard(Vector2Int p)
    {
        return p.x >= 0 && p.x <= 8 &&
               p.y >= 0 && p.y <= 8;
    }
    private void HideAdjecentTiles()
    {
        Vector2Int left = Coordinate + Vector2Int.down;
        Vector2Int right = Coordinate + Vector2Int.up;
        Vector2Int up = Coordinate - Vector2Int.right;
        Vector2Int down = Coordinate - Vector2Int.left;
        //get the tiles from the nodes that are adjacent to the player 
        if (IsInsideBoard(left))
        {
            tileDownIm = board.nodes[left.x, left.y].tileInNode.GetComponent<Image>();
            tileDownIm.color = Color.white;
        }
        if (IsInsideBoard(right))
        {
            tileUpIm = board.nodes[right.x, right.y].tileInNode.GetComponent<Image>();
            tileUpIm.color = Color.white;
        }
        if (IsInsideBoard(down))
        {
            tileleftIm = board.nodes[down.x, down.y].tileInNode.GetComponent<Image>();
            tileleftIm.color = Color.white;
        }
        if (IsInsideBoard(up))
        {
            tileRightIm = board.nodes[up.x, up.y].tileInNode.GetComponent<Image>();
            tileRightIm.color = Color.white;
        }
    }
    bool CanMove(Vector2Int from, Vector2Int to)
    {
        if (board == null) return false;

        Node current = board.nodes[from.x, from.y];
        Node target = board.nodes[to.x, to.y];

        Vector2Int dir = to - from;

        // Check walls on current node
        if (dir == Vector2Int.up && current.wallRight) return false;
        if (dir == Vector2Int.down && current.wallLeft) return false;
        if (dir == Vector2Int.left && current.wallUp) return false;
        if (dir == Vector2Int.right && current.wallDown) return false;

        // (Optional safety check: target node agrees)
        if (dir == Vector2Int.up && target.wallLeft) return false;
        if (dir == Vector2Int.down && target.wallRight) return false;
        if (dir == Vector2Int.left && target.wallDown) return false;
        if (dir == Vector2Int.right && target.wallUp) return false;

        if (target.pawnOnIt == true) return false;

        return true;
    }
}
