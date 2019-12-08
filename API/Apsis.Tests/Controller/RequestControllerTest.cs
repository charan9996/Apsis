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
using Apsis.Models.Constants;
using System.Linq;
using Apsis.Abstractions;

namespace Apsis.Tests.Controller
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
        RequestSearchFilter noContentSearchFilter = new RequestSearchFilter();
        RequestListResponse mockRequestListResponse = new RequestListResponse
        {
            IsSuccess = true,
            Message = "",
            requestViews = new List<RequestView>
            {
                new RequestView()
            }
        };
        Response mockresponse = new Response {
            IsSuccess = true,
            Message = "Due date updated."
        };
        Guid mockrequestId = new Guid("56ae2fe8-f96d-47f2-ad25-003228c67ca2");
        DateTime mocknewAssignmentDueDate = DateTime.Now;
        FileDownloadResponse mockFileDownloadResponse = new FileDownloadResponse
        {
            IsSuccess = true,
            FileUrl = "abc.com"
        };
        public RequestControllerTest()
        {
            _mockRequestManager = new Mock<IRequestManager>();

            var _mockContextProvider = new Mock<IContextProvider>();
            var _mockFileOperationManager = new Mock<IFileOperationsManager>();

            _mockRequestManager.Setup(x => x.GetLearnerRequests()).ReturnsAsync(mockRequestModelList);
            _mockRequestManager.Setup(x => x.ListAllRequests(It.IsAny<RequestSearchFilter>())).ReturnsAsync(mockRequestListResponse);
            _mockRequestManager.Setup(x => x.DownloadAssignment(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(mockFileDownloadResponse);
            _mockRequestManager.Setup(x => x.UpdateAssignmentDueDate(mockrequestId,mocknewAssignmentDueDate)).ReturnsAsync(mockresponse);
            
            requestController = new RequestController(_mockContextProvider.Object, _mockRequestManager.Object, _mockFileOperationManager.Object);
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
            var result = await requestController.GetRequestsFiltered(null,false);
            Assert.IsNotNull(result);
            var status = (result as BadRequestObjectResult).StatusCode;

            Assert.AreEqual(status, 400);
        }

        [TestMethod]  
        public async Task GetRequestsFiltered_Test_NoContent()
        { 
            var result = await requestController.GetRequestsFiltered(noContentSearchFilter,false);
            Assert.IsNotNull(result);
            var status = (result as NoContentResult).StatusCode;

            Assert.AreEqual(status, 204);
        }

        [TestMethod]
        [DataRow(0, E_RESULT_FILTER.ALL, "", E_SORT_ORDER.SUBMISSION_DATE_DESC, 10)]
        public async Task GetRequestsFiltered_Test_Ok(int currentPage, E_RESULT_FILTER filter, string keyword, E_SORT_ORDER sort, int pageSize)
        {
            var result = await requestController.GetRequestsFiltered(new RequestSearchFilter
            {
                CurrentPage = currentPage,
                Filter = filter,
                Keyword = keyword,
                PageSize = pageSize,
                Sort = sort
            } , true);
            Assert.IsNotNull(result);
            var status = (result as OkObjectResult).StatusCode;
            var list = 
                (((result as OkObjectResult).Value) as List<RequestView>);
            Assert.AreEqual(status, 200);
            Assert.AreEqual(list.Count(), mockRequestListResponse.requestViews.Count());
        } 

        [TestMethod]
        public async Task DownloadAssignmentSolution_Test_BadRequest()
        {
            IEnumerable<Guid> requestIds = null;
            var result = await requestController.DownloadAssignmentSolution(requestIds);

            Assert.IsNotNull(result);
            var status = (((result as BadRequestObjectResult).Value) as Response).IsSuccess;
            Assert.AreEqual(status, false);
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
            var _requestController = new RequestController(null, null, null);

            Guid requestId = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512");
            var result = await _requestController.GetRequestDetails(requestId);

            Assert.IsNotNull(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 500);

        }
        
        public async Task GetRequestsFiltered_Test_Ok()
        {
            IEnumerable<Guid> requestIds = new List<Guid> { Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") };
            var result = await requestController.DownloadAssignmentSolution(requestIds);
            Assert.IsNotNull(result);
            var status = (((result as OkObjectResult).Value) as FileDownloadResponse);
            Assert.AreEqual(status.IsSuccess, mockFileDownloadResponse.IsSuccess);
            Assert.AreEqual(status.FileUrl, mockFileDownloadResponse.FileUrl);
        }
        [TestMethod]
        public async Task PutResubmissionDate_Test()
        {
            var result = await requestController.PutResubmissionDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Int32 statuscode = (result as ObjectResult).StatusCode.Value;
            Assert.AreEqual(statuscode, 200);
        }

        [TestMethod]
        public async Task PutResubmissionDate_Test1()
        {
            Guid mockrequestId = Guid.Empty;
            DateTime mocknewAssignmentDueDate = DateTime.Now;
            var result = await requestController.PutResubmissionDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            Int32 statuscode = (result as BadRequestResult).StatusCode;
            Assert.AreEqual(statuscode, 400);
        }

        [TestMethod]
        public async Task PutResubmissionDate_Test2()
        {
            var wrongController = new RequestController(null, null, null);
          var result= await wrongController.PutResubmissionDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Int32 statuscode = (result as ObjectResult).StatusCode.Value;
            Assert.AreEqual(statuscode, 500);
            Assert.IsNotNull(result);
        }


    }
}