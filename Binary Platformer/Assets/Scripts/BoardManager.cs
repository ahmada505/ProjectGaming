﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardRows, boardColumns;
    public int minRoomSize, maxRoomSize;
    public GameObject room;
    public GameObject wallTile;
    private bool[,] boardPositionsFloor;

   
    public GameObject corridorTile;
    private Vector3 beginPos = new Vector3(15, 10, 0.55f);
    private Vector3 endPos = new Vector3(1, 1, 1);
    public GameObject lastRoom;
    public GameObject player;

    void Start()
    {
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();
        boardPositionsFloor = new bool[boardRows, boardColumns];
        DrawRooms(rootSubDungeon);
        DrawCorridors(rootSubDungeon);
        DrawWalls(rootSubDungeon);
        lastRoom.name = "Last Room";
        Instantiate(player, beginPos, Quaternion.identity);

    }

    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0); // i.e null
        public List<Rect> corridors = new List<Rect>();
        public int debugId;

        private static int debugCounter = 0;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;
        }

        public Rect GetRoom()
        {
            if (IAmLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                {
                    return lroom;
                }
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                {
                    return rroom;
                }
            }

            // workaround non nullable structs
            return new Rect(-1, -1, 0, 0);
        }

        public bool IAmLeaf()
        {
            return left == null && right == null;
        }

        public void CreateRoom()
        {
            if (left != null)
            {
                left.CreateRoom();
            }
            if (right != null)
            {
                right.CreateRoom();
            }
            if (left != null && right != null)
            {
                CreateCorridorBetween(left, right);
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
            }
        }

        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();

            Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

            // attach the corridor to a random point in each room
            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            // always be sure that left point is on the left to simplify the code
            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);

            Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

            // if the points are not aligned horizontally
            if (w != 0)
            {
                // choose at random to go horizontal then vertical or the opposite
                if (Random.Range(0, 1) > 2)
                {
                    // add a corridor to the right
                    corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));
                    corridors.Add(new Rect(lpoint.x, lpoint.y + 1, Mathf.Abs(w) + 1, 1));
                    corridors.Add(new Rect(lpoint.x, lpoint.y - 1, Mathf.Abs(w) + 1, 1));

                    // if left point is below right point go up
                    // otherwise go down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(rpoint.x + 1, lpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(rpoint.x - 1, lpoint.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
                        corridors.Add(new Rect(rpoint.x + 1, lpoint.y, 1, -Mathf.Abs(h)));
                        corridors.Add(new Rect(rpoint.x - 1, lpoint.y, 1, -Mathf.Abs(h)));
                    }
                }
                else
                {
                    // go up or down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(lpoint.x + 1, lpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(lpoint.x - 1, lpoint.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(lpoint.x + 1, rpoint.y, 1, Mathf.Abs(h)));
                        corridors.Add(new Rect(lpoint.x - 1, rpoint.y, 1, Mathf.Abs(h)));
                    }

                    // then go right
                    corridors.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));
                    corridors.Add(new Rect(lpoint.x, rpoint.y + 1, Mathf.Abs(w) + 1, 1));
                    corridors.Add(new Rect(lpoint.x, rpoint.y - 1, Mathf.Abs(w) + 1, 1));
                }
            }
            else
            {
                // if the points are aligned horizontally
                // go up or down depending on the positions
                if (h < 0)
                {
                    corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
                    corridors.Add(new Rect((int)lpoint.x + 1, (int)lpoint.y, 1, Mathf.Abs(h)));
                    corridors.Add(new Rect((int)lpoint.x - 1, (int)lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
                    corridors.Add(new Rect((int)rpoint.x + 1, (int)rpoint.y, 1, Mathf.Abs(h)));
                    corridors.Add(new Rect((int)rpoint.x - 1, (int)rpoint.y, 1, Mathf.Abs(h)));
                }
            }

            Debug.Log("Corridors: ");
            foreach (Rect corridor in corridors)
            {
                Debug.Log("corridor: " + corridor);
            }
        }


        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if (!IAmLeaf())
            {
                return false;
            }

            // choose a vertical or horizontal split depending on the proportions
            // i.e. if too wide split vertically, or too long horizontally,
            // or if nearly square choose vertical or horizontal at random
            bool splitH;
            if (rect.width / rect.height >= 1.25)
            {
                splitH = false;
            }
            else if (rect.height / rect.width >= 1.25)
            {
                splitH = true;
            }
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
            }

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
                return false;
            }

            if (splitH)
            {
                // split so that the resulting sub-dungeons widths are not too small
                // (since we are splitting horizontally)
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }
    }

    void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        DrawCorridors(subDungeon.left);
        DrawCorridors(subDungeon.right);

        foreach (Rect corridor in subDungeon.corridors)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    if (boardPositionsFloor[i, j] == false)
                    {
                        //moving the corridors and rotating
                        GameObject instance = Instantiate(corridorTile, new Vector3(i, j, 0.6f), new Quaternion(0, -1, 1, 0)) as GameObject;
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[i, j] = true;
                    }
                }
            }
        }
    }

    void DrawWalls(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        foreach (Rect corridor in subDungeon.corridors)
        {
            for (int i = 0; i < boardRows; i++)
            {
                for (int j = 0; j < boardColumns; j++)
                {
                    if (boardPositionsFloor[i, j] == false)
                    {
                        GameObject instance = Instantiate(wallTile, new Vector3(i, j, 0.5f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[i, j] = true;
                    }
                }
            }
        }
    }

    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {
            float sizeX = subDungeon.room.xMax - subDungeon.room.x;
            float sizeY = subDungeon.room.yMax - subDungeon.room.y;
            //float sizeZ = subDungeon.room.ZMax - subDungeon.room.Z;
            if (subDungeon.room.position.x < beginPos.x && subDungeon.room.y < beginPos.z)
            {
                beginPos.x = subDungeon.room.x + sizeX / 2;
                beginPos.z = subDungeon.room.y + sizeY / 2;
            }

            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
            {
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {

                    boardPositionsFloor[i, j] = true;
                }
            }
            GameObject instance = Instantiate(room, new Vector3(subDungeon.room.x + sizeX / 2 - 0.5f, subDungeon.room.y + sizeY / 2 - 0.5f, 0.6f),new Quaternion(0,-1,1,0), transform) as GameObject;
            instance.transform.localScale = new Vector3(sizeX * 0.1f, 1, sizeY * 0.1f);

            if (subDungeon.room.position.x > endPos.x && subDungeon.room.y > endPos.z)
            {
                endPos.x = subDungeon.room.x;
                endPos.z = subDungeon.room.y;
                lastRoom = instance;
            }
            return;
        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
        }
    }

    public void CreateBSP(SubDungeon subDungeon)
    {
        Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large
            if (subDungeon.rect.width > maxRoomSize
              || subDungeon.rect.height > maxRoomSize
              || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
                      + subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
                      + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }


}

