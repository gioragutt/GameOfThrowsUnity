using UnityEngine;

namespace Assets.Scripts.PlayerClasses
{
    public class LocalPlayerMovement : PlayerMovement
    {
        protected override bool IsLocal
        {
            get { return true; }
        }

        protected override void UpdateInput()
        {
            UpdateInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}
