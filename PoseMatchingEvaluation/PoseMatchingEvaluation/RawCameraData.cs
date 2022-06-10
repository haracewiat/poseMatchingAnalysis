using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoseMatchingEvaluation
{
    /// <summary>
    /// Represents raw camera data.
    /// </summary>
    [Serializable]
    public struct RawCameraData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawCameraData"/> struct.
        /// </summary>
        /// <param name="landmarks">The labels of the landmarks.</param>
        public RawCameraData(Dictionary<string, int[]> landmarks)
        {
            Landmarks = landmarks;
        }

        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        [JsonProperty("landmarks")]
        public Dictionary<string, int[]> Landmarks { get; set; }

        public Vector3 GetKeypointPosition(string name)
        {
            int[] coordinates = Landmarks[name];

            return new Vector3(coordinates[0], coordinates[1], coordinates[2]);
        }

        public Vector3 GetKeypointPosition(int index)
        {
            int[] coordinates = Landmarks.ElementAt(index).Value;

            return new Vector3(coordinates[0], coordinates[1], coordinates[2]);
        }

    }
}