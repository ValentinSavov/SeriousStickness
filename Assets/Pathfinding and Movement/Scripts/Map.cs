using UnityEngine;

[System.Serializable]
public enum TileType
{
    Empty,
    Block,
    OneWay
}

[System.Serializable]
public partial class Map : MonoBehaviour 
{
	
	/// <summary>
	/// The map's position in world space. Bottom left corner.
	/// </summary>
	public Vector3 position;
	
	/// <summary>
	/// The base tile sprite prefab that populates the map.
	/// Assigned in the inspector.
	/// </summary>
	public SpriteRenderer tilePrefab;
	
	/// <summary>
	/// The nodes that are fed to pathfinder.
	/// </summary>
	[HideInInspector]
	public byte[,] mGrid;

    /// <summary>
    /// The map's tile data.
    /// </summary>
    [HideInInspector]
    private TileType[,] tiles;

	/// <summary>
	/// The map's sprites.
	/// </summary>
	private SpriteRenderer[,] tilesSprites;
	
	/// <summary>
	/// A parent for all the sprites. Assigned from the inspector.
	/// </summary>
	public Transform mSpritesContainer;
	
	/// <summary>
	/// The size of a tile in meters.
	/// </summary>
	public float cTileSize = 1;
	
	/// <summary>
	/// The width of the map in tiles.
	/// </summary>
	public int mWidth = 50;
	/// <summary>
	/// The height of the map in tiles.
	/// </summary>
	public int mHeight = 50;

    //public MapRoomData mapRoom;

    //public Camera gameCamera;
    //public NavMoveAgent player;

    int lastMouseTileX = -1;
    int lastMouseTileY = -1;

	public TileType GetTile(int x, int y) 
	{
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return TileType.Block;

		return tiles[x, y]; 
	}

    public bool IsOneWayPlatform(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return false;

        return (tiles[x, y] == TileType.OneWay);
    }

    public bool IsGround(int x, int y)
    {
        if (x < 0 || x >= mWidth
           || y < 0 || y >= mHeight)
            return false;

        return (tiles[x, y] == TileType.OneWay || tiles[x, y] == TileType.Block);
    }

    public bool IsObstacle(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return true;

        return (tiles[x, y] == TileType.Block);
    }

    public bool IsNotEmpty(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return true;

        return (tiles[x, y] != TileType.Empty);
    }
    
	public bool IsOnMap(Vector3 point)
    {
        if( (point.x < (position.x + (mWidth * cTileSize))) && (point.y < (position.y + (mHeight * cTileSize))) )
        {
            if ((point.x > position.x) && (point.y > position.y))
            {
                return true;
            }
        }
        return false;
    }

	public void GetMapTileAtPoint(Vector2 point, out int tileIndexX, out int tileIndexY)
	{
		tileIndexY =(int)((point.y - position.y + cTileSize/2.0f)/(float)(cTileSize));
		tileIndexX =(int)((point.x - position.x + cTileSize/2.0f)/(float)(cTileSize));
	}
	
	public Vector2i GetMapTileAtPoint(Vector2 point)
	{
		return new Vector2i((int)((point.x - position.x + cTileSize/2.0f)/(float)(cTileSize)),
					(int)((point.y - position.y + cTileSize/2.0f)/(float)(cTileSize)));
	}
	
	public Vector2 GetMapTilePosition(int tileIndexX, int tileIndexY)
	{
		return new Vector2(
				(float) (tileIndexX * cTileSize) + position.x,
				(float) (tileIndexY * cTileSize) + position.y
			);
	}

	public Vector2 GetMapTilePosition(Vector2i tileCoords)
	{
		return new Vector2(
			(float) (tileCoords.x * cTileSize) + position.x,
			(float) (tileCoords.y * cTileSize) + position.y
			);
	}
	
	public bool CollidesWithMapTile(AABB aabb, int tileIndexX, int tileIndexY)
	{
		var tilePos = GetMapTilePosition (tileIndexX, tileIndexY);
		
		return aabb.Overlaps(tilePos, new Vector2( (float)(cTileSize)/2.0f, (float)(cTileSize)/2.0f));
	}

    public bool AnySolidBlockInRectangle(Vector2 start, Vector2 end)
    {
        return AnySolidBlockInRectangle(GetMapTileAtPoint(start), GetMapTileAtPoint(end));
    }

    public bool AnySolidBlockInStripe(int x, int y0, int y1)
    {
        int startY, endY;

        if (y0 <= y1)
        {
            startY = y0;
            endY = y1;
        }
        else
        {
            startY = y1;
            endY = y0;
        }

        for (int y = startY; y <= endY; ++y)
        {
            if (GetTile(x, y) == TileType.Block)
                return true;
        }

        return false;
    }

    public bool AnySolidBlockInRectangle(Vector2i start, Vector2i end)
    {
        int startX, startY, endX, endY;

        if (start.x <= end.x)
        {
            startX = start.x;
            endX = end.x;
        }
        else
        {
            startX = end.x;
            endX = start.x;
        }

        if (start.y <= end.y)
        {
            startY = start.y;
            endY = end.y;
        }
        else
        {
            startY = end.y;
            endY = start.y;
        }

        for (int y = startY; y <= endY; ++y)
        {
            for (int x = startX; x <= endX; ++x)
            {
                if (GetTile(x, y) == TileType.Block)
                    return true;
            }
        }

        return false;
    }

