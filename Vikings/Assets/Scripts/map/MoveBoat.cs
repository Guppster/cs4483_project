﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveBoat : MonoBehaviour {

    public GameObject targetUISprite;

    public Tilemap collisionMap;
    public Tilemap portMap;
    public Tilemap monsterMap;

    public GameObject portUI;
    public GameObject monsterUI;

    int xCoordinate = 0;
    int yCoordinate = 0;

    bool move = false;
    Vector3 movement;

    Vector3Int uiPosition;

    int speed = 4;

    Queue<Vector3Int> path;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("xCoord") || !PlayerPrefs.HasKey("yCoord"))
        {
            uiPosition = new Vector3Int(0, 0, 0);
        }
        else
        {
            if (PlayerPrefs.HasKey("xCoord"))
            {
                xCoordinate = PlayerPrefs.GetInt("xCoord");
            }

            if (PlayerPrefs.HasKey("yCoord"))
            {
                yCoordinate = PlayerPrefs.GetInt("yCoord");
            }

            transform.position = new Vector3Int(xCoordinate, yCoordinate, 0);
            uiPosition = new Vector3Int(xCoordinate * 2, yCoordinate * 2, 0);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }

        path = new Queue<Vector3Int>();
    }

    private void Update()
    {
        if (Time.timeScale == 0.0f)
        {
            return;
        }

        bool updateCoords = false;

        if (Input.GetKeyDown(KeyCode.RightArrow)
        && !collisionMap.HasTile(uiPosition + (new Vector3Int(2, 0, 0)))
        && (Mathf.Abs(xCoordinate + 1 - Camera.main.transform.position.x) < 2*CameraFollow.width))
        {
            xCoordinate++;
            updateCoords = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)
            && !collisionMap.HasTile(uiPosition + (new Vector3Int(-2, 0, 0)))
            && (Mathf.Abs(xCoordinate - 1 - Camera.main.transform.position.x) < 2*CameraFollow.width))
        {
            xCoordinate--;
            updateCoords = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) 
            && !collisionMap.HasTile(uiPosition + (new Vector3Int(0, 2, 0)))
            && (Mathf.Abs(yCoordinate + 1 - Camera.main.transform.position.y) < 2*CameraFollow.height))
        {
            yCoordinate++;
            updateCoords = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) 
            && !collisionMap.HasTile(uiPosition + (new Vector3Int(0, -2, 0)))
            && (Mathf.Abs(yCoordinate - 1 - Camera.main.transform.position.y) < 2*CameraFollow.height))
        {
            yCoordinate--;
            updateCoords = true;
        }
        if (portMap.HasTile(uiPosition))
        {
            portUI.SetActive(true);
            monsterUI.SetActive(false);
        }
        else if (monsterMap.HasTile(uiPosition))
        {
            monsterUI.SetActive(true);
            portUI.SetActive(false);
        }
        else
        {
            portUI.SetActive(false);
            monsterUI.SetActive(false);
        }
        if (updateCoords)
        {
            PlayerPrefs.SetInt("xCoord", xCoordinate);
            PlayerPrefs.SetInt("yCoord", yCoordinate);
            PlayerPrefs.Save();

            uiPosition = new Vector3Int(xCoordinate * 2, yCoordinate * 2, 0);

            Vector3Int addToPath = new Vector3Int(xCoordinate, yCoordinate, 0);

            if (!path.Contains(addToPath))
            {
                path.Enqueue(addToPath);
            }
            else
            {
                Queue<Vector3Int> newPath = new Queue<Vector3Int>();
                while (path.Peek() != addToPath)
                {
                    newPath.Enqueue(path.Dequeue());
                }
                newPath.Enqueue(addToPath);

                path = newPath;
            }

            targetUISprite.transform.position = new Vector2(xCoordinate, yCoordinate);
        }        
    }

    private void FixedUpdate()
    {
        if (path.Count > 0)
        {
            Vector3Int nextPosition = path.Peek();

            movement = Vector3.zero;

            if (transform.position.x < nextPosition.x)
            {
                movement += new Vector3(speed * 0.0625f, 0f, 0f);
            }

            if (transform.position.x > nextPosition.x)
            {
                movement += new Vector3(speed * -0.0625f, 0f, 0f);
            }

            if (transform.position.y < nextPosition.y)
            {
                movement += new Vector3(0f, speed * 0.0625f, 0f);
            }

            if (transform.position.y > nextPosition.y)
            {
                movement += new Vector3(0f, speed * -0.0625f, 0f);
            }

            transform.position += movement;

            if (transform.position == nextPosition)
            {
                path.Dequeue();
            }
        }
    }    
}
