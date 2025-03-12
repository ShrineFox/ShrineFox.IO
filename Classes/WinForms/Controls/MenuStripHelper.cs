using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class MenuStripHelper
    {
        public static List<Tuple<string,string>> GetMenuStripIconPairs(string txtPath)
        {
            string path = Path.GetFullPath(txtPath);
            List<Tuple<string, string>> pairs = new List<Tuple<string, string>>();
            
            if (File.Exists(path))
                foreach (var line in File.ReadAllLines(Path.GetFullPath(path)))
                    pairs.Add(new Tuple<string, string>(line.Split(' ')[0], line.Split(' ')[1]));

            return pairs;
        }

        public static void SetMenuStripIcons(List<Tuple<string, string>> iconNames, Form form)
        {
            // Apply to Child Context Menu Strips
            foreach (System.Windows.Forms.ContextMenuStrip menuStrip in form.FlattenChildren<System.Windows.Forms.ContextMenuStrip>())
                ApplyIconsFromList(menuStrip.Items, iconNames);

            // Apply to Child Menu Strip Items
            foreach (System.Windows.Forms.MenuStrip menuStrip in form.FlattenChildren<System.Windows.Forms.MenuStrip>())
                ApplyIconsFromList(menuStrip.Items, iconNames);
        }

        private static void ApplyIconsFromList(ToolStripItemCollection items, List<Tuple<string, string>> menuStripIcons)
        {
            foreach (dynamic tsmi in items)
            {
                if (tsmi.GetType() == typeof(ToolStripMenuItem))
                {
                    // Apply context menu icon
                    if (menuStripIcons.Any(x => x.Item1 == tsmi.Name))
                        ApplyIconFromFile(tsmi, menuStripIcons);

                    // Apply drop down menu icon
                    foreach (dynamic item in tsmi.DropDownItems)
                    {
                        if (menuStripIcons.Any(x => x.Item1 == item.Name))
                            ApplyIconFromFile(item, menuStripIcons);
                    }
                }
            }
        }

        private static void ApplyIconFromFile(dynamic tsmi, List<Tuple<string, string>> menuStripIcons)
        {
            string iconPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    $"Dependencies\\Icons\\{menuStripIcons.Single(x => x.Item1 == tsmi.Name).Item2}.png");
            if (!File.Exists(iconPath))
                ExtractIconFrom7z(iconPath);

            tsmi.Image = Image.FromFile(iconPath);
            tsmi.ImageScaling = ToolStripItemImageScaling.None;
        }

        public static void ExtractIconFrom7z(string iconPath)
        {
            string exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string icon7zFile = Path.Combine(exeFolder, "Dependencies\\Icons.7z");
            string exe7zPath = Path.Combine(exeFolder, "Dependencies\\7z\\7z.exe");

            if (File.Exists(icon7zFile) && File.Exists(exe7zPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(iconPath));
                Exe.Run(exe7zPath, $"e \"{icon7zFile}\" -o\"{Path.GetDirectoryName(iconPath)}\" {Path.GetFileName(iconPath)}");
                using (FileSys.WaitForFile(iconPath)) { }
            }
        }
    }
}
