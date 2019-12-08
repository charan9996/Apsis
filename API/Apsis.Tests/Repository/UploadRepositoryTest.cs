using Apsis.Abstractions.Repository;
using Apsis.Models.Entities;
using Apsis.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Tests.Repository
{
    [TestClass]
    public class UploadRepositoryTest
    {
        UploadRepository uploadRepository;

        Upload upload = new Upload()
        { 
             FileName ="C#filename"

         ,FilePath="c/user/m1046907" 
    };
        public UploadRepositoryTest()

        {
            var _mockBaseRepository = new Mock<IRepository>();
            _mockBaseRepository.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(0);
            uploadRepository = new UploadRepository(_mockBaseRepository.Object);
        }

        [TestMethod]
        public async Task UploadAssignment_Test()
        {
            var result = await uploadRepository.UploadFileAsync(upload);
            Assert.AreEqual(result, 0);
        }
    }
}
