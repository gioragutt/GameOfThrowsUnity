﻿using UnityEngine;

namespace Assets.Scripts
{
    internal class NetworkPlayerMovement : PlayerMovement
    {
        protected override bool IsLocal
        {
            get { return false; }
        }

        protected override void UpdateInput()
        {
            UpdateInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}