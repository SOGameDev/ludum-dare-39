﻿namespace Assets.Scripts.Infrastructure
{
    using System.Collections.Generic;
    using Tiled2Unity;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    ///     Manages a scrolling background in the horizontal direction.
    /// </summary>
    public class ScrollingBackground : MonoBehaviour
    {
        [SerializeField]
        private float speed = 10;

        [SerializeField]
        private List<string> parts;

        [SerializeField]
        private int nextPart = 0;

        [SerializeField]
        private string partsDirectory = "";

        private TiledMap back;

        private TiledMap front;

        private Camera mainCamera;

        private float CameraHalfWidth
        {
            get { if (mainCamera == null) return 0; return mainCamera.orthographicSize*mainCamera.aspect;  }
        }

		private TiledMap LoadNextMapPart(float baseX)
        {
            if (parts.Count == 0) return null;

            if (nextPart >= parts.Count)
            {
                nextPart = 0;
            }
            else if (nextPart < 0)
            {
                nextPart = 0;
            }

            string partPath = partsDirectory + "/" + parts[nextPart];
            GameObject partPrefabGO = Resources.Load(partPath) as GameObject;
            Assert.IsTrue(partPrefabGO != null, "The part prefab does not exist! Path:" + partPath);
            TiledMap partPrefab = partPrefabGO.GetComponent<TiledMap>();
            Assert.IsTrue(partPrefab != null, "The part prefab does not have a TiledMap component.");

            nextPart++;
            if (nextPart >= parts.Count)
            {
                nextPart = 0;
            }

            return Instantiate(partPrefab, new Vector3(baseX, partPrefab.GetMapHeightInPixelsScaled()/2), Quaternion.identity, transform);
        }

        private void FixedUpdate()
        {
            front.transform.position += new Vector3(-speed*Time.deltaTime, 0);

            back.transform.position = front.transform.position + new Vector3(front.GetMapWidthInPixelsScaled(), 0);

            if (front.transform.position.x + front.GetMapWidthInPixelsScaled() <= -CameraHalfWidth)
            {
                front.transform.position = back.transform.position + new Vector3(back.GetMapWidthInPixelsScaled(), 0);
                Destroy(front.gameObject);
                front = back;
                back = LoadNextMapPart(front.transform.position.x + front.GetMapWidthInPixelsScaled());
            }
        }

        private void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            front = LoadNextMapPart(CameraHalfWidth);
            back = LoadNextMapPart(front.transform.position.x + front.GetMapWidthInPixelsScaled());
        }
    }
}