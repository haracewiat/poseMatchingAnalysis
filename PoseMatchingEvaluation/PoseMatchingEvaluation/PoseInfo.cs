using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoseMatchingEvaluation
{
    /// <summary>
    /// A class containing information about the pose.
    /// </summary>
    [Serializable]
    public class PoseInfo
    {
        //private List<JointPairRule> jointPairRules = new List<JointPairRule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PoseInfo"/> class.
        /// </summary>
        /// <param name="rawTrackingData">The <see cref="TrackingData"/> object this Pose Info is associated with.</param>
        /// <param name="name">The name of the pose. Default value is "New pose".</param>
        public PoseInfo(RawCameraData rawCameraData, string name = "Unnamed pose", List<JointPairRule> jointPairRules = null)
        {
            RawCameraData = rawCameraData;
            Name = name;
            JointPairRules = jointPairRules;
        }

        /// <summary>
        /// Gets or sets the name of the pose.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="TrackingData"/> associated with this pose.
        /// </summary>
        public RawCameraData RawCameraData { get; set; }


        /// <summary>
        /// Gets the list of <see cref="PartsEnum"/>s that are marked as important for this pose.
        /// </summary>
        public List<string> ImportantJoints { get; set; }

        /// <summary>
        /// Gets list of pose joint pair rules.
        /// </summary>
        public List<JointPairRule> JointPairRules { get; set; }
    }
}
