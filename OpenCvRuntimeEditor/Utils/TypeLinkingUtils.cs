namespace OpenCvRuntimeEditor.Utils
{
    using System;
    using Emgu.CV;

    public static class TypeLinkingUtils
    {
        public static TypeLinkingResult CheckPinsLinking(Type outPin, Type inPin)
        {
            var sourceType = outPin;
            var targetType = inPin;

            if (sourceType.IsConstructedGenericType)
                sourceType = sourceType.GetGenericTypeDefinition();

            if (targetType.IsConstructedGenericType)
                targetType = targetType.GetGenericTypeDefinition();

            var allowed = targetType.IsAssignableFrom(sourceType);
            var converterType = ConverterNodeType.None;

            if (!allowed)
            {
                if (sourceType.IsPrimitive && targetType.IsPrimitive)
                {
                    if (CanConvertFrom(sourceType, targetType))
                    {
                        allowed = true;
                    }
                }
                else if (sourceType == typeof(Mat) && targetType.IsAssignableFrom(typeof(Image<,>)))
                {
                    allowed = true;
                    converterType = ConverterNodeType.MatToImage;
                }
                else if (sourceType.IsSubclassOf(typeof(Image<,>)) && targetType == typeof(Mat))
                {
                    allowed = true;
                    converterType = ConverterNodeType.ImageToMat;
                }
            }

            return new TypeLinkingResult(allowed, converterType);
        }

        #region NumbersConversion

        public static bool CanConvertFrom(Type type1, Type type2)
        {
            if (type1.IsPrimitive && type2.IsPrimitive)
            {
                var typeCode1 = Type.GetTypeCode(type1);
                var typeCode2 = Type.GetTypeCode(type2);

                // If both type1 and type2 have the same type, return true.
                if (typeCode1 == typeCode2)
                    return true;

                // Possible conversions from Char follow.
                if (typeCode1 == TypeCode.Char)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.UInt16: return true;
                        case TypeCode.UInt32: return true;
                        case TypeCode.Int32: return true;
                        case TypeCode.UInt64: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from Byte follow.
                if (typeCode1 == TypeCode.Byte)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Char: return true;
                        case TypeCode.UInt16: return true;
                        case TypeCode.Int16: return true;
                        case TypeCode.UInt32: return true;
                        case TypeCode.Int32: return true;
                        case TypeCode.UInt64: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from SByte follow.
                if (typeCode1 == TypeCode.SByte)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Int16: return true;
                        case TypeCode.Int32: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from UInt16 follow.
                if (typeCode1 == TypeCode.UInt16)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.UInt32: return true;
                        case TypeCode.Int32: return true;
                        case TypeCode.UInt64: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from Int16 follow.
                if (typeCode1 == TypeCode.Int16)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Int32: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from UInt32 follow.
                if (typeCode1 == TypeCode.UInt32)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.UInt64: return true;
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from Int32 follow.
                if (typeCode1 == TypeCode.Int32)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Int64: return true;
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from UInt64 follow.
                if (typeCode1 == TypeCode.UInt64)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from Int64 follow.
                if (typeCode1 == TypeCode.Int64)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Single: return true;
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }

                // Possible conversions from Single follow.
                if (typeCode1 == TypeCode.Single)
                {
                    switch (typeCode2)
                    {
                        case TypeCode.Double: return true;
                        default: return false;
                    }
                }
            }

            return false;
        }

        #endregion
    }

    public struct TypeLinkingResult
    {
        public TypeLinkingResult(bool allowed, ConverterNodeType converterType)
        {
            Allowed = allowed;
            ConverterType = converterType;
        }

        public bool Allowed { get; }
        public ConverterNodeType ConverterType { get; }
    }

    public enum ConverterNodeType
    {
        None,
        MatToImage,
        ImageToMat
    }
}
