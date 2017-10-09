using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class Node
    {
        internal Node AddToken(Token token)
        {
            tokens.Add(token);
            return this;
        }

        internal SyntaxTokenList CreateSyntaxTokenList()
        {
            var tokenList = new SyntaxTokenList();
            foreach (var token in tokens)
            {
                tokenList = tokenList.Add(token.CreateSyntaxToken());
            }
            return tokenList;
        }

        internal abstract XmlNodeSyntax CreateSyntaxToken();

        private List<Token> tokens = new List<Token>();
    }

    internal class TextNode : Node
    {
        internal TextNode() { }

        internal override XmlNodeSyntax CreateSyntaxToken()
        {
            var tokenList = CreateSyntaxTokenList();
            var xmlText = SyntaxFactory.XmlText()
                .WithTextTokens(tokenList);
            return xmlText;
        }

    }

    internal class TextElementNode : Node
    {
        internal TextElementNode(string startEndTag)
        {
            tag = startEndTag;
        }

        internal override XmlNodeSyntax CreateSyntaxToken()
        {
            var tokenList = CreateSyntaxTokenList();
            return SyntaxFactory.XmlExampleElement(
                SyntaxFactory.SingletonList<XmlNodeSyntax>(
                    SyntaxFactory.XmlText()
                    .WithTextTokens(
                        SyntaxFactory.TokenList(
                            tokenList))))
                .WithStartTag(SyntaxFactory.XmlElementStartTag(
                    SyntaxFactory.XmlName(
                        SyntaxFactory.Identifier(tag))))
                .WithEndTag(SyntaxFactory.XmlElementEndTag(
                    SyntaxFactory.XmlName(
                        SyntaxFactory.Identifier(tag))));
        }

//        private Tokens tokens;
        private string tag = "";
    }
}
