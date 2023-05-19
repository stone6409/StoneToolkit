using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StoneToolkit.Common.Naming
{
    public class NameValidationHelper
    {
        private static HashSet<string>? _sharpKeywords;

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

        public static bool IsValidIdentifierName(string name)
        {
            // Grammar:
            // <identifier> ::= <identifier_start> ( <identifier_start> | <identifier_extend> )* 
            // <identifier_start> ::= [{Lu}{Ll}{Lt}{Lo}{Nl}('_')]
            // <identifier_extend> ::= [{Mn}{Mc}{Lm}{Nd}]
            UnicodeCategory uc;
            for (int i = 0; i < name.Length; i++)
            {
                uc = char.GetUnicodeCategory(name[i]);
                bool idStart = (uc == UnicodeCategory.UppercaseLetter || // (Lu) 
                                uc == UnicodeCategory.LowercaseLetter || // (Ll)
                                uc == UnicodeCategory.TitlecaseLetter || // (Lt) 
                                uc == UnicodeCategory.OtherLetter || // (Lo)
                                uc == UnicodeCategory.LetterNumber || // (Nl)
                                name[i] == '_');
                bool idExtend = (uc == UnicodeCategory.NonSpacingMark || // (Mn) 
                                 uc == UnicodeCategory.SpacingCombiningMark || // (Mc)
                                 uc == UnicodeCategory.ModifierLetter || // (Lm) 
                                 uc == UnicodeCategory.DecimalDigitNumber); // (Nd) 
                if (i == 0)
                {
                    if (!idStart)
                    {
                        return false;
                    }
                }
                else if (!(idStart || idExtend))
                {
                    return false;
                }
            }

            if (SharpKeywords.Contains(name))
            {
                return false;
            }

            return true;
        }

        public static string GenerateValidName(ValidateNameCallback validateNameCallback, string baseName, string? idealNewName = null)
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
