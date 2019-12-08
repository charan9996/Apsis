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
using Apsis.Abstractions.Repository;
using Apsis.Models.Entities;
using System.Linq;
using Apsis.Models.Constants;
using Apsis.Abstractions;

namespace Apsis.Tests.Repository
{
    [TestClass]
    public class RequestRepositoryTest
    {
        RequestRepository requestRepository;
        List<RequestModel> mockRequestModelList = new List<RequestModel>
        {
           new RequestModel { YorbitCourseId = "19000" }
        };
        List<AssignmentAttempt> mockAssignmentDueDateListCriteriaCheck = new List<AssignmentAttempt>();
        DateTime mockDate = DateTime.Now;
        Guid mockrequestId = new Guid("56ae2fe8-f96d-47f2-ad25-003228c67ca2");
        List<Guid> mockYorbitStatusId = new List<Guid>
        {
            Constants.YetToSubmitYorbitStatusId
        };

        public RequestRepositoryTest()
        {
            Guid learnerId = Guid.Empty;
            var _mockBaseRepository = new Mock<IRepository>();
            var _mockContextProvider = new Mock<IContextProvider>();
            _mockBaseRepository.Setup(x => x.QueryAsync<RequestModel>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(mockRequestModelList);
            _mockBaseRepository.Setup(x=> x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);
            requestRepository = new RequestRepository(_mockBaseRepository.Object, _mockContextProvider.Object);
            _mockBaseRepository.Setup(x => x.QueryAsync<AssignmentAttempt>(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(mockAssignmentDueDateListCriteriaCheck);
            _mockBaseRepository.Setup(x => x.QueryAsync<Guid>(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(mockYorbitStatusId);
            _mockBaseRepository.Setup(x=>x.ExecuteAsync(It.IsAny<string>(),It.IsAny<object>())).ReturnsAsync(1);
        }

                   
        [TestMethod]                
        public async Task GetLearnerList_Test()
        {
            Guid notClearedId = RequestResultCodes.Project_Not_Cleared;
            Guid clearedId = RequestResultCodes.Project_Cleared;
            var result = await requestRepository.GetLearnerRequests(notClearedId, clearedId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].YorbitCourseId, mockRequestModelList[0].YorbitCourseId);
        }

        [TestMethod]
        public async Task AddRequests_Test()
        {
            var result = await requestRepository.AddRequests(mockRequestModelList);
            Assert.AreEqual(result.IsSuccess, true);
            var wrongRequestRepository = new RequestRepository(null, null);

            var resultError = await wrongRequestRepository.AddRequests(mockRequestModelList);
            Assert.IsFalse(resultError.IsSuccess);
            Assert.AreEqual(resultError.Message, "File upload failed : Insert query to Request not working");
        }

        //public void PrepareRequestQueryWithFilters_Test()
        //{
        //    throw new NotSupportedException();
        //}
       [TestMethod]
       public async Task CheckCriteriaUpdateAssignmentDueDate_Test()
        {
            bool result = await requestRepository.CheckCriteriaUpdateAssignmentDueDate(mockrequestId);
            Assert.AreEqual(result,false);

        }

        [TestMethod]
        public async Task UpdateAssignmentDueDate_Test()
        {
            bool result = await requestRepository.UpdateAssignmentDueDate(mockrequestId,mockDate );
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public async Task UpdateAssignmentDueDate_Test1()
        {
            Guid mockrequestId = new Guid("56ae2fe8-f96d-47f2-ad25-003229c67ca2");
            var wrongRequestRepository = new RequestRepository(null, null);
            bool result = await wrongRequestRepository.UpdateAssignmentDueDate(mockrequestId, mockDate);
            Assert.AreEqual(result, false);
        }

    }
}
