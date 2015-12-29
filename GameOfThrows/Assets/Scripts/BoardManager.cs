using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable UseNullPropagation

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
        public const int BORDER_COL = -1;
        public int columns = 15;
        public int rows = 15;

        // Up, Down, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight
        public GameObject[] wallTiles;
        public GameObject[] floorTiles;
        
        private readonly List<Vector3> floorPositions = new List<Vector3>();
        private readonly List<Vector3> takenPositions = new List<Vector3>(); 

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
                return col == columns 
                    ? BlockPosition.BottomRight 
                    : BlockPosition.Bottom;
            }
            if (row != rows)
            {
                if (col == BORDER_COL)
                    return BlockPosition.Left;
                return col == columns ? BlockPosition.Right : BlockPosition.None;
            }
            if (col == BORDER_COL)
                return BlockPosition.TopLeft;
            return col == columns ? BlockPosition.TopRight : BlockPosition.Top;
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

                    takenPositions.Add(new Vector3(col, row, 0f));

                    if (instance == null) continue;

                    instance.transform.SetParent(wallsHolder);
                    instance.gameObject.GetComponent<BoxCollider2D>().enabled = false;
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

        private void LayoutObjectsAtRandom([NotNull]IList<GameObject> objects, int amount, [NotNull]Transform parent, bool solid)
        {
            if (amount < 0) throw new ArgumentException("Amount must be higher or equal to 0");

            for (int i = 0; i < amount; ++i)
            {
                Vector3 position = RandomPosition();

                if (solid)
                    takenPositions.Add(position);

                GameObject instantiatedObject = objects[Random.Range(0, objects.Count)];
                GameObject instantiated = Instantiate(instantiatedObject, position, Quaternion.identity) as GameObject;

                if (instantiated == null) continue;

                instantiated.transform.SetParent(parent);
            }
        }

        private void CreateWallsColliders()
        {
            GameObject colliders = new GameObject("Colliders");
            colliders.transform.position = Vector3.zero;

            GameObject leftCollider = new GameObject("LeftCollider");
            leftCollider.transform.position = Vector3.zero;
            leftCollider.AddComponent<BoxCollider2D>();
            leftCollider.transform.parent = colliders.transform;

            GameObject rightCollider = new GameObject("RightCollider");
            rightCollider.transform.position = Vector3.zero;
            rightCollider.AddComponent<BoxCollider2D>();
            rightCollider.transform.parent = colliders.transform;

            GameObject topCollider = new GameObject("TopCollider");
            topCollider.transform.position = Vector3.zero;
            topCollider.AddComponent<BoxCollider2D>();
            topCollider.transform.parent = colliders.transform;

            GameObject bottomCollider = new GameObject("BottomCollider");
            bottomCollider.transform.position = Vector3.zero;
            bottomCollider.AddComponent<BoxCollider2D>();
            bottomCollider.transform.parent = colliders.transform;

            const float FIX = 0.5f;

            leftCollider.transform.position = new Vector3(BORDER_COL, rows / 2 - FIX);
            leftCollider.transform.localScale = new Vector3(1, rows, 1);

            rightCollider.transform.position = new Vector3(columns, rows / 2 - FIX);
            rightCollider.transform.localScale = new Vector3(1, rows, 1);

            topCollider.transform.position = new Vector3(columns / 2 - FIX, rows);
            topCollider.transform.localScale = new Vector3(columns, 1, 1);

            bottomCollider.transform.position = new Vector3(columns / 2 - FIX, BORDER_COL);
            bottomCollider.transform.localScale = new Vector3(columns, 1, 1);
        }

        private void SetRandomPlayerPosition()
        {
            GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>().position = GetAvailablePositionOnMap();
        }

        private Vector2 GetAvailablePositionOnMap(bool takePlace = false)
        {
            Vector2 availablePosition = new Vector2(Random.Range(-1, columns), Random.Range(-1, rows));

            while (takenPositions.Contains(availablePosition))
                availablePosition = new Vector2(Random.Range(-1, columns), Random.Range(-1, rows));

            if (takePlace)
                takenPositions.Add(availablePosition);

            return availablePosition;
        }

        /// <summary>
        /// Sets up the floors and the extraWalls
        /// </summary>
        /// <param name="extraWalls">for dev purposes, amount extra walls to be spread around the map</param>
        public void SetUpScene(int extraWalls)
        {
            InitializeFloorPositionsList();
            SetUpWalls();
            LayoutObjectsAtRandom(wallTiles, extraWalls, wallsHolder, true);
            LayoutObjectsAtRandom(floorTiles, floorPositions.Count, floorHolder, false);
            CreateWallsColliders();
            SetRandomPlayerPosition();
        }
    }
}
