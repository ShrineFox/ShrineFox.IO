using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class TreeViewBuilder
    {
        public static ImageList ImageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit };
        public static List<Tuple<string,string>> ExtensionList = new List<Tuple<string, string>>();

        public static void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection nodes)
        {
            string path = directoryInfo.FullName;
            if (Directory.Exists(path))
                path += ".folder";
            // Set icon
            TreeNode curNode = nodes.Add(directoryInfo.FullName, directoryInfo.Name, GetIconIndex(path), GetIconIndex(path));
            curNode.TreeView.ImageList = ImageList;
            // Add subdirectories
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
                BuildTree(subdir, curNode.Nodes);
            // Add files
            foreach (FileInfo file in directoryInfo.GetFiles())
                curNode.Nodes.Add(file.FullName, file.Name, GetIconIndex(file.FullName), GetIconIndex(file.FullName));
        }

        public static int GetIconIndex(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (!TreeViewBuilder.ExtensionList.Any(x => x.Item1.Equals(ext)))
                return TreeViewBuilder.ExtensionList.Count - 1;
            return TreeViewBuilder.ExtensionList.IndexOf(TreeViewBuilder.ExtensionList.First(x => x.Item1.Equals(ext)));
        }

        public static void BuildImageList(string txtPath)
        {
            // Create new image and extension lists
            ImageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit };
            ExtensionList = new List<Tuple<string, string>>();

            // Extract icons required for image list
            string iconDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "Dependencies\\Icons");
            if (File.Exists(txtPath))
            {
                foreach (var line in File.ReadAllLines(txtPath))
                {
                    ExtensionList.Add(new Tuple<string, string>(line.Split(' ')[0], line.Split(' ')[1]));

                    string iconPath = Path.Combine(iconDir, line.Split(' ')[1] + ".png");
                    if (!File.Exists(iconPath))
                        MenuStripHelper.ExtractIconFrom7z(iconPath);
                }
            }

            // Build treeview image list
            foreach (var ext in ExtensionList)
                ImageList.Images.Add(Image.FromFile(Path.Combine(iconDir, ext.Item2 + ".png")));
        }
    }

    // Lazy Loading of Subfolder contents: https://stackoverflow.com/a/39686042
    public class FolderFileNode : TreeNode
    {
        private readonly string _path;

        private readonly bool _isFile;

        public FolderFileNode(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            Text = Path.GetFileName(path);
            Tag = path;
            Name = Path.GetFileName(path);
            _isFile = File.Exists(path);
            _path = path;

            if (!_isFile && Directory.EnumerateFileSystemEntries(_path).Any())
            {
                Nodes.Add(new TreeNode());
            }
            SetIcon();
        }

        public void SetIcon()
        {
            ImageIndex = _isFile ? ImageIndex = TreeViewBuilder.GetIconIndex(this.Name) : IsExpanded ? TreeViewBuilder.GetIconIndex(".folder_open") : TreeViewBuilder.GetIconIndex(".folder");
            SelectedImageIndex = _isFile ? ImageIndex = TreeViewBuilder.GetIconIndex(this.Name) : IsExpanded ? TreeViewBuilder.GetIconIndex(".folder_open") : TreeViewBuilder.GetIconIndex(".folder");
        }

        private IEnumerable<string> _children;
        public void LoadNodes()
        {
            if (_path == null)
                return;

            if (!_isFile && _children == null)
            {
                // _children = Directory.EnumerateFileSystemEntries(_path);
                // Or Add Directories first
                _children = Directory.EnumerateDirectories(_path).ToList();
                ((List<string>)_children).AddRange(Directory.EnumerateFiles(_path));

                //Theres one added in the constructor to indicate it has children 
                Nodes.Clear();

                Nodes.AddRange(
                    _children.Select(x =>
                        (TreeNode)new FolderFileNode(x))
                        .ToArray());
            }
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
