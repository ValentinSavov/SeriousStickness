using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    
    public List<Vector2i> FindPath(Vector2i start,  Vector2i end, Byte[,] mGrid, int characterWidth, int characterHeight, short maxCharacterJumpHeight)
    {
        List<Vector2i> path = new List<Vector2i>();
        path.Add(end);
        Dictionary<Vector2i, Vector2i> camefrom = new Dictionary<Vector2i, Vector2i>();
        Queue<Vector2i> frontier = new Queue<Vector2i>();

        if (!CharacterFitsInTargetLocation(characterWidth, characterHeight, end, mGrid))
        {
            return null;
        }

        Vector2i current = start;
        frontier.Enqueue(start);
        camefrom.Add(start, new Vector2i(0, 0));

        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();
            if (current == end)
            {
                break;
            }
            Vector2i[] neighbours = {
                    current + new Vector2i(1, 0),
                    current + new Vector2i(1, 1),
                    current + new Vector2i(0, 1),
                    current + new Vector2i(-1, 1),
                    current + new Vector2i(-1, 0),
                    current + new Vector2i(-1, -1),
                    current + new Vector2i(0, -1),
                    current + new Vector2i(1, -1)
                };
            foreach (Vector2i tile in neighbours)
            {
                if (!camefrom.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    camefrom.Add(tile, current);
                }
            }
        }
        if (camefrom.ContainsKey(end))
        {
            current = start;
            while (current != start)
            {
                path.Add(current);
                current = camefrom[current];
            }

            path.Add(start);

            return path;
        }
        return null;

    }


    bool CharacterFitsInTargetLocation(int characterWidth, int characterHeight, Vector2i loc, Byte[,] mGrid)
    {
        bool fits = true;
        for (var i = 0; i < characterWidth; ++i)
        {
            fits = true;
            for (var w = 0; w < characterWidth; ++w)
            {
                if (mGrid[loc.x + w, loc.y] == 0
                    || mGrid[loc.x + w, loc.y + characterHeight - 1] == 0)
                {
                    fits = false;
                    break;
                }

            }
            if (fits == true)
            {
                for (var h = 1; h < characterHeight - 1; ++h)
                {
                    if (mGrid[loc.x, loc.y + h] == 0
                        || mGrid[loc.x + characterWidth - 1, loc.y + h] == 0)
                    {
                        fits = false;
                        break;
                    }
                }
            }

            if (!fits)
                loc.x -= characterWidth - 1;
            else
                break;
        }
        return fits;
    }
}
