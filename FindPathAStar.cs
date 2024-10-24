using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{

    public MapLocation location;
    public float h;
    public float v;
    public float b;
    public GameObject marker;
    public PathMarker parent;

    //construtor
    public PathMarker(MapLocation l, float h, float v, float b, GameObject marker, PathMarker p)
    {
        this.location = l;
        this.h = h;
        this.v = v;
        this.b = b;
        this.marker = marker;
        this.parent = p;
    }

    public override bool Equals(object obj) //testing for if one pathmarker is another pathmarker
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

    void RemoveAllMarkers()  //destroys all markers for a restart
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach(GameObject m in markers)
            Destroy(m);

     }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();


        //picking 2 locations for the maze to start that aren't a wall. 1 represents a wall. Add a location after checking x and z positon.
        List<MapLocation> locations = new List<MapLocation>();
        for (int z = 1; z < maze.width - 1; z++)
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x,z] != 1)
                    locations.Add(new MapLocation(x,z));
            }

        locations.Shuffle();
        
        //maze locations multiplied by maze scale size
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale); 
        startNode = new PathMarker (new MapLocation(locations[0].x, locations[0].z), 0,0,0, Instantiate(start, startLocation,
                                                Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, goalLocation,
                                                Quaternion.identity), null);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //press W to instantiate markers
        if (Input.GetKeyDown(KeyCode.W)) BeginSearch(); 
    }
}
