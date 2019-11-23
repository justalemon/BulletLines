using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulletLines.Config
{
    /// <summary>
    /// Main mod configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The R value for the bullet line and bullet end color.
        /// </summary>
        [JsonProperty("color_r")]
        public int ColorR { get; set; } = 255;
        /// <summary>
        /// The G value for the bullet line and bullet end color.
        /// </summary>
        [JsonProperty("color_g")]
        public int ColorG { get; set; } = 0;
        /// <summary>
        /// The B value for the bullet line and bullet end color.
        /// </summary>
        [JsonProperty("color_b")]
        public int ColorB { get; set; } = 0;
        /// <summary>
        /// The A value for the bullet line and bullet end color.
        /// </summary>
        [JsonProperty("color_a")]
        public int ColorA { get; set; } = 255;
        /// <summary>
        /// If the bullet line should be shown.
        /// </summary>
        [JsonProperty("bullet_line")]
        public bool BulletLine { get; set; } = true;
        /// <summary>
        /// If a sphere should be shown where the bullet line ends.
        /// </summary>
        [JsonProperty("bullet_line_end")]
        public bool BulletLineEnd { get; set; } = false;
        /// <summary>
        /// If the bullet line should be shown when using the sniper sights.
        /// </summary>
        [JsonProperty("line_on_sniper")]
        public bool LineOnSniper { get; set; } = false;
        /// <summary>
        /// If the weapon reticle should be disabled when the manual aim is not being used with a sniper.
        /// </summary>
        [JsonProperty("disable_reticle")]
        public bool DisableReticle { get; set; } = true;

        private static string GetConfigLocation()
        {
            // Get the assembly that is calling 
            Assembly assembly = Assembly.GetCallingAssembly();
            // Get the location of the DLL that is using this function
            string location = new Uri(Path.GetDirectoryName(assembly.CodeBase)).LocalPath;
            // Then, make the path based on the assembly name and path and return it
            return Path.Combine(location, $"{assembly.GetName().Name}.json");
        }
        public static Configuration Load()
        {
            // Get the location of the configuration
            string path = GetConfigLocation();

            // Then, if the file exist
            if (File.Exists(path))
            {
                // Get the contents of the file
                string contents = File.ReadAllText(path);
                // And return the parsed contents
                return JsonConvert.DeserializeObject<Configuration>(contents);
            }
            // If not
            else
            {
                // And return it
                return Regenerate();
            }
        }
        public static Configuration Regenerate()
        {
            // Create a new configuration object
            Configuration config = new Configuration();
            // Save it
            config.Save();
            // And return it
            return config;
        }
        public void Save()
        {
            // Get the output of the serialization
            string output = JsonConvert.SerializeObject(this, Formatting.Indented) + Environment.NewLine;
            // And dump the contents of the file
            File.WriteAllText(GetConfigLocation(), output);
        }
    }
}
