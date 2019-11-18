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

                // If this is a player ped and is not aiming, shooting or attacking, continue
                if (ped.IsPlayer && (!Game.Player.IsAiming && !ped.IsShooting && !Game.IsControlPressed(0, Control.Attack)))
                {
                    continue;
                }
                
                // If the ped is not doing any of the tasks that we need
                if (!Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 4) &&    // CTaskAimGunOnFoot
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 12) &&   // CTaskAimGun
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 56) &&   // CTaskSwapWeapon
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 295) &&  // CTaskAimGunVehicleDriveBy
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 298) &&  // CTaskReloadGun
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 304) &&  // CTaskAimGunBlindFire
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 355) &&  // CTaskShootAtTarget
                    !Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, 368))    // CTaskShootOutTire
                {
                    continue;
                }

                // Get the location of the muzzle
                int index = weapon.GetBoneIndex("gun_muzzle");
                Vector3 origin = weapon.GetBoneCoord(index);

                // Calculate the offset of the weapon
                Vector3 offset = weapon.GetOffsetInWorldCoords(new Vector3(2500, 0, 0));

                // Calculate an approximate raycast from the weapon to the destination
                RaycastResult result = World.Raycast(origin, offset, IntersectOptions.Map | IntersectOptions.Mission_Entities | IntersectOptions.Objects | IntersectOptions.Peds1);
                // And set the destination to the raycast coordinates (if it did hit something) or the approximate end of the line
                Vector3 destination = result.DitHitAnything ? result.HitCoords : offset;

                // Finally, draw a line between those points and a dot at the end of it
                Function.Call(Hash.DRAW_LINE, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z, LineColor.R, LineColor.G, LineColor.B, LineColor.A);
                World.DrawMarker(MarkerType.DebugSphere, destination, Vector3.Zero, Vector3.Zero, new Vector3(0.017f, 0.017f, 0.017f), LineColor);
            }
        }
    }
}
