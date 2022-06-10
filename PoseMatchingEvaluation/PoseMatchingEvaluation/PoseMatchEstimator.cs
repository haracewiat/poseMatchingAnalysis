using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Accord.Math.Distances;

namespace PoseMatchingEvaluation
{
    public abstract class PoseMatchEstimator
    {
        internal abstract string Name { get; }
        internal abstract float MinimumScoreThreshold { get; }
        internal abstract float GetPoseMatchAccuracy(PoseInfo desiredPose, RawCameraData currentPose);
    }


    public class RulesPoseMatchEstimator : PoseMatchEstimator
    {
        private const CameraRotation Rotation = CameraRotation.Default;

        internal override string Name => "RulesMethod";
        internal override float MinimumScoreThreshold => 1f;

        internal override float GetPoseMatchAccuracy(PoseInfo desiredPose, RawCameraData currentPose)
        {
            List<JointPairRule> rules = desiredPose.JointPairRules;

            return rules.Count(rule => rule.IsSatisfied(currentPose, Rotation)) / (float)rules.Count;
        }
    }

    public class RelativeMetricsPoseMatchEstimator : PoseMatchEstimator
    {
        private float angleThreshold = 20;
        private float distanceThreshold = 0.40f;
        private bool useDistance = true;

        private const CameraRotation Rotation = CameraRotation.Default;

        internal override float MinimumScoreThreshold => 0.7f;
        internal override string Name => $"RelativeMetricsMethod_{angleThreshold}_{distanceThreshold*100}_{useDistance}";

        public RelativeMetricsPoseMatchEstimator(float angleThreshold, float distanceThreshold, bool useDistance)
        {
            this.angleThreshold = angleThreshold;
            this.distanceThreshold = distanceThreshold;
            this.useDistance = useDistance;
        }

        internal override float GetPoseMatchAccuracy(PoseInfo desiredPose, RawCameraData currentPose)
        {
            float temporaryAngleScore = 0;
            float temporaryDistanceScore = 0;

            foreach (string joint in desiredPose.ImportantJoints)
            {
                // Get relative direction (angle) for the joint from reference point (middle of the shoulders)

                // Angle
                float angle = Mathf.Abs(CalculateAngle(desiredPose.RawCameraData, joint) - CalculateAngle(currentPose, joint));
                if (angle <= angleThreshold) temporaryAngleScore += (1 - (angle / angleThreshold)) / desiredPose.ImportantJoints.Count;

                // Distance
                if (useDistance)
                {
                    float distance = Mathf.Abs(CalculateDistance(desiredPose.RawCameraData, joint) - CalculateDistance(currentPose, joint));
                    if (distance <= distanceThreshold) temporaryDistanceScore += (1 - (distance / distanceThreshold)) / desiredPose.ImportantJoints.Count;
                }
            }

            return useDistance ? (temporaryAngleScore + temporaryDistanceScore) / 2 : temporaryAngleScore;
        }

        private float CalculateAngle(RawCameraData trackingData, string joint)
        {
            Vector3 relativeJointPosition = Vector3.Lerp(trackingData.GetKeypointPosition("LeftShoulder"), trackingData.GetKeypointPosition("RightShoulder"), 0.5f);
            Vector3 secondaryJointPosition = trackingData.GetKeypointPosition(joint);

            Vector3 jointDirection = (secondaryJointPosition - relativeJointPosition).normalized;

            return Vector3.Angle(Rotation.CameraRotationAsVector(), jointDirection);
        }

        private float CalculateDistance(RawCameraData trackingData, string joint)
        {
            float baseDistance = Vector3.Distance(trackingData.GetKeypointPosition("LeftShoulder"), trackingData.GetKeypointPosition("LeftElbow"));

            float normalizationFactor = 1 / baseDistance;

            Vector3 middleShoulderPoint = (trackingData.GetKeypointPosition("LeftShoulder") + trackingData.GetKeypointPosition("RightShoulder")) * 0.5f;

            return Vector3.Distance(trackingData.GetKeypointPosition(joint), middleShoulderPoint) * normalizationFactor;
        }

    }

}

