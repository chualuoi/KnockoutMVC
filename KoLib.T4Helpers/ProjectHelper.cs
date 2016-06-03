using System;
using System.IO;
using System.Linq;
using EnvDTE;
namespace KoLib.T4Helpers
{
    /// <summary>
    /// This class contains helper methods to manipulate visual studio project
    /// such as adding file, folder to project
    /// </summary>
    public class ProjectHelper
    {
        /// <summary>
        /// Includes the directory in project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="absolutePath">The absolute path of directory to add.</param>
        /// <returns></returns>
        public static ProjectItem IncludeDirectoryInProject(Project project, string absolutePath)
        {
            if (!Directory.Exists(absolutePath))
            {
                throw new FileNotFoundException("Directory could not be found", absolutePath);
            }
            var directoryInfo = new DirectoryInfo(absolutePath);
            var projectDir = Path.GetDirectoryName(project.FullName);
            if (projectDir == null)
            {
                return null;
            }
            if (!directoryInfo.FullName.ToLower().Contains(projectDir.ToLower()))
            {
                throw new ArgumentException("Directory to add must exists in project structure");
            }

            ProjectItem rootNode;

            //If this directory already exists in project
            if ((rootNode = FindProjectItem(project, absolutePath)) != null)
            {
                return rootNode;
            }

            //Finding the root node for this directory.
            //The root node is node already exist in solution where this directory will belong to
            while ((rootNode = FindProjectItem(project, directoryInfo.Parent.FullName)) == null)
            {
                if (string.Equals(directoryInfo.FullName, projectDir, StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                directoryInfo = directoryInfo.Parent;
            }
            return rootNode == null ? project.ProjectItems.AddFromDirectory(directoryInfo.FullName) 
                                    : rootNode.ProjectItems.AddFromDirectory(directoryInfo.FullName);
        }

        /// <summary>
        /// Determines whether the specified project is included.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the specified project is included; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIncluded(Project project, string path)
        {
            return FindProjectItem(project, path) != null;
        }

        /// <summary>
        /// Finds the project item which has specific path.
        /// We have to use this method because EnvDTE._Solution.FindProjectItem method only find project file,
        /// not folder.
        /// </summary>
        /// <param name="project">The project in which we find.</param>
        /// <param name="path">The path of project item to find.</param>
        /// <returns></returns>
        public static ProjectItem FindProjectItem(Project project, string path)
        {
            return (from ProjectItem item in project.ProjectItems select FindSubProjectItem(item, path)).FirstOrDefault(tempResult => tempResult != null);
        }

        /// <summary>
        /// Finds the sub project item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static ProjectItem FindSubProjectItem(ProjectItem item, string path)
        {
            return string.Equals(item.Properties.Item("FullPath").Value.ToString().TrimEnd('\\'), Path.GetFullPath(path).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) ? 
                    item : (from ProjectItem subItem in item.ProjectItems select FindSubProjectItem(subItem, path)).FirstOrDefault(tempResult => tempResult != null);
        }

        /// <summary>
        /// Creates the namespace structure in project.
        /// </summary>
        /// <param name="t">The type from that we will get its namespace.</param>
        /// <param name="project">The project in which we create namespace structure.</param>
        /// <param name="rootPath">The root path in project where the namespace folder will be added.</param>
        /// <param name="rootNamespaces">The root namespace.</param>
        /// <returns></returns>
        public static ProjectItem CreateNamespaceNode(Type t, Project project, string rootPath, string[] rootNamespaces)
        {
            var dirInfo = new DirectoryInfo(rootPath);
            ProjectItem rootItem = null;
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            if (FindProjectItem(project, rootPath) == null)
            {
                rootItem = IncludeDirectoryInProject(project, rootPath);
            }
            if (string.IsNullOrWhiteSpace(t.Namespace))
            {
                return rootItem;
            }
            var subNamespace = t.Namespace;
            var rootNamespace = FindMostMatchNamespace(t.Namespace, rootNamespaces);
            if (rootNamespace != null)
            {
                if (t.Namespace.StartsWith(rootNamespace))
                {
                    var index = t.Namespace.IndexOf(rootNamespace);
                    subNamespace =
                    t.Namespace.Substring(index + rootNamespace.Length);
                }
            }
            var splittedName = subNamespace.Split(new[] { '.' });

            var path = rootPath;
            ProjectItem item = null;
            foreach (var t1 in splittedName)
            {
                path = Path.Combine(path, t1);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                item = FindProjectItem(project, path) ?? IncludeDirectoryInProject(project, path);
            }
            return item ?? FindProjectItem(project, rootPath);
        }

        /// <summary>
        /// Creates the namespace structure in project.
        /// </summary>
        /// <param name="t">The type from that we will get its namespace.</param>
        /// <param name="rootPath">The root path in project where the namespace folder will be added.</param>
        /// <param name="rootNamespaces">The root namespace.</param>
        /// <returns></returns>
        public static string GetParentItem(Type t, string rootPath, string[] rootNamespaces)
        {
            if (string.IsNullOrWhiteSpace(t.Namespace))
            {
                return null;
            }
            var subNamespace = t.Namespace;
            var rootNamespace = FindMostMatchNamespace(t.Namespace, rootNamespaces);
            if (rootNamespace != null)
            {
                if (t.Namespace.StartsWith(rootNamespace))
                {
                    var index = t.Namespace.IndexOf(rootNamespace);
                    subNamespace = t.Namespace.Substring(index + rootNamespace.Length);
                }
            }
            var splittedName = subNamespace.Split(new[] { '.' });

            return splittedName.Aggregate(rootPath, Path.Combine);
        }

        /// <summary>
        /// Creates the namespace node (folder).
        /// </summary>
        /// <param name="typeNamespace">The type namespace.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="rootNamespaces">The root namespaces.</param>
        /// <returns></returns>
        public static string CreateNamespaceNode(string typeNamespace, string rootPath, string[] rootNamespaces)
        {
            var dirInfo = new DirectoryInfo(rootPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            var subNamespace = typeNamespace;
            if (string.IsNullOrWhiteSpace(typeNamespace))
            {
                return rootPath;
            }
            var rootNamespace = FindMostMatchNamespace(typeNamespace, rootNamespaces);
            if (rootNamespace != null)
            {
                if (typeNamespace.StartsWith(rootNamespace))
                {
                    var index = typeNamespace.IndexOf(rootNamespace);
                    subNamespace =
                    typeNamespace.Substring(index + rootNamespace.Length);
                }
            }
            var splittedName = subNamespace.Split(new[] { '.' });

            var path = rootPath;
            foreach (var t1 in splittedName)
            {
                path = Path.Combine(path, t1);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }

        /// <summary>
        /// Finds the most match namespace.
        /// </summary>
        /// <param name="childNamespace">The child namespace.</param>
        /// <param name="rootNamespaces">The root namespaces.</param>
        /// <returns></returns>
        public static string FindMostMatchNamespace(string childNamespace, string[] rootNamespaces)
        {
            var matches = rootNamespaces.Where(childNamespace.StartsWith).ToList();
            return matches.OrderByDescending(x => x.Length).FirstOrDefault();
        }
    }
}