using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = SDE.Begone.Test.CSharpCodeFixVerifier< SDE.Begone.SDEBegoneAnalyzer, SDE.Begone.SDEBegoneCodeFixProvider>;

namespace SDE.Begone.Test
{
    [TestClass]
    public class SDEBegoneUnitTest
    {
        [TestMethod]
        public async Task Should_Produce_No_Diagnostics()
        {
            // Arrange
            var test = @"";

            // Act and Assert
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Should_Produce_Diagnostic()
        {
            // Arrange
            var test = @"
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    class TestClass
    {   

    }
}";

            var expectedSystemDataEntityDiagnosticResult = new DiagnosticResult("SDEBegone", DiagnosticSeverity.Warning).WithSpan(4, 1, 4, 26).WithArguments("System.Data.Entity");
            var expectedEntityNamespaceNotFoundDiagnosticResult = new DiagnosticResult("CS0234", DiagnosticSeverity.Error).WithSpan(4, 19, 4, 25).WithArguments("Entity", "System.Data");
            
            // Act and Assert
            await VerifyCS.VerifyAnalyzerAsync(test, expectedSystemDataEntityDiagnosticResult, expectedEntityNamespaceNotFoundDiagnosticResult);
        }

        [TestMethod]
        public async Task Should_Produce_Diagnostic_And_Apply_Code_Fix()
        {
            // Arrange
            var test = @"
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApplication1
{
    class TestClass
    {   

    }
}";

            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    class TestClass
    {   

    }
}";

            var expectedSystemDataEntityDiagnosticResult = new DiagnosticResult("SDEBegone", DiagnosticSeverity.Warning).WithSpan(4, 1, 4, 26);

            // Act and Assert
            await VerifyCS.VerifyCodeFixAsync(test, expected: new[] { expectedSystemDataEntityDiagnosticResult, },
            fixtest);
        }
    }
}
