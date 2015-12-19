﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal enum BlockPosition
    {
        None = -1,
        Top = 0,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class BoardManager : MonoBehaviour
    {
        private const int BORDER_COL = -1;
        public int columns = 15;
        public int rows = 15;

        // Up, Down, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight
        public GameObject[] wallTiles;
        public GameObject[] floorTiles;
        
        private readonly List<Vector3> floorPositions = new List<Vector3>();

        private Transform boardHolder;
        private Transform floorHolder;
        private Transform wallsHolder;

        private void InitializeFloorPositionsList()
        {
            floorPositions.Clear();

            for (int i = 0; i < columns; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    floorPositions.Add(new Vector3(i, j, 0f));
                }
            }
        }

        /// <summary>
        /// Gets the BlockPosition based on where on the wall grid the block is
        /// </summary>
        /// <param name="row">Row of the block</param>
        /// <param name="col">Column of the block</param>
        /// <returns>BlockPosition representing the position of the wall, or BlockPosition.None if it's a center block(a floor)</returns>
        private BlockPosition GetBlockIndex(int row, int col)
        {
            ///////////
            // 1 2 3 //
            // 4 5 6 // Number represents position in map
            // 7 8 9 //
            ///////////
            if (row == BORDER_COL)
            {
                if (col == BORDER_COL)
                    return BlockPosition.BottomLeft;    // 7
                if (col == columns)
                    return BlockPosition.BottomRight;   // 9
                return BlockPosition.Bottom;            // 8
            }
            if (row == rows)
            {
                if (col == BORDER_COL)
                    return BlockPosition.TopLeft;       // 1
                if (col == columns)
                    return BlockPosition.TopRight;      // 3
                return BlockPosition.Top;               // 2
            }
            if (col == BORDER_COL)
                return BlockPosition.Left;              // 4
            if (col == columns)
                return BlockPosition.Right;             // 6
            return BlockPosition.None;                  // 5
        }

        private void SetUpWalls()
        {
            boardHolder = new GameObject("Board").transform;
            floorHolder = new GameObject("Floors").transform;
            floorHolder.parent = boardHolder;
            wallsHolder = new GameObject("Walls").transform;
            wallsHolder.parent = boardHolder;

            for (int col = BORDER_COL; col < columns + 1; col++)
            {
                for (int row = BORDER_COL; row < rows + 1; row++)
                {
                    BlockPosition pos = GetBlockIndex(row, col);
                    if (pos == BlockPosition.None) continue;

                    GameObject toInstantiate = wallTiles[(int)pos];
                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(col, row, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(wallsHolder);
                }
            }
        }

        private Vector3 RandomPosition()
        {
            int randomIndex = Random.Range(0, floorPositions.Count);
            Vector3 position = floorPositions[randomIndex];
            floorPositions.RemoveAt(randomIndex);
            return position;
        }

        private void LayoutObjectsAtRandom(GameObject[] objects, int amount, Transform parent)
        {
            for (int i = 0; i < amount; ++i)
            {
                Vector3 position = RandomPosition();
                GameObject instantiatedObject = objects[Random.Range(0, objects.Length)];
                GameObject instantiated = Instantiate(instantiatedObject, position, Quaternion.identity) as GameObject;
                instantiated.transform.SetParent(parent);
            }
        }

        /// <summary>
        /// Sets up the floors and the extraWalls
        /// </summary>
        /// <param name="extraWalls">for dev purposes, amount extra walls to be spread around the map</param>
        public void SetUpScene(int extraWalls)
        {
            InitializeFloorPositionsList();
            SetUpWalls();
            LayoutObjectsAtRandom(wallTiles, extraWalls, wallsHolder);
            LayoutObjectsAtRandom(floorTiles, floorPositions.Count, floorHolder);
        }
    }
}