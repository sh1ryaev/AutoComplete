using System;
using System.Collections.Generic;

namespace AutoComplete
{

    public struct FullName
    {
        public string Name;
        public string Surname;
        public string Patronymic;

        public override string ToString()
        {
            var str = string.Join(" ", Surname?.Trim(), Name?.Trim(), Patronymic?.Trim()).Trim();
            return str;
        }
    }
    public class TrieNode
    {
        private string? fullNameString;
        private Dictionary<char, TrieNode> nodeChildren = new Dictionary<char, TrieNode>();

        public void Add(string path, string data)
        {
            if (path == null) return;
            Add(path, 0, data);
        }

        private void Add(string path, int index, string data)
        {
            if (index == path.Length)
            {
                this.fullNameString = data;
                return;
            }

            var c = path[index];
            if (!nodeChildren.ContainsKey(c)) nodeChildren.Add(c, new TrieNode());
            nodeChildren[c].Add(path, index + 1, data);
        }

        public void GetByPrefix(string prefix, List<string> result)
        {
            GetByPrefix(prefix, 0, result);
        }

        private void GetByPrefix(string prefix, int index, List<string> result)
        {
            if (!string.IsNullOrEmpty(fullNameString))
                result.Add(fullNameString);

            if (index >= prefix.Length)
            {
                foreach (var node in nodeChildren.Values)
                    node.GetByPrefix(prefix, index, result);
            }
            else
            {
                var c = prefix[index];
                if (nodeChildren.ContainsKey(c))
                    nodeChildren[c].GetByPrefix(prefix, index + 1, result);
            }
        }
    }

    public class AutoCompleter
    {
        private List<string>? stringFullNames { get; set; }
        private TrieNode? rootOfTrie { get; set; }

        public void AddToSearch(List<FullName> fullNames)
        {
            var copyList = new List<string>();
            TrieNode copyRoot = new TrieNode();
            foreach (var fullName in fullNames)
            {
                copyList.Add(fullName.ToString());
            }
            stringFullNames = copyList;
            foreach (var name in stringFullNames)
            {
                copyRoot.Add(name, name);
            }
            rootOfTrie = copyRoot;
        }

        public List<string> Search(string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException();
            if (prefix.Length > 100)
                throw new ArgumentException();
            var searchResult = new List<string>();
            rootOfTrie.GetByPrefix(prefix, searchResult);
            return searchResult;
        }
    }
}
