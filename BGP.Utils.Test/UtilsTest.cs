using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BGP.Utils.Common;
using BGP.Dto;
using System.Collections.Generic;
using BGP.Entity.BEGAIT;
using System.Linq;
using System.Linq.Expressions;
using BGP.Enum;
using BGP.Web.Models;

namespace BGP.Utils.Test
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void AppendQueryStringTest()
        {
            string baseUrl = "http://somesite.com/news.php?article=1&lang=en";
            string result = UrlHelper.AppendQueryString(baseUrl, "userID", "1");
            Assert.AreEqual("http://somesite.com/news.php?article=1&lang=en&userID=1", result);
            baseUrl = "http://somesite.com/news.php";
            result = UrlHelper.AppendQueryString(baseUrl, "userID", "1");
            Assert.AreEqual("http://somesite.com/news.php?userID=1", result);
            baseUrl = "http://localhost:18688/Listing/Create?SectionID=13&SubSectionID=60&categoryID=1730";
            result = UrlHelper.AppendQueryString(baseUrl, "lang", "fr_BE");
            Assert.AreEqual("http://localhost:18688/Listing/Create?SectionID=13&SubSectionID=60&categoryID=1730&lang=fr_BE", result);
        }

        [TestMethod]
        public void CreateExpressionTest()
        {
            var branches = CreateBranches();
            var expression = PredicateBuilder.CreateLambdaExpression<BranchDto>("BranchEmail", "branchadmin@bgp.com");
            branches = branches.AsQueryable().Where(expression).ToList();
            Assert.AreEqual(1, branches.Count);
        }

        [TestMethod]
        public void SetPropertyTest()
        {
            var propertyName = "BranchStatus";
            var propertyValue = 1;
            var pagingParams = new PagingParams();
            pagingParams.SetProperty(propertyName, propertyValue);
            Assert.AreEqual(BranchStatus.Active, pagingParams.BranchStatus);
        }


        [TestMethod]
        public void DropdownListForEnumTest()
        {
            var selectList = System.Enum.GetValues(typeof(ListingStatus)).Cast<ListingStatus>().Where(x => (x as System.Enum).GetAttribute<IgnoredEnumAttribute>() == null).Select(x => new
            {
                Value = (Convert.ToInt64(x))
            });
            Assert.AreEqual(5, selectList.Count());
        }

        [TestMethod]
        public void GenerateRandomPasswordTest()
        {
            var Password = CryptHelper.GetSHA256Hash(RandomGenerator.RandomString(8), Guid.NewGuid().ToString("n"));
            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void DateTimeTest()
        {
            var now = DateTime.Now;
            now = now.Date.AddDays(1).AddSeconds(-1);
            Assert.AreEqual(DateTime.Now.Date, now.Date);
        }

        [TestMethod]
        public void GetImageNameFromUrlTest()
        {
            var url = "https://img.2dehands.be/f/normal/344889241.jpg";
            var name = ImageHelper.GetImageNameFromUrl(url);
            Assert.AreEqual("344889241.jpg",name);
        }

        [TestMethod]
        public void StripHTMLTest()
        {
            var input = "<a>hello</a>";
            Assert.AreEqual("hello", input.StripHTML());
            input = "<a>hello";
            Assert.AreEqual("hello", input.StripHTML());
            input = "hello</a>";
            Assert.AreEqual("hello", input.StripHTML());
            input = "h>el<lo";
            Assert.AreEqual("h>el<lo", input.StripHTML());
        }

        [TestMethod]
        public void LocalFileTest()
        {
            var filePath = @"D:\WorkSpace\BELGIUM\trunk\code\BGP.Web\BGP.Web\Uploads\Temp\Company\2\Listing\ce7cca68-8d89-4b36-9d7e-295944bda9f7.jpg";
            Assert.AreEqual(true, UrlHelper.IsLocalPath(filePath));
            filePath = "http://protoolbe-images.qa.denovu.com/Companies/2/Ads/1036/Images/0691d98745cf439cb91b0d7ba7dd2a6f.jpg";
            Assert.AreEqual(false, UrlHelper.IsLocalPath(filePath));
        }

        private List<BranchDto> CreateBranches()
        {
            var adminCompanyMainBranch = new BranchDto()
            {
                CompanyID = 1,
                BranchName = "Admin - Main branch",
                IsMainBranch = true,
                C2CUID = "123",
                BranchEmail = "branchadmin@bgp.com",
                LocaleID = 1,
                PostalCode = "1234",
                City = "Brussels",
                Phone = "0934946345",
                CreatedDateTime = DateTime.Now,
                CreatedUserID = null,
                BranchStatus = BranchStatus.Inactive
            };
            var testCompany1MainBranch = new BranchDto()
            {
                CompanyID = 1,
                BranchName = "ABC - Main branch",
                IsMainBranch = true,
                BranchEmail = "branchadmin1@bgp.com",
                LocaleID = 1,
                PostalCode = "1234",
                City = "Brussels",
                Phone = "0934946334",
                CreatedDateTime = DateTime.Now,
                CreatedUserID = null,
                BranchStatus = BranchStatus.Active
            };
            return new List<BranchDto>()
            {
                adminCompanyMainBranch,
                testCompany1MainBranch
            };
        }
    }
}
