using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class ElementTag
    {
        /// <param name="tagName">Name of the tag.</param>
        internal ElementTag(string tagName)
        {
            this.tagName = tagName;
        }

        protected string tagName;
    }

    internal class StartTag : ElementTag
    {
        internal StartTag(string tagName)
            : base(tagName)
        {
            Attribute = null;
        }

        internal XmlElementStartTagSyntax CreateTag()
        {
            var tag = SyntaxFactory.XmlElementStartTag(
                SyntaxFactory.XmlName(
                    SyntaxFactory.Identifier(tagName)));
            if (Attribute != null)
            {
                tag = tag.WithAttributes(
                    SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                        Attribute.CreateAttribute()));
            }
            return tag;
        }

        internal Attribute Attribute { get; set; }
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

    internal class Attribute
    {
        internal Attribute(string attributeName, string attributeValue)
        {
            name = attributeName;
            value = attributeValue;
        }

        internal XmlNameAttributeSyntax CreateAttribute()
        {
            return SyntaxFactory.XmlNameAttribute(
                        SyntaxFactory.XmlName(
                            // " " needed to separate attribute name from surrounding text
                            SyntaxFactory.Identifier(" " + name)),
                        SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                        SyntaxFactory.IdentifierName(value),
                        SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken));

        }

        public string name;
        public string value;
    }
}
