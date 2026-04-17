using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlaceWall;


public class Node
{
    public Vector2Int gridPos;
    public bool wallUp;
    public bool wallDown;
    public bool wallLeft;
    public bool wallRight;
    public bool pawnOnIt;
    public bool itemOnIt;

    //for AI maybe player later 
    public GameObject tileInNode;

    public Node(Vector2Int pos)
    {
        gridPos = pos;
    }

    public Node Clone()
    {
        return new Node(this.gridPos) // copy grid position
        {
            wallUp = this.wallUp,
            wallDown = this.wallDown,
            wallLeft = this.wallLeft,
            wallRight = this.wallRight,
            tileInNode = this.tileInNode // optional: shallow copy of GameObject reference
        };
    }
}
public class Board : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;
    public Node[,] nodes = new Node[9, 9];

    //walls place 
    private int playerWallOnBoardNb = 9;
    private int player2WallOnBoardNb = 10;
    public TextMeshProUGUI wallTextNb;
    public TextMeshProUGUI wallAITextNb;
    [SerializeField] private GameObject[] placeWallObg;
    [SerializeField] private PlaceWall[] placeWallScript;
    PlaceWall correctWall;
    PlaceWall foundwall;
    Vector2Int nextStep;

    //pawns
    public GameObject pawn1;
    public GameObject pawn2;

    private AIpawn aiPawnScript;
    private pawnCoordinates pawnScript;

    //turn base
    public bool player1Turn = true;
    //number of turn its been
    private int turnNb = 0;

    // Start is called before the first frame update
    void Start()
    {
        placeWallObg = GameObject.FindGameObjectsWithTag("Wall");

        placeWallScript = new PlaceWall[placeWallObg.Length];

        for (int i = 0; i < placeWallObg.Length; i++)
        {
            placeWallScript[i] = placeWallObg[i].GetComponent<PlaceWall>();
        }

        pawnScript = pawn1.GetComponent<pawnCoordinates>();
        aiPawnScript = pawn2.GetComponent<AIpawn>();
        //make the board - need to instantiate andplace them on the board
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                GameObject tile = Instantiate(tilePrefab, gridParent);
                tile.transform.position = new Vector3(x, y);
                nodes[x, y] = new Node(new Vector2Int(x, y));
                nodes[x,y].tileInNode = tile;
                slotPos slotScript = tile.GetComponent<slotPos>();
                if (slotScript != null) {
                    slotScript.PlaceNode(nodes[x,y]);
                }
               // Debug.Log(nodes[x,y].gridPos);
            }
        }
    }

    public bool CanPlaceWalls()
    {
        if(playerWallOnBoardNb > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void PlayerPlaceWall()
    {
        playerWallOnBoardNb--;
        //change text
        wallTextNb.text = "X" + playerWallOnBoardNb.ToString();
       PlayerTurnEnd();
    }

    public void PlayerTurnEnd()
    {
        player1Turn = false;
        turnNb++;
        if(turnNb == 3) { spawnItems(); turnNb = 0; }
        EnemyTurn();
    }

    //For now Enemy Turn
    void EnemyTurn()
    {
        List<Vector2Int> pathAI = FindShortestPath(aiPawnScript.Coordinate, 8);

        List<Vector2Int> pathPlayer = FindShortestPath(pawnScript.Coordinate,0);


        //getting the first tile for the most optimal path
        if (pathAI != null && pathAI.Count >= 1)
        {
            if (player2WallOnBoardNb > 0)
            {
                      
                nextStep = pathAI[1];

                if (CheckIfPlayerBlocking(nextStep))
                {
                    //can't move
                    EnemyPlaceWallAdvance();
                    return;
                }
                //finding if we should place wall or not 
                int TileAwayEnemy = pathAI.Count;
                int TileAwayPlayer = pathPlayer.Count;
                int difference = TileAwayEnemy - TileAwayPlayer;
                if (difference > 1)
                {
                    //option 2 place wall
                    EnemyPlaceWallAdvance();
                    // EnemyPlaceWall(plMostLikelyNextStep,nextStep);
                }
                else if (difference < 0)
                {
                    //option 1 move advantage over player
                    EnemyMove(nextStep);
                }
                else
                {
                    if (Random.value < 0.5f)
                    {
                        //option1
                        EnemyMove(nextStep);
                    }
                    else
                    {
                        EnemyPlaceWallAdvance();
                    }
                }
            }
            else
            {
                //move not at the next step but at the open lane if player is blocking
                EnemyNoMoreWallMove(nextStep);
            }
        }
    }

    void EnemyNoMoreWallMove(Vector2Int nextStep)
    {
        if (CheckIfPlayerBlocking(nextStep))
        {
            Vector2Int startPos = aiPawnScript.Coordinate;
            Node node = nodes[startPos.x,startPos.y];
            Vector2Int positionCheck;
            positionCheck = FindSpotToMove(startPos);
            //move to any spot open
            EnemyMove(positionCheck);
        }
        else
        {
            EnemyMove(nextStep);
        }
    }
    Vector2Int FindSpotToMove(Vector2Int from)
    {
        Node node = nodes[from.x, from.y];

        // All 4 possible directions
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(-1, 0), // Up
        new Vector2Int(1, 0),  // Down
        new Vector2Int(0, -1), // Left
        new Vector2Int(0, 1)   // Right
        };

        foreach (Vector2Int dir in directions)
        {
            int newX = from.x + dir.x;
            int newY = from.y + dir.y;

            // 2 Check wall
            bool allowed = false;

            if (dir.x == -1 && !node.wallUp) allowed = true;
            if (dir.x == 1 && !node.wallDown) allowed = true;
            if (dir.y == -1 && !node.wallLeft) allowed = true;
            if (dir.y == 1 && !node.wallRight) allowed = true;

            if (!allowed)
                continue;

            // 3️⃣ Check pawn blocking
            if (nodes[newX, newY].pawnOnIt)
                continue;

            //  First valid move found
            return new Vector2Int(newX, newY);
        }

        // No valid move
        return from;
    }
    bool CheckIfPlayerBlocking(Vector2Int nextStep)
    {
        if (nodes[nextStep.x, nextStep.y].pawnOnIt)
        {
            return true;
        }
        return false;
    }

    void EnemyMove(Vector2Int nextStep)
    {
        aiPawnScript.moveAIPawn(nextStep, nodes[nextStep.x, nextStep.y].tileInNode.transform);
    }

    //place wall(might need to check this)
    void EnemyPlaceWallAdvance()
    {
            int longestPathWallPlacement = 0;
            foundwall = null;

            foreach (PlaceWall walls in placeWallScript)
            {

                if (!walls.canPlaceWall(walls.anchorGridPos,walls.orientation)) 
                    continue;

                walls.ApplyWallToNodes(walls.anchorGridPos, walls.orientation);

                List<Vector2Int> playerPath = FindShortestPath(pawnScript.Coordinate, 0);
                List<Vector2Int> aiPath = FindShortestPath(aiPawnScript.Coordinate, 1);

                if (playerPath == null || aiPath == null)
                {
                    walls.RemoveWallNodes( walls.anchorGridPos, walls.orientation);
                    continue;
                }

                int score = playerPath.Count - aiPath.Count;

                if(score > longestPathWallPlacement)
                {
                    longestPathWallPlacement = score;
                    foundwall = walls;
                }
                walls.RemoveWallNodes( walls.anchorGridPos, walls.orientation);
            }
            if(foundwall == null)
            {
                EnemyMove(nextStep);
                return;
            }
            player2WallOnBoardNb--;
            wallAITextNb.text = "X" + player2WallOnBoardNb.ToString();
            foundwall.EnemyPlaceWall();
            EnemyTurnEnd();
        
    }
    //checking before placing because don't want wall to overlaped
    public void EnemyTurnEnd()
    {
        player1Turn = true;
        turnNb++;
        if (turnNb == 3) { spawnItems(); turnNb = 0; }
        pawnScript.ShowAdjecentTiles();
    }

    //for enemy AI Decision -- give a path
    public List<Vector2Int> FindShortestPath(Vector2Int startPos, int goalX)
    {

        bool[,] visited = new bool[9, 9];
        Vector2Int[,] cameFrom = new Vector2Int[9, 9];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;
        cameFrom[startPos.x, startPos.y] = startPos;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // GOAL FOUND
            if (current.x == goalX)
                return ReconstructPath(cameFrom, startPos, current);

            Node node = nodes[current.x, current.y];

            TryMove(current, current.x-1, current.y, !node.wallUp);
            TryMove(current, current.x +1, current.y, !node.wallDown);
            TryMove(current, current.x, current.y-1, !node.wallLeft);
            TryMove(current, current.x, current.y+1, !node.wallRight);
        }

        //  No path
        return null;

        void TryMove(Vector2Int from, int x, int y, bool allowed)
        {
            if (!allowed) return;
            if (x < 0 || x > 8 || y < 0 || y > 8) return;
            if (visited[x, y]) return;

            Node targetNode = nodes[x, y];

            visited[x, y] = true;
            cameFrom[x, y] = from;
            queue.Enqueue(new Vector2Int(x, y));
        }
    }

    List<Vector2Int> ReconstructPath(Vector2Int[,] cameFrom,Vector2Int start,Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current.x, current.y];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }


    public void Win()
    {
        //for now once you reach the last area you win.
        Debug.Log("Win");
        SceneManager.LoadScene("Win");
    }
    public void Lose()
    {
        Debug.Log("Lose");
    }

    void spawnItems()
    {
        // Get random number
        int r = Random.Range(0, 9);
        int r2 = Random.Range(0, 9);

        slotPos slotScript = nodes[r, r2].tileInNode.GetComponent<slotPos>();
        nodes[r, r2].itemOnIt = true;
        slotScript.AddItemToSlot();
    }

}
