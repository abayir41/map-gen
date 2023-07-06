using System;
using System.Collections.Generic;

namespace MapGen.TunnelSystem
{
    // C# program to find shortest
    // path in an undirected graph
    using System;
    using System.Collections.Generic;

    public class PathFinder
    {
        private int v;
        private List<List<int>> adj;

        public PathFinder(int vertices)
        {
            v = vertices;
            adj = new List<List<int>>(v);
            for (var i = 0; i < v; i++) adj.Add(new List<int>());
        }
        
        //

        // function to form edge between
        // two vertices source and dest
        public void AddEdge(int i, int j)
        {
            adj[i].Add(j);
            adj[j].Add(i);
        }

        // function to print the shortest
        // distance and path between source
        // vertex and destination vertex
        public List<int> FindShortestPath(int s, int dest)
        {
            // predecessor[i] array stores
            // predecessor of i and distance
            // array stores distance of i
            // from s
            var pred = new int[v];
            var dist = new int[v];

            if (BFS(s, dest, pred, dist) == false)
            {
                return null;
            }

            // List to store path
            var path = new List<int>();
            var crawl = dest;
            path.Add(crawl);

            while (pred[crawl] != -1)
            {
                path.Add(pred[crawl]);
                crawl = pred[crawl];
            }

            for (var i = path.Count - 1; i >= 0; i--) Console.Write(path[i] + " ");

            return path;
        }

        
        private bool BFS(int src, int dest, int[] pred, int[] dist)
        {
            var queue = new List<int>();
            
            var visited = new bool[v];
            
            for (var i = 0; i < v; i++)
            {
                visited[i] = false;
                dist[i] = int.MaxValue;
                pred[i] = -1;
            }
            
            visited[src] = true;
            dist[src] = 0;
            queue.Add(src);

            while (queue.Count != 0)
            {
                var u = queue[0];
                queue.RemoveAt(0);

                for (var i = 0; i < adj[u].Count; i++)
                    if (visited[adj[u][i]] == false)
                    {
                        visited[adj[u][i]] = true;
                        dist[adj[u][i]] = dist[u] + 1;
                        pred[adj[u][i]] = u;
                        queue.Add(adj[u][i]);

                        
                        if (adj[u][i] == dest) return true;
                    }
            }

            return false;
        }
    }
}