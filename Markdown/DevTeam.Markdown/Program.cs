namespace DevTeam.Markdown
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                return;
            }

            using (var stream = File.OpenText(args[0]))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    var doc = XDocument.Load(reader);
                    var assemblyName = doc.XPathSelectElement("/doc/assembly/name").Value;
                    Console.WriteLine($"## {assemblyName}");
                    foreach (var memberElement in doc.XPathSelectElements("/doc/members/member"))
                    {
                        var nameStr = memberElement.Attribute("name").Value;
                        var memberType = nameStr[0];
                        var fullName = nameStr.Substring(2, nameStr.Length - 2);
                        var namespaceName = string.Empty;
                        var memberName = fullName.Substring(namespaceName.Length, fullName.Length - namespaceName.Length);
                        switch (memberType)
                        {
                            case 'T':
                                var parts = fullName.Split('.');
                                namespaceName = string.Join(".", parts.Reverse().Skip(1).Reverse());
                                var className = fullName.Substring(namespaceName.Length, fullName.Length - namespaceName.Length);
                                Console.WriteLine($"### {namespaceName} {className}");
                                break;

                            case 'M':
                                Console.WriteLine($"#### {memberName}");
                                break;
                        }
                    }
                }
            }            
        }
    }
}
