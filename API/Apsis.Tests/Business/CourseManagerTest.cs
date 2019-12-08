using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.API.Controllers;
using Apsis.AzureServices;
using Apsis.Business;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Tests.Business
{
    [TestClass]
    public class CourseManagerTest
    {
        Guid newGuid = new Guid("2EA91CF3-C30E-4310-AD64-216C28BDD6CF");
        CourseManager courseManager;
        IRepository _requestRepository;

        List<CourseModel> mockCourseData = new List<CourseModel>()
        {
            new CourseModel{YorbitCourseId ="L201",
            CourseName="Angular"}

        };
        FileDownloadResponse mockDataReponse = new FileDownloadResponse
        {
            FileUrl = "c:\\M1046907\\downloads",
            IsSuccess = true,
            Message = ""
        };

        CourseFilter courseFilter = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
        };
        CourseFilter courseFilter2 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = -1,
            PageSize = 10,
        };
        public CourseManagerTest()
        {
            Guid learnerId = Guid.Empty;
            var _mockCourseRepository = new Mock<ICourseRepository>();
            var _mockCourseManager = new Mock<ICourseManager>();
            var _mockBlobManager = new Mock<IBlobHelper>();
            var _mockUploadRepsitory = new Mock<IUploadRepository>();
            var _mockUploadHelper = new Mock<IUploadHelper>();
            var _mockUserManager = new Mock<IUserManager>();
            var _mockIBlobHelper = new Mock<IBlobHelper>();

            _mockCourseRepository.Setup(x => x.GetAllCoursesAsync(courseFilter)).ReturnsAsync(mockCourseData);
            courseManager = new CourseManager(_mockCourseRepository.Object, _mockUploadHelper.Object, _mockUserManager.Object, _mockIBlobHelper.Object);
        }

        [TestMethod]
        public async Task GetCourseAsyncOkObject_Test()
        {
            var result = await courseManager.GetAllCoursesAsync(courseFilter);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }
        [TestMethod]
        public async Task UpdateAssigmentAsync_Test()
        {
            var result = await courseManager.UpdateAssigmentAsync(newGuid, It.IsAny<IFormFile>());
            // Assert.AreNotEqual(result.FileUrl, "");
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Message, "Error uploading assignment.");
        }
        [TestMethod]
        public async Task GetCourseAsyncCurrentPageSize_Test()
        {
            var result = await courseManager.GetAllCoursesAsync(courseFilter2);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.IsNull(list);
        }

    }

}
