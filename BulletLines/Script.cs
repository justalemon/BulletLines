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
        private Ped[] WorldPeds = new Ped[0];
        private int NextUpdate = 0;

        public BulletLines()
        {
            // Add all of our events
            Tick += BulletLines_Tick;
            // Set the next time wher we should update the mod
            NextUpdate = Game.GameTime + 1000;
        }

        private void BulletLines_Tick(object sender, EventArgs e)
        {
            // Disable the on screen reticle
            UI.HideHudComponentThisFrame(HudComponent.Reticle);

            // If the next update time is higher or equal than the current time
            if (Game.GameTime >= NextUpdate)
            {
                WorldPeds = World.GetAllPeds();
                NextUpdate = Game.GameTime + 1000;
            }

            // For every ped in the world
            foreach (Ped ped in WorldPeds)
            {
                // Try to get the weapon of the entity
                Entity weapon = ped.Weapons.CurrentWeaponObject;

                // If the weapon doesn't exists, continue to the next iteration
                if (weapon == null)
                {
                    continue;
                }

                // Save the origin and destination for our positions
                Vector3 origin = weapon.Position;
                Vector3 destination;

                // If the ped is the player and is aiming, shooting or pressing the fire button
                if (ped.IsPlayer && (Game.Player.IsAiming || Game.Player.Character.IsShooting || Game.IsControlPressed(0, Control.Attack)))
                {
                    // Try to get where the weapon is aiming at
                    RaycastResult result = World.GetCrosshairCoordinates();
                    // If the weapon is aiming at nowhere, continue
                    if (!result.DitHitAnything)
                    {
                        continue;
                    }
                    // If is being aimed at something, 
                    destination = result.HitCoords;
                }
                // Otherwise if the ped is in combat against the player and has a weapon
                else if (ped.IsInCombatAgainst(Game.Player.Character) && ped.Weapons.Current.Hash != WeaponHash.Unarmed)
                {
                    // Otherwise, set the player head as the destination
                    destination = Game.Player.Character.GetBoneCoord(Bone.SKEL_Head);
                }
                // If we got here, continue to the next iteration
                else
                {
                    continue;
                }

                // Finally, draw a line between those points and a dot at the end of it
                Function.Call(Hash.DRAW_LINE, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z, LineColor.R, LineColor.G, LineColor.B, LineColor.A);
                World.DrawMarker(MarkerType.DebugSphere, destination, Vector3.Zero, Vector3.Zero, new Vector3(0.017f, 0.017f, 0.017f), LineColor);
            }
        }
    }
}
