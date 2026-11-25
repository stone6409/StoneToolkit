using System;
using System.Collections.Generic;
using System.Text;

namespace StoneToolkit.Common.Naming
{
    public class NameMap
    {
        public NamingRule NamingRule { get; private set; }

        public NameMap(NamingRule namingRule = NamingRule.IdentifierNaming)
        {
            NamingRule = namingRule;
            _nameDictionary = new HashSet<string>();
        }

        public NameMap(HashSet<string> nameDictionary, NamingRule namingRule = NamingRule.IdentifierNaming)
        {
            NamingRule = namingRule;
            NameDictionary = nameDictionary;
        }

        private HashSet<string> _nameDictionary;

        public HashSet<string> NameDictionary
        {
            get
            {
                return _nameDictionary;
            }
            set
            {
                _nameDictionary = value;
            }
        }

        public bool IsValidName(string name)
        {
            return NamingRulesValidator.IsValidName(NamingRule, name) && !ContainsName(name);
        }

        public ValidationResult ValidateName(string name)
        {
            ValidationResult validationResult = NamingRulesValidator.ValidateName(NamingRule, name);
            if (validationResult.IsValid)
            {
                if (ContainsName(name))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "名字已经被其他对象使用"
                    };
                }
            }

            return validationResult;
        }

        public string GenerateValidName(string baseName, string idealNewName = null)
        {
            string name = NamingRulesValidator.GenerateValidName(IsValidName, baseName, idealNewName);
            return name;
        }

        public string GenerateAndAddValidName(string baseName, string idealNewName = null)
        {
            string name = GenerateValidName(baseName, idealNewName);
            AddName(name);

            return name;
        }

        public bool ChangeName(string oldName, string newName)
        {
            if (IsValidName(newName))
            {
                if (oldName != null)
                {
                    RemoveMame(oldName);
                }

                AddName(newName);
                return true;
            }

            return false;
        }

        public void AddName(string name)
        {
            _nameDictionary.Add(name);
        }

        public void RemoveMame(string name)
        {
            _nameDictionary.Remove(name);
        }

        public bool ContainsName(string name)
        {
            return _nameDictionary.Contains(name);
        }
    }
}
