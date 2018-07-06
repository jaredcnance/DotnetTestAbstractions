using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp.Articles;
using AppContext = WebApp.Data.AppContext;
using Xunit;

// namespaces that you need to add
using DotnetTestAbstractions.Fixtures;

namespace WebApp.Tests
{
    public class ArticlesController_Tests
        : JsonServerFixture<TestStartup, AppContext>
    {
        [Fact]
        public async Task Can_Get_All_Articles()
        {
            // arrange
            var article = new Article();
            await DbContext.Articles.AddAsync(article);
            await DbContext.SaveChangesAsync();

            // act
            var articles = await GetAllAsync<Article>("articles");

            // assert
            Assert.Single(articles);
        }

        [Fact]
        public async Task Can_Get_Articles_By_Id()
        {
            // arrange
            var article = new Article();
            await DbContext.Articles.AddAsync(article);
            await DbContext.SaveChangesAsync();

            // act
            var resultArticle = await GetAsync<Article>($"articles/{article.Id}");

            // assert
            Assert.NotNull(resultArticle);
            Assert.Equal(article.Id, resultArticle.Id);
        }

        [Fact]
        public async Task Can_Create_Articles()
        {
            // arrange
            var article = new Article();

            // act
            var resultArticle = await PostAsync("articles", article);

            // assert
            Assert.NotNull(resultArticle);
            var persistedArticle = await DbContext.Articles.FindAsync(resultArticle.Id);
            Assert.NotNull(persistedArticle);
        }

        [Fact]
        public async Task Can_Create_And_Get_Articles()
        {
            // arrange
            var article = new Article();

            // act
            var articlePostResult = await PostAsync("articles", article);
            var articleGetResult = await GetAsync<Article>($"articles/{articlePostResult.Id}");

            // assert
            Assert.NotNull(articlePostResult);
            Assert.NotNull(articleGetResult);
            Assert.Equal(articlePostResult.Id, articleGetResult.Id);
        }

        [Fact]
        public async Task The_Database_Is_Empty()
        {
            // act
            var allArticles = await DbContext.Articles.ToListAsync();

            // assert
            Assert.Empty(allArticles);
        }
    }
}
