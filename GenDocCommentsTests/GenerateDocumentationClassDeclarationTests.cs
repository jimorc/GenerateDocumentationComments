﻿using GenerateDocumentationComments;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GenerateDocumentationCommentsTests
{
    public class GenerateDocumentationClassDeclarationTests
    {
        [Fact]
        public void ShouldAddSummaryDocCommentsToIndented4SpacesPublicClassDeclaration()
        {
            var classDecl =
                @"    public class Class1
    {
    }";
            var expected =
                @"    /// <summary>
    /// 
    /// </summary>
    public class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new GenerateDocumentationComments.DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldAddSummaryDocCommentsToIndented8SpacesPublicClassDeclaration()
        {
            var classDecl =
                @"        public class Class1
        {
        }";
            var expected =
                @"        /// <summary>
        /// 
        /// </summary>
        public class Class1
        {
        }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldAddSummaryDocCommentsToIndentedNoSpacesPublicClassDeclaration()
        {
            var classDecl =
                @"public class Class1
{
}";
            var expected =
                @"/// <summary>
/// 
/// </summary>
public class Class1
{
}";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldAddSummaryDocCommentsToIndented4SpacesProtectedClassDeclaration()
        {
            var classDecl =
@"    protected class Class1
    {
    }";
            var expected =
@"    /// <summary>
    /// 
    /// </summary>
    protected class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldAddSummaryDocCommentsToInternalClassDeclaration()
        {
            var classDecl =
@"    internal class Class1
    {
    }";
            var expected =
@"    /// <summary>
    /// 
    /// </summary>
    internal class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldNotAddSummaryDocCommentsToPrivateClassDeclaration()
        {
            var classDecl =
@"    private class Class1
    {
    }";
            var expected =
@"    private class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldNotAddSummaryDocCommentsToNoAccessClassDeclaration()
        {
            var classDecl =
@"    class Class1
    {
    }";
            var expected =
    @"    class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(expected, result.ToFullString());
        }

        [Fact]
        public void ShouldNotAddSummaryDocCommentsClassDeclarationWithSummaryComments()
        {
            var classDecl =
@"    /// <summary>
    /// A summary description
    /// </summary>
    public class Class1
    {
    }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(classDecl, result.ToFullString());
        }

        [Fact]
        public void ShouldNotAddSummaryDocCommentsClassDeclarationWithMultilineSummaryComments()
        {
            var classDecl =
@"    /** <summary>
         A summary description
         </summary> */
        public class Class1
        {
        }";
            var tree = CSharpSyntaxTree.ParseText(classDecl);
            var rewriter = new DocumentCommentsRewriter();

            var result = rewriter.Visit(tree.GetRoot());

            Assert.Equal(classDecl, result.ToFullString());
        }
    }
}
