using System.Collections.Generic;
using UnityEngine;

public class Quadtree
{
    private int maxObjects;
    private int maxLevels;
    private int level;
    private Rect bounds;
    private List<GameObject> objects;
    private Quadtree[] nodes;

    public Quadtree(int level, Rect bounds, int maxObjects = 10, int maxLevels = 5)
    {
        this.level = level;
        this.bounds = bounds;
        this.maxObjects = maxObjects;
        this.maxLevels = maxLevels;
        this.objects = new List<GameObject>();
        this.nodes = null;
    }

    public void Clear()
    {
        objects.Clear();
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                node.Clear();
            }
            nodes = null;
        }
    }

    private void Split()
    {
        float subWidth = bounds.width / 2f;
        float subHeight = bounds.height / 2f;
        float x = bounds.x;
        float y = bounds.y;

        nodes = new Quadtree[4];
        nodes[0] = new Quadtree(level + 1, new Rect(x, y, subWidth, subHeight), maxObjects, maxLevels);
        nodes[1] = new Quadtree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight), maxObjects, maxLevels);
        nodes[2] = new Quadtree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight), maxObjects, maxLevels);
        nodes[3] = new Quadtree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight), maxObjects, maxLevels);
    }

    private int GetIndex(GameObject gameObject)
    {
        Rect objBounds = new Rect(gameObject.transform.position.x, gameObject.transform.position.y, 1, 1); // 假定1x1大小
        int index = -1;
        float verticalMidpoint = bounds.x + bounds.width / 2f;
        float horizontalMidpoint = bounds.y + bounds.height / 2f;

        bool topQuadrant = objBounds.y < horizontalMidpoint && objBounds.y + objBounds.height < horizontalMidpoint;
        bool bottomQuadrant = objBounds.y > horizontalMidpoint;

        if (objBounds.x < verticalMidpoint && objBounds.x + objBounds.width < verticalMidpoint)
        {
            if (topQuadrant) index = 0;
            else if (bottomQuadrant) index = 2;
        }
        else if (objBounds.x > verticalMidpoint)
        {
            if (topQuadrant) index = 1;
            else if (bottomQuadrant) index = 3;
        }

        return index;
    }

    public void Insert(GameObject gameObject)
    {
        if (nodes != null)
        {
            int index = GetIndex(gameObject);
            if (index != -1)
            {
                nodes[index].Insert(gameObject);
                return;
            }
        }

        objects.Add(gameObject);

        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(objects[i]);
                if (index != -1)
                {
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<GameObject> Retrieve(List<GameObject> returnObjects, Rect range)
    {
        int index = GetIndex(new GameObject { transform = { position = new Vector3(range.x, range.y) } });
        if (index != -1 && nodes != null)
        {
            nodes[index].Retrieve(returnObjects, range);
        }

        returnObjects.AddRange(objects);
        return returnObjects;
    }
}
