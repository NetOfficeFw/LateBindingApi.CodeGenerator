using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NetOffice
{
    /// <summary>
    /// Encapsulate current appdomain with exception tolerant methods
    /// </summary>
    internal class CurrentAppDomain
    {
        #region Ctor

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="owner">owner core</param>
        internal CurrentAppDomain(Core owner)
        {
            if (null == owner)
                throw new ArgumentNullException("owner");
            Owner = owner;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Owner Core
        /// </summary>
        internal Core Owner { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns all loaded assemblies in current appdomain
        /// </summary>
        /// <returns>loaded assemblies</returns>
        internal Assembly[] GetAssemblies()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }
            catch
            {
                return new Assembly[0];
            }
        }

        /// <summary>
        /// Try load an assembly
        /// </summary>
        /// <param name="fileName">full qualified file path</param>
        /// <returns>Assembly instance or null</returns>
        internal Assembly Load(string fileName)
        {
            return Assembly.Load(fileName);
        }

        /// <summary>
        /// Try load an assembly
        /// </summary>
        /// <param name="name">assembly reference name</param>
        /// <returns>Assembly instance or null</returns>
        internal Assembly Load(AssemblyName name)
        {
            try
            {
                return Assembly.Load(name);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Try load an assembly
        /// </summary>
        /// <param name="fileName">full qualified name</param>
        /// <returns>Assembly instance or null</returns>
        internal Assembly LoadFile(string fileName)
        {
            try
            {
                return Assembly.LoadFile(fileName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Try load an assembly
        /// </summary>
        /// <param name="fileName">full qualified file name</param>
        /// <returns>Assembly instance or null</returns>
        internal Assembly LoadFrom(string fileName)
        {
            try
            {
                return Assembly.LoadFrom(fileName);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Trigger

        /// <summary>
        /// Occurs when the resolution of an assembly fails.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="args">A System.ResolveEventArgs that contains the event data</param>
        /// <returns>The System.Reflection.Assembly that resolves the type, assembly, or resource or null if the assembly cannot be resolved</returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                // dont care for resources
                if ((!String.IsNullOrEmpty(args.Name) && args.Name.ToLower().Trim().IndexOf(".resources") > -1))
                    return null;

                string directoryName = Owner.ThisAssembly.CodeBase.Substring(0, Owner.ThisAssembly.CodeBase.LastIndexOf("/"));
                directoryName = directoryName.Replace("/", "\\").Substring(8);
                string fileName = args.Name.Substring(0, args.Name.IndexOf(","));
                string fullFileName = System.IO.Path.Combine(directoryName, fileName + ".dll");
                if (System.IO.File.Exists(fullFileName))
                {
                    Console.WriteLine(string.Format("Try to resolve assembly {0}", args.Name));
                    Assembly assembly = Load(args.Name);
                    return assembly;
                }
                else
                {
                    Console.WriteLine(string.Format("Unable to resolve assembly {0}. The file doesnt exists in current codebase.", args.Name));
                    return null;
                }
            }
            catch (Exception exception)
            {
                Owner.Console.WriteException(exception);
                return null;
            }
        }

        #endregion
    }
}
