namespace PoseMatchingEvaluation
{
    /// <summary>
    /// Defines possible camera rotation settings.
    /// </summary>
    public enum CameraRotation
    {
        /// <summary>
        /// Default camera orientation, i.e. horizontal.
        /// </summary>
        Default,

        /// <summary>
        /// 90 degrees clockwise, vertical.
        /// </summary>
        Clockwise,

        /// <summary>
        /// 180 degrees clockwise, horizontal.
        /// </summary>
        UpsideDown,

        /// <summary>
        /// 90 degrees counterclockwise, vertical.
        /// </summary>
        CounterClockwise,
    }
}
