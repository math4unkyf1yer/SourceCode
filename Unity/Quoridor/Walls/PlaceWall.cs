
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceWall : MonoBehaviour
{
    public GameObject wall;
    public Transform parent;
    private RowManager rowManagerScript;
    public int index;

    public WallOrientation orientation;
    public Vector2Int anchorGridPos; // set when clicking tile
    private Board board;
    private PathChecker pathCheckerScript;
    private pawnCoordinates pawnScript1;
    private AIpawn pawn2;

    private WallMenu wallUIScript;
    //the nodes it will affect


    private void Start()
    {
        rowManagerScript = GetComponentInParent<RowManager>();
        board = FindObjectOfType<Board>();
        pathCheckerScript = FindAnyObjectByType<PathChecker>();
        pawnScript1 = GameObject.Find("PlayerPawn").GetComponent<pawnCoordinates>();
        pawn2 = GameObject.Find("EnemyPawn").GetComponent<AIpawn>();

        wallUIScript = GameObject.Find("SideWallsHolder").GetComponent<WallMenu>();

    }
    public void ClickPlaceWall()
    {
        Debug.Log("Click");
        if(board.CanPlaceWalls() && board.player1Turn)
        {
            if (canPlaceWall(anchorGridPos, orientation))
            {
                ApplyWallToNodes(anchorGridPos, orientation);
                wall.SetActive(true);//need to instanstiate at the same button position and not as a parent
                rowManagerScript.HideAtIndex(index);

                board.PlayerPlaceWall();

                if(orientation == WallOrientation.Horizontal)
                {
                    //making it look better
                    wallUIScript.MenuOpenOrClose();
                }
                else
                {
                    wallUIScript.MenuDownOpen();
                }
            }
        }
    }

    public bool EnemyPlaceWall()
    {
        
            if (orientation == WallOrientation.Horizontal)
            {
                wallUIScript.MenuOpenOrClose();
            }
            else
            {
                wallUIScript.MenuDownOpen();
            }
            ApplyWallToNodes(anchorGridPos, orientation);
            wall.SetActive(true);
            rowManagerScript.HideAtIndex(index);
            if (orientation == WallOrientation.Horizontal)
            {
                wallUIScript.MenuOpenOrClose();
            }
            else
            {
                wallUIScript.MenuDownOpen();
            }
            return true;
    }

    public enum WallOrientation
    {
        Horizontal,
        Vertical
    }
    public void ApplyWallToNodes( Vector2Int pos, WallOrientation orientation)
    {
        if (board == null) return;

        Node[,] nodes = board.nodes;

        int x = pos.x;
        int y = pos.y;

        // Safety check
        if (x < 0 || y < 0 || x >= 8 || y >= 8) return;

        if (orientation == WallOrientation.Horizontal)
        {
            // Blocks UP/DOWN movement
            nodes[x, y].wallDown = true;
            nodes[x, y + 1].wallDown = true;

            nodes[x + 1, y].wallUp = true;
            nodes[x + 1, y + 1].wallUp = true;
        }
        else // Vertical
        {
            // Blocks LEFT/RIGHT movement
            nodes[x, y].wallRight = true;
            nodes[x + 1, y].wallRight = true;

            nodes[x, y + 1].wallLeft = true;
            nodes[x + 1, y + 1].wallLeft = true;
        }
    }


    public void RemoveWallNodes( Vector2Int pos, WallOrientation orientation)
    {
        int x = pos.x;
        int y = pos.y;

        if (orientation == WallOrientation.Horizontal)
        {
            board.nodes[x, y].wallDown = false;
            board.nodes[x, y + 1].wallDown = false;

            board.nodes[x + 1, y].wallUp = false;
            board.nodes[x + 1, y + 1].wallUp = false;
        }
        else
        {
            board.nodes[x, y].wallRight = false;
            board.nodes[x + 1, y].wallRight = false;

            board.nodes[x, y + 1].wallLeft = false;
            board.nodes[x + 1, y + 1].wallLeft = false;
        }
    }

    public bool canPlaceWall(Vector2Int wallPos, WallOrientation orientation)
    {
        if (!IsWallPlacementClear())
            return false;
        // 1. Apply wall TEMPORARILY
        ApplyWallToNodes(wallPos, orientation);

        // 2. Check paths
        bool pawn1HasPath = PathChecker.HasPathToGoal(
            board,
            pawnScript1.Coordinate,
            0 // top row
        );

        bool pawn2HasPath = PathChecker.HasPathToGoal(
            board,
            pawn2.Coordinate,
            8 // bottom row
        );

        // 3. Undo wall
        RemoveWallNodes(wallPos, orientation);

        // 4. Valid only if BOTH have paths
        return pawn1HasPath && pawn2HasPath;
    }
    bool IsWallPlacementClear()
    {
        Node[,] nodes = board.nodes;
        int x = anchorGridPos.x;
        int y = anchorGridPos.y;

        if (orientation == WallOrientation.Horizontal)
        {
            if (nodes[x, y].wallDown == true || nodes[x, y + 1].wallDown == true || nodes[x + 1, y].wallUp == true || nodes[x + 1, y + 1].wallUp == true)
            {
                return false;
            }
        }
        else
        {
            if (nodes[x, y].wallRight == true || nodes[x + 1, y].wallRight == true || nodes[x, y + 1].wallLeft == true || nodes[x + 1, y + 1].wallLeft == true)
            {
                return false;
            }
        }
        return true;
    }
}