    public void Awake()
    {

        //Application.targetFrameRate = 60;

        //set the position
        position = transform.position;


        tiles = new TileType[mWidth, mHeight];
        tilesSprites = new SpriteRenderer[mWidth, mHeight];

        mGrid = new byte[Mathf.NextPowerOfTwo((int)mWidth), Mathf.NextPowerOfTwo((int)mHeight)];

        //Camera.main.orthographicSize = Camera.main.pixelHeight / 2;


        for (int y = 0; y < mHeight; ++y)
        {
            for (int x = 0; x < mWidth; ++x)
            {
                bool colFound = false;
                Collider2D[] cols = Physics2D.OverlapPointAll(position + new Vector3(cTileSize * x, cTileSize * y, 0));
                foreach (Collider2D col in cols)
                {
                    if (col.gameObject.isStatic)
                    {
                        colFound = true;
                        break;
                    }
                }

                //if (Physics2D.OverlapPoint(position + new Vector3(cTileSize * x, cTileSize * y, 0)))
                if (colFound)
                {
                    Debug.DrawLine(position + new Vector3((cTileSize * x) - (cTileSize / 8), (cTileSize * y), 0), position + new Vector3((cTileSize * x) + (cTileSize / 8), (cTileSize * y), 0), Color.red, 1000f);
                    Debug.DrawLine(position + new Vector3((cTileSize * x), (cTileSize * y) - (cTileSize / 8), 0), position + new Vector3((cTileSize * x), (cTileSize * y) + (cTileSize / 8), 0), Color.red, 1000f);
                    tiles[x, y] = TileType.Block;
                    mGrid[x, y] = 0;
                    tilesSprites[x, y] = Instantiate<SpriteRenderer>(tilePrefab);
                    tilesSprites[x, y].transform.parent = transform;
                    tilesSprites[x, y].transform.position = position + new Vector3(cTileSize * x, cTileSize * y, 10.0f);
                    tilesSprites[x, y].transform.localScale *= cTileSize;

                }
                else
                {
                    tiles[x, y] = TileType.Empty;
                    mGrid[x, y] = 1;
                }
            }
        }

        for (int y = 0; y < mHeight; ++y)
        {
            tiles[0, y] = TileType.Block;
            tiles[1, y] = TileType.Block;
            tiles[mWidth - 2, y] = TileType.Block;
            tiles[mWidth - 1, y] = TileType.Block;
        }

        for (int x = 0; x < mWidth; ++x)
        {
            tiles[x, 0] = TileType.Block;
            tiles[x, 1] = TileType.Block;
            tiles[x, mHeight - 2] = TileType.Block;
            tiles[x, mHeight - 1] = TileType.Block;
        }

        for (int y = 0; y < mHeight; ++y)
        {
            for (int x = 0; x < mWidth; ++x)
            {
                if(tiles[x,y] == TileType.Empty)
                {
                    Debug.DrawLine(position + new Vector3((cTileSize * x) - (cTileSize / 8), (cTileSize * y), 0), position + new Vector3((cTileSize * x) + (cTileSize / 8), (cTileSize * y), 0), Color.green, 1000f);
                    Debug.DrawLine(position + new Vector3((cTileSize * x), (cTileSize * y) - (cTileSize / 8), 0), position + new Vector3((cTileSize * x), (cTileSize * y) + (cTileSize / 8), 0), Color.green, 1000f);
                }
                else
                {
                    if(tiles[x, y] == TileType.OneWay)
                    {
                        Debug.DrawLine(position + new Vector3((cTileSize * x) - (cTileSize / 8), (cTileSize * y), 0), position + new Vector3((cTileSize * x) + (cTileSize / 8), (cTileSize * y), 0), Color.blue, 1000f);
                        Debug.DrawLine(position + new Vector3((cTileSize * x), (cTileSize * y) - (cTileSize / 8), 0), position + new Vector3((cTileSize * x), (cTileSize * y) + (cTileSize / 8), 0), Color.blue, 1000f);
                    }
                    else
                    {
                        Debug.DrawLine(position + new Vector3((cTileSize * x) - (cTileSize / 8), (cTileSize * y), 0), position + new Vector3((cTileSize * x) + (cTileSize / 8), (cTileSize * y), 0), Color.red, 1000f);
                        Debug.DrawLine(position + new Vector3((cTileSize * x), (cTileSize * y) - (cTileSize / 8), 0), position + new Vector3((cTileSize * x), (cTileSize * y) + (cTileSize / 8), 0), Color.red, 1000f);
                    }
                }
            }
        }
    }

    Camera gameCamera;
    void UpdateBcp()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
            lastMouseTileX = lastMouseTileY = -1;

        Vector2 mousePos = Input.mousePosition;
        Vector2 cameraPos = Camera.main.transform.position;
        var mousePosInWorld = cameraPos + mousePos - new Vector2(gameCamera.pixelWidth / 2, gameCamera.pixelHeight / 2);

        int mouseTileX, mouseTileY;
        GetMapTileAtPoint(mousePosInWorld, out mouseTileX, out mouseTileY);

    }
}
