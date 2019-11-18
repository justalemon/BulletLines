using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletLines
{
    public class BulletLines : Script
    {
        private Color LineColor = Color.Red;

        public BulletLines()
        {
            // Add all of our events
            Tick += BulletLines_Tick;
        }

        private void BulletLines_Tick(object sender, EventArgs e)
        {
            // Disable the on screen reticle
            UI.HideHudComponentThisFrame(HudComponent.Reticle);

            // Try to get the weapon of the player
            Entity playerWeapon = Function.Call<Entity>(Hash.GET_CURRENT_PED_WEAPON_ENTITY_INDEX, Game.Player.Character);

            // If the player is aiming and the weapon exists
            if (Game.Player.IsAiming && playerWeapon != null)
            {
                // Try to get where the weapon is aiming at
                RaycastResult result = World.GetCrosshairCoordinates();
                // If the weapon is aiming at nowhere, return
                if (!result.DitHitAnything)
                {
                    return;
                }
                // Get the origin and destination of the line
                Vector3 origin = playerWeapon.Position;
                Vector3 destination = result.HitCoords;
                // And draw a line between those points
                Function.Call(Hash.DRAW_LINE, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z, LineColor.R, LineColor.G, LineColor.B, LineColor.A);
            }
        }
    }
}
