using Apsis.Abstractions.Business;
using Apsis.API.Controllers;
using Apsis.Models.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Apsis.Models.Common;
using Apsis.Models.Response;
using System.Linq;


namespace Apsis.Tests
{
    [TestClass]
    public class RequestControllerTest
    {
        RequestController requestController;
        Mock<IRequestManager> _mockRequestManager;
        List<RequestModel> mockRequestModelList = new List<RequestModel>
        {
            new RequestModel { YorbitCourseId = "19000", }
        };

        RequestSearchFilter requestSearchFilter;
        RequestListResponse mockRequestListResponse = new RequestListResponse
        {
            IsSuccess = true,
            Message = "",
            requestViews = new List<RequestView>
            {

            }
        };
        public RequestControllerTest()
        {

            _mockRequestManager = new Mock<IRequestManager>();
            var _mockUploadManager = new Mock<IUploadManager>();

            _mockRequestManager.Setup(x => x.GetLearnerRequests()).ReturnsAsync(mockRequestModelList);
            _mockRequestManager.Setup(x => x.ListAllRequests(requestSearchFilter)).ReturnsAsync(mockRequestListResponse);

            requestController = new RequestController(_mockRequestManager.Object, _mockUploadManager.Object);
        }

        [TestMethod]
        public async Task Get_Test()
        {
            var result = await requestController.Get();
            Assert.IsNotNull(result);
            var list = (result as OkObjectResult).Value as List<RequestModel>;
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].YorbitCourseId, mockRequestModelList[0].YorbitCourseId);
        }

        [TestMethod]
        public async Task GetRequestsFiltered_Test_BadRequest()
        {
            var result = await requestController.GetRequestsFiltered(null);

            var status = (result as BadRequestObjectResult).StatusCode;

            Assert.AreEqual(status, 400);
        }

        [TestMethod] 
        //[DataRow(12, 3, 4)]
        //[DataRow(12, 2, 6)]
        //[DataRow(12, 4, 3)]
        public async Task GetRequestsFiltered_Test_NoContent()
        {
            requestSearchFilter = new RequestSearchFilter();
            var result = await requestController.GetRequestsFiltered(requestSearchFilter);

            var status = (result as NoContentResult).StatusCode;

            Assert.AreEqual(expected: status, actual: 204);
        }
        
        public async Task GetRequestsFiltered_Test_Ok()
        {
            requestSearchFilter = new RequestSearchFilter();
            var result = await requestController.GetRequestsFiltered(requestSearchFilter);

            var status = (result as OkObjectResult).StatusCode;

            Assert.AreEqual(status, 200);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_BadRequest()
        {
            Guid requestId = Guid.Empty;
            var result = await requestController.GetRequestDetails(requestId);

            Assert.IsNotNull(result);
            var status = (((result as BadRequestObjectResult).Value) as Response).IsSuccess;
            Assert.AreEqual(status, false);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_Ok()
        {
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { IsSuccess = true };
            _mockRequestManager.Setup(x => x.GetRequestDetails(It.IsAny<Guid>())).ReturnsAsync(mockRequestDetailsModel);
            var result = await requestController.GetRequestDetails(requestId);

            Assert.IsNotNull(result);
            var status = (((result as OkObjectResult).Value) as RequestDetailsModel);
            Assert.AreEqual(status.IsSuccess, mockRequestDetailsModel.IsSuccess);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_Exception()
        {
            var _requestController = new RequestController(null, null);

            Guid requestId = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512");
            var result = await _requestController.GetRequestDetails(requestId);

            Assert.IsNotNull(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 500);

        }


    }
}
