using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateDocumentationComments
{
    internal abstract class Token
    {
        internal abstract SyntaxToken CreateXmlToken(string docCommentExterior);
    }

    internal class NewlineToken : Token
    {
        internal override SyntaxToken CreateXmlToken(string docCommentExterior)
        {
            return SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                Environment.NewLine,
                Environment.NewLine,
                SyntaxFactory.TriviaList());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GenerateDocumentationComments.Token" />
    internal class LiteralTextToken : Token
    {
        internal LiteralTextToken(string literalText)
        {
            text = literalText;
        }

        internal override SyntaxToken CreateXmlToken(string docCommentExterior)
        {
            return SyntaxFactory.XmlTextLiteral(
                SyntaxFactory.TriviaList(
                    SyntaxFactory.DocumentationCommentExterior(docCommentExterior)),
                text,
                text,
                SyntaxFactory.TriviaList());
        }
        private string text;
    }
}
