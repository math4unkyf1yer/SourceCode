using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public static bool HasPathToGoal(Board board,Vector2Int start,int goalX)
    {
        bool[,] visited = new bool[9, 9];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        /* while (queue.Count > 0)
         {
             Vector2Int current = queue.Dequeue();

             // Reached goal row
             if (current.x == goalX)
                 return true;

             Node node = board.nodes[current.x, current.y];

             TryVisit(board, node, current, Vector2Int.right, !node.wallUp, visited, queue);
             TryVisit(board, node, current, Vector2Int.left, !node.wallDown, visited, queue);
             TryVisit(board, node, current, Vector2Int.down, !node.wallLeft, visited, queue);
             TryVisit(board, node, current, Vector2Int.up, !node.wallRight, visited, queue);
         }*/
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // ✅ GOAL CHECK (logical vertical)
            if (current.x == goalX)
                return true;

            Node node = board.nodes[current.x, current.y];

            // UP (visual)
            if (!node.wallUp)
                TryEnqueue(current.x - 1, current.y);

            // DOWN (visual)
            if (!node.wallDown)
                TryEnqueue(current.x + 1, current.y );

            // LEFT (visual)
            if (!node.wallLeft)
                TryEnqueue(current.x, current.y -1);

            // RIGHT (visual)
            if (!node.wallRight)
                TryEnqueue(current.x, current.y+1);
        }

        return false;

        void TryEnqueue(int x, int y)
        {
            if (x < 0 || x > 8 || y < 0 || y > 8)
                return;

            if (visited[x, y])
                return;

            visited[x, y] = true;
            queue.Enqueue(new Vector2Int(x, y));
        }
    }

    static void TryVisit( Board board,Node node, Vector2Int current,Vector2Int dir,bool canMove, bool[,] visited,Queue<Vector2Int> queue)
    {
        if (!canMove) return;

        Vector2Int next = current + dir;

        if (next.x < 0 || next.y < 0 || next.x >= 9 || next.y >= 9)
            return;

        if (visited[next.x, next.y])
            return;

        visited[next.x, next.y] = true;
        queue.Enqueue(next);
    }

    //test function

}
