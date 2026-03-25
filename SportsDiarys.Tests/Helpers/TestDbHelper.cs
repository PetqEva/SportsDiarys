using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;

namespace SportsDiarys.Tests.Helpers
{
    public static class TestDbHelper
    {
        public static AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}