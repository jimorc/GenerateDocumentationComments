using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateDocumentationComments
{
    internal class Token
    {
        internal Token(string docCommentExterior = null)
        {
            CommentExterior = docCommentExterior;
        }

        internal virtual SyntaxToken CreateSyntaxToken()
        {
            throw new NotImplementedException("CreateSyntaxToken method must be overridden in child classes.");
        }

        protected string CommentExterior
        {
            get;
        }
            
    }

    internal class NewLineToken : Token
    {
        internal override SyntaxToken CreateSyntaxToken()
        {
            return SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                Environment.NewLine,
                Environment.NewLine,
                SyntaxFactory.TriviaList());
        }
    }

    internal class TextLiteralToken : Token
    {
        internal TextLiteralToken(string literalText, string docCommentExterior)
            : base(docCommentExterior)
        {
            text = literalText;
        }
        internal override SyntaxToken CreateSyntaxToken()
        {
            return SyntaxFactory.XmlTextLiteral(
                SyntaxFactory.TriviaList(
                    SyntaxFactory.DocumentationCommentExterior(CommentExterior)),
                text,
                text,
                SyntaxFactory.TriviaList());
        }

        private string text;
    }
}
