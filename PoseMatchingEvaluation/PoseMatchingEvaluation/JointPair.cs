using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace PoseMatchingEvaluation
{
    /// <summary>
    /// Represents a pair of joints, where the position of the secondary joint can be described in
    /// relation to the reference joint.
    /// </summary>
    public struct JointPair
    {
        private const int SectorsCount = 8;
        private const float SectorAngle = 360f / SectorsCount;
        private const float SectorCounterclockwiseDisplacement = SectorAngle * .5f;

        private string referenceJoint;

        private string secondaryJoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="JointPair"/> struct.
        /// </summary>
        /// <param name="referenceJoint">The reference joint.</param>
        /// <param name="secondaryJoint">The secondary joint.</param>
        public JointPair(string referenceJoint, string secondaryJoint)
        {
            this.referenceJoint = referenceJoint;
            this.secondaryJoint = secondaryJoint;
        }

        /// <summary>
        /// Gets the reference joint.
        /// </summary>
        public string ReferenceJoint => referenceJoint;

        /// <summary>
        /// Gets the secondary joint.
        /// </summary>
        public string SecondaryJoint => secondaryJoint;

        /// <summary>
        /// Calculates the position of the secondary joint with relation to the reference joint.
        /// </summary>
        /// <param name="trackingData">The raw tracking data containing position vectors for all body joints.</param>
        /// <param name="cameraRotation">The rotation of the camera.</param>
        /// <returns>Returns the secondary joint's relative position.</returns>
        public JointRelativePosition CalculateRelativePosition(RawCameraData trackingData, CameraRotation cameraRotation)
        {
            Vector3 relativeJointPosition = trackingData.GetKeypointPosition(referenceJoint);
            Vector3 secondaryJointPosition = trackingData.GetKeypointPosition(secondaryJoint);

            Vector3 jointDirection = (secondaryJointPosition - relativeJointPosition).normalized;

            return (JointRelativePosition)Mathf.FloorToInt(CalculateAngleTo(jointDirection, cameraRotation) / SectorAngle);
        }

        private float CalculateAngleTo(Vector3 jointDirection, CameraRotation cameraRotation)
        {
            // Calculates angle in range [0; 360]
            float angle = (180f + (Vector2.SignedAngle(jointDirection, cameraRotation.CameraRotationAsVector()) % 360f)) % 360f;

            // Apply angle displacement and convert angle to range [0; 360]
            angle = (((angle + SectorCounterclockwiseDisplacement) % 360f) + 360f) % 360f;

            return angle;
        }

        public float CalculateDegreesFrom(RawCameraData trackingData, CameraRotation cameraRotation)
        {
            Vector3 relativeJointPosition = trackingData.GetKeypointPosition(referenceJoint);
            Vector3 secondaryJointPosition = trackingData.GetKeypointPosition(secondaryJoint);

            Vector3 jointDirection = (secondaryJointPosition - relativeJointPosition).normalized;

            return Vector2.SignedAngle(jointDirection, cameraRotation.CameraRotationAsVector());
        }
    }
}
