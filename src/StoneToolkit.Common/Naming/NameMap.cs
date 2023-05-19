using System;
using System.Collections.Generic;
using System.Text;

namespace StoneToolkit.Common.Naming
{
    public class NameMap
    {
        private HashSet<string> _nameDictionary;

        public NameMap()
        {
            _nameDictionary = new HashSet<string>();
        }

        public NameMap(HashSet<string> nameDictionary)
        {
            _nameDictionary = nameDictionary;
        }

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
            return NameValidationHelper.IsValidIdentifierName(name) && !ContainsName(name);
        }

        public string GenerateValidName(string baseName, string? idealNewName = null)
        {
            string name = NameValidationHelper.GenerateValidName(IsValidName, baseName, idealNewName);
            return name;
        }

        public string GenerateAndAddValidName(string baseName, string? idealNewName = null)
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
