﻿/******************************************************************************
* Filename    = ParsedDLLFile.cs
* 
* Author      = Nikhitha Atyam, Yukta Salunkhe
* 
* Product     = Analyzer
* 
* Project     = Analyzer
*
* Description = Parses a single dll file and creates parsed class,interfaces objects
*****************************************************************************/

using Mono.Cecil;
using System.Reflection;


namespace Analyzer.Parsing
{
    /// <summary>
    /// Parses a single dll file and creates parsed class,interfaces objects
    /// </summary>
    public class ParsedDLLFile
    {
        private string _dllPath { get; }
        public string DLLFileName { get; }      

        // System.Reflection parsed class, interface object lists
        public List<ParsedClass> classObjList = new();
        public List<ParsedInterface> interfaceObjList = new();

        // MONO.CECIL objects lists (considering single module assembly)
        public List<ParsedClassMonoCecil> classObjListMC = new();

        /// <summary>
        /// Parses the DLL file using Reflection & Mono.Cecil
        /// </summary>
        /// <param name="path">path of DLL file</param>
        public ParsedDLLFile(string path) 
        {
            _dllPath = path;
            DLLFileName = Path.GetFileName(path);
            
            ReflectionParsingDLL();
            MonoCecilParsingDLL();
        }


        /// <summary>
        /// Parsing the DLL using System.Reflection
        /// </summary>
        private void ReflectionParsingDLL()
        {
            Assembly assembly = Assembly.Load( File.ReadAllBytes(_dllPath) );

            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();

                // Finding class and interface types in the assembly
                foreach (Type type in types)
                {
                    // Ignoring the types from System & Microsoft packages
                    if(type.Namespace != null)
                    {
                        if (type.Namespace.StartsWith("System") || type.Namespace.StartsWith("Microsoft") || type.Namespace.StartsWith("Mono.Cecil"))
                        {
                            continue;
                        }
                    }
                    
                    if(type.IsClass && type.FullName != "<Module>")
                    {
                        // To avoid structures and delegates
                        if (!type.IsValueType && !typeof(Delegate).IsAssignableFrom(type))
                        {
                            ParsedClass classObj = new(type);
                            classObjList.Add( classObj );
                        }
                    }
                    else if (type.IsInterface)
                    {
                        ParsedInterface interfaceObj = new(type);
                        interfaceObjList.Add( interfaceObj );
                    }
                }
            }
        }


        /// <summary>
        /// Parsing the DLL using Mono.Cecil
        /// </summary>
        private void MonoCecilParsingDLL()
        {
            AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(_dllPath);

            if (assemblyDef != null)
            {
                // considering only single module programs
                ModuleDefinition mainModule = assemblyDef.MainModule;

                if (mainModule != null)
                {
                    // Finding class and interface types in the Main module
                    foreach (TypeDefinition type in mainModule.Types)
                    {
                        if (type.Namespace != null)
                        {
                            // Ignoring the types from System & Microsoft packages
                            if (type.Namespace.StartsWith("System") || type.Namespace.StartsWith("Microsoft") || type.Namespace.StartsWith("Mono.Cecil"))
                            {
                                continue;
                            }
                        }

                        // ignoring structures and delegates
                        if (type.IsClass && !type.IsValueType && type.BaseType?.FullName != "System.MulticastDelegate" && type.FullName != "<Module>")
                        {
                            ParsedClassMonoCecil classObj = new( type );
                            classObjListMC.Add( classObj );
                        }
                    }
                }

                // Releasing the assembly resources
                assemblyDef.Dispose();
            }
        }
    }
}
