/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using System.Diagnostics;
namespace org.systemsbiology.util
{

    /// <summary> Implements a class registry for a given interface (that must itself
    /// extend the {@link IAliasableClass} marker interface).  This registry
    /// is capable of searching the entire java CLASSPATH for all classes
    /// that implement the specified interface.  A hash of instances of
    /// the given interface is also stored, so that an instance of any class
    /// implementing the interface, can be retrieved by referring to the
    /// &quot;class alias&quot;.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public class ClassRegistry
    {

        private System.Collections.Hashtable Instances
        {
            get
            {
                return (mInstances);
            }

            /*========================================*
            * accessor/mutator methods
            *========================================*/

            set
            {
                mInstances = value;
            }

        }
        private System.Type Interface
        {
            get
            {
                return (mInterface);
            }

            set
            {
                mInterface = value;
            }

        }

        private System.Collections.Hashtable Registry
        {
            get
            {
                return (mRegistry);
            }

            set
            {
                mRegistry = value;
            }

        }
        /// <summary> Return a Set containing copies of all of the
        /// aliases (as strings) for objects implementing
        /// the interface that was passed to the ClassRegistry
        /// constructor.
        ///
        /// </summary>
        /// <returns> a Set containing copies of all of the
        /// aliases (as strings) for objects implementing
        /// the interface that was passed to the ClassRegistry
        /// constructor.
        /// </returns>
        virtual public SupportClass.SetSupport RegistryAliasesCopy
        {
            get
            {

                SupportClass.HashSetSupport newRegistryAliases = new SupportClass.HashSetSupport();
                //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
                SupportClass.SetSupport registryAliases = new SupportClass.HashSetSupport(Registry.Keys);
                System.Collections.IEnumerator aliasesIter = registryAliases.GetEnumerator();

                while (aliasesIter.MoveNext())
                {

                    System.String alias = (System.String) aliasesIter.Current;
                    newRegistryAliases.Add(alias);
                }
                return (newRegistryAliases);
            }

        }
        /*========================================*
        * constants
        *========================================*/
        private const System.String FIELD_NAME_CLASS_ALIAS = "CLASS_ALIAS";
        private const System.String MANIFEST_DIR_NAME = "META-INF";

        private static SupportClass.HashSetSupport sAliasableClasses;
        private const System.String PACKAGE_ROOT = "org.systemsbiology";

        /*========================================*
        * member data
        *========================================*/
        private System.Type mInterface;

        private System.Collections.Hashtable mRegistry;

        private System.Collections.Hashtable mInstances;

        /*========================================*
        * initialization methods
        *========================================*/
        /*========================================*
        * constructors
        *========================================*/
        /// <summary> Create a ClassRegistry instance.  The
        /// [code]pInterface[/code] argument must specify
        /// an interface that extends the [code]IAliasableClass[/code]
        /// interface.  Lets assume the interface is called
        /// &quot;[code]IFoo[/code]&quot;.  The class registry
        /// instance (when you call [code]buildRegistry[/code])
        /// will build a list of all objects in the classpath
        /// that implement the [code]IFoo[/code] interface, and
        /// that contain the [code]CLASS_ALIAS[/code] public static
        /// String field.
        /// </summary>
        public ClassRegistry(System.Type pInterface)
        {
            checkInterface(pInterface);
            Interface = pInterface;

            Registry = new System.Collections.Hashtable();

            Instances = new System.Collections.Hashtable();
        }

        /*========================================*
        * private methods
        *========================================*/

        private void  registerClassIfImplementingInterface(System.String pClassName, System.Type pInterface, System.Collections.Hashtable pRegistry)
        {
/*J
            System.Type theClass = null;
            try
            {
                //UPGRADE_ISSUE: Method 'java.lang.ClassLoader.loadClass' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
                //UPGRADE_ISSUE: Method 'java.lang.Class.getClassLoader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassgetClassLoader'"
                theClass = GetType().getClassLoader().loadClass(pClassName);
            }
            catch (System.ApplicationException)
            {
                // apparently this error message happens some times, not clear why; just ignore any
                // class that has no class definition found, but log an error message
                System.Console.Error.WriteLine("class definition not found: " + pClassName);
                return;
            }
            //UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
            catch (System.Exception)
            {
                System.Console.Error.WriteLine("class file is not a valid class: " + pClassName);
                return;
            }

            if (theClass.IsInterface)
            {
                // interfaces are to be skipped, because they cannot implement interfaces
                return ;
            }

            if (pInterface.IsAssignableFrom(theClass))
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Class.getName' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                System.String className = theClass.FullName;

                System.Reflection.FieldInfo aliasField = null;
                try
                {
                    //UPGRADE_TODO: The differences in the expected value  of parameters for method 'java.lang.Class.getDeclaredField'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                    aliasField = theClass.GetField(FIELD_NAME_CLASS_ALIAS, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static);
                }
                catch (System.FieldAccessException e)
                {
                    System.Console.Error.WriteLine(FIELD_NAME_CLASS_ALIAS + " field does not exist in class: " + className);
                    return ;
                }

                System.String classAlias = null;

                try
                {
                    classAlias = ((System.String) aliasField.GetValue(null));
                }
                catch (System.UnauthorizedAccessException e)
                {
                    System.Console.Error.WriteLine(FIELD_NAME_CLASS_ALIAS + " field is not public in class: " + className);
                    return ;
                }

                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                System.String foundClassName = (System.String) pRegistry[classAlias];
                if (null != foundClassName && !foundClassName.Equals(className))
                {
                    System.Console.Error.WriteLine("two classes are found to have the same class alias \"" + classAlias + "\"; first class is: \"" + foundClassName + "\"; second class is \"" + className + "\"; ignoring second class");
                }
                pRegistry[classAlias] = className;
            }
*/
            throw new NotImplementedException();
        }

        private bool classImplementsInterface(System.String pClassName, System.Type pInterface)
        {
            bool retVal = false;

            System.Type theClass = null;
            try
            {
                //UPGRADE_ISSUE: Method 'java.lang.ClassLoader.loadClass' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
                //UPGRADE_ISSUE: Method 'java.lang.Class.getClassLoader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassgetClassLoader'"
                theClass = GetType().getClassLoader().loadClass(pClassName);
                if (!theClass.IsInterface && pInterface.IsAssignableFrom(theClass))
                {
                    retVal = true;
                }
            }
            //UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
            catch (System.Exception e)
            {
                System.Console.Error.WriteLine("warning:  there is a problem with class file \"" + pClassName + "\"");
            }

            return retVal;
        }


        private void  searchForClassesImplementingInterface(System.String pPackageName, SupportClass.HashSetSupport pPackagesAlreadySearched, System.Type pInterface, SupportClass.HashSetSupport pClassesImplementingInterface)
        {
            System.String resourceName = pPackageName.Replace('.', '/');
            if (!resourceName.StartsWith("/"))
            {
                resourceName = "/" + resourceName;
            }
            if (resourceName.EndsWith("/"))
            {
                resourceName = resourceName.Substring(0, (resourceName.Length - 1) - (0));
            }
            Debug.Assert(!resourceName.endsWith('/'), "resource name ended with slash: " + resourceName);

            //UPGRADE_TODO: Method 'java.lang.Class.getResource' was converted to 'System.Uri' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangClassgetResource_javalangString'"
            System.Uri url = new System.Uri(System.IO.Path.GetFullPath(resourceName));
            //UPGRADE_ISSUE: Class 'java.lang.ClassLoader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
            //UPGRADE_ISSUE: Method 'java.lang.ClassLoader.getSystemClassLoader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
            ClassLoader systemClassLoader = ClassLoader.getSystemClassLoader();
            if (null == url)
            {
                //UPGRADE_TODO: Method 'java.lang.Class.getResource' was converted to 'System.Uri' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangClassgetResource_javalangString'"
                url = new System.Uri(System.IO.Path.GetFullPath(resourceName));
                if (null == url)
                {
                    //UPGRADE_ISSUE: Method 'java.lang.ClassLoader.getResource' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
                    url = systemClassLoader.getResource(resourceName);
                }
            }
            if (null != url)
            {
                System.IO.FileInfo directory = new System.IO.FileInfo(url.PathAndQuery);
                System.String directoryName = directory.FullName;
                bool tmpBool;
                if (System.IO.File.Exists(directory.FullName))
                    tmpBool = true;
                else
                    tmpBool = System.IO.Directory.Exists(directory.FullName);
                if (tmpBool)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.io.File.list' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    System.String[] files = System.IO.Directory.GetFileSystemEntries(directory.FullName);
                    int numFiles = files.Length;
                    for (int fileCtr = 0; fileCtr < numFiles; ++fileCtr)
                    {
                        System.String fileName = files[fileCtr];
                        if (fileName.Equals("CVS"))
                        {
                            continue;
                        }

                        System.String fullFileName = directoryName + "/" + fileName;
                        System.IO.FileInfo subFile = new System.IO.FileInfo(fullFileName);
                        if (System.IO.Directory.Exists(subFile.FullName))
                        {
                            // this entry is a package
                            System.String subPackageResourceName = resourceName + "/" + fileName;
                            System.String subPackageName = (subPackageResourceName.Substring(1, (subPackageResourceName.Length) - (1))).Replace('/', '.');

                            if (pPackagesAlreadySearched.Contains(subPackageName))
                            {
                                continue;
                            }
                            pPackagesAlreadySearched.Add(subPackageName);

                            searchForClassesImplementingInterface(subPackageName, pPackagesAlreadySearched, pInterface, pClassesImplementingInterface);
                        }


                        if (fileName.EndsWith(".class"))
                        {
                            // it is a class file; remove the ".class" extension to get the class name
                            System.String packageName = null;
                            if (resourceName.StartsWith("/"))
                            {
                                packageName = resourceName.Substring(1, (resourceName.Length) - (1));
                            }
                            else
                            {
                                packageName = resourceName;
                            }
                            packageName = packageName.Replace('/', '.');
                            System.String className = packageName + "." + fileName.Substring(0, (fileName.Length - 6) - (0));
                            if (!pClassesImplementingInterface.Contains(className) && classImplementsInterface(className, pInterface))
                            {
                                pClassesImplementingInterface.Add(className);
                            }
                        }
                    }
                }
                else
                {
                    // must be a jar file
                    System.Net.HttpWebRequest uconn = (System.Net.HttpWebRequest) System.Net.WebRequest.Create(url);
                    //UPGRADE_ISSUE: Class 'java.net.JarURLConnection' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javanetJarURLConnection'"
                    if (uconn is JarURLConnection)
                    {
                        //UPGRADE_ISSUE: Class 'java.net.JarURLConnection' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javanetJarURLConnection'"
                        JarURLConnection conn = (JarURLConnection) uconn;
                        //UPGRADE_ISSUE: Method 'java.net.JarURLConnection.getEntryName' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javanetJarURLConnection'"
                        System.String starts = conn.getEntryName();
                        //UPGRADE_ISSUE: Class 'java.util.jar.JarFile' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautiljarJarFile'"
                        //UPGRADE_ISSUE: Method 'java.net.JarURLConnection.getJarFile' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javanetJarURLConnection'"
                        JarFile jfile = conn.getJarFile();
                        //UPGRADE_ISSUE: Method 'java.util.jar.JarFile.entries' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautiljarJarFile'"
                        System.Collections.IEnumerator e = jfile.entries();
                        //UPGRADE_TODO: Method 'java.util.Enumeration.hasMoreElements' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationhasMoreElements'"
                        while (e.MoveNext())
                        {
                            //UPGRADE_ISSUE: Class 'java.util.zip.ZipEntry' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipEntry'"
                            //UPGRADE_TODO: Method 'java.util.Enumeration.nextElement' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationnextElement'"
                            ZipEntry entry = (ZipEntry) e.Current;
                            //UPGRADE_ISSUE: Method 'java.util.zip.ZipEntry.getName' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipEntry'"
                            System.String entryName = entry.getName();
                            if (entryName.EndsWith("/"))
                            {
                                // this entry is a directory
                                if (entryName.Equals(MANIFEST_DIR_NAME + "/"))
                                {
                                    continue;
                                }

                                System.String subPackageName = entryName.Replace('/', '.');
                                if (pPackagesAlreadySearched.Contains(subPackageName))
                                {
                                    continue;
                                }
                                pPackagesAlreadySearched.Add(subPackageName);

                                searchForClassesImplementingInterface(subPackageName, pPackagesAlreadySearched, pInterface, pClassesImplementingInterface);
                            }
                            if (entryName.StartsWith(starts) && (entryName.LastIndexOf('/') <= starts.Length) && entryName.EndsWith(".class"))
                            {
                                System.String classname = entryName.Substring(0, (entryName.Length - 6) - (0));
                                if (classname.StartsWith("/"))
                                    classname = classname.Substring(1);
                                classname = classname.Replace('/', '.');
                                if (!pClassesImplementingInterface.Contains(classname) && classImplementsInterface(classname, pInterface))
                                {
                                    pClassesImplementingInterface.Add(classname);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void  searchForClassesImplementingInterface(SupportClass.HashSetSupport pPackagesAlreadySearched, System.Type pInterface, SupportClass.HashSetSupport pClassesImplementingInterface)
        {
            // get list of all packages known to the JRE
            //UPGRADE_ISSUE: Class 'java.lang.Package' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangPackage'"
            //UPGRADE_ISSUE: Method 'java.lang.Package.getPackages' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangPackage'"
            Package[] packages = Package.getPackages();
            int numPackages = packages.Length;
            for (int packageCtr = 0; packageCtr < numPackages; ++packageCtr)
            {
                //UPGRADE_ISSUE: Class 'java.lang.Package' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangPackage'"
                Package thePackage = packages[packageCtr];
                //UPGRADE_ISSUE: Method 'java.lang.Package.getName' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangPackage'"
                System.String packageName = thePackage.getName();
                searchForClassesImplementingInterface(packageName, pPackagesAlreadySearched, pInterface, pClassesImplementingInterface);
            }
        }

        private void  checkInterface(System.Type pInterface)
        {
            if (!pInterface.IsInterface)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Class.getName' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                throw new System.ArgumentException("class argument is not an interface: " + pInterface.FullName);
            }
        }

        /*========================================*
        * protected methods
        *========================================*/

        /*========================================*
        * public methods
        *========================================*/

        /// <summary> Searches the classpath for all classes implementing the interface
        /// that was specified in the constructor.  This method will take a while
        /// to complete, because it is searching the entire classpath.
        /// </summary>
        public virtual void  buildRegistry()
        {
            if (null == sAliasableClasses)
            {

                sAliasableClasses = new SupportClass.HashSetSupport();
                System.Type aliasableClassesInterfaceClass = typeof(IAliasableClass);

                SupportClass.HashSetSupport packagesAlreadySearched = new SupportClass.HashSetSupport();
                searchForClassesImplementingInterface(packagesAlreadySearched, aliasableClassesInterfaceClass, sAliasableClasses);
                searchForClassesImplementingInterface(PACKAGE_ROOT, packagesAlreadySearched, aliasableClassesInterfaceClass, sAliasableClasses);
            }

            System.Collections.IEnumerator classNamesIter = sAliasableClasses.GetEnumerator();

            System.Collections.Hashtable registry = Registry;
            System.Type targetInterface = Interface;

            while (classNamesIter.MoveNext())
            {

                System.String className = (System.String) classNamesIter.Current;
                registerClassIfImplementingInterface(className, targetInterface, registry);
            }
        }

        /// <summary> Returns the Class object associated with the specified
        /// class alias (a string identifier that uniquely identifies
        /// a particular implementation of the interface that was passed
        /// to the constructor for this ClassRegistry object).  If no
        /// class is found corresponding to the specified alias, an
        /// exception is thrown.
        ///
        /// </summary>
        /// <param name="pClassAlias">the alias of the class that is to be returned
        ///
        /// </param>
        /// <returns> the Class object associated with the specified
        /// class alias
        /// </returns>
        public virtual System.Type getClass(System.String pClassAlias)
        {

            System.Collections.Hashtable registry = Registry;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            System.String className = (System.String) registry[pClassAlias];
            if (null == className)
            {
                throw new DataNotFoundException("unable to locate class for alias: " + pClassAlias);
            }
            System.Type retClass = null;
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                retClass = System.Type.GetType(className);
            }
            //UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
            catch (System.Exception e)
            {
                Debug.Assert(false, "class not found for classname: " + className);
            }
            return (retClass);
        }

        /// <summary> Returns an instance of the class corresponding to the
        /// specified class alias [code]pClassAlias[/code].  This
        /// object will be an instance of a class that implements the
        /// interface that was passed to the [code]ClassRegistry[/code]
        /// constructor.  If no such instance exists, but the class
        /// corresponding to the alias is known, an instance will be
        /// created and the reference will be stored and returned.
        ///
        /// </summary>
        /// <param name="pClassAlias">the alias of the class that is to be returned
        ///
        /// </param>
        /// <returns> an instance of the class corresponding to the specified
        /// class alias [code]pClassAlias[/code]
        /// </returns>
        public virtual System.Object getInstance(System.String pClassAlias)
        {

            System.Collections.Hashtable registry = Registry;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            System.String buildClassName = (System.String) registry[pClassAlias];

            System.Type interfaceClass = Interface;

            if (null == buildClassName)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Class.getName' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                throw new DataNotFoundException("unable to find class implementing interface \"" + interfaceClass.FullName + "\" with alias \"" + pClassAlias + "\"");
            }

            System.Type buildClass = null;
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                buildClass = System.Type.GetType(buildClassName);
            }
            //UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
            catch (System.Exception e)
            {
                throw new DataNotFoundException("unable to find class " + buildClassName, e);
            }

            Debug.Assert(interfaceClass.isAssignableFrom(buildClass), "error in class registry; interface class " + interfaceClass.getName() + " is not assignable from build class: " + buildClassName);


            System.Collections.Hashtable instances = Instances;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            System.Object instance = instances[buildClassName];
            if (null == instance)
            {
                try
                {
                    //UPGRADE_TODO: Method 'java.lang.Class.newInstance' was converted to 'System.Activator.CreateInstance' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangClassnewInstance'"
                    instance = System.Activator.CreateInstance(buildClass);
                }
                catch (System.Exception e)
                {
                    throw new DataNotFoundException("unable to instantiate class " + buildClassName, e);
                }
                instances[buildClassName] = instance;
            }
*/
            if (!instances.ContainsKey(buildClassName))
            {
                try
                {
                    //UPGRADE_TODO: Method 'java.lang.Class.newInstance' was converted to 'System.Activator.CreateInstance' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangClassnewInstance'"
                    instance = System.Activator.CreateInstance(buildClass);
                }
                catch (System.Exception e)
                {
                    throw new DataNotFoundException("unable to instantiate class " + buildClassName, e);
                }

                instances[buildClassName] = instance;
            }

            return (instance);
        }

        /// <summary> Print out a summary of the contents of the
        /// class registry, to the specified PrintStream
        /// [code]pString[/code].
        /// </summary>
        //UPGRADE_ISSUE: Class hierarchy differences between 'java.io.PrintStream' and 'System.IO.StreamWriter' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
        public virtual void  printRegistry(System.IO.StreamWriter pStream)
        {

            System.Collections.Hashtable registry = Registry;
            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            SupportClass.SetSupport aliases = new SupportClass.HashSetSupport(registry.Keys);
            System.Collections.IEnumerator aliasesIter = aliases.GetEnumerator();

            while (aliasesIter.MoveNext())
            {

                System.String alias = (System.String) aliasesIter.Current;
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                System.String className = (System.String) registry[alias];
                pStream.WriteLine(alias + " -> " + className);
            }
        }

        public virtual void  clearInstances()
        {

            System.Collections.Hashtable instances = Instances;
            instances.Clear();
        }

        /// <summary> Test method for this class</summary>
        [STAThread]
        public static void  Main(System.String[] pArgs)
        {
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                ClassRegistry classRegistry = new ClassRegistry(System.Type.GetType(pArgs[0]));
                classRegistry.buildRegistry();
                System.IO.StreamWriter temp_writer;
                temp_writer = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), System.Console.Out.Encoding);
                temp_writer.AutoFlush = true;
                classRegistry.printRegistry(temp_writer);
            }
            catch (System.Exception e)
            {
                SupportClass.WriteStackTrace(e, System.Console.Error);
            }
        }
        static ClassRegistry()
        {
            {
                sAliasableClasses = null;
            }
        }
    }
}