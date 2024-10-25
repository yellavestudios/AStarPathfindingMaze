using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{

    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    
    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p)  // Construtor
    {
        this.location = l;
        this.G = g;
        this.H = h;
        this.F = f;
        this.marker = m;
        this.parent = p;
    }

    public override bool Equals(object obj) // testing for if one pathmarker is another pathmarker
    {
     
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker) obj).location);
    }

    public override int GetHashCode()
    {
        return 0;
    }

}

public class FindPathAStar : MonoBehaviour
{

    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;

    List<PathMarker> open = new List<PathMarker> ();
    List<PathMarker > closed = new List<PathMarker> ();

    public GameObject start;
    public GameObject end;  
    public GameObject pathP;

    PathMarker goalNode;
    PathMarker startNode;

    PathMarker lastPosition;
    bool done = false;
    bool hasStarted = false;

    void RemoveAllMarkers()  // destroys all markers for a restart
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach(GameObject m in markers)
            Destroy(m);

     }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();


        // picking 2 locations for the maze to start that aren't a wall. 1 represents a wall. Add a location after checking x and z positon.
        List<MapLocation> locations = new List<MapLocation>();
        for (int z = 1; z < maze.width - 1; z++)
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x,z] != 1)
                    locations.Add(new MapLocation(x,z));
            }

        locations.Shuffle();
        
        // maze locations multiplied by maze scale size
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale); 
        startNode = new PathMarker (new MapLocation(locations[0].x, locations[0].z), 0,0,0, Instantiate(start, startLocation,
                                                Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, goalLocation,
                                                Quaternion.identity), null);

        open.Clear(); // clear open list
        closed.Clear(); // clear closed list
        open.Add(startNode);  
        lastPosition = startNode;
    }

    void Search(PathMarker thisNode)
    {

        if (thisNode == null) return;  //prevents user from moving forward until W is pressed before R key
        if (thisNode.Equals(goalNode)) 
        {
            done = true; return; // goal has been found
        }

        foreach(MapLocation dir in maze.directions)
        {
            MapLocation neighbor = dir + thisNode.location;
            if (maze.map[neighbor.x,neighbor.z] == 1) continue; // check if neighbor is a wall. if so, continue to next map direction. neighbor is a postion on the map
            if (neighbor.x < 1 || neighbor.x >= maze.width || neighbor.z < 1 || neighbor.z >= maze.depth) continue; // check if neighbor is outside of desired location
            if (IsClosed(neighbor)) continue;

            //pythagorean thereom 
            float g = Vector2.Distance(thisNode.location.ToVector(), neighbor.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbor.ToVector(), goalNode.location.ToVector()); 
            float f = g + h;

            //create a marker in maze represented by pathP
            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbor.x * maze.scale, 0, neighbor.z * maze.scale),
                                                Quaternion.identity);

            //update values for each marker
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>(); 
            values[0].text = "G: " + g.ToString("0.00");
            values[1].text = "H: " + h.ToString("0.00");
            values[2].text = "F: " + f.ToString("0.00");

            if (!UpdateMarker(neighbor, g, h, f, thisNode))
            {
                open.Add(new PathMarker(neighbor, g, h, f, pathBlock, thisNode));
            } 
        } 

        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker pm = (PathMarker) open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = closedMaterial;

        lastPosition = pm;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker) // check if a marker location is in closed list 
    {
        foreach (PathMarker p in closed) { 
        
            if (p.location.Equals(marker)) return true;
        }
        return false;   
    }

   

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            BeginSearch(); //press W to instantiate start and end markers
            hasStarted = true;
        }
        if (hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.T)) Search(lastPosition);
        }
    }
}
