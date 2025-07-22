using UnityEngine;

namespace Rotation
{
    /// <summary>
    /// Continuously rotates the attached GameObject around specified axes at specified speeds.
    /// </summary>
    [AddComponentMenu("Rotation/XYZ Axis Rotator")]
    public sealed class RotateSphere : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField, Range(0f, 360f)]
        [Tooltip("Rotation speed around X-axis in degrees per second")]
        private float rotationSpeedX = 0f;

        [SerializeField, Range(0f, 360f)]
        [Tooltip("Rotation speed around Y-axis in degrees per second")]
        private float rotationSpeedY = 90f;

        [SerializeField, Range(0f, 360f)]
        [Tooltip("Rotation speed around Z-axis in degrees per second")]
        private float rotationSpeedZ = 0f;

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            Rotate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Rotates the transform around X, Y, and Z axes based on rotation speeds and delta time.
        /// </summary>
        private void Rotate()
        {
            float x = rotationSpeedX * Time.deltaTime;
            float y = rotationSpeedY * Time.deltaTime;
            float z = rotationSpeedZ * Time.deltaTime;

            transform.Rotate(x, y, z, Space.World);
        }

        #endregion
    }
}
