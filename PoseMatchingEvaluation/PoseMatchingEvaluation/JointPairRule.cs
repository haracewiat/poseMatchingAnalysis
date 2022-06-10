using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;

namespace PoseMatchingEvaluation
{
    /// <summary>
    /// Represents a rule for a pair of joints in terms of the position of the secondary joint with relation to
    /// the relative joint.
    /// </summary>
    public struct JointPairRule
    {
        private JointPair jointPair;
        private List<JointRelativePosition> possiblePositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JointPairRule"/> struct.
        /// </summary>
        /// <param name="jointPair">The joint pair this rule applies to.</param>
        /// <param name="possiblePositions">The list of possible positions that satisfy the rule.</param>
        public JointPairRule(JointPair jointPair, List<JointRelativePosition> possiblePositions)
        {
            this.jointPair = jointPair;
            this.possiblePositions = possiblePositions;
        }

        /// <summary>
        /// Gets the joint pair.
        /// </summary>
        public JointPair JointPair => jointPair;

        /// <summary>
        /// Gets the list of possible relative joint positions that satisfy the rule.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<JointRelativePosition> PossiblePositions => possiblePositions;

        /// <summary>
        /// Checks whether the relative positioning of joints included in the joint pair meet the rule requirement.
        /// </summary>
        /// <param name="trackingData">The raw tracking data containing position vectors of all body joints.</param>
        /// <param name="cameraRotation">The roation of the camera.</param>
        /// <returns>Returns a value indicating whether the rule is satisfied.</returns>
        public bool IsSatisfied(RawCameraData trackingData, CameraRotation cameraRotation = CameraRotation.Default)
        {
            return possiblePositions.Contains(jointPair.CalculateRelativePosition(trackingData, cameraRotation));
        }
    }
}
