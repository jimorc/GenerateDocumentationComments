using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class ElementTag
    {
        internal ElementTag(string tagName)
        {
            this.tagName = tagName;
        }

        protected string tagName;
    }

    internal class StartTag : ElementTag
    {
        internal StartTag(string tagName)
            : base(tagName) { }
        internal XmlElementStartTagSyntax CreateTag()
        {
            return SyntaxFactory.XmlElementStartTag(
                SyntaxFactory.XmlName(
                    SyntaxFactory.Identifier(tagName)));
        }
    }

    internal class EndTag : ElementTag
    {
        internal EndTag(string tagName)
            : base(tagName) { }
        internal XmlElementEndTagSyntax CreateTag()
        {
            return SyntaxFactory.XmlElementEndTag(
                SyntaxFactory.XmlName(
                    SyntaxFactory.Identifier(tagName)));
        }
    }
}
