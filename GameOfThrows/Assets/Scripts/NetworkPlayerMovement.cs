using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class NetworkPlayerMovement : IPlayerMovement
    {
        #region Abstract Methods
        protected override void UpdateLocation()
        {
            UpdateLocation(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        #endregion
    }
}
