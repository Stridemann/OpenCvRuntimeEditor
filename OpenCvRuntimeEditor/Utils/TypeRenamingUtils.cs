namespace OpenCvRuntimeEditor.Utils
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Emgu.CV;
    using Emgu.CV.Structure;
    using Microsoft.CSharp;

    public static class TypeRenamingUtils
    {
        private static readonly Dictionary<Type, string> _renamingCache = new Dictionary<Type, string>();
        private static readonly CSharpCodeProvider _primitiveConverter = new CSharpCodeProvider();

        public static string RenameType(Type type, bool shortGeberics = true)
        {
            if (type == null)
                return "null";

            if (type.IsPrimitive)
            {
                var primitiveRef = new CodeTypeReference(type);
                return _primitiveConverter.GetTypeOutput(primitiveRef);
            }

            if (_renamingCache.TryGetValue(type, out var cachedName))
                return cachedName;

            if (type.IsGenericParameter)
                return type.Name;

            string clearName;
            var arrayName = "";

            if (type.IsGenericType)
            {
                try
                {
                    var nameNoNs = type.Name;
                    clearName = nameNoNs.Substring(0, nameNoNs.LastIndexOf("`", StringComparison.Ordinal));
                    var args = type.GetGenericArguments();

                    if (type.IsGenericTypeDefinition)
                    {
                        clearName = clearName + "<" + new string(',', args.Length - 1) + ">";
                    }
                    else
                    {
                        clearName = clearName + "<" + string.Join(", ", args.Select(x => RenameType(x, shortGeberics)).ToArray()) + ">";
                    }
                }
                catch
                {
                    clearName = type.Name;
                }
            }
            else if (type.IsArray)
            {
                arrayName = type.Name.Substring(type.Name.IndexOf("[", StringComparison.Ordinal));
                clearName = type.Name.Replace(arrayName, "");
            }
            else
                clearName = type.Name;

            var finalName = clearName + arrayName;

            _renamingCache.Add(type, finalName);

            return finalName;
        }

        public static string RenameOperationMethod(string name)
        {
            if (name == "op_Addition")
                return "Add";

            if (name == "op_Division")
                return "Divide";

            if (name == "op_Equality")
                return "Equal";

            if (name == "op_Implicit")
                return "Implicit";

            if (name == "op_Inequality")
                return "NotEqual";

            if (name == "op_Multiply")
                return "Multiply";

            if (name == "op_Subtraction")
                return "Subtract";

            if (name == "op_UnaryNegation")
                return "UnaryNegation";

            return name;
        }

        public static string RenameValue(object value)
        {
            if (value == null)
                return "Null";

            var type = value.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                return value.ToString();
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Image<,>))
            {
                value = CvUtils.MatFromImage(value);
            }

            if (value is Mat mat)
            {
                return $"Depth: {mat.Depth}, Ch: {mat.NumberOfChannels}";
            }

            if (value is RangeF range)
            {
                return $"[{range.Min} - {range.Max}]";
            }

           

            return value.ToString();
        }
    }
}
