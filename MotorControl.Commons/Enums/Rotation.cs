namespace MotorControl.Commons.Enums
{
    /// <summary>
    /// Options for motor direction rotation
    /// </summary>
    public enum Rotation
    {
        /// <summary>
        /// Counter-clockwise, the default
        /// </summary>
        CCW,
        /// <summary>
        /// Clockwise
        /// </summary>
        CW
    }

    public enum RotationAllowed
    {
        CCW_Only,
        CW_Only,
        Both
    }
}