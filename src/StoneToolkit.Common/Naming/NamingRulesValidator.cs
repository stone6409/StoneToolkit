using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace StoneToolkit.Common.Naming
{
    public class NamingRulesValidator
    {
        private static HashSet<string> _sharpKeywords;

        public static HashSet<string> SharpKeywords
        {
            get
            {
                if (_sharpKeywords == null)
                {
                    _sharpKeywords = new HashSet<string>()
                    {
                        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
                        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
                        "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
                        "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
                        "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
                        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
                        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
                        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
                    };
                }

                return _sharpKeywords;
            }
        }

        public static bool IsCSharpValidIdentifierName(string name)
        {
            ValidationResult validationResult = ValidateCSharpIdentifierName(name);
            return validationResult.IsValid;
        }

        public static ValidationResult ValidateCSharpIdentifierName(string name)
        {
            ValidationResult validationResult = ValidateIdentifierName(name);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            if (SharpKeywords.Contains(name))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "标识符不能是C#的关键字。"
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public static bool IsValidIdentifierName(string name)
        {
            ValidationResult validationResult = ValidateIdentifierName(name);
            return validationResult.IsValid;
        }

        public static ValidationResult ValidateIdentifierName(string name)
        {
            // Grammar:
            // <identifier> ::= <identifier_start> ( <identifier_start> | <identifier_extend> )* 
            // <identifier_start> ::= [{Lu}{Ll}{Lt}{Lo}{Nl}('_')]
            // <identifier_extend> ::= [{Mn}{Mc}{Lm}{Nd}]

            for (int i = 0; i < name.Length; i++)
            {
                char currentChar = name[i];
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(currentChar);

                bool idStart = (uc == UnicodeCategory.UppercaseLetter || // Lu
                                uc == UnicodeCategory.LowercaseLetter || // Ll
                                uc == UnicodeCategory.TitlecaseLetter || // Lt
                                uc == UnicodeCategory.OtherLetter || // Lo
                                uc == UnicodeCategory.LetterNumber || // Nl
                                currentChar == '_');

                bool idExtend = (uc == UnicodeCategory.NonSpacingMark || // Mn
                                 uc == UnicodeCategory.SpacingCombiningMark || // Mc
                                 uc == UnicodeCategory.ModifierLetter || // Lm
                                 uc == UnicodeCategory.DecimalDigitNumber); // Nd

                if (i == 0)
                {
                    if (!idStart)
                    {
                        return new ValidationResult
                        {
                            IsValid = false,
                            ErrorMessage = "标识符必须以字母或下划线开头，且只能包含字母、数字和下划线。"
                        };
                    }
                }
                else if (!(idStart || idExtend))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "标识符只能包含字母、数字和下划线。"
                    };
                }
            }

            return new ValidationResult { IsValid = true };
        }

        public static bool IsValidWindowsFileName(string name)
        {
            ValidationResult validationResult = ValidateWindowsFileName(name);
            return validationResult.IsValid;
        }

        private static ValidationResult ValidateWindowsFileName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (name.Any(c => invalidChars.Contains(c)) || name.EndsWith("."))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "文件名不能包含以下字符：" + string.Join(" ", invalidChars) + "，且不能以点结尾。"
                };
            }

            if (name.Length == 0 || name.Length > 255)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Windows文件名长度必须在1到255个字符之间。"
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public static bool IsValidWindowsFolderName(string name)
        {
            ValidationResult validationResult = ValidateWindowsFileName(name);
            return validationResult.IsValid;
        }

        public static ValidationResult ValidateWindowsFolderName(string name)
        {
            // 首先使用文件名验证
            var fileNameResult = ValidateWindowsFileName(name);
            if (!fileNameResult.IsValid)
            {
                return fileNameResult;
            }

            // 额外的文件夹名称验证
            string[] reservedNames = new[]
            {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            if (reservedNames.Contains(name.ToUpperInvariant()))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"'{name}' 是Windows保留名称，不能用作文件夹名。"
                };
            }

            // 检查是否以空格结尾
            if (name.EndsWith(" "))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "文件夹名称不能以空格结尾。"
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public static bool IsValidName(NamingRule namingRule, string name)
        {
            switch (namingRule)
            {
                case NamingRule.IdentifierNaming:
                    return IsValidIdentifierName(name);
                case NamingRule.CSharpIdentifierNaming:
                    return IsCSharpValidIdentifierName(name);

                case NamingRule.FileSystemNaming:
                    return IsValidWindowsFileName(name);

                default:
                    throw new ArgumentOutOfRangeException(nameof(namingRule), "未知的命名规则类型。");
            }
        }

        public static ValidationResult ValidateName(NamingRule namingRule, string name)
        {
            switch (namingRule)
            {
                case NamingRule.CSharpIdentifierNaming:
                    return ValidateCSharpIdentifierName(name);

                case NamingRule.FileSystemNaming:
                    return ValidateWindowsFileName(name);

                default:
                    throw new ArgumentOutOfRangeException(nameof(namingRule), "未知的命名规则类型。");
            }
        }

        public static string GenerateValidName(ValidateNameCallback validateNameCallback, string baseName, string idealNewName = null)
        {
            if (!string.IsNullOrEmpty(idealNewName))
            {
                if (validateNameCallback(idealNewName))
                {
                    return idealNewName;
                }
            }

            string newName = baseName;
            uint number = 1;

            while (true)
            {
                if (validateNameCallback(newName))
                {
                    break;
                }

                newName = baseName + number;
                ++number;
            }

            return newName;
        }
    }
}
