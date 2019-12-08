using Apsis.Business;
using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Apsis.Abstractions.Business;
using Apsis.Repository;
using Apsis.Abstractions.Repository;

namespace Apsis.Tests.Repository
{
    [TestClass]
    public class UserRepositoryTest
    {
        UserRepository userRepository;
        public List<User> mockUserList = new List<User>
        {
           new User { Mid = "M1047300" }
        };

        public UserRepositoryTest()
        {
            var _mockBaseRepository = new Mock<IRepository>();
            _mockBaseRepository.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);
            userRepository = new UserRepository(_mockBaseRepository.Object);
        }

        [TestMethod]
        public async Task AddUsers_Test()
        {
            var result = await userRepository.AddUsers(mockUserList);
            Assert.AreEqual(result.IsSuccess,true);

            var wrongUserRepository = new UserRepository(null);
            result = await wrongUserRepository.AddUsers(mockUserList);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.Message, "File upload failed : Insert query to User not working");
        }
    }
}
