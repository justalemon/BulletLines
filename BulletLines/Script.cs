using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletLines
{
    public class BulletLines : Script
    {
        public BulletLines()
        {
            // Add all of our events
            Tick += BulletLines_Tick;
        }

        private void BulletLines_Tick(object sender, EventArgs e)
        {
            // Disable the on screen reticle
            UI.HideHudComponentThisFrame(HudComponent.Reticle);
        }
    }
}
