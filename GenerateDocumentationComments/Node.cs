using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class Node
    {
        internal Node(string commentDelimiter)
        {
            delimiter = commentDelimiter;
        }
        internal abstract XmlNodeSyntax CreateXmlNode();

        internal void AddToken(Token token)
        {
            tokens.Add(token);
        }

        internal void AddTokenList(List<Token> tokenList)
        {
            tokens.AddRange(tokenList);
        }

        protected SyntaxTokenList CreateTokenList()
        {
            var tokenList = SyntaxFactory.TokenList();
            foreach (var token in tokens)
            {
                SyntaxToken sToken = token.CreateXmlToken(delimiter);
                tokenList = tokenList.Add(sToken);
            }
            return tokenList;
        }

        private List<Token> tokens = new List<Token>();
        private string delimiter;
    }

    internal class TextNode : Node
    {
        internal TextNode(string commentDelimiter)
            : base(commentDelimiter) { }

        internal override XmlNodeSyntax CreateXmlNode()
        {
            var tokenList = CreateTokenList();
            return SyntaxFactory.XmlText()
                .WithTextTokens(tokenList);
        }
    }

    internal class ExampleElementNode : Node
    {
        internal ExampleElementNode(string commentDelimiter)
            : base(commentDelimiter) { }

        internal SyntaxList<XmlNodeSyntax> CreateNodeList()
        {
            var nodeList = SyntaxFactory.List<XmlNodeSyntax>();
            foreach (var node in nodes)
            {
                var xmlNode = node.CreateXmlNode();
                nodeList = nodeList.Add(xmlNode);
            }
            return nodeList;
        }

        internal override XmlNodeSyntax CreateXmlNode()
        {
            var nodeList = CreateNodeList();
            XmlElementSyntax elementSyntax;
            if (nodeList.Count == 1)
            {
                elementSyntax = SyntaxFactory.XmlExampleElement(
                    SyntaxFactory.SingletonList<XmlNodeSyntax>(
                        nodeList.First()));
            }
            else
            {
                elementSyntax = SyntaxFactory.XmlExampleElement(
                    nodeList);
            }
            if(StartTag != null && EndTag != null)
            {
                elementSyntax = elementSyntax
                    .WithStartTag(StartTag.CreateTag())
                    .WithEndTag(EndTag.CreateTag());
            }
            return elementSyntax;
        }

        internal void AddNode(Node node)
        {
            nodes.Add(node);
        }

        private List<Node> nodes = new List<Node>();
        internal StartTag StartTag{ get; set; }
        internal EndTag EndTag { get; set; }
    }

    internal class CrefNode : Node
    {
        internal CrefNode(string crefName)
            : base("")
        {
            name = crefName;
        }

        internal override XmlNodeSyntax CreateXmlNode()
        {
            return SyntaxFactory.XmlNullKeywordElement()
                .WithAttributes(
                SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                    SyntaxFactory.XmlCrefAttribute(
                        SyntaxFactory.NameMemberCref(
                            SyntaxFactory.IdentifierName(name)))));
        }

        private string name;
    }

    internal class Nodes
    {
        internal XmlNodeSyntax[] CreateXmlNodes()
        {
            var xmlNodes = SyntaxFactory.List<XmlNodeSyntax>();
            foreach(var node in nodes)
            {
                xmlNodes = xmlNodes.Add(node.CreateXmlNode());
            }
            return xmlNodes.ToArray();
        }

        internal void AddNode(Node node)
        {
            nodes.Add(node);
        }

        internal void AddNodesRange(List<Node> nodesToAdd)
        {
            nodes.AddRange(nodesToAdd);
        }

        private List<Node> nodes = new List<Node>();

    }

}
