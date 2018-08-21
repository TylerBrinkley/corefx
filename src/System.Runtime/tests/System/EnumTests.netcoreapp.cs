// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Tests
{
    public partial class EnumTests
    {
        [Theory]
        [MemberData(nameof(Parse_TestData))]
        public static void Parse_NetCoreApp11<T>(string value, bool ignoreCase, T expected) where T : struct, Enum
        {
            object result;
            if (!ignoreCase)
            {
                Assert.True(Enum.TryParse(expected.GetType(), value, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, Enum.Parse<T>(value));
            }

            Assert.True(Enum.TryParse(expected.GetType(), value, ignoreCase, out result));
            Assert.Equal(expected, result);

            Assert.Equal(expected, Enum.Parse<T>(value, ignoreCase));
        }
        
        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Invalid_NetCoreApp11(Type enumType, string value, bool ignoreCase, Type exceptionType)
        {
            Type typeArgument = enumType == null || !enumType.GetTypeInfo().IsEnum ? typeof(SimpleEnum) : enumType;
            MethodInfo parseMethod = typeof(EnumTests).GetTypeInfo().GetMethod(nameof(Parse_Generic_Invalid_NetCoreApp11)).MakeGenericMethod(typeArgument);
            parseMethod.Invoke(null, new object[] { enumType, value, ignoreCase, exceptionType });
        }

        public static void Parse_Generic_Invalid_NetCoreApp11<T>(Type enumType, string value, bool ignoreCase, Type exceptionType) where T : struct, Enum
        {
            object result = null;
            if (!ignoreCase)
            {
                if (enumType != null && enumType.IsEnum)
                {
                    Assert.False(Enum.TryParse(enumType, value, out result));
                    Assert.Equal(default(object), result);

                    Assert.Throws(exceptionType, () => Enum.Parse<T>(value));
                }
                else
                {
                    Assert.Throws(exceptionType, () => Enum.TryParse(enumType, value, out result));
                    Assert.Equal(default(object), result);
                }
            }

            if (enumType != null && enumType.IsEnum)
            {
                Assert.False(Enum.TryParse(enumType, value, ignoreCase, out result));
                Assert.Equal(default(object), result);

                Assert.Throws(exceptionType, () => Enum.Parse<T>(value, ignoreCase));
            }
            else
            {
                Assert.Throws(exceptionType, () => Enum.TryParse(enumType, value, ignoreCase, out result));
                Assert.Equal(default(object), result);
            }
        }

#if netcoreapp
        [Fact]
        public static void ToString_InvalidUnicodeChars()
        {
            // TODO: move into ToString_Format_TestData when dotnet/buildtools#1091 is fixed
            ToString_Format((Enum)Enum.ToObject(s_charEnumType, char.MaxValue), "D", char.MaxValue.ToString());
            ToString_Format((Enum)Enum.ToObject(s_charEnumType, char.MaxValue), "X", "FFFF");
            ToString_Format((Enum)Enum.ToObject(s_charEnumType, char.MaxValue), "F", char.MaxValue.ToString());
            ToString_Format((Enum)Enum.ToObject(s_charEnumType, char.MaxValue), "G", char.MaxValue.ToString());
        }
#endif //netcoreapp

#if netcoreapp // .NetNative does not support RefEmit nor any other way to create Enum types with unusual backing types.
        private static EnumBuilder GetNonRuntimeEnumTypeBuilder(Type underlyingType)
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Name"), AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule("Name");

            return module.DefineEnum("TestName_" + underlyingType.Name, TypeAttributes.Public, underlyingType);
        }

        private static Type s_boolEnumType = GetBoolEnumType();
        private static Type GetBoolEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(bool));
            enumBuilder.DefineLiteral("Value1", true);
            enumBuilder.DefineLiteral("Value2", false);

            return enumBuilder.CreateTypeInfo().AsType();
        }

        private static Type s_charEnumType = GetCharEnumType();
        private static Type GetCharEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(char));
            enumBuilder.DefineLiteral("Value1", (char)1);
            enumBuilder.DefineLiteral("Value2", (char)2);

            enumBuilder.DefineLiteral("Value0x3f06", (char)0x3f06);
            enumBuilder.DefineLiteral("Value0x3000", (char)0x3000);
            enumBuilder.DefineLiteral("Value0x0f06", (char)0x0f06);
            enumBuilder.DefineLiteral("Value0x1000", (char)0x1000);
            enumBuilder.DefineLiteral("Value0x0000", (char)0x0000);
            enumBuilder.DefineLiteral("Value0x0010", (char)0x0010);
            enumBuilder.DefineLiteral("Value0x3f16", (char)0x3f16);

            return enumBuilder.CreateTypeInfo().AsType();
        }

        private static Type s_floatEnumType = GetFloatEnumType();
        private static Type GetFloatEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(float));
            enumBuilder.DefineLiteral("Value1", 1.0f);
            enumBuilder.DefineLiteral("Value2", 2.0f);

            enumBuilder.DefineLiteral("Value0x3f06", (float)0x3f06);
            enumBuilder.DefineLiteral("Value0x3000", (float)0x3000);
            enumBuilder.DefineLiteral("Value0x0f06", (float)0x0f06);
            enumBuilder.DefineLiteral("Value0x1000", (float)0x1000);
            enumBuilder.DefineLiteral("Value0x0000", (float)0x0000);
            enumBuilder.DefineLiteral("Value0x0010", (float)0x0010);
            enumBuilder.DefineLiteral("Value0x3f16", (float)0x3f16);

            return enumBuilder.CreateTypeInfo().AsType();
        }

        private static Type s_doubleEnumType = GetDoubleEnumType();
        private static Type GetDoubleEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(double));
            enumBuilder.DefineLiteral("Value1", 1.0);
            enumBuilder.DefineLiteral("Value2", 2.0);

            enumBuilder.DefineLiteral("Value0x3f06", (double)0x3f06);
            enumBuilder.DefineLiteral("Value0x3000", (double)0x3000);
            enumBuilder.DefineLiteral("Value0x0f06", (double)0x0f06);
            enumBuilder.DefineLiteral("Value0x1000", (double)0x1000);
            enumBuilder.DefineLiteral("Value0x0000", (double)0x0000);
            enumBuilder.DefineLiteral("Value0x0010", (double)0x0010);
            enumBuilder.DefineLiteral("Value0x3f16", (double)0x3f16);

            return enumBuilder.CreateTypeInfo().AsType();
        }

        private static Type s_intPtrEnumType = GetIntPtrEnumType();
        private static Type GetIntPtrEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(IntPtr));

            return enumBuilder.CreateTypeInfo().AsType();
        }

        private static Type s_uintPtrEnumType = GetUIntPtrEnumType();
        private static Type GetUIntPtrEnumType()
        {
            EnumBuilder enumBuilder = GetNonRuntimeEnumTypeBuilder(typeof(UIntPtr));

            return enumBuilder.CreateTypeInfo().AsType();
        }
#endif //netcoreapp        
    }
}
