using Apsis.Abstractions.Business;
using Apsis.API.Controllers;
using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Tests.Controller
{
    [TestClass]
    public class CourseControllerTest
    {
        CourseController courseController;
        Guid newGuid = Guid.NewGuid();
        List<CourseModel> mockCourseModelList = new List<CourseModel>
        {
            new CourseModel { YorbitCourseId = "19000" }
        };

        CourseFilter courseFilter = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
        };

        CourseFilter courseFilter2 = new CourseFilter
        {
            Keyword = "FSDFSDFGSDFSDFSDF",
            CurrentPage = 0,
            PageSize = 10,
        };

        CourseFilter courseFilter3;

        FileDownloadResponse fileDownloadResponse = new FileDownloadResponse()
        {
            FileUrl = "c//m1046907",IsSuccess=true
        };
        public CourseControllerTest()
        {
            var fileMock = new Mock<IFormFile>();
            var _mockCourseManager = new Mock<ICourseManager>();
            var _mockHttpRequest = new Mock<HttpContext>();
            var _mockHostingEnvironment = new Mock<IHostingEnvironment>();
            var _mockFileOperationsManager = new Mock<IFileOperationsManager>();
            _mockCourseManager.Setup(x => x.GetAllCoursesAsync(courseFilter)).ReturnsAsync(mockCourseModelList);
            courseController = new CourseController(_mockCourseManager.Object, _mockFileOperationsManager.Object);
            _mockCourseManager.Setup(x=>x.UpdateAssigmentAsync(newGuid,It.IsAny<IFormFile>())).ReturnsAsync(fileDownloadResponse);
            var _mockCourseController = new CourseController(_mockCourseManager.Object, _mockFileOperationsManager.Object);

            //Assert
  

        }

        [TestMethod] 
        public async Task GetCourseAsyncOkObject_Test()
        {
            var result = await courseController.GetCoursesFiltered(courseFilter);
            Assert.IsNotNull(result);
            var list = (result as OkObjectResult).Value as List<CourseModel>;
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 1);
        }


        [TestMethod]
        public async Task GetCourseAsyncNoContent_Test()
        {
            var result = await courseController.GetCoursesFiltered(courseFilter2);
            var status = (result as NoContentResult).StatusCode; 
            Assert.AreEqual(status, 204);
        }

        [TestMethod]
        public async Task GetCourseAsyncBadRequest_Test()
        {
            var result = await courseController.GetCoursesFiltered(courseFilter3);
            var status = (result as BadRequestObjectResult).StatusCode;
            Assert.AreEqual(status, 400);
        }

        [TestMethod]
        public async Task PostAssignment_Test()
        {
            var _mockCourseManager = new Mock<ICourseManager>();
            var _mockHttpRequest = new Mock<HttpContext>();
            var _mockHostingEnvironment = new Mock<IHostingEnvironment>();
            var _mockFileOperationsManager = new Mock<IFileOperationsManager>();
            var _mockCourseController = new CourseController(_mockCourseManager.Object, _mockFileOperationsManager.Object);
            courseController.ControllerContext = RequestWithFile(); 
            var result = await courseController.PostAssignment(Guid.Empty);
            Assert.AreEqual(result,200);
        }

        private ControllerContext RequestWithFile()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 10, "Data", "dummy.txt");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });
            var actx = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
            return new ControllerContext(actx);
        }

    }
    

}
