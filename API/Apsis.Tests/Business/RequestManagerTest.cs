using Apsis.Abstractions.Repository;
using Apsis.Business;
using Apsis.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Apsis.Abstractions.Business;
using Apsis.Repository;
using Apsis.AzureServices;
using Apsis.Models.Response;
using Apsis.Models.Entities;
using Apsis.Notification;
using Apsis.Abstractions;

namespace Apsis.Tests.Business
{
    [TestClass]
    public class RequestManagerTest
    {
        RequestManager requestManager;
        Mock<IFileOperationsManager> fileOperationsManager;
        IRepository _requestRepository;
        Mock<IRequestRepository> _mockRequestRepository;
        
     
        List<RequestModel> mockRequestModelList = new List<RequestModel>
        {
            new RequestModel { YorbitCourseId = "19000" }
        };
        FileDownloadResponse mockFileDownloadResponse = new FileDownloadResponse
        {
            IsSuccess = true,
            FileUrl = "abc.com"
        };

        Guid mockrequestId = new Guid("56ae2fe8-f96d-47f2-ad25-003228c67ca2");
        DateTime mocknewAssignmentDueDate = DateTime.Now;

        public RequestManagerTest()
        {
            Guid learnerId = Guid.Empty;
            _mockRequestRepository = new Mock<IRequestRepository>();
            var _mockRequestManager = new Mock<IRequestRepository>();
            var _mockBlobManager = new Mock<IBlobHelper>();
            var _mockEmailManager = new Mock<IEmailManager>();
            var _mockContextProvidor = new Mock<IContextProvider>();
            fileOperationsManager = new Mock<IFileOperationsManager>();
            _mockRequestRepository.Setup(x => x.GetLearnerRequests(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(mockRequestModelList);
            
            requestManager = new RequestManager(_mockRequestRepository.Object , fileOperationsManager.Object, _mockBlobManager.Object,_mockEmailManager.Object, _mockContextProvidor.Object);
            //_mockRequestRepository.Setup(x => x.GetLearnerRequests()).ReturnsAsync(mockRequestModelList);
            _mockRequestRepository.Setup(x => x.CheckCriteriaUpdateAssignmentDueDate(It.IsAny<Guid>())).ReturnsAsync(true);
            _mockRequestRepository.Setup(x => x.UpdateAssignmentDueDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            requestManager = new RequestManager(_mockRequestRepository.Object , fileOperationsManager.Object, _mockBlobManager.Object, _mockEmailManager.Object, _mockContextProvidor.Object);
        }

        [TestMethod]
        public async Task GetLearnerList_Test()
        {
            var result = await requestManager.GetLearnerRequests();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].YorbitCourseId, mockRequestModelList[0].YorbitCourseId);
        }

        [TestMethod]
        public async Task DownloadAssignment_Test_EmptyFileIds()
        {
            IEnumerable<Guid> requestIds = new List<Guid> { Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") };
            var result = await requestManager.DownloadAssignment(requestIds);
            Assert.IsNotNull(result);
            var status = (result as FileDownloadResponse).IsSuccess;
            var message = (result as FileDownloadResponse).Message;
            Assert.AreEqual(status, false);
            Assert.AreEqual(message, "No files to download.");
        }

        [TestMethod]
        public async Task DownloadAssignment_Test_EmptyUploads()
        {
            var requestIds = new List<Guid> { Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") };
            //var uploads = new List<Upload> { new Upload { Id = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") } };
            List<Upload> uploads = null;
            _mockRequestRepository.Setup(x => x.GetAssignmentAttemptsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(requestIds);
            _mockRequestRepository.Setup(x => x.GetUploadsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(uploads);
            var result = await requestManager.DownloadAssignment(requestIds);
            Assert.IsNotNull(result);
            var status = (result as FileDownloadResponse).IsSuccess;
            var message = (result as FileDownloadResponse).Message;
            Assert.AreEqual(status, false);
            Assert.AreEqual(message, "No files to download.");
        }

        [TestMethod]
        public async Task DownloadAssignment_Test_UploadsCount1()
        {
            var requestIds = new List<Guid> { Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") };
            var uploads = new List<Upload> { new Upload { Id = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") } };
            var fileDetails = new FileDownloadResponse { IsSuccess = true };
            _mockRequestRepository.Setup(x => x.GetAssignmentAttemptsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(requestIds);
            _mockRequestRepository.Setup(x => x.GetUploadsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(uploads);
            fileOperationsManager.Setup(x => x.DownloadFile(uploads)).ReturnsAsync(fileDetails);
            var result = await requestManager.DownloadAssignment(requestIds);
            Assert.IsNotNull(result);
            var status = (result as FileDownloadResponse).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task DownloadAssignment_Test_UploadsCount1Plus()
        {
            var requestIds = new List<Guid> { Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512"), Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512")  };
            var uploads = new List<Upload> { new Upload { Id = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") }, new Upload { Id = Guid.Parse("6F5A7376-2D21-46C2-A459-E5E612238512") } };
            var fileDetails = new FileDownloadResponse { IsSuccess = true };
            _mockRequestRepository.Setup(x => x.GetAssignmentAttemptsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(requestIds);
            _mockRequestRepository.Setup(x => x.GetUploadsDetails(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(uploads);
            fileOperationsManager.Setup(x => x.DownloadZipFile(uploads)).ReturnsAsync(fileDetails);
            var result = await requestManager.DownloadAssignment(requestIds);
            Assert.IsNotNull(result);
            var status = (result as FileDownloadResponse).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_NoFileName()
        {
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { AssignmentSolutionFileName = null, ScoreCardFileName = null };
            _mockRequestRepository.Setup(x => x.GetRequestDetails(It.IsAny<Guid>())).ReturnsAsync(mockRequestDetailsModel);
            var result = await requestManager.GetRequestDetails(requestId);
            Assert.IsNotNull(result);
            var status = (result as RequestDetailsModel).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_AssignmentFileName()
        {
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { AssignmentSolutionFileName = "assignment", ScoreCardFileName = null };
            _mockRequestRepository.Setup(x => x.GetRequestDetails(It.IsAny<Guid>())).ReturnsAsync(mockRequestDetailsModel);
            var result = await requestManager.GetRequestDetails(requestId);
            Assert.IsNotNull(result);
            var status = (result as RequestDetailsModel).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_ScoreCardFileName()
        {
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { AssignmentSolutionFileName = null, ScoreCardFileName = "scoreCard" };
            _mockRequestRepository.Setup(x => x.GetRequestDetails(It.IsAny<Guid>())).ReturnsAsync(mockRequestDetailsModel);
            var result = await requestManager.GetRequestDetails(requestId);
            Assert.IsNotNull(result);
            var status = (result as RequestDetailsModel).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_BothFileName()
        {
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { AssignmentSolutionFileName = "assignment", ScoreCardFileName = "scoreCard" };
            _mockRequestRepository.Setup(x => x.GetRequestDetails(It.IsAny<Guid>())).ReturnsAsync(mockRequestDetailsModel);
            var result = await requestManager.GetRequestDetails(requestId);
            Assert.IsNotNull(result);
            var status = (result as RequestDetailsModel).IsSuccess;
            Assert.AreEqual(status, true);
        }

        [TestMethod]
        public async Task GetRequestDetails_Test_Exception()
        {
            var _requestManager = new RequestManager(null, null, null, null, null);
            RequestDetailsModel mockRequestDetailsModel = new RequestDetailsModel { Message = "Error Getting Request Details" };
            Guid requestId = Guid.Parse("D2BD833D-35C3-46B2-A6AA-870B4B54A5A9");
            var result = await _requestManager.GetRequestDetails(requestId);

            Assert.IsNotNull(result);
            var message = result as RequestDetailsModel;
            Assert.AreEqual(message.Message, mockRequestDetailsModel.Message);
        }


        [TestMethod]
        public async Task TestUpdateAssignmentDueDate()
        {
            _mockRequestRepository.Setup(x => x.CheckCriteriaUpdateAssignmentDueDate(It.IsAny<Guid>())).ReturnsAsync(true);
            _mockRequestRepository.Setup(x => x.UpdateAssignmentDueDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            Response _actualresponse =
                 await requestManager.UpdateAssignmentDueDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(_actualresponse);
            Assert.AreEqual(true, _actualresponse.IsSuccess);
            Assert.AreEqual("Due date updated.", _actualresponse.Message);
        }

        [TestMethod]
        public async Task TestUpdateAssignmentDueDate_CriteriaFailed()
        {
            _mockRequestRepository.Setup(x => x.CheckCriteriaUpdateAssignmentDueDate(It.IsAny<Guid>())).ReturnsAsync(false);
            _mockRequestRepository.Setup(x => x.UpdateAssignmentDueDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            Response _actualresponse =
                 await requestManager.UpdateAssignmentDueDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(_actualresponse);
            Assert.AreEqual(false, _actualresponse.IsSuccess);
            Assert.AreEqual("Due date cannot be updated.", _actualresponse.Message);
        }

        [TestMethod]
        public async Task TestUpdateAssignmentDueDate_Exception()
        {
            var wrongManager = new RequestManager(null, null, null,null, null);
            Response _actualresponse =
                 await wrongManager.UpdateAssignmentDueDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(_actualresponse);
            Assert.AreEqual(false, _actualresponse.IsSuccess);
            Assert.AreEqual("Error Occured..Try Again", _actualresponse.Message);
        }

        [TestMethod]
        public async Task TestUpdateAssignmentDueDate_UpdateFailed()
        {
            _mockRequestRepository.Setup(x => x.CheckCriteriaUpdateAssignmentDueDate(It.IsAny<Guid>())).ReturnsAsync(true);
            _mockRequestRepository.Setup(x => x.UpdateAssignmentDueDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            Response _actualresponse =
                 await requestManager.UpdateAssignmentDueDate(mockrequestId, mocknewAssignmentDueDate);
            Assert.IsNotNull(_actualresponse);
            Assert.AreEqual(false, _actualresponse.IsSuccess);
            Assert.AreEqual("Error updating due date.", _actualresponse.Message);
        }
    }
}
