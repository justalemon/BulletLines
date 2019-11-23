using NativeUI;

namespace BulletLines.Menus
{
    /// <summary>
    /// Customized menu pool with all of the menus that we need.
    /// </summary>
    public class MainPool : MenuPool
    {
        /// <summary>
        /// The menu with configuration values.
        /// </summary>
        public ConfigMenu Menu { get; } = new ConfigMenu();

        public MainPool() : base()
        {
            // Add the menu to our pool
            Add(Menu);
        }
    }
}
