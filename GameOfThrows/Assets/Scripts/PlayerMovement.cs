using System;
using System.Text;
using GotLib;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Local

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerMovement : IPlayerMovement
    {
        #region Abstract Methods
        protected override void UpdateLocation()
        {
            UpdateLocation(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        #endregion
    }
}
