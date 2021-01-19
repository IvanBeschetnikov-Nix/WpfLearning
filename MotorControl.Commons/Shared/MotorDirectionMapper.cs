using MotorControl.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Shared
{
    public static class MotorDirectionMapper
    {
        private static Rotation? _rotationAllowedToRotationMap(RotationAllowed rotationAllowed) =>
            rotationAllowed switch
            {
                RotationAllowed.CCW_Only => Rotation.CCW,
                RotationAllowed.CW_Only => Rotation.CW,
                _ => null
            };

        private static RotationAllowed _rotationToRotationAllowedMap(Rotation? rotation) =>
            rotation switch
            {
                Rotation.CCW => RotationAllowed.CCW_Only,
                Rotation.CW => RotationAllowed.CW_Only,
                _ => RotationAllowed.Both
            };


        public static RotationAllowed GetFromRotation(Rotation? rotation) => _rotationToRotationAllowedMap(rotation);

        public static Rotation? GetFromRotationAllowed(RotationAllowed rotationAllowed) => _rotationAllowedToRotationMap(rotationAllowed);
    }
}
