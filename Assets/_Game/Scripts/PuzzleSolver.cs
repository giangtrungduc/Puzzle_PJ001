using System.Collections.Generic;
using UnityEngine;

public struct BoardState
{
    public Vector2Int[] positions;

    public BoardState(Vector2Int[] pos)
    {
        positions = pos;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is BoardState other)) return false;
        if (positions.Length != other.positions.Length) return false;
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] != other.positions[i]) return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        for (int i = 0; i < positions.Length; i++)
        {
            hash = hash * 31 + positions[i].GetHashCode();
        }
        return hash;
    }
}

public class SolverStep
{
    public int blockIndex;
    public Vector2Int targetGridPos;
    public BoardState state;
    public SolverStep previousStep;

    public SolverStep(int blockIndex, Vector2Int targetGridPos, BoardState state, SolverStep previousStep)
    {
        this.blockIndex = blockIndex;
        this.targetGridPos = targetGridPos;
        this.state = state;
        this.previousStep = previousStep;
    }
}

public static class PuzzleSolver
{
    public static SolverStep Solve(List<BlockEntity> blocks, GridManager grid, Vector2Int exitPos)
    {
        int count = blocks.Count;
        Vector2Int[] startPositions = new Vector2Int[count];
        for (int i = 0; i < count; i++)
        {
            startPositions[i] = blocks[i].gridPos;
        }

        BoardState rootState = new BoardState(startPositions);
        Queue<SolverStep> queue = new Queue<SolverStep>();
        HashSet<BoardState> visited = new HashSet<BoardState>();

        queue.Enqueue(new SolverStep(-1, Vector2Int.zero, rootState, null));
        visited.Add(rootState);

        while (queue.Count > 0)
        {
            SolverStep current = queue.Dequeue();

            int targetIndex = -1;
            for (int i = 0; i < count; i++)
            {
                if (blocks[i].isTargetBlock)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex != -1)
            {
                Vector2Int targetPos = current.state.positions[targetIndex];
                if (targetPos.x + blocks[targetIndex].length - 1 == exitPos.x && targetPos.y == exitPos.y)
                {
                    return GetFirstStep(current);
                }
            }

            bool[,] map = BuildOccupiedMap(current.state, blocks, 6, 6);

            for (int i = 0; i < count; i++)
            {
                BlockEntity b = blocks[i];
                Vector2Int currPos = current.state.positions[i];

                ClearBlockOnMap(map, currPos, b);

                List<Vector2Int> validMoves = GetValidMovesForBlock(map, currPos, b, 6, 6);
                foreach (Vector2Int nextPos in validMoves)
                {
                    Vector2Int[] nextPositions = (Vector2Int[])current.state.positions.Clone();
                    nextPositions[i] = nextPos;
                    BoardState nextState = new BoardState(nextPositions);

                    if (!visited.Contains(nextState))
                    {
                        visited.Add(nextState);
                        queue.Enqueue(new SolverStep(i, nextPos, nextState, current));
                    }
                }

                SetBlockOnMap(map, currPos, b, true);
            }
        }

        return null;
    }

    private static SolverStep GetFirstStep(SolverStep winStep)
    {
        SolverStep curr = winStep;
        while (curr.previousStep != null && curr.previousStep.previousStep != null)
        {
            curr = curr.previousStep;
        }
        return curr;
    }

    private static bool[,] BuildOccupiedMap(BoardState state, List<BlockEntity> blocks, int w, int h)
    {
        bool[,] map = new bool[w, h];
        for (int i = 0; i < blocks.Count; i++)
        {
            SetBlockOnMap(map, state.positions[i], blocks[i], true);
        }
        return map;
    }

    private static void SetBlockOnMap(bool[,] map, Vector2Int pos, BlockEntity b, bool val)
    {
        for (int l = 0; l < b.length; l++)
        {
            int x = b.direction == BlockDirection.Horizontal ? pos.x + l : pos.x;
            int y = b.direction == BlockDirection.Horizontal ? pos.y : pos.y + l;
            if (x >= 0 && x < 6 && y >= 0 && y < 6) map[x, y] = val;
        }
    }

    private static void ClearBlockOnMap(bool[,] map, Vector2Int pos, BlockEntity b)
    {
        SetBlockOnMap(map, pos, b, false);
    }

    private static List<Vector2Int> GetValidMovesForBlock(bool[,] map, Vector2Int pos, BlockEntity b, int w, int h)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        if (b.direction == BlockDirection.Horizontal)
        {
            for (int x = pos.x - 1; x >= 0; x--)
            {
                if (map[x, pos.y]) break;
                moves.Add(new Vector2Int(x, pos.y));
            }
            for (int x = pos.x + 1; x + b.length - 1 < w; x++)
            {
                if (map[x + b.length - 1, pos.y]) break;
                moves.Add(new Vector2Int(x, pos.y));
            }
        }
        else
        {
            for (int y = pos.y - 1; y >= 0; y--)
            {
                if (map[pos.x, y]) break;
                moves.Add(new Vector2Int(pos.x, y));
            }
            for (int y = pos.y + 1; y + b.length - 1 < h; y++)
            {
                if (map[pos.x, y + b.length - 1]) break;
                moves.Add(new Vector2Int(pos.x, y));
            }
        }

        return moves;
    }
}