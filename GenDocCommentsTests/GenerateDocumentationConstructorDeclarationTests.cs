using GenerateDocumentationComments;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace GenerateDocumentationCommentsTests
{
    // The source code to test the generation of document comments for must begin
    // with a class declaration because otherwise the constructors are treated as
    // method declarations rather than constructor declarations.
    public class GenerateDocumentationConstructorDeclarationTests
    {
        [Fact]
        public void ShouldAddSummaryDocCommentsToConstructorDeclarationNoArguments()
        {
            var consDecl =
    @"public class Class1
{
    public Class1()
    {
    }
}";
            var expected =
                @"/// <summary>
/// 
/// </summary>
public class Class1
{
    /// <summary>
    /// 
    /// </summary>
    public Class1()
    {
    }
}";
            var tree = CSharpSyntaxTree.ParseText(consDecl);
            var rewriter = new GenerateDocumentationComments.DocumentCommentsRewriter();
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var result = rewriter.Visit(root);

            Assert.Equal(expected, result.ToFullString());
        }
    }
}
