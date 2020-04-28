using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class PaginationRequestValidatorTest : ServiceBaseTest
    {
        [TestMethod]
        public void InvalidPageNumber()
        {
            var payload = new PaginationRequest
            {
                PageNumber = 0,
                PageSize = 20
            };

            var validator = GetValidator(payload);
            var result = validator.Validate(payload);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual("PageNumber", result.Errors[0].PropertyName);
        }

        [TestMethod]
        public void InvalidPageSize()
        {
            var payload = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 0
            };

            var validator = GetValidator(payload);
            var result = validator.Validate(payload);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual("PageSize", result.Errors[0].PropertyName);
        }

        [TestMethod]
        public void SortKeyInvalid()
        {
            var payload = new PaginationRequest<Cat>
            {
                PageNumber = 1,
                PageSize = 20,
                SortKey = "Ranking"
            };

            var validator = GetValidator(payload);
            var result = validator.Validate(payload);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual("SortKey", result.Errors[0].PropertyName);
        }

        [TestMethod]
        public void SortDirectionInvalid()
        {
            var payload = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 20,
                SortKey = "Name",
                SortDirection = "Ascending"
            };

            var validator = GetValidator(payload);
            var result = validator.Validate(payload);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual("SortDirection", result.Errors[0].PropertyName);
        }

        [TestMethod]
        public void Valid()
        {
            var payload = new PaginationRequest<Cat>
            {
                PageNumber = 1,
                PageSize = 20,
                SortKey = "Name",
                SortDirection = "asc"
            };

            var validator = GetValidator(payload);
            var result = validator.Validate(payload);

            Assert.AreEqual(true, result.IsValid);
        }
    }
}
