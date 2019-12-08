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
using Apsis.Models.Entities;
using Apsis.Abstractions.Repository;
using Apsis.Models.Common;
using Apsis.Models.Constants;
namespace Apsis.Tests.Repository
{
    [TestClass]
    public class CourseRepositoryTest
    {
        CourseRepository courseRepository;
        List<CourseModel> mockCourseList = new List<CourseModel>
        {
            new CourseModel { YorbitCourseId = "19000"}
        };
   
        List<Course> mockCourseModelList = new List<Course>
        {
            new Course { YorbitCourseId = "19000"}
        };

        public CourseRepositoryTest()
        {
            var _mockBaseRepository = new Mock<IRepository>();
            _mockBaseRepository.Setup(x => x.QueryAsync<CourseModel>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(mockCourseList);
            _mockBaseRepository.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);
            courseRepository = new CourseRepository(_mockBaseRepository.Object);
        }

        //-----Test Scenarios-----//
        CourseFilter courseFilterSortDefault = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
        };

        CourseFilter courseFilterSort2 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_DESC
        };

        CourseFilter courseFilterSort3 = new CourseFilter
        {
            CurrentPage = 0,
            Keyword = "",
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_TYPE_ASC
        };

        CourseFilter courseFilterSort4 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_TYPE_DESC
        };
        CourseFilter courseFilterSort5 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.YORBIT_COURSE_ID_ASC
        };
        CourseFilter courseFilterSort6 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.ACADEMY_NAME_ASC
        };
        CourseFilter courseFilterSort7 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.ACADEMY_NAME_DESC
        };
        CourseFilter courseFilterSort8 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.YORBIT_COURSE_ID_DESC
        };
        CourseFilter courseFilterSort9 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.BATCH_TYPE_ASC
        };
        CourseFilter courseFilterSort10 = new CourseFilter
        {
            Keyword = "",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.BATCH_TYPE_DESC
        };
        CourseFilter courseFilterSearch1 = new CourseFilter
        {
            Keyword = "Angular L201",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };
        CourseFilter courseFilterSearch2 = new CourseFilter
        {
            Keyword = "DI_923",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };
        CourseFilter courseFilterSearch3 = new CourseFilter
        {
            Keyword = "Self-Paced",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };
        CourseFilter courseFilterSearch4 = new CourseFilter
        {
            Keyword = "Java",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };
        CourseFilter courseFilterSearch5 = new CourseFilter
        {
            Keyword = "Angular L201",
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };


        //-----Test Methods-----//
        [TestMethod]
        public async Task GetCourseAsyncOkObject_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSortDefault);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort2_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort2);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort3_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort3);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort4_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort4);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }
        CourseFilter courseFilterNullKeyword = new CourseFilter
        {
            CurrentPage = 0,
            PageSize = 10,
            Sort = E_COURSE_SORT.COURSE_NAME_ASC
        };

        [TestMethod]
        public async Task GetCourseAsyncSort5_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort5);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort6_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort6);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort7_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort7);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort8_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort8);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort9_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort9);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSort10_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSort10);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSearch1_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSearch1);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSearch2_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSearch2);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSearch3_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSearch3);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSearch4_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSearch4);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncSearch5_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterSearch5);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task GetCourseAsyncNull_Test()
        {
            var wrongCourseRepository = new CourseRepository(null);
            var result = await wrongCourseRepository.GetAllCoursesAsync(courseFilterSortDefault);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetCourseAsyncNullKeyword_Test()
        {
            var result = await courseRepository.GetAllCoursesAsync(courseFilterNullKeyword);
            Assert.IsNotNull(result);
            var list = result as List<CourseModel>;
            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public async Task UpdateCourseAsync_Test()
        {
            Guid courseId = Guid.NewGuid();
            Guid updateId = Guid.NewGuid();
            var result = await courseRepository.UpdateCourseAsync(courseId, updateId);
            Assert.AreEqual(result.IsSuccess, true);
        }
        [TestMethod]
        public async Task AddCourses_Test()
        {
            var result = await courseRepository.AddCourses(mockCourseModelList);
            Assert.AreEqual(result.IsSuccess, true);

            var wrongCourseRepository = new CourseRepository(null);
            var resultError = await wrongCourseRepository.AddCourses(mockCourseModelList);
            Assert.IsFalse(resultError.IsSuccess);
            Assert.AreEqual(resultError.Message, "File upload failed : Insert query to Course not working");
        }
    }
}
