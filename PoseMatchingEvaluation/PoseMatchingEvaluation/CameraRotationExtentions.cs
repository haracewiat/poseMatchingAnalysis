using UnityEngine;

namespace PoseMatchingEvaluation
{
    /// <summary>
    /// Contains operations for the camera.
    /// </summary>
    public static class CameraRotationExtentions
    {
        /// <summary>
        /// Converts the camera's rotation to a normalized vector. Used to use as a reference vector to calculate angles between joints.
        /// </summary>
        /// <param name="rotation">The rotation of the camera.</param>
        /// <returns>Returns a vector indicating the upright direction of the camera accounting for its rotation.</returns>
        public static Vector3 CameraRotationAsVector(this CameraRotation rotation) => rotation switch
        {
            CameraRotation.Default => new Vector3(0, -1, 0),
            CameraRotation.Clockwise => new Vector3(1, 0, 0),
            CameraRotation.UpsideDown => new Vector3(0, 1, 0),
            CameraRotation.CounterClockwise => new Vector3(-1, 0, 0),
            _ => new Vector3(0, -1, 0),
        };
    }
}