using GTA;
using NativeUI;

namespace BulletLines.Menus
{
    /// <summary>
    /// Menu with the configuration options for BulletLines.
    /// </summary>
    public class ConfigMenu : UIMenu
    {
        private UIMenuSliderItem ColorR { get; } = new UIMenuSliderItem("Color - Red Value", "The Red value of the ARGB color.") { Maximum = 255, Value = BulletLines.Config.ColorR };
        private UIMenuSliderItem ColorG { get; } = new UIMenuSliderItem("Color - Green Value", "The Green value of the ARGB color.") { Maximum = 255, Value = BulletLines.Config.ColorG };
        private UIMenuSliderItem ColorB { get; } = new UIMenuSliderItem("Color - Blue Value", "The Blue value of the ARGB color.") { Maximum = 255, Value = BulletLines.Config.ColorB };
        private UIMenuSliderItem ColorA { get; } = new UIMenuSliderItem("Color - Alpha Value", "The Alpha/Transparency value of the ARGB color.") { Maximum = 255, Value = BulletLines.Config.ColorA };
        private UIMenuCheckboxItem BulletLine { get; } = new UIMenuCheckboxItem("Show Bullet Lines", BulletLines.Config.BulletLine, "If the bullet line should be shown for all peds.");
        private UIMenuCheckboxItem BulletLineEnd { get; } = new UIMenuCheckboxItem("Show End of Bullet Lines", BulletLines.Config.BulletLineEnd, "If the end of the bullet line should be marked with a dot.");
        private UIMenuCheckboxItem LineOnSniper { get; } = new UIMenuCheckboxItem("Show Bullet Line on Snipers", BulletLines.Config.LineOnSniper, "If the bullet line should be used when manually aiming with a sniper.");
        private UIMenuCheckboxItem DisableReticle { get; } = new UIMenuCheckboxItem("Disable Weapon Reticle", BulletLines.Config.DisableReticle, "If the on screen weapon reticle should be disabled when not manually aiming snipers.");
        private UIMenuItem Save { get; } = new UIMenuItem("Save", "Saves the current configuration settings.");

        public ConfigMenu() : base("BulletLines", "BulletLines")
        {
            // Add all of the items
            AddItem(ColorR);
            AddItem(ColorG);
            AddItem(ColorB);
            AddItem(ColorA);
            AddItem(BulletLine);
            AddItem(BulletLineEnd);
            AddItem(LineOnSniper);
            AddItem(DisableReticle);
            AddItem(Save);

            // And their events
            ColorR.OnSliderChanged += ColorR_SliderChanged;
            ColorG.OnSliderChanged += ColorG_SliderChanged;
            ColorB.OnSliderChanged += ColorB_SliderChanged;
            ColorA.OnSliderChanged += ColorA_SliderChanged;
            BulletLine.CheckboxEvent += BulletLine_CheckboxChanged;
            BulletLineEnd.CheckboxEvent += BulletLineEnd_CheckboxChanged;
            LineOnSniper.CheckboxEvent += LineOnSniper_CheckboxChanged;
            DisableReticle.CheckboxEvent += DisableReticle_CheckboxChanged;
            Save.Activated += Save_Activated;
        }

        public void ColorR_SliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            BulletLines.Config.ColorR = newIndex;
        }
        public void ColorG_SliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            BulletLines.Config.ColorG = newIndex;
        }
        public void ColorB_SliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            BulletLines.Config.ColorB = newIndex;
        }
        public void ColorA_SliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            BulletLines.Config.ColorA = newIndex;
        }
        public void BulletLine_CheckboxChanged(UIMenuCheckboxItem sender, bool Checked)
        {
            BulletLines.Config.BulletLine = Checked;
        }
        public void BulletLineEnd_CheckboxChanged(UIMenuCheckboxItem sender, bool Checked)
        {
            BulletLines.Config.BulletLineEnd = Checked;
        }
        public void LineOnSniper_CheckboxChanged(UIMenuCheckboxItem sender, bool Checked)
        {
            BulletLines.Config.LineOnSniper = Checked;
        }
        public void DisableReticle_CheckboxChanged(UIMenuCheckboxItem sender, bool Checked)
        {
            BulletLines.Config.DisableReticle = Checked;
        }
        public void Save_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            BulletLines.Config.Save();
            UI.Notify("BulletLines Configuration Saved!");
        }
    }
}
