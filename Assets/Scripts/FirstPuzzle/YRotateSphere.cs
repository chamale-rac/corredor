using UnityEngine;

namespace Rotation
{
    /// <summary>
    /// Continuously rotates the attached GameObject around the Y-axis at a specified speed.
    /// </summary>
    [AddComponentMenu("Rotation/Y Axis Rotator")]
    public sealed class YRotateSphere : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField, Range(0f, 360f)]
        [Tooltip("Rotation speed in degrees per second")]
        private float rotationSpeed = 90f;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Update()
        {
            RotateAroundYAxis();
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Rotates the transform around the Y-axis based on the rotation speed and delta time.
        /// </summary>
        private void RotateAroundYAxis()
        {
            var rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f, Space.World);
        }
        
        #endregion
    }
}