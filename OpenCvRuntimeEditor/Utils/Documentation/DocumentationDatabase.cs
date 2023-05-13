namespace OpenCvRuntimeEditor.Utils.Documentation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Settings;

    public class DocumentationDatabase
    {
        private static readonly Dictionary<Assembly, XmlDocument> _assemblyDocCache = new Dictionary<Assembly, XmlDocument>();

        public static XmlElement GetXmlDocumentation(object item)
        {
            if (item == null)
                return null;

            if (!GeneralSettings.Instance.LoadDocumentation)
                return null;

            if (item is MemberInfo member)
                return GetDocFromName(member.Module.Assembly, GetMemberName(member));

            return null;
        }

        private static string GetMemberName(MemberInfo member)
        {
            char prefixCode;

            var memberName = member is Type
                ? ((Type) member).FullName // member is a Type
                : member.DeclaringType.FullName + "." + member.Name; // member belongs to a Type

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    // XML documentation uses slightly different constructor names
                    memberName = memberName.Replace(".ctor", "#ctor");
                    goto case MemberTypes.Method;
                case MemberTypes.Method:
                    prefixCode = 'M';

                    // parameters are listed according to their type, not their name
                    var paramTypesList = string.Join(
                        ",",
                        ((MethodBase) member).GetParameters()
                        .Select(x => x.ParameterType.FullName
                        ).ToArray()
                    );

                    if (!string.IsNullOrEmpty(paramTypesList))
                        memberName += "(" + paramTypesList + ")";

                    break;

                case MemberTypes.Event:
                    prefixCode = 'E';
                    break;

                case MemberTypes.Field:
                    prefixCode = 'F';
                    break;

                case MemberTypes.NestedType:
                    // XML documentation uses slightly different nested type names
                    memberName = memberName.Replace('+', '.');
                    goto case MemberTypes.TypeInfo;
                case MemberTypes.TypeInfo:
                    prefixCode = 'T';
                    break;

                case MemberTypes.Property:
                    prefixCode = 'P';
                    break;

                default:
                    throw new ArgumentException("Can't show documentation. Unknown member type", "member");
            }

            // elements are of the form "M:Namespace.Class.Method"
            return $"{prefixCode}:{memberName}";
        }

        private static XmlElement GetDocFromName(Assembly assembly, string queryName)
        {
            var xmlDocument = GetDocFromAssembly(assembly);

            if (xmlDocument == null)
                return null;

            XmlElement matchedElement = null;

            foreach (XmlElement xmlElement in xmlDocument["doc"]["members"])
            {
                if (xmlElement.Attributes["name"].Value.Equals(queryName))
                    matchedElement = xmlElement;
            }

            return matchedElement;
        }

        public static XmlDocument GetDocFromAssembly(Assembly assembly)
        {
            try
            {
                if (!_assemblyDocCache.ContainsKey(assembly))
                    _assemblyDocCache[assembly] = LoadAssemblyXml(assembly);

                return _assemblyDocCache[assembly];
            }
            catch (Exception)
            {
                _assemblyDocCache[assembly] = null;
            }

            return null;
        }

        private static XmlDocument LoadAssemblyXml(Assembly assembly)
        {
            var assemblyFilename = assembly.CodeBase;

            const string PREFIX = "file:///";

            if (assemblyFilename.StartsWith(PREFIX))
            {
                try
                {
                    using (var streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(PREFIX.Length), ".xml")))
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(streamReader);
                        return xmlDocument;
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
