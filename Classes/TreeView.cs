using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class TreeViewBuilder
    {
        public static ImageList ImageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit };
        public static List<KeyValuePair<string,string>> ExtensionList = new List<KeyValuePair<string, string>>();

        public static void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection nodes)
        {
            string path = directoryInfo.FullName;
            if (Directory.Exists(path))
                path += ".folder";
            // Set icon
            TreeNode curNode = nodes.Add(directoryInfo.FullName, directoryInfo.Name, GetIcon(path), GetIcon(path));
            curNode.TreeView.ImageList = ImageList;
            // Add subdirectories
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
                BuildTree(subdir, curNode.Nodes);
            // Add files
            foreach (FileInfo file in directoryInfo.GetFiles())
                curNode.Nodes.Add(file.FullName, file.Name, GetIcon(file.FullName), GetIcon(file.FullName));
        }

        public static void SetIcon(string imgPath, string extensions)
        {
            ExtensionList.Add(new KeyValuePair<string, string>(imgPath, extensions));
            ImageList.Images.Add(Image.FromFile(imgPath));
        }

        private static int GetIcon(string ext)
        {
            ext = Path.GetExtension(ext).ToLower();
            for (int i = 0; i < ExtensionList.Count; i++)
            {
                foreach (var splitExt in ExtensionList[i].Value.Split(' '))
                if (splitExt.ToLower().Trim().Equals(ext))
                    return i;
            }
            return 0;
        }
    }

    public static class TreeViewExtensions
    {
        public static List<string> GetExpansionState(this TreeNodeCollection nodes)
        {
            return nodes.Descendants()
                        .Where(n => n.IsExpanded)
                        .Select(n => n.FullPath)
                        .ToList();
        }

        public static void SetExpansionState(this TreeNodeCollection nodes, List<string> savedExpansionState)
        {
            foreach (var node in nodes.Descendants()
                                      .Where(n => savedExpansionState.Contains(n.FullPath)))
            {
                node.Expand();
            }
        }

        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }
}
