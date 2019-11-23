using BulletLines.Config;
using BulletLines.Menus;
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
        /// <summary>
        /// The mod configuration.
        /// </summary>
        internal static readonly Configuration Config = Configuration.Load();
        /// <summary>
        /// The color of the bullet line and the end of it.
        /// </summary>
        private static Color LineColor => Color.FromArgb(Config.ColorA, Config.ColorR, Config.ColorG, Config.ColorB);
        /// <summary>
        /// THe list of peds available in the game world.
        /// </summary>
        private static Ped[] WorldPeds = new Ped[0];
        /// <summary>
        /// The time where the next set of peds should be fetched.
        /// </summary>
        private static int NextUpdate = 0;
        /// <summary>
        /// The NativeUI pool of menus.
        /// </summary>
        private static readonly MainPool Pool = new MainPool();

        /// <summary>
        /// If the player has a finger on the gun trigger (aka on the analog or digital controller trigger or mouse button).
        /// </summary>
        private static bool IsPlayerFingerOnTrigger
        {
            get
            {
                // Get the hash of the weapon
                WeaponHash hash = Game.Player.Character.Weapons.Current.Hash;
                // And get the values for the Aim and Attack controls (L2/LT and R2/RT respectively)
                float aimValue = Game.GetControlNormal(0, Control.Aim);
                float attackValue = Game.GetControlNormal(0, Control.Attack);

                // Finally, check that the player is not unarmed and either the Aim or Attack values are over zero
                return hash != WeaponHash.Unarmed && (aimValue > 0 || attackValue > 0);
            }
        }
        /// <summary>
        /// If the player is using the sniper sight while aiming the weapon.
        /// </summary>
        private static bool IsPlayerUsingSniperSights
        {
            get
            {
                // Get the type or group of weapon that the player is using
                WeaponGroup group = Game.Player.Character.Weapons.Current.Group;
                // And check if is a sniper and is aiming
                return group == WeaponGroup.Sniper && Game.GetControlNormal(0, Control.Aim) > 0;
            }
        }

        public BulletLines()
        {
            // Add all of our events
            Tick += BulletLines_Tick;
        }

        private void BulletLines_Tick(object sender, EventArgs e)
        {
            // Draw the menu pool contents
            Pool.ProcessMenus();

            // If the user wants to disable the on screen reticle and the current weapon is not a sniper being aimed, do it
            if (Config.DisableReticle && !IsPlayerUsingSniperSights)
            {
                UI.HideHudComponentThisFrame(HudComponent.Reticle);
            }

            // If the next update time is higher or equal than the current time or is zero
            if (Game.GameTime >= NextUpdate || NextUpdate == 0)
            {
                WorldPeds = World.GetAllPeds();
                NextUpdate = Game.GameTime + Config.UpdateTime;
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

                // If this is a player ped and is not aiming or shooting, continue
                if (ped.IsPlayer && !IsPlayerFingerOnTrigger)
                {
                    continue;
                }

                // If this is a player ped, is using the sniper sights and the related option is disabled, continue
                if (ped.IsPlayer && IsPlayerUsingSniperSights && !Config.LineOnSniper)
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
                RaycastResult result = ped.IsPlayer ? World.GetCrosshairCoordinates() : World.Raycast(origin, offset, IntersectOptions.Map | IntersectOptions.Mission_Entities | IntersectOptions.Objects | IntersectOptions.Peds1);
                // And set the destination to the raycast coordinates (if it did hit something) or the approximate end of the line
                Vector3 destination = result.DitHitAnything ? result.HitCoords : offset;

                // If the user wants the bullet line, draw a line between those points
                if (Config.BulletLine)
                {
                    Function.Call(Hash.DRAW_LINE, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z, LineColor.R, LineColor.G, LineColor.B, LineColor.A);
                }
                // If the user wants the end of the bullet line to be shown, draw a sphere
                if (Config.BulletLineEnd)
                {
                    World.DrawMarker(MarkerType.DebugSphere, destination, Vector3.Zero, Vector3.Zero, new Vector3(0.017f, 0.017f, 0.017f), LineColor);
                }
            }
        }
    }
}
