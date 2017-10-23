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
        public void ShouldAddSummaryDocCommentsToClass1ConstructorDeclarationNoArguments()
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
    /// Initializes a new instance of the <see cref=""Class1""/> class.
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

        [Fact]
        public void ShouldAddSummaryDocCommentsToClass2ConstructorDeclarationNoArguments()
        {
            var consDecl =
    @"public class Class2
{
    public Class2()
    {
    }
}";
            var expected =
                @"/// <summary>
/// 
/// </summary>
public class Class2
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Class2""/> class.
    /// </summary>
    public Class2()
    {
    }
}";
            var tree = CSharpSyntaxTree.ParseText(consDecl);
            var rewriter = new GenerateDocumentationComments.DocumentCommentsRewriter();
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var result = rewriter.Visit(root);

            Assert.Equal(expected, result.ToFullString());
        }
        
        [Fact]
        public void ShouldNotAddSummaryDocCommentsToConstructorDeclarationWithCommentsNoArguments()
        {
            var consDecl =
    @"public class Class1
{
    /// <summary>
    /// Non-standard constructor comment.
    /// </summary>
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
    /// Non-standard constructor comment.
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

        [Theory]
        [InlineData("text", "The text.")]
        [InlineData("name", "The name.")]
        [InlineData("syntaxNodeName", "Name of the syntax node.")]
        [InlineData("docCommentDelimiter", "The document comment delimiter.")]
        [InlineData("programIDValue", "The program ID value.")]
        [InlineData("anIBMComputer", "An IBM computer.")]
        public void ShouldAddParamDocCommentsToClass2ConstructorDeclarationOneArgument(string argument, string text)
        {
            var consDecl =
@"public class Class2
{
    public Class2(string " + argument +
    @")
    {
    }
}";
            var expected =
            @"/// <summary>
/// 
/// </summary>
public class Class2
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Class2""/> class.
    /// </summary>
    /// <param name=""" + argument + @""">" + text +
                @"</param>
    public Class2(string " + argument + @")
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
