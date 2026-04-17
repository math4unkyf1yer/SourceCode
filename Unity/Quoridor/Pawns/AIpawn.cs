using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIpawn : MonoBehaviour
{
    public Vector2Int Coordinate;
    public Transform startPosition;

    private Board board;

    private void Start()
    {
        //    Coordinate = new Vector2Int(8,4);
        transform.position = startPosition.position;
        board = FindObjectOfType<Board>();
    }

    public void moveAIPawn(Vector2Int newPos, Transform parent)
    {
        board.nodes[Coordinate.x, Coordinate.y].pawnOnIt = false;
        Coordinate = newPos;
        board.nodes[Coordinate.x, Coordinate.y].pawnOnIt = true;
        transform.position = parent.position;
        if(board.nodes[Coordinate.x, Coordinate.y].itemOnIt)
        {
            board.nodes[Coordinate.x, Coordinate.y].itemOnIt = false;
            slotPos slotPosition = board.nodes[Coordinate.x, Coordinate.y].tileInNode.GetComponent<slotPos>();
            slotPosition.RemoveItemToSlot();
        }
        board.EnemyTurnEnd();
        if (Coordinate.x == 0)
        {
            board.Lose();
        }
    }
}
